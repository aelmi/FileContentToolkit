using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Settings
{
    /// <summary>Serialisable window rectangle. See <see cref="AppSettings.WindowBounds"/>.</summary>
    public sealed class WindowBoundsSetting
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

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
        public long MaxFileSizeBytes { get; set; } // 0 = unlimited
        public bool SkipBinaryFiles { get; set; } = true;
        public bool AutoDetectEncoding { get; set; } = true;
        public bool UseGitIgnoreFiles { get; set; } = true;

        /// <summary>
        /// Off by default and deliberately separate from <see cref="UseGitIgnoreFiles"/>: merging
        /// .dockerignore into the gitignore rule set silently excluded every file in ordinary
        /// Node and .NET repositories.
        /// </summary>
        public bool UseDockerIgnoreFiles { get; set; }

        public bool WatchFolderForChanges { get; set; } // explicit default off

        // Search toggles
        public bool RegexSearch { get; set; }
        public bool CaseSensitiveSearch { get; set; }
        public bool WholeWordSearch { get; set; }

        // Appearance

        /// <summary>
        /// The selected theme. Replaces the old <c>DarkMode</c> boolean, which could not express
        /// "follow the system".
        /// </summary>
        public ThemeMode Mode { get; set; } = ThemeMode.Light;

        /// <summary>
        /// The pre-token appearance setting, retained for one-way migration only.
        /// </summary>
        /// <remarks>
        /// Serialised so that an existing settings file still binds to it on load; migration to
        /// <see cref="Mode"/> happens in <see cref="Load(string)"/> and the property is then
        /// cleared, so it is written back as null and disappears from the file on the next save.
        /// Existing users keep the dark mode they chose.
        /// </remarks>
        [JsonPropertyName("DarkMode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? LegacyDarkMode { get; set; }

        // Shell layout and placement.
        //
        // All four are additive with defaults, so a settings file written by an earlier build
        // binds cleanly and simply gets the defaults for these.

        /// <summary>
        /// Restored window position, or null on first run. Stored as four ints rather than a
        /// <see cref="System.Drawing.Rectangle"/> because that type serialises its computed
        /// Location/Size properties as well as X/Y/Width/Height and round-trips badly.
        /// </summary>
        public WindowBoundsSetting? WindowBounds { get; set; }

        /// <summary>
        /// Restored window state. Minimised is deliberately never persisted — see
        /// <c>MainForm.SaveWindowPlacement</c>; restoring to a minimised window looks like a
        /// failure to launch.
        /// </summary>
        public FormWindowState WindowState { get; set; } = FormWindowState.Normal;

        /// <summary>Splitter position in logical pixels. 0 means "use the design default".</summary>
        public int SplitterDistance { get; set; }

        /// <summary>Set once the user has dismissed or acted on the first-run state.</summary>
        public bool HasCompletedFirstRun { get; set; }

        // Trust, budgeting and templates.
        //
        // Additive with defaults like the shell settings above, so a file written by an earlier
        // build binds cleanly. The two secret settings default ON: the failure they prevent is a
        // production credential in a third-party chat transcript, which is not recoverable by
        // changing a setting afterwards.

        /// <summary>Replace detected credentials with a redaction marker before the pack leaves the app.</summary>
        public bool RedactSecrets { get; set; } = true;

        /// <summary>Show the review dialog when a copy or export would carry a detected credential.</summary>
        public bool WarnOnSecrets { get; set; } = true;

        /// <summary>
        /// Which context window the token gauge measures against. A literal rather than
        /// <c>TokenBudget.Claude.Id</c> so that a field initialiser here cannot depend on another
        /// type's static construction order; <c>TokenBudget.Resolve</c> falls back to the same
        /// model for any unrecognised value.
        /// </summary>
        public string TokenModelId { get; set; } = "claude";

        /// <summary>The window used when <see cref="TokenModelId"/> is the custom entry.</summary>
        public int CustomTokenBudget { get; set; }

        /// <summary>
        /// The user's prompt-template library. Empty on an older settings file and re-seeded with
        /// the two built-ins by <see cref="PromptTemplateStore.Load"/>.
        /// </summary>
        public List<PromptTemplate> PromptTemplates { get; set; } = new();

        /// <summary>Folds a legacy <c>DarkMode</c> boolean into <see cref="Mode"/>.</summary>
        private void MigrateLegacyAppearance()
        {
            if (LegacyDarkMode is bool dark)
            {
                Mode = dark ? ThemeMode.Dark : ThemeMode.Light;
                LegacyDarkMode = null;
            }
        }

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
                    "CodeShuttle");
                return Path.Combine(dir, "settings.json");
            }
        }

        /// <summary>
        /// Settings path used before the FileContentToolkit -> CodeShuttle rename.
        /// Retained solely so existing installs can be migrated once.
        /// </summary>
        private static string LegacySettingsPath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FileContentToolkit",
                "settings.json");

        /// <summary>
        /// One-time migration for the rename: if no CodeShuttle settings file exists but the
        /// old FileContentToolkit one does, copy it across so existing users keep their presets
        /// and recent folders. Best-effort; the original is left in place untouched.
        /// </summary>
        private static void MigrateLegacySettingsIfNeeded()
        {
            try
            {
                var path = SettingsPath;
                if (File.Exists(path)) return;

                var legacy = LegacySettingsPath;
                if (!File.Exists(legacy)) return;

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.Copy(legacy, path, overwrite: false);
            }
            catch
            {
                // Best-effort: a failed migration must never stop the app starting.
            }
        }

        /// <summary>
        /// Set when the settings file existed but could not be read. Silently returning defaults
        /// looks to the user exactly like "the app lost all my presets", so callers should show
        /// this rather than say nothing.
        /// </summary>
        public static string? LastLoadError { get; private set; }

        public static AppSettings Load() => Load(SettingsPath);

        internal static AppSettings Load(string path)
        {
            LastLoadError = null;
            try
            {
                if (path == SettingsPath) MigrateLegacySettingsIfNeeded();
                if (!File.Exists(path)) return new AppSettings();

                var json = File.ReadAllText(path);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json, JsonOpts);
                if (loaded != null)
                {
                    loaded.MigrateLegacyAppearance();
                    return loaded;
                }

                LastLoadError = "The settings file was empty, so defaults were restored.";
                return new AppSettings();
            }
            catch (JsonException ex)
            {
                // Keep the damaged file rather than overwriting it on the next save: it may be
                // the user's only copy of their presets.
                var quarantine = QuarantineBadSettings(path);
                LastLoadError = quarantine == null
                    ? $"The settings file could not be read ({ex.Message}), so defaults were restored."
                    : $"The settings file could not be read ({ex.Message}). It has been kept as '{quarantine}' and defaults were restored.";
                return new AppSettings();
            }
            catch (Exception ex)
            {
                LastLoadError = $"The settings file could not be read ({ex.Message}), so defaults were restored.";
                return new AppSettings();
            }
        }

        private static string? QuarantineBadSettings(string path)
        {
            try
            {
                var bad = path + ".bad";
                if (File.Exists(bad)) File.Delete(bad);
                File.Move(path, bad);
                return bad;
            }
            catch
            {
                return null;
            }
        }

        public void Save() => Save(SettingsPath);

        internal void Save(string path)
        {
            try
            {
                // Non-atomic writes meant a crash mid-write left truncated JSON, and Load() then
                // quietly returned defaults — every preset and recent folder gone.
                AtomicFile.WriteAllText(path, JsonSerializer.Serialize(this, JsonOpts), new UTF8Encoding(false));
            }
            catch
            {
                // swallow — settings are best-effort
            }
        }

        // -------------- debounced saving --------------
        // Saving synchronously on the UI thread after every search, every theme toggle and every
        // find/replace was a file write per keystroke-ish interaction.

        private readonly object _saveGate = new();
        private System.Threading.Timer? _saveTimer;

        public void SaveDebounced(int delayMs = 750)
        {
            lock (_saveGate)
            {
                _saveTimer ??= new System.Threading.Timer(_ => Save(), null, Timeout.Infinite, Timeout.Infinite);
                _saveTimer.Change(delayMs, Timeout.Infinite);
            }
        }

        /// <summary>Cancels any pending debounced save and writes immediately. Call before exit.</summary>
        public void FlushPendingSave()
        {
            lock (_saveGate)
            {
                _saveTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            }
            Save();
        }
    }
}
