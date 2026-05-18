// C:\Users\alelm\OneDrive\Projects\FileContentToolkit\FileContentService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileContentToolkit.Filters;

namespace FileContentToolkit
{
    public class FileContentService
    {
        public string FolderPath { get; private set; } = string.Empty;
        public List<string> Extensions { get; } = new List<string>();
        public List<string> SelectedFiles { get; } = new List<string>();
        public bool IncludeSubfolders { get; set; } = true;
        public List<string> IgnorePatterns { get; } = new List<string>();

        // Filter settings (driven by AppSettings via MainForm)
        public long MaxFileSizeBytes { get; set; } = 0; // 0 = unlimited
        public bool SkipBinaryFiles { get; set; } = true;
        public bool AutoDetectEncoding { get; set; } = true;
        public bool UseGitIgnoreFiles { get; set; } = true;

        public void SetFolderPath(string path)
        {
            FolderPath = path;
        }

        public void AddExtension(string ext)
        {
            if (!Extensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                Extensions.Add(ext);
            }
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

        public void SetIncludeSubfolders(bool include)
        {
            IncludeSubfolders = include;
        }

        public void AddFiles(IEnumerable<string> files)
        {
            var existing = new HashSet<string>(SelectedFiles, StringComparer.OrdinalIgnoreCase);
            foreach (var file in files)
            {
                if (existing.Add(file))
                    SelectedFiles.Add(file);
            }
        }

        public void RemoveFileAt(int index)
        {
            if (index >= 0 && index < SelectedFiles.Count)
                SelectedFiles.RemoveAt(index);
        }

        public void RemoveFiles(IEnumerable<string> filesToRemove)
        {
            var set = new HashSet<string>(filesToRemove, StringComparer.OrdinalIgnoreCase);
            SelectedFiles.RemoveAll(f => set.Contains(f));
        }

        /// <summary>Reads a file using the configured encoding strategy.</summary>
        public string ReadFileText(string path, Encoding fallback)
        {
            var enc = AutoDetectEncoding ? EncodingDetector.Detect(path, fallback) : fallback;
            return File.ReadAllText(path, enc);
        }

        private static bool IsIgnoredByUserPatterns(string filePath, string folder, IReadOnlyList<string> patterns)
        {
            if (patterns.Count == 0) return false;
            string relPath = string.IsNullOrEmpty(folder) ? filePath : Path.GetRelativePath(folder, filePath);
            for (int i = 0; i < patterns.Count; i++)
            {
                var pattern = patterns[i];
                if (pattern.StartsWith("*."))
                {
                    if (Path.GetExtension(filePath).Equals(pattern.Substring(1), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                else if (pattern.EndsWith("/"))
                {
                    if (relPath.StartsWith(pattern, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                else if (relPath.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public void RefreshFiles()
        {
            SelectedFiles.Clear();

            if (string.IsNullOrEmpty(FolderPath) || !Directory.Exists(FolderPath) || Extensions.Count == 0)
                return;

            var searchOption = IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var extSet = new HashSet<string>(Extensions, StringComparer.OrdinalIgnoreCase);
            var patterns = IgnorePatterns.ToList();
            var folder = FolderPath;
            var gitIgnore = UseGitIgnoreFiles ? GitIgnoreParser.FromFolder(folder) : new GitIgnoreParser();
            var maxBytes = MaxFileSizeBytes;
            var skipBinary = SkipBinaryFiles;

            var files = Directory.EnumerateFiles(FolderPath, "*.*", searchOption)
                .Where(file => extSet.Contains(Path.GetExtension(file)))
                .Where(file => PassesAllFilters(file, folder, patterns, gitIgnore, maxBytes, skipBinary))
                .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                .ToList();

            SelectedFiles.AddRange(files);
        }

        public async Task RefreshFilesAsync(IProgress<int> progress, CancellationToken ct)
        {
            var folder = FolderPath;
            var extensions = Extensions.ToList();
            var patterns = IgnorePatterns.ToList();
            var includeSubfolders = IncludeSubfolders;
            var useGitIgnore = UseGitIgnoreFiles;
            var maxBytes = MaxFileSizeBytes;
            var skipBinary = SkipBinaryFiles;

            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder) || extensions.Count == 0)
            {
                SelectedFiles.Clear();
                progress?.Report(100);
                return;
            }

            var result = await Task.Run(() =>
            {
                var extSet = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);
                var gitIgnore = useGitIgnore ? GitIgnoreParser.FromFolder(folder) : new GitIgnoreParser();
                var enumOptions = new EnumerationOptions
                {
                    IgnoreInaccessible = true,
                    RecurseSubdirectories = includeSubfolders
                };

                var allFiles = Directory.EnumerateFiles(folder, "*.*", enumOptions).ToList();
                ct.ThrowIfCancellationRequested();

                var filtered = new List<string>(allFiles.Count);
                int total = Math.Max(1, allFiles.Count);
                int lastReported = -1;

                for (int i = 0; i < allFiles.Count; i++)
                {
                    if ((i & 0x3FF) == 0) ct.ThrowIfCancellationRequested();
                    var file = allFiles[i];
                    if (extSet.Contains(Path.GetExtension(file)) &&
                        PassesAllFilters(file, folder, patterns, gitIgnore, maxBytes, skipBinary))
                    {
                        filtered.Add(file);
                    }

                    int pct = (int)((long)(i + 1) * 100 / total);
                    if (pct != lastReported)
                    {
                        lastReported = pct;
                        progress?.Report(pct);
                    }
                }

                filtered.Sort(StringComparer.OrdinalIgnoreCase);
                return filtered;
            }, ct);

            SelectedFiles.Clear();
            SelectedFiles.AddRange(result);
        }

        private static bool PassesAllFilters(
            string file,
            string folder,
            IReadOnlyList<string> patterns,
            GitIgnoreParser gitIgnore,
            long maxBytes,
            bool skipBinary)
        {
            if (IsIgnoredByUserPatterns(file, folder, patterns)) return false;

            if (gitIgnore.HasRules)
            {
                var rel = Path.GetRelativePath(folder, file);
                if (gitIgnore.IsIgnored(rel)) return false;
            }

            if (maxBytes > 0)
            {
                try
                {
                    var len = new FileInfo(file).Length;
                    if (len > maxBytes) return false;
                }
                catch { return false; }
            }

            if (skipBinary && BinaryFileDetector.IsBinary(file)) return false;

            return true;
        }

        public List<(string Extension, int Count)> GetAvailableExtensionCounts(bool onlyConfigured)
        {
            var result = new List<(string, int)>();

            if (string.IsNullOrEmpty(FolderPath) || !Directory.Exists(FolderPath))
                return result;

            var searchOption = IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(FolderPath, "*.*", searchOption);
            }
            catch
            {
                return result;
            }

            var groups = files
            .GroupBy(f => (Path.GetExtension(f) ?? string.Empty).ToLowerInvariant())
            .Select(g => new { Ext = string.IsNullOrEmpty(g.Key) ? "(no ext)" : g.Key, Count = g.Count() });

            if (onlyConfigured && Extensions.Count > 0)
            {
                var set = new HashSet<string>(Extensions, StringComparer.OrdinalIgnoreCase);
                groups = groups.Where(g => set.Contains(g.Ext.Equals("(no ext)") ? string.Empty : g.Ext));
            }

            return groups
            .OrderByDescending(g => g.Count)
            .ThenBy(g => g.Ext)
            .Select(g => (g.Ext, g.Count))
            .ToList();
        }
    }
}
