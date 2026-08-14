using System;
using System.IO;
using System.Linq;
using CodeShuttle.Settings;
using CodeShuttle.Theming;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// Load() swallowed every exception and returned defaults, so a corrupt file silently wiped
    /// every preset the user had.
    /// </summary>
    [Collection(AppSettingsCollection.Name)]
    public class AppSettingsTests
    {
        [Theory]
        [InlineData(true, ThemeMode.Dark)]
        [InlineData(false, ThemeMode.Light)]
        public void LegacyDarkModeBooleanMigratesToThemeMode(bool darkMode, ThemeMode expected)
        {
            // The appearance setting used to be a bool, which could not express "follow the
            // system". Existing users must keep the theme they chose across the change.
            using var temp = new TempDir();
            var path = temp.File("settings.json");
            File.WriteAllText(path, $"{{ \"DarkMode\": {darkMode.ToString().ToLowerInvariant()} }}");

            var loaded = AppSettings.Load(path);

            Assert.Equal(expected, loaded.Mode);
            Assert.Null(loaded.LegacyDarkMode);
        }

        [Fact]
        public void MigrationIsOneWayAndDropsTheLegacyKeyOnSave()
        {
            using var temp = new TempDir();
            var path = temp.File("settings.json");
            File.WriteAllText(path, "{ \"DarkMode\": true }");

            var loaded = AppSettings.Load(path);
            loaded.Save(path);

            var json = File.ReadAllText(path);
            Assert.DoesNotContain("DarkMode", json);
            Assert.Equal(ThemeMode.Dark, AppSettings.Load(path).Mode);
        }

        [Fact]
        public void ModeSurvivesARoundTrip()
        {
            using var temp = new TempDir();
            var path = temp.File("settings.json");
            new AppSettings { Mode = ThemeMode.Dark }.Save(path);
            Assert.Equal(ThemeMode.Dark, AppSettings.Load(path).Mode);
        }

        [Fact]
        public void RoundTripPreservesPresets()
        {
            using var temp = new TempDir();
            var path = temp.File("settings.json");

            var settings = new AppSettings();
            settings.Presets.Add(new Preset
            {
                Name = "Python",
                FolderPath = @"C:\proj",
                Extensions = { ".py", ".pyi" },
                IgnorePatterns = { "__pycache__/" },
                IncludeSubfolders = false
            });
            settings.MaxFileSizeBytes = 4096;
            settings.UseDockerIgnoreFiles = true;
            settings.Save(path);

            var loaded = AppSettings.Load(path);

            var preset = Assert.Single(loaded.Presets);
            Assert.Equal("Python", preset.Name);
            Assert.Equal(new[] { ".py", ".pyi" }, preset.Extensions);
            Assert.Equal(new[] { "__pycache__/" }, preset.IgnorePatterns);
            Assert.False(preset.IncludeSubfolders);
            Assert.Equal(4096, loaded.MaxFileSizeBytes);
            Assert.True(loaded.UseDockerIgnoreFiles);
        }

        [Fact]
        public void MissingFileReturnsDefaultsWithoutAnError()
        {
            using var temp = new TempDir();

            var loaded = AppSettings.Load(temp.File("nope.json"));

            Assert.Empty(loaded.Presets);
            Assert.Null(AppSettings.LastLoadError);
        }

        /// <summary>
        /// P2-4: malformed JSON must not throw, must not be overwritten, and must be reported —
        /// silently returning defaults is indistinguishable from losing all the user's data.
        /// </summary>
        [Fact]
        public void MalformedJsonReturnsDefaultsAndQuarantinesTheFile()
        {
            using var temp = new TempDir();
            var path = temp.File("settings.json");
            File.WriteAllText(path, "{ this is not valid json");

            var loaded = AppSettings.Load(path);

            Assert.Empty(loaded.Presets);
            Assert.NotNull(AppSettings.LastLoadError);
            Assert.True(File.Exists(path + ".bad"), "the damaged file should be kept, not discarded");
            Assert.False(File.Exists(path));
        }

        [Fact]
        public void SaveIsAtomicAndLeavesNoTempFilesBehind()
        {
            using var temp = new TempDir();
            var path = temp.File("settings.json");

            new AppSettings().Save(path);

            Assert.True(File.Exists(path));
            Assert.Empty(Directory.GetFiles(temp.Path, "*.cstmp"));
        }

        [Fact]
        public void RecentFoldersDedupeCaseInsensitively()
        {
            var settings = new AppSettings();
            settings.AddRecentFolder(@"C:\Proj");
            settings.AddRecentFolder(@"c:\proj");

            Assert.Single(settings.RecentFolders);
            Assert.Equal(@"c:\proj", settings.RecentFolders[0]);
        }

        /// <summary>
        /// The asymmetry is intentional and pinned here: folders are a case-insensitive
        /// filesystem concept, search terms are literal text the user typed.
        /// </summary>
        [Fact]
        public void RecentSearchesDedupeCaseSensitively()
        {
            var settings = new AppSettings();
            settings.AddRecentSearch("Foo");
            settings.AddRecentSearch("foo");

            Assert.Equal(2, settings.RecentSearches.Count);
        }

        [Fact]
        public void RecentListsAreCappedAtFifteen()
        {
            var settings = new AppSettings();
            for (int i = 0; i < 30; i++) settings.AddRecentFolder($@"C:\folder{i}");

            Assert.Equal(15, settings.RecentFolders.Count);
            Assert.Equal(@"C:\folder29", settings.RecentFolders[0]); // most recent first
        }

        [Fact]
        public void BlankRecentEntriesAreIgnored()
        {
            var settings = new AppSettings();
            settings.AddRecentFolder("");
            settings.AddRecentFolder("   ");
            settings.AddRecentSearch(null!);

            Assert.Empty(settings.RecentFolders);
            Assert.Empty(settings.RecentSearches);
        }
    }
}
