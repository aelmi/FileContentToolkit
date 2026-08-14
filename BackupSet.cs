using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace CodeShuttle
{
    /// <summary>One file that was copied aside before being overwritten.</summary>
    public sealed class BackupEntry
    {
        public string TargetPath { get; set; } = "";
        public string BackupRelativePath { get; set; } = "";
    }

    /// <summary>Serialisable description of a backup set, written alongside the copies.</summary>
    public sealed class BackupManifest
    {
        public string CreatedUtc { get; set; } = "";
        public string TargetRoot { get; set; } = "";
        public List<BackupEntry> Entries { get; set; } = new();
    }

    /// <summary>
    /// A timestamped snapshot of every file an apply is about to overwrite, kept under
    /// %APPDATA%\CodeShuttle\backups\ together with a manifest so a later "undo last apply"
    /// has everything it needs. New files (nothing on disk yet) are not copied but are still
    /// recorded, so an undo can delete them.
    /// </summary>
    public sealed class BackupSet
    {
        private readonly BackupManifest _manifest = new();

        public string Directory { get; }

        public IReadOnlyList<BackupEntry> Entries => _manifest.Entries;

        private BackupSet(string directory, string targetRoot)
        {
            Directory = directory;
            _manifest.CreatedUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
            _manifest.TargetRoot = targetRoot;
        }

        public static string BackupsRoot => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CodeShuttle", "backups");

        /// <summary>Creates a new timestamped backup folder. Collisions get a numeric suffix.</summary>
        public static BackupSet Create(string targetRoot) => Create(targetRoot, BackupsRoot);

        public static BackupSet Create(string targetRoot, string backupsRoot)
        {
            var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
            var dir = Path.Combine(backupsRoot, stamp);
            int suffix = 1;
            while (System.IO.Directory.Exists(dir))
                dir = Path.Combine(backupsRoot, $"{stamp}-{suffix++}");
            System.IO.Directory.CreateDirectory(dir);
            return new BackupSet(dir, targetRoot);
        }

        /// <summary>
        /// Copies <paramref name="targetPath"/> into the backup set, preserving its position
        /// relative to the target root. Safe to call for files that do not exist yet.
        /// </summary>
        public void Backup(string targetPath)
        {
            string relative;
            try
            {
                relative = Path.GetRelativePath(_manifest.TargetRoot, targetPath);
            }
            catch
            {
                relative = Path.GetFileName(targetPath);
            }

            if (relative.StartsWith("..", StringComparison.Ordinal) || Path.IsPathRooted(relative))
                relative = Path.GetFileName(targetPath);

            if (File.Exists(targetPath))
            {
                var destination = Path.Combine(Directory, relative);
                var destinationDir = Path.GetDirectoryName(destination);
                if (!string.IsNullOrEmpty(destinationDir)) System.IO.Directory.CreateDirectory(destinationDir);
                File.Copy(targetPath, destination, overwrite: true);
                _manifest.Entries.Add(new BackupEntry { TargetPath = targetPath, BackupRelativePath = relative });
            }
            else
            {
                // Recorded with no copy: the undo action for this one is "delete".
                _manifest.Entries.Add(new BackupEntry { TargetPath = targetPath, BackupRelativePath = "" });
            }
        }

        public string ManifestPath => Path.Combine(Directory, "manifest.json");

        // Cached rather than constructed per write: JsonSerializerOptions builds and caches
        // reflection metadata, which a fresh instance per call discards (CA1869).
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public void WriteManifest()
        {
            var json = JsonSerializer.Serialize(_manifest, _jsonOptions);
            AtomicFile.WriteAllText(ManifestPath, json, new System.Text.UTF8Encoding(false));
        }
    }
}
