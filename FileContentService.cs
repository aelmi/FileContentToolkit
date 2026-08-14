using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeShuttle.Filters;

namespace CodeShuttle
{
    /// <summary>The outcome of one scan. Returned rather than applied, so a superseded scan cannot clobber a newer one.</summary>
    public sealed class ScanResult
    {
        public List<string> Files { get; init; } = new();
        public List<SkippedFile> Skipped { get; init; } = new();

        /// <summary>Total size of <see cref="Files"/>, captured during enumeration so the status bar never re-stats.</summary>
        public long TotalBytes { get; init; }

        /// <summary>Set when an ignore rule set excluded almost everything — usually a misconfigured pattern.</summary>
        public string? RuleWarning { get; init; }
    }

    public class FileContentService
    {
        public string FolderPath { get; private set; } = string.Empty;
        public List<string> Extensions { get; } = new List<string>();
        public List<string> SelectedFiles { get; } = new List<string>();
        public bool IncludeSubfolders { get; set; } = true;
        public List<string> IgnorePatterns { get; } = new List<string>();

        // Filter settings (driven by AppSettings via MainForm)
        public long MaxFileSizeBytes { get; set; } // 0 = unlimited
        public bool SkipBinaryFiles { get; set; } = true;
        public bool AutoDetectEncoding { get; set; } = true;
        public bool UseGitIgnoreFiles { get; set; } = true;

        /// <summary>
        /// Off by default and deliberately separate from the gitignore setting: a typical
        /// .dockerignore begins with "*" and re-includes with "!", which merged into one rule
        /// list excluded every file in an ordinary repository with no explanation.
        /// </summary>
        public bool UseDockerIgnoreFiles { get; set; }

        /// <summary>How deep the enumeration will follow directories. Bounds junction-loop damage.</summary>
        public int MaxRecursionDepth { get; set; } = 64;

        private readonly List<SkippedFile> _skipped = new();

        /// <summary>Files left out of the last scan, with the reason for each.</summary>
        public IReadOnlyList<SkippedFile> SkippedFiles => _skipped;

        /// <summary>Total size of <see cref="SelectedFiles"/> as measured during the last scan.</summary>
        public long TotalSelectedBytes { get; private set; }

        public void SetFolderPath(string path) => FolderPath = path;

        public void AddExtension(string ext)
        {
            if (!Extensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                Extensions.Add(ext);
        }

        public void RemoveExtension(string ext)
        {
            Extensions.RemoveAll(e => e.Equals(ext, StringComparison.OrdinalIgnoreCase));
        }

        public void SetExtensions(IEnumerable<string> exts)
        {
            Extensions.Clear();
            Extensions.AddRange(exts);
        }

        public void SetIncludeSubfolders(bool include) => IncludeSubfolders = include;

        /// <summary>
        /// Adds files, de-duplicating on the canonical full path. Comparing raw strings counted
        /// "C:\a\x.cs", "c:/a/x.cs" and the 8.3 short form as three separate files.
        /// </summary>
        public void AddFiles(IEnumerable<string> files)
        {
            var existing = new HashSet<string>(SelectedFiles.Select(Canonicalise), StringComparer.OrdinalIgnoreCase);
            foreach (var file in files)
            {
                if (string.IsNullOrWhiteSpace(file)) continue;

                // Dropping a directory used to add it as a "file", producing "[Error reading file: …]".
                try { if (Directory.Exists(file)) continue; } catch { continue; }

                if (existing.Add(Canonicalise(file)))
                    SelectedFiles.Add(file);
            }
            RecomputeTotalBytes();
        }

        private static string Canonicalise(string path)
        {
            try { return Path.GetFullPath(path); }
            catch { return path; }
        }

        public void RemoveFiles(IEnumerable<string> filesToRemove)
        {
            var set = new HashSet<string>(filesToRemove.Select(Canonicalise), StringComparer.OrdinalIgnoreCase);
            SelectedFiles.RemoveAll(f => set.Contains(Canonicalise(f)));
            RecomputeTotalBytes();
        }

        private void RecomputeTotalBytes()
        {
            long total = 0;
            foreach (var f in SelectedFiles)
            {
                try { total += new FileInfo(f).Length; } catch { /* unreadable: contributes nothing */ }
            }
            TotalSelectedBytes = total;
        }

        // -------------------- reading --------------------

        /// <summary>
        /// Reads a file using the configured encoding strategy. Decoding is strict: reading a
        /// UTF-8 file as ASCII used to replace every non-ASCII byte with '?', and that corrupted
        /// text flowed into exports and back over the user's real files.
        /// </summary>
        public string ReadFileText(string path, Encoding fallback)
        {
            var enc = AutoDetectEncoding ? EncodingDetector.Detect(path, fallback) : fallback;
            return File.ReadAllText(path, Strict(enc));
        }

        /// <summary>Wraps an encoding so invalid bytes throw instead of being silently replaced.</summary>
        public static Encoding Strict(Encoding encoding)
        {
            try
            {
                return Encoding.GetEncoding(encoding.CodePage,
                    EncoderFallback.ExceptionFallback,
                    DecoderFallback.ExceptionFallback);
            }
            catch
            {
                return encoding;
            }
        }

        // -------------------- filtering --------------------

        /// <summary>
        /// User ignore patterns. Previously a raw substring test, so the pattern "bin" excluded
        /// Robinson.cs and Binder.cs, and the documented "dir/" form never matched anything at
        /// all because Windows relative paths use backslashes.
        /// </summary>
        internal static bool IsIgnoredByUserPatterns(string filePath, string folder, IReadOnlyList<string> patterns)
        {
            if (patterns.Count == 0) return false;

            string relPath = string.IsNullOrEmpty(folder) ? filePath : Path.GetRelativePath(folder, filePath);
            relPath = relPath.Replace('\\', '/').TrimStart('/');

            for (int i = 0; i < patterns.Count; i++)
            {
                var pattern = (patterns[i] ?? "").Trim().Replace('\\', '/');
                if (pattern.Length == 0) continue;

                // Route through the same glob engine as .gitignore so one mental model covers both.
                var parser = new GitIgnoreParser(ignoreCase: true);
                parser.AddRules(new[] { pattern });
                if (parser.IsIgnored(relPath)) return true;
            }
            return false;
        }

        // -------------------- enumeration --------------------

        private EnumerationOptions BuildEnumerationOptions() => new()
        {
            // One access-denied folder used to abort the entire enumeration.
            IgnoreInaccessible = true,
            RecurseSubdirectories = IncludeSubfolders,
            // .NET does not detect reparse cycles, so a junction pointing at an ancestor recurses
            // until PathTooLongException. Skipping reparse points makes following them opt-in.
            AttributesToSkip = FileAttributes.ReparsePoint | FileAttributes.Hidden | FileAttributes.System,
            MaxRecursionDepth = MaxRecursionDepth
        };

        public List<string> EnumerateMatchingFiles(string folder)
            => EnumerateMatchingFilesCore(folder, CancellationToken.None, null, out _, out _);

        private List<string> EnumerateMatchingFilesCore(
            string folder,
            CancellationToken ct,
            IProgress<int>? progress,
            out List<SkippedFile> skipped,
            out long totalBytes)
        {
            skipped = new List<SkippedFile>();
            totalBytes = 0;

            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder) || Extensions.Count == 0)
                return new List<string>();

