using System;
using System.Collections.Generic;
using System.IO;

namespace FileContentToolkit
{
    public static class FileRecreator
    {
        /// <summary>
        /// Parses the output and recreates files in the target folder.
        /// </summary>
        /// <param name="output">The output text (file paths and contents).</param>
        /// <param name="baseFolder">The folder to create files in.</param>
        /// <param name="originalBaseFolder">The original base folder (for relative paths).</param>
        /// <returns>The number of files created.</returns>
        public static int RecreateFilesFromOutput(string output, string baseFolder, string originalBaseFolder)
        {
            var lines = output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            string currentFile = null;
            List<string> content = new List<string>();
            int fileCount = 0;

            foreach (var line in lines)
            {
                // Detect file path line (ends with ':', and looks like a path)
                if (line.EndsWith(":") && (line.Length > 2 && (line[1] == ':' || line.StartsWith(".\\"))))
                {
                    if (currentFile != null)
                    {
                        WriteFile(currentFile, content, baseFolder, originalBaseFolder);
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
                WriteFile(currentFile, content, baseFolder, originalBaseFolder);
                fileCount++;
            }
            return fileCount;
        }

        private static void WriteFile(string filePath, List<string> content, string baseFolder, string originalBaseFolder)
        {
            // Use relative path if possible
            string relativePath = filePath;
            if (!string.IsNullOrEmpty(originalBaseFolder) && Path.IsPathRooted(filePath))
            {
                try
                {
                    relativePath = Path.GetRelativePath(originalBaseFolder, filePath);
                }
                catch
                {
                    relativePath = Path.GetFileName(filePath);
                }
            }
            string outPath = Path.Combine(baseFolder, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(outPath));
            File.WriteAllText(outPath, string.Join(Environment.NewLine, content));
        }
    }
}