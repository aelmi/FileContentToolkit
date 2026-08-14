using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace CodeShuttle.Settings
{
    /// <summary>
    /// Whole-settings export and import: presets, prompt templates, filters and appearance in one
    /// file, so a team can share a configuration and a reinstall is not a fresh start.
    /// </summary>
    /// <remarks>
    /// Deliberately excludes window geometry and the first-run flag. Those describe one machine's
    /// monitors, and importing them is how a shared configuration file puts somebody else's
    /// window off the edge of your screen.
    /// </remarks>
    public static class SettingsTransfer
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
        };

        public const string FileFilter = "CodeShuttle settings (*.json)|*.json|All files (*.*)|*.*";
        public const string DefaultFileName = "codeshuttle-settings.json";

        /// <summary>Serialises the portable subset of <paramref name="settings"/>.</summary>
        public static string Export(AppSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            return JsonSerializer.Serialize(Portable.From(settings), Options);
        }

        public static void ExportToFile(AppSettings settings, string path)
        {
            // Through AtomicFile so a failure part-way cannot leave a truncated file that the
            // user later imports and loses their presets to.
            AtomicFile.WriteAllText(path, Export(settings), new UTF8Encoding(false));
        }

        /// <summary>
        /// Applies an exported document onto <paramref name="target"/>.
        /// </summary>
        /// <exception cref="InvalidDataException">The document is not readable settings.</exception>
        public static void Import(AppSettings target, string json)
        {
            ArgumentNullException.ThrowIfNull(target);

            Portable? portable;
            try
            {
                portable = JsonSerializer.Deserialize<Portable>(json, Options);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("The file is not a valid CodeShuttle settings file.", ex);
            }

            if (portable == null)
                throw new InvalidDataException("The settings file was empty.");

            portable.ApplyTo(target);
        }

        public static void ImportFromFile(AppSettings target, string path) =>
            Import(target, File.ReadAllText(path));

        /// <summary>
        /// The exported shape. A dedicated type rather than reusing <see cref="AppSettings"/> so
        /// that adding a machine-specific setting later cannot accidentally start travelling
        /// between machines.
        /// </summary>
        private sealed class Portable
        {
            public int SchemaVersion { get; set; } = 1;

            public List<string> RecentFolders { get; set; } = new();
            public List<string> RecentSearches { get; set; } = new();
            public List<Preset> Presets { get; set; } = new();
            public List<PromptTemplate> PromptTemplates { get; set; } = new();

            public long MaxFileSizeBytes { get; set; }
            public bool SkipBinaryFiles { get; set; } = true;
            public bool AutoDetectEncoding { get; set; } = true;
            public bool UseGitIgnoreFiles { get; set; } = true;
            public bool UseDockerIgnoreFiles { get; set; }
            public bool WatchFolderForChanges { get; set; }

            public bool RegexSearch { get; set; }
            public bool CaseSensitiveSearch { get; set; }
            public bool WholeWordSearch { get; set; }

            public Theming.ThemeMode Mode { get; set; } = Theming.ThemeMode.Light;

            public bool RedactSecrets { get; set; } = true;
            public bool WarnOnSecrets { get; set; } = true;
            public string TokenModelId { get; set; } = TokenBudget.Claude.Id;
            public int CustomTokenBudget { get; set; }

            public static Portable From(AppSettings s) => new()
            {
                RecentFolders = new List<string>(s.RecentFolders),
                RecentSearches = new List<string>(s.RecentSearches),
                Presets = new List<Preset>(s.Presets),
                PromptTemplates = new List<PromptTemplate>(s.PromptTemplates),
                MaxFileSizeBytes = s.MaxFileSizeBytes,
                SkipBinaryFiles = s.SkipBinaryFiles,
                AutoDetectEncoding = s.AutoDetectEncoding,
                UseGitIgnoreFiles = s.UseGitIgnoreFiles,
                UseDockerIgnoreFiles = s.UseDockerIgnoreFiles,
                WatchFolderForChanges = s.WatchFolderForChanges,
                RegexSearch = s.RegexSearch,
                CaseSensitiveSearch = s.CaseSensitiveSearch,
                WholeWordSearch = s.WholeWordSearch,
                Mode = s.Mode,
                RedactSecrets = s.RedactSecrets,
                WarnOnSecrets = s.WarnOnSecrets,
                TokenModelId = s.TokenModelId,
                CustomTokenBudget = s.CustomTokenBudget,
            };

            public void ApplyTo(AppSettings s)
            {
                s.RecentFolders.Clear();
                s.RecentFolders.AddRange(RecentFolders);

                s.RecentSearches.Clear();
                s.RecentSearches.AddRange(RecentSearches);

                s.Presets.Clear();
                s.Presets.AddRange(Presets);

                s.PromptTemplates.Clear();
                s.PromptTemplates.AddRange(PromptTemplates);

                s.MaxFileSizeBytes = MaxFileSizeBytes;
                s.SkipBinaryFiles = SkipBinaryFiles;
                s.AutoDetectEncoding = AutoDetectEncoding;
                s.UseGitIgnoreFiles = UseGitIgnoreFiles;
                s.UseDockerIgnoreFiles = UseDockerIgnoreFiles;
                s.WatchFolderForChanges = WatchFolderForChanges;

                s.RegexSearch = RegexSearch;
                s.CaseSensitiveSearch = CaseSensitiveSearch;
                s.WholeWordSearch = WholeWordSearch;

                s.Mode = Mode;

                s.RedactSecrets = RedactSecrets;
                s.WarnOnSecrets = WarnOnSecrets;
                s.TokenModelId = TokenModelId;
                s.CustomTokenBudget = CustomTokenBudget;
            }
        }
    }
}
