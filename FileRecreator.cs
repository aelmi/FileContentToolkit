using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileContentToolkit
{
    public static class FileRecreator
    {
        /// <summary>
        /// Parses the output and recreates files in the target folder.
        /// </summary>
        /// <param name="output">The output text (file paths and contents).</param>
        /// <param name="baseFolder">The folder to create files in.</param>
        /// <param name="originalBaseFolder">Unused, kept for compatibility.</param>
        /// <returns>The number of files created.</returns>
        public static int RecreateFilesFromOutput(string output, string baseFolder, string originalBaseFolder)
        {
            var lines = output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            string currentFile = null;
            List<string> content = new List<string>();
            int fileCount = 0;
            var allFilePaths = new List<string>();

            // First pass: collect all file paths
            foreach (var line in lines)
            {
                if (line.EndsWith(":") && (line.Length > 2 && (line[1] == ':' || line.StartsWith(".\\"))))
                {
                    string filePath = line.TrimEnd(':');
                    allFilePaths.Add(filePath);
                }
            }

            // Find common root
            string commonRoot = FindCommonRoot(allFilePaths);

            // Second pass: create files
            foreach (var line in lines)
            {
                if (line.EndsWith(":") && (line.Length > 2 && (line[1] == ':' || line.StartsWith(".\\"))))
                {
                    if (currentFile != null)
                    {
                        WriteFile(currentFile, content, baseFolder, commonRoot);
                        fileCount++;
                        content.Clear();
                    }
                    currentFile = line.TrimEnd(':');
                }
                else
                {
                    content.Add(line);
                }
            }
            if (currentFile != null)
            {
                WriteFile(currentFile, content, baseFolder, commonRoot);
                fileCount++;
            }
            return fileCount;
        }

        // Helper to find the common root of all file paths
        private static string FindCommonRoot(List<string> paths)
        {
            if (paths == null || paths.Count == 0)
                return string.Empty;

            var splitPaths = paths
                .Select(p => p.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
                .ToList();

            int minLength = splitPaths.Min(parts => parts.Length);
            int commonLength = 0;

            for (int i = 0; i < minLength; i++)
            {
                string part = splitPaths[0][i];
                if (splitPaths.All(parts => string.Equals(parts[i], part, StringComparison.OrdinalIgnoreCase)))
                {
                    commonLength++;
                }
                else
                {
                    break;
                }
            }

            if (commonLength == 0)
                return string.Empty;

            // For Windows drive root, add back the colon (e.g., "C:")
            string[] firstParts = splitPaths[0];
            string root = string.Join(Path.DirectorySeparatorChar.ToString(), firstParts.Take(commonLength));
            if (root.Length == 2 && root[1] == ':')
                root += Path.DirectorySeparatorChar;

            return root;
        }

        private static void WriteFile(string filePath, List<string> content, string baseFolder, string commonRoot)
        {
            string relativePath = filePath;

            if (!string.IsNullOrEmpty(commonRoot))
            {
                // Remove common root from filePath
                if (filePath.StartsWith(commonRoot, StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = filePath.Substring(commonRoot.Length);
                    // Remove leading separator if present
                    if (relativePath.StartsWith(Path.DirectorySeparatorChar.ToString()) || relativePath.StartsWith(Path.AltDirectorySeparatorChar.ToString()))
                        relativePath = relativePath.Substring(1);
                }
                else
                {
                    relativePath = Path.GetFileName(filePath);
                }
            }
            else
            {
                relativePath = Path.GetFileName(filePath);
            }

            string outPath = Path.Combine(baseFolder, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(outPath));
            File.WriteAllText(outPath, string.Join(Environment.NewLine, content));
        }
    }
}