            var extSet = new HashSet<string>(Extensions, StringComparer.OrdinalIgnoreCase);
            var patterns = IgnorePatterns.ToList();
            var gitIgnore = UseGitIgnoreFiles ? GitIgnoreParser.FromFolderCached(folder) : new GitIgnoreParser();
            var dockerIgnore = UseDockerIgnoreFiles ? GitIgnoreParser.FromDockerIgnoreCached(folder) : new GitIgnoreParser();
            var maxBytes = MaxFileSizeBytes;
            var skipBinary = SkipBinaryFiles;

            var result = new List<string>();
            long bytes = 0;
            int seen = 0;
            int candidates = 0;

            var root = new DirectoryInfo(folder);
            // DirectoryInfo enumeration carries Length with it, so the status bar never needs a
            // second metadata round-trip per file.
            foreach (var info in root.EnumerateFiles("*", BuildEnumerationOptions()))
            {
                if ((seen++ & 0x3FF) == 0) ct.ThrowIfCancellationRequested();

                if (!extSet.Contains(info.Extension)) continue;
                candidates++;

                var relative = Path.GetRelativePath(folder, info.FullName).Replace('\\', '/');

                if (IsIgnoredByUserPatterns(info.FullName, folder, patterns) ||
                    (gitIgnore.HasRules && gitIgnore.IsIgnored(relative)) ||
                    (dockerIgnore.HasRules && dockerIgnore.IsIgnored(relative)))
                {
                    skipped.Add(new SkippedFile { Path = info.FullName, Reason = SkipReason.IgnoredByRule });
                    continue;
                }

                if (maxBytes > 0 && info.Length > maxBytes)
                {
                    skipped.Add(new SkippedFile
                    {
                        Path = info.FullName,
                        Reason = SkipReason.TooLarge,
                        Detail = $"{info.Length:N0} bytes"
                    });
                    continue;
                }

                if (skipBinary)
                {
                    var classification = BinaryFileDetector.Classify(info.FullName);
                    if (classification == FileReadability.Binary)
                    {
                        skipped.Add(new SkippedFile { Path = info.FullName, Reason = SkipReason.Binary });
                        continue;
                    }
                    if (classification == FileReadability.AccessDenied)
                    {
                        skipped.Add(new SkippedFile { Path = info.FullName, Reason = SkipReason.AccessDenied });
                        continue;
                    }
                    if (classification == FileReadability.IoError)
                    {
                        skipped.Add(new SkippedFile { Path = info.FullName, Reason = SkipReason.IoError });
                        continue;
                    }
                }

                result.Add(info.FullName);
                bytes += info.Length;

                if (progress != null && candidates % 256 == 0)
                    progress.Report(-1); // indeterminate tick; the caller decides what to display
            }

