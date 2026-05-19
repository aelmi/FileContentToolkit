using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FileContentToolkit.Settings
{
    public class Preset
    {
        public string Name { get; set; } = "";
        public string FolderPath { get; set; } = "";
        public List<string> Extensions { get; set; } = new();
        public List<string> IgnorePatterns { get; set; } = new();
        public bool IncludeSubfolders { get; set; } = true;
    }

    public class AppSettings
    {
        private const int MaxRecents = 15;

        public List<string> RecentFolders { get; set; } = new();
        public List<string> RecentSearches { get; set; } = new();
        public List<Preset> Presets { get; set; } = new();

        // Filters
        public long MaxFileSizeBytes { get; set; } = 0; // 0 = unlimited
        public bool SkipBinaryFiles { get; set; } = true;
        public bool AutoDetectEncoding { get; set; } = true;
        public bool UseGitIgnoreFiles { get; set; } = true;
        public bool WatchFolderForChanges { get; set; } = false; // explicit default off

        // Search toggles
        public bool RegexSearch { get; set; } = false;
        public bool CaseSensitiveSearch { get; set; } = false;
        public bool WholeWordSearch { get; set; } = false;

        // Appearance
        public bool DarkMode { get; set; } = false;

        public void AddRecentFolder(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder)) return;
            AddRecent(RecentFolders, folder, StringComparer.OrdinalIgnoreCase);
        }

        public void AddRecentSearch(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return;
            AddRecent(RecentSearches, term, StringComparer.Ordinal);
        }

        private static void AddRecent(List<string> list, string value, StringComparer comparer)
        {
            list.RemoveAll(x => comparer.Equals(x, value));
            list.Insert(0, value);
            while (list.Count > MaxRecents)
                list.RemoveAt(list.Count - 1);
        }

        // -------------- persistence --------------

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static string SettingsPath
        {
            get
            {
                var dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FileContentToolkit");
                return Path.Combine(dir, "settings.json");
            }
        }

        public static AppSettings Load()
        {
            try
            {
                var path = SettingsPath;
                if (!File.Exists(path)) return new AppSettings();
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<AppSettings>(json, JsonOpts) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void Save()
        {
            try
            {
                var path = SettingsPath;
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllText(path, JsonSerializer.Serialize(this, JsonOpts));
            }
            catch
            {
                // swallow — settings are best-effort
            }
        }
    }
}
