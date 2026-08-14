using System;
using System.IO;
using System.Linq;

namespace CodeShuttle
{
    /// <summary>
    /// Containment checks for paths that arrive from untrusted input (bundle headers pasted
    /// back from an AI chat, files on the clipboard, and so on).
    ///
    /// The rule is simple and deliberately strict: a relative path may only ever resolve to a
    /// location *strictly underneath* the root the user chose. Anything that could escape, name
    /// a device, or address an alternate data stream is rejected with a reason the UI can show.
    /// </summary>
    public static class PathSafety
    {
        /// <summary>Both Windows path separators. Hoisted so Split does not allocate per call.</summary>
        private static readonly char[] PathSeparators = { '\\', '/' };

        private static readonly string[] ReservedDeviceNames =
        {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };

        /// <summary>
        /// Resolves <paramref name="relative"/> against <paramref name="root"/>, returning false
        /// (with a human-readable <paramref name="reason"/>) if the result would not be contained
        /// within the root.
        /// </summary>
        public static bool TryResolveContained(string root, string relative, out string full, out string reason)
        {
            full = string.Empty;
            reason = string.Empty;

            if (string.IsNullOrWhiteSpace(root))
            {
                reason = "No target folder was supplied.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(relative))
            {
                reason = "The entry has an empty path.";
                return false;
            }

            if (Path.IsPathRooted(relative))
            {
                reason = "Absolute paths are not allowed; the entry must be relative to the target folder.";
                return false;
            }

            // A colon anywhere in the relative portion means either a drive specifier we already
            // rejected above, or an NTFS alternate data stream (file.txt:hidden).
            if (relative.Contains(':'))
            {
                reason = "The path contains ':' (drive or alternate data stream), which is not allowed.";
                return false;
            }

            if (relative.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                reason = "The path contains characters that are not valid in a file path.";
                return false;
            }

            // Wildcards would be interpreted by some APIs; a literal file never needs them.
            if (relative.AsSpan().IndexOfAny('*', '?') >= 0)
            {
                reason = "The path contains a wildcard character.";
                return false;
            }

            var segments = relative.Split(PathSeparators, StringSplitOptions.None);
            foreach (var raw in segments)
            {
                var segment = raw;
                if (segment.Length == 0) continue; // collapsed separator; harmless

                if (segment == "." || segment == "..")
                {
                    reason = $"The path contains a '{segment}' segment, which could escape the target folder.";
                    return false;
                }

                // Trailing dots and spaces are silently stripped by Win32, which lets
                // "evil.bat." and "evil.bat" resolve to the same file via different strings.
                if (segment.EndsWith('.') || segment.EndsWith(' '))
                {
                    reason = $"The path segment '{segment}' ends with a dot or space.";
                    return false;
                }

                if (IsReservedDeviceName(segment))
                {
                    reason = $"The path segment '{segment}' is a reserved Windows device name.";
                    return false;
                }

                if (segment.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    reason = $"The path segment '{segment}' contains characters that are not valid in a file name.";
                    return false;
                }
            }

            string rootFull;
            string candidate;
            try
            {
                rootFull = Path.GetFullPath(root);
                candidate = Path.GetFullPath(Path.Combine(rootFull, relative));
            }
            catch (Exception ex)
            {
                reason = "The path could not be resolved: " + ex.Message;
                return false;
            }

            var prefix = rootFull.EndsWith(Path.DirectorySeparatorChar)
                ? rootFull
                : rootFull + Path.DirectorySeparatorChar;

            if (!candidate.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                reason = "The path resolves outside the target folder.";
                return false;
            }

            full = candidate;
            return true;
        }

        /// <summary>True if the name (ignoring any extension) is a reserved Windows device name.</summary>
        public static bool IsReservedDeviceName(string segment)
        {
            if (string.IsNullOrEmpty(segment)) return false;
            var stem = segment;
            int dot = stem.IndexOf('.');
            if (dot >= 0) stem = stem.Substring(0, dot);
            return ReservedDeviceNames.Any(n => string.Equals(n, stem, StringComparison.OrdinalIgnoreCase));
        }
    }
}