            result.Sort(StringComparer.OrdinalIgnoreCase);
            totalBytes = bytes;
            return result;
        }

        /// <summary>
        /// Scans and RETURNS the result. The caller assigns it only if its scan is still the
        /// current one — an unconditional in-place assignment let a superseded scan overwrite a
        /// newer one's results.
        /// </summary>
        public async Task<ScanResult> ScanAsync(IProgress<int>? progress, CancellationToken ct)
        {
            var folder = FolderPath;
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder) || Extensions.Count == 0)
            {
                progress?.Report(100);
                return new ScanResult();
            }

            return await Task.Run(() =>
            {
                int reported = 0;
                var tick = new Progress<int>(_ =>
                {
                    // Real percentages need a denominator we do not have while streaming, so we
                    // creep toward 95% and let completion supply the last step.
                    reported = Math.Min(95, reported + 1);
                    progress?.Report(reported);
                });

                var files = EnumerateMatchingFilesCore(folder, ct, tick, out var skipped, out var totalBytes);

                int candidates = files.Count + skipped.Count(s => s.Reason == SkipReason.IgnoredByRule);
                string? warning = null;
                if (candidates >= 20 && files.Count * 20 < candidates)
                {
                    warning =
                        $"Ignore rules excluded {candidates - files.Count:N0} of {candidates:N0} matching files. " +
                        "Check the .gitignore/.dockerignore settings and your ignore patterns if that looks wrong.";
                }

                progress?.Report(100);
                return new ScanResult
                {
                    Files = files,
                    Skipped = skipped,
                    TotalBytes = totalBytes,
                    RuleWarning = warning
                };
            }, ct).ConfigureAwait(true);
        }

        /// <summary>Adopts a scan result as the current selection.</summary>
        public void ApplyScanResult(ScanResult result)
        {
            SelectedFiles.Clear();
            SelectedFiles.AddRange(result.Files);
            _skipped.Clear();
            _skipped.AddRange(result.Skipped);
            TotalSelectedBytes = result.TotalBytes;
        }

        public void RefreshFiles()
        {
            SelectedFiles.Clear();
            SelectedFiles.AddRange(EnumerateMatchingFilesCore(FolderPath, CancellationToken.None, null, out var skipped, out var bytes));
            _skipped.Clear();
            _skipped.AddRange(skipped);
            TotalSelectedBytes = bytes;
        }

        // -------------------- extension counts --------------------

        public List<(string Extension, int Count)> GetAvailableExtensionCounts(bool onlyConfigured)
            => GetAvailableExtensionCounts(onlyConfigured, CancellationToken.None);

        public List<(string Extension, int Count)> GetAvailableExtensionCounts(bool onlyConfigured, CancellationToken ct)
        {
            var result = new List<(string, int)>();
            if (string.IsNullOrEmpty(FolderPath) || !Directory.Exists(FolderPath))
                return result;

            var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            try
            {
                int seen = 0;
                // The old version wrapped only the CALL in try/catch, not the deferred
                // enumeration, so an access-denied folder escaped anyway.
                foreach (var file in Directory.EnumerateFiles(FolderPath, "*", BuildEnumerationOptions()))
                {
                    if ((seen++ & 0x3FF) == 0) ct.ThrowIfCancellationRequested();
                    var ext = (Path.GetExtension(file) ?? string.Empty).ToLowerInvariant();
                    var key = string.IsNullOrEmpty(ext) ? "(no ext)" : ext;
                    counts[key] = counts.TryGetValue(key, out var n) ? n + 1 : 1;
                }
            }
            catch (OperationCanceledException) { throw; }
            catch { /* partial counts are still useful */ }

            IEnumerable<KeyValuePair<string, int>> groups = counts;
            if (onlyConfigured && Extensions.Count > 0)
            {
                var set = new HashSet<string>(Extensions, StringComparer.OrdinalIgnoreCase);
                groups = groups.Where(g => set.Contains(g.Key.Equals("(no ext)", StringComparison.Ordinal) ? string.Empty : g.Key));
            }

            return groups
                .OrderByDescending(g => g.Value)
                .ThenBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
                .Select(g => (g.Key, g.Value))
                .ToList();
        }

        public Task<List<(string Extension, int Count)>> GetAvailableExtensionCountsAsync(bool onlyConfigured, CancellationToken ct)
            => Task.Run(() => GetAvailableExtensionCounts(onlyConfigured, ct), ct);
    }
}
