using System;
using System.Linq;
using System.Windows.Forms;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// The project-type catalogue and the menus built from it.
    /// </summary>
    /// <remarks>
    /// The catalogue replaced a flat list of sixteen languages. A language is not what anyone has
    /// — they have a WinForms app or a Django site — and the old "C# project" entry proved the
    /// point by offering Razor extensions to desktop developers and no XAML to WPF ones.
    /// </remarks>
    /// <remarks>
    /// Serialised with the other tests that construct <c>MainForm</c>. The form touches process-wide
    /// state — the theme manager, the settings file — so two of them being built on different STA
    /// threads at once fails intermittently and in whichever class happens to lose the race.
    /// </remarks>
    [Collection(AppSettingsCollection.Name)]
    public class ProjectPresetTests
    {
        [Fact]
        public void Every_extension_is_well_formed()
        {
            foreach (var preset in ProjectPresets.All)
            {
                Assert.NotEmpty(preset.Extensions);
                foreach (var ext in preset.Extensions)
                {
                    Assert.StartsWith(".", ext, StringComparison.Ordinal);
                    Assert.DoesNotContain(" ", ext, StringComparison.Ordinal);
                    Assert.True(ext.Length > 1, $"'{ext}' in {preset.Name} is just a dot");
                }
            }
        }

        [Fact]
        public void Preset_names_are_unique_within_a_group()
        {
            foreach (var group in ProjectPresets.ByGroup)
            {
                var duplicates = group
                    .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                Assert.True(duplicates.Count == 0,
                    $"{group.Key} has duplicate entries: {string.Join(", ", duplicates)}");
            }
        }

        [Fact]
        public void No_preset_repeats_an_extension()
        {
            foreach (var preset in ProjectPresets.All)
            {
                var duplicates = preset.Extensions
                    .GroupBy(e => e, StringComparer.OrdinalIgnoreCase)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                Assert.True(duplicates.Count == 0,
                    $"{preset.Name} repeats {string.Join(", ", duplicates)}");
            }
        }

        /// <summary>
        /// The specific failure that motivated the rewrite: one "C# project" entry could not serve
        /// both a desktop app and a web app, and served neither well.
        /// </summary>
        [Fact]
        public void Dotnet_presets_are_differentiated_by_ui_stack()
        {
            var winforms = Find("C# WinForms");
            var wpf = Find("C# WPF");
            var web = Find("ASP.NET Core");

            Assert.Contains(".resx", winforms.Extensions);
            Assert.DoesNotContain(".xaml", winforms.Extensions);

            Assert.Contains(".xaml", wpf.Extensions);
            Assert.DoesNotContain(".cshtml", wpf.Extensions);

            Assert.Contains(".cshtml", web.Extensions);
            Assert.DoesNotContain(".xaml", web.Extensions);
        }

        [Fact]
        public void Stacks_with_heavy_build_output_carry_ignore_rules()
        {
            Assert.Contains("obj/", Find("C# WinForms").Ignore);
            Assert.Contains("node_modules/", Find("TypeScript / React").Ignore);
            Assert.Contains("__pycache__/", Find("Python").Ignore);
            Assert.Contains("target/", Find("Rust").Ignore);
        }

        /// <summary>
        /// Selecting a project type replaces the extension list but merges the ignore list: the
        /// first is a statement about what the project is, the second is usually hand-tuned and
        /// must not be silently discarded.
        /// </summary>
        [Fact]
        public void Selecting_a_project_type_replaces_extensions_and_merges_ignores()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    var ignoreBox = (TextBox)FindByName(form, "txtIgnorePatterns");
                    ignoreBox.Text = "my-own-rule/";

                    var chips = (CodeShuttle.Controls.ChipList)FindByName(form, "chipExtensions");
                    ClickPreset(form, ".NET", "C# WPF");

                    var wpf = Find("C# WPF");
                    Assert.Equal(wpf.Extensions.OrderBy(x => x), chips.Items.OrderBy(x => x));

                    Assert.Contains("my-own-rule/", ignoreBox.Text);
                    Assert.Contains("obj/", ignoreBox.Text);
                }
                finally { form.Hide(); }
            });
        }

        [Fact]
        public void Project_catalogue_is_reachable_from_the_presets_menu()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                var presets = MenuOf(form, "cmsPresets");
                var projectType = presets.Items.Cast<ToolStripItem>()
                    .OfType<ToolStripMenuItem>()
                    .FirstOrDefault(i => i.Text == "Project type");

                Assert.True(projectType != null,
                    "'Project type' is missing from the Presets menu, which is where it is looked for.");
                Assert.NotEmpty(projectType!.DropDownItems);
            });
        }

        // ---------------------------------------------------------------- helpers

        private static ProjectPreset Find(string name) =>
            ProjectPresets.All.Single(p => p.Name == name);

        private static void ClickPreset(Form form, string group, string name)
        {
            var menu = MenuOf(form, "cmsPresets");
            var projectType = menu.Items.Cast<ToolStripItem>()
                .OfType<ToolStripMenuItem>()
                .Single(i => i.Text == "Project type");

            var groupItem = projectType.DropDownItems.Cast<ToolStripItem>()
                .OfType<ToolStripMenuItem>()
                .Single(i => i.Text == group);

            groupItem.DropDownItems.Cast<ToolStripItem>()
                .OfType<ToolStripMenuItem>()
                .Single(i => i.Text == name)
                .PerformClick();
        }

        private static ContextMenuStrip MenuOf(Form form, string fieldName)
        {
            var field = typeof(MainForm).GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.True(field != null, $"field '{fieldName}' not found on MainForm");
            return (ContextMenuStrip)field!.GetValue(form)!;
        }

        private static Control FindByName(Control root, string name)
        {
            foreach (Control child in root.Controls)
            {
                if (child.Name == name) return child;
                var deeper = FindByNameOrNull(child, name);
                if (deeper != null) return deeper;
            }
            throw new Xunit.Sdk.XunitException($"control '{name}' not found");
        }

        private static Control? FindByNameOrNull(Control root, string name)
        {
            foreach (Control child in root.Controls)
            {
                if (child.Name == name) return child;
                var deeper = FindByNameOrNull(child, name);
                if (deeper != null) return deeper;
            }
            return null;
        }
    }
}
