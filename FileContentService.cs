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
    }
}