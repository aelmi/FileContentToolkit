using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileContentToolkit
{
    public enum FilePlanStatus { New, Unchanged, Modified }

    public class FilePlan
    {
        public string OriginalHeader { get; init; } = "";
        public string RelativePath { get; init; } = "";
        public string TargetPath { get; init; } = "";
        public string NewContent { get; init; } = "";
        public string? ExistingContent { get; init; }
        public FilePlanStatus Status { get; init; }
        public bool Include { get; set; } = true;
    }

    public static class FileRecreator
    {
        /// <summary>
        /// Parses the output into a plan: every entry is a file the output would write,
        /// paired with the existing on-disk content (if any) so callers can diff before writing.
        /// </summary>
        public static List<FilePlan> Plan(string output, string baseFolder)
        {
            var (headers, contents) = ParseOutput(output);
            string commonRoot = FindCommonRoot(headers);
            var result = new List<FilePlan>(headers.Count);

            for (int i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                var newContent = contents[i];

                string relativePath = ComputeRelativePath(header, commonRoot);
                string targetPath = Path.Combine(baseFolder, relativePath);

                string? existing = null;
                if (File.Exists(targetPath))
                {
                    try { existing = File.ReadAllText(targetPath); }
                    catch { existing = null; }
                }

                var status = existing == null
                    ? FilePlanStatus.New
                    : (string.Equals(existing, newContent, StringComparison.Ordinal)
                        ? FilePlanStatus.Unchanged
                        : FilePlanStatus.Modified);

                result.Add(new FilePlan
                {
                    OriginalHeader = header,
                    RelativePath = relativePath,
                    TargetPath = targetPath,
                    NewContent = newContent,
                    ExistingContent = existing,
                    Status = status,
                    Include = status != FilePlanStatus.Unchanged
                });
            }
            return result;
        }

        /// <summary>Writes every plan whose Include = true. Returns the number of files written.</summary>
        public static int Execute(IEnumerable<FilePlan> plans)
        {
            int n = 0;
            foreach (var p in plans)
            {
                if (!p.Include) continue;
                var dir = Path.GetDirectoryName(p.TargetPath);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(p.TargetPath, p.NewContent);
                n++;
            }
            return n;
        }

        /// <summary>Legacy: parse + write everything in one call.</summary>
        public static int RecreateFilesFromOutput(string output, string baseFolder, string originalBaseFolder)
        {
            var plans = Plan(output, baseFolder);
            return Execute(plans);
        }

        // -------------------- parsing --------------------

        private static (List<string> Headers, List<string> Contents) ParseOutput(string output)
        {
            var headers = new List<string>();
            var contents = new List<string>();
            var lines = (output ?? string.Empty).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            string? current = null;
            var buf = new List<string>();
            foreach (var line in lines)
            {
                if (IsHeader(line))
                {
                    if (current != null)
                    {
                        headers.Add(current);
                        contents.Add(string.Join(Environment.NewLine, TrimTrailingBlankLines(buf)));
                        buf.Clear();
                    }
                    current = line.TrimEnd(':');
                }
                else
                {
                    if (current != null) buf.Add(line);
                }
            }
            if (current != null)
            {
                headers.Add(current);
                contents.Add(string.Join(Environment.NewLine, TrimTrailingBlankLines(buf)));
            }
            return (headers, contents);
        }

        private static bool IsHeader(string line)
        {
            // Matches the format used by ProcessFilesAsync: a full path followed by ":" at end.
            // Accepts drive-letter paths ("C:\…:") and dot-relative paths (".\…:").
            return line.EndsWith(":") && line.Length > 2 && (line[1] == ':' || line.StartsWith(".\\"));
        }

        private static List<string> TrimTrailingBlankLines(List<string> lines)
        {
            int end = lines.Count;
            while (end > 0 && string.IsNullOrEmpty(lines[end - 1])) end--;
            return end == lines.Count ? lines : lines.GetRange(0, end);
        }

        private static string FindCommonRoot(List<string> paths)
        {
            if (paths == null || paths.Count == 0) return string.Empty;

            var split = paths
                .Select(p => p.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                                     StringSplitOptions.RemoveEmptyEntries))
                .ToList();

            int minLength = split.Min(parts => parts.Length);
            int commonLength = 0;
            for (int i = 0; i < minLength; i++)
            {
                string part = split[0][i];
                if (split.All(parts => string.Equals(parts[i], part, StringComparison.OrdinalIgnoreCase)))
                    commonLength++;
                else
                    break;
            }
            if (commonLength == 0) return string.Empty;

            string[] firstParts = split[0];
            string root = string.Join(Path.DirectorySeparatorChar.ToString(), firstParts.Take(commonLength));
            if (root.Length == 2 && root[1] == ':')
                root += Path.DirectorySeparatorChar;
            return root;
        }

        private static string ComputeRelativePath(string filePath, string commonRoot)
        {
            if (!string.IsNullOrEmpty(commonRoot) &&
                filePath.StartsWith(commonRoot, StringComparison.OrdinalIgnoreCase))
            {
                var rel = filePath.Substring(commonRoot.Length);
                if (rel.StartsWith(Path.DirectorySeparatorChar.ToString()) ||
                    rel.StartsWith(Path.AltDirectorySeparatorChar.ToString()))
                    rel = rel.Substring(1);
                return rel;
            }
            return Path.GetFileName(filePath);
        }
    }
}
