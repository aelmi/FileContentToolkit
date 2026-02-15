// C:\Users\alelm\OneDrive\Projects\FileContentToolkit\FileContentService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileContentToolkit
{
    public class FileContentService
    {
        public string FolderPath { get; private set; } = string.Empty;
        public List<string> Extensions { get; } = new List<string>();
        public List<string> SelectedFiles { get; } = new List<string>();
        public bool IncludeSubfolders { get; set; } = false;
        public List<string> IgnorePatterns { get; } = new List<string>(); // New: for smart ignore rules

        public void SetFolderPath(string path)
        {
            FolderPath = path;
            // RefreshFiles(); // Now called externally, as async
        }

        public void AddExtension(string ext)
        {
            if (!Extensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                Extensions.Add(ext);
                // RefreshFiles(); // Now external
            }
        }

        public void RemoveExtension(string ext)
        {
            Extensions.RemoveAll(e => e.Equals(ext, StringComparison.OrdinalIgnoreCase));
            // RefreshFiles();
        }

        public void SetExtensions(IEnumerable<string> exts)
        {
            Extensions.Clear();
            Extensions.AddRange(exts);
            // RefreshFiles();
        }

        public void SetIncludeSubfolders(bool include)
        {
            IncludeSubfolders = include;
            // RefreshFiles();
        }

        public void AddFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                if (!SelectedFiles.Contains(file))
                    SelectedFiles.Add(file);
            }
        }

        public void RemoveFileAt(int index)
        {
            if (index >= 0 && index < SelectedFiles.Count)
                SelectedFiles.RemoveAt(index);
        }

        // New method to remove multiple files
        public void RemoveFiles(IEnumerable<string> filesToRemove)
        {
            foreach (var file in filesToRemove.ToList()) // ToList() to avoid modification during enumeration
            {
                SelectedFiles.Remove(file);
            }
        }

        // Modified: Added ignore rules matching (simple wildcard/path)
        private bool IsIgnored(string filePath)
        {
            string relPath = GetRelativePath(FolderPath, filePath);
            foreach (var pattern in IgnorePatterns)
            {
                if (pattern.StartsWith("*.")) // Extension ignore
                {
                    if (Path.GetExtension(filePath).Equals(pattern.Substring(1), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                else if (pattern.EndsWith("/")) // Directory ignore
                {
                    if (relPath.StartsWith(pattern, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                else if (relPath.Contains(pattern, StringComparison.OrdinalIgnoreCase)) // Simple contains
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
            var files = Directory.GetFiles(FolderPath, "*.*", searchOption)
                .Where(file => Extensions.Contains(Path.GetExtension(file).ToLowerInvariant(), StringComparer.OrdinalIgnoreCase))
                .Where(file => !IsIgnored(file)) // Apply ignore rules
                .OrderBy(file => file)
                .ToList();

            SelectedFiles.AddRange(files);
        }

        // New: Async version for background scan
        public async Task RefreshFilesAsync(IProgress<int> progress)
        {
            SelectedFiles.Clear();

            if (string.IsNullOrEmpty(FolderPath) || !Directory.Exists(FolderPath) || Extensions.Count == 0)
                return;

            var searchOption = IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var allFiles = await Task.Run(() => Directory.EnumerateFiles(FolderPath, "*.*", searchOption).ToList());

            var filteredFiles = new List<string>();
            int total = allFiles.Count;
            int count = 0;

            foreach (var file in allFiles)
            {
                if (Extensions.Contains(Path.GetExtension(file).ToLowerInvariant(), StringComparer.OrdinalIgnoreCase) && !IsIgnored(file))
                {
                    filteredFiles.Add(file);
                }
                count++;
                progress?.Report((int)((double)count / total * 100));
                await Task.Delay(1); // Simulate for progress
            }

            filteredFiles.Sort();
            SelectedFiles.AddRange(filteredFiles);
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

        // Helper from MainForm
        private static string GetRelativePath(string basePath, string fullPath)
        {
            Uri baseUri = new Uri(AppendDirectorySeparatorChar(basePath));
            Uri fullUri = new Uri(fullPath);
            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);
            return Uri.UnescapeDataString(relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return path + Path.DirectorySeparatorChar;
            return path;
        }
    }
}