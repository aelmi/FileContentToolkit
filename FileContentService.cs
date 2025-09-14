// C:\Users\alelm\OneDrive\Projects\FileContentToolkit\FileContentService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileContentToolkit
{
    public class FileContentService
    {
        public string FolderPath { get; private set; } = string.Empty;
        public List<string> Extensions { get; } = new List<string>();
        public List<string> SelectedFiles { get; } = new List<string>();
        public bool IncludeSubfolders { get; set; } = false;

        public void SetFolderPath(string path)
        {
            FolderPath = path;
            RefreshFiles();
        }

        public void AddExtension(string ext)
        {
            if (!Extensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                Extensions.Add(ext);
                RefreshFiles();
            }
        }

        public void RemoveExtension(string ext)
        {
            Extensions.RemoveAll(e => e.Equals(ext, StringComparison.OrdinalIgnoreCase));
            RefreshFiles();
        }

        public void SetExtensions(IEnumerable<string> exts)
        {
            Extensions.Clear();
            Extensions.AddRange(exts);
            RefreshFiles();
        }

        public void SetIncludeSubfolders(bool include)
        {
            IncludeSubfolders = include;
            RefreshFiles();
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

        public void RefreshFiles()
        {
            SelectedFiles.Clear();

            if (string.IsNullOrEmpty(FolderPath) || !Directory.Exists(FolderPath) || Extensions.Count == 0)
                return;

            var searchOption = IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(FolderPath, "*.*", searchOption)
                .Where(file => Extensions.Contains(Path.GetExtension(file).ToLower()))
                .OrderBy(file => file)
                .ToList();

            SelectedFiles.AddRange(files);
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


