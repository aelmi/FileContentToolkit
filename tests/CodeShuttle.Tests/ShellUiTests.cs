using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeShuttle.Dialogs;
using CodeShuttle.Settings;
using CodeShuttle.UI;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// Structural checks over the shell and dialog work: keyboard bindings, control ordering that
    /// UI Automation depends on, and the one layout overlap that made a form's only feedback
    /// channel invisible.
    /// </summary>
    /// <remarks>
    /// These instantiate real forms but never show them, so no message loop and no interactive
    /// window is involved. <c>STAThread</c> is required because WinForms controls create their
    /// handles on an apartment-threaded COM context.
    /// </remarks>
    [Collection(AppSettingsCollection.Name)]
    public class ShellUiTests
    {
        // ---------------------------------------------------------------- shortcuts

        [Fact]
        public void Shortcut_table_declares_at_least_ten_distinct_chords()
        {
            var distinct = Shortcuts.All.Select(b => b.Keys).Distinct().Count();
            Assert.True(distinct >= 10, $"expected >= 10 distinct chords, found {distinct}");
        }

        [Fact]
        public void Every_shortcut_has_an_action_and_a_description()
        {
            Assert.All(Shortcuts.All, b =>
            {
                Assert.False(string.IsNullOrWhiteSpace(b.Action));
                Assert.False(string.IsNullOrWhiteSpace(b.Description));
            });
        }

        [Theory]
        [InlineData(Keys.Control | Keys.O, "Ctrl+O")]
        [InlineData(Keys.Control | Keys.Shift | Keys.O, "Ctrl+Shift+O")]
        [InlineData(Keys.F5, "F5")]
        [InlineData(Keys.Control | Keys.Oemcomma, "Ctrl+,")]
        [InlineData(Keys.Escape, "Esc")]
        [InlineData(Keys.Delete, "Del")]
        public void Chords_format_the_way_Windows_writes_them(Keys keys, string expected)
        {
            Assert.Equal(expected, Shortcuts.Format(keys));
        }

        /// <summary>
        /// The help window used to keep its own list, which had already drifted into two
        /// identical Ctrl+F / Ctrl+H rows. It now renders this table, so duplicates here would
        /// come straight back out on screen.
        /// </summary>
        [Fact]
        public void Shortcut_actions_are_not_duplicated_for_the_same_chord()
        {
            var duplicated = Shortcuts.All
                .GroupBy(b => b.Keys)
                .Where(g => g.Count() > 1)
                .Select(g => Shortcuts.Format(g.Key))
                .ToList();

            Assert.True(duplicated.Count == 0, "duplicate chords: " + string.Join(", ", duplicated));
        }

        // ---------------------------------------------------------------- settings

        [Fact]
        public void New_shell_settings_are_additive_with_defaults()
        {
            var settings = new AppSettings();

            Assert.Null(settings.WindowBounds);
            Assert.Equal(FormWindowState.Normal, settings.WindowState);
            Assert.Equal(0, settings.SplitterDistance);
            Assert.False(settings.HasCompletedFirstRun);
        }

        [Fact]
        public void Shell_settings_round_trip_through_the_settings_file()
        {
            using var dir = new TempDir();
            var path = System.IO.Path.Combine(dir.Path, "settings.json");

            var saved = new AppSettings
            {
                WindowBounds = new WindowBoundsSetting { X = 120, Y = 80, Width = 1200, Height = 800 },
                WindowState = FormWindowState.Maximized,
                SplitterDistance = 415,
                HasCompletedFirstRun = true,
            };
            saved.Save(path);

            var loaded = AppSettings.Load(path);

            Assert.NotNull(loaded.WindowBounds);
            Assert.Equal(120, loaded.WindowBounds!.X);
            Assert.Equal(80, loaded.WindowBounds.Y);
            Assert.Equal(1200, loaded.WindowBounds.Width);
            Assert.Equal(800, loaded.WindowBounds.Height);
            Assert.Equal(FormWindowState.Maximized, loaded.WindowState);
            Assert.Equal(415, loaded.SplitterDistance);
            Assert.True(loaded.HasCompletedFirstRun);
        }

        /// <summary>A file written before these fields existed must still load.</summary>
        [Fact]
        public void Settings_file_without_the_shell_fields_still_loads()
        {
            using var dir = new TempDir();
            var path = System.IO.Path.Combine(dir.Path, "settings.json");
            System.IO.File.WriteAllText(path, "{ \"RecentFolders\": [ \"C:\\\\code\" ] }");

            var loaded = AppSettings.Load(path);

            Assert.Single(loaded.RecentFolders);
            Assert.Null(loaded.WindowBounds);
            Assert.Equal(0, loaded.SplitterDistance);
            Assert.False(loaded.HasCompletedFirstRun);
        }

        // ---------------------------------------------------------------- dialogs

        /// <summary>
        /// UI Automation infers a text box's name from the label that precedes it in the Controls
        /// collection. PromptDialog is the product's generic text-input primitive, so while the
        /// order was reversed every text entry in the application had an unnamed edit box.
        /// </summary>
        [Fact]
        public void PromptDialog_adds_its_label_before_its_input()
        {
            StaRunner.Run(() =>
            {
                using var dlg = new PromptDialog("Save preset", "Preset name:", "");
                var body = FindControl(dlg, "pnlBody");

                int label = IndexOfChild(body, "lblPrompt");
                int input = IndexOfChild(body, "txtInput");

                Assert.True(label >= 0 && input >= 0, "both controls must be present");
                Assert.True(label < input, $"lblPrompt (index {label}) must precede txtInput (index {input})");
            });
        }

        [Fact]
        public void PromptDialog_names_its_input_after_the_caller_supplied_prompt()
        {
            StaRunner.Run(() =>
            {
                using var dlg = new PromptDialog("Rename preset", "New name:", "");
                var input = FindControl(dlg, "txtInput");
                Assert.Equal("New name", input.AccessibleName);
            });
        }

        [Fact]
        public void PasswordDialog_adds_its_label_before_its_password_box()
        {
            StaRunner.Run(() =>
            {
                using var dlg = new PasswordDialog();
                var content = FindControl(dlg, "pnlContent");

                int label = IndexOfChild(content, "lblPrompt");
                int input = IndexOfChild(content, "txtPassword");

                Assert.True(label >= 0 && input >= 0, "both controls must be present");
                Assert.True(label < input, $"lblPrompt (index {label}) must precede txtPassword (index {input})");
            });
        }

        /// <summary>
        /// lblStatus sat at y=87 inside the button row's y=84..107 band and was added last, so
        /// the form's only feedback surface — including its only regex-error message — was drawn
        /// underneath the buttons.
        /// </summary>
        [Fact]
        public void FindReplaceForm_status_label_does_not_overlap_any_button()
        {
            StaRunner.Run(() =>
            {
                using var host = new RichTextBox();
                using var dlg = new FindReplaceForm(host, new List<string>());

                var status = FindControl(dlg, "lblStatus");
                var overlapping = new List<string>();

                foreach (var name in new[] { "btnNext", "btnPrev", "btnReplace", "btnReplaceAll" })
                {
                    var button = FindControl(dlg, name);
                    if (status.Bounds.IntersectsWith(button.Bounds)) overlapping.Add(name);
                }

                Assert.True(overlapping.Count == 0,
                    "lblStatus " + status.Bounds + " overlaps: " + string.Join(", ", overlapping));
            });
        }

        /// <summary>Enter in the file list used to write to disk.</summary>
        [Fact]
        public void DiffViewerForm_does_not_default_to_the_write_button()
        {
            StaRunner.Run(() =>
            {
                using var dlg = new DiffViewerForm(new List<FilePlan>());
                var write = FindControl(dlg, "btnWrite");

                Assert.NotSame(write, dlg.AcceptButton);
            });
        }

        [Fact]
        public void DiffViewerForm_wraps_the_diff_so_the_plus_minus_prefix_stays_visible()
        {
            StaRunner.Run(() =>
            {
                using var dlg = new DiffViewerForm(new List<FilePlan>());
                var diff = (RichTextBox)FindControl(dlg, "rtbDiff");

                Assert.True(diff.WordWrap);
            });
        }

        /// <summary>Escape did not close this modal at all — it had neither button set.</summary>
        [Fact]
        public void ExtensionCountsForm_can_be_dismissed_with_escape()
        {
            StaRunner.Run(() =>
            {
                var service = new FileContentService();
                using var dlg = new ExtensionCountsForm(service);

                Assert.NotNull(dlg.CancelButton);
                Assert.Equal(DialogResult.Cancel, dlg.CancelButton!.DialogResult);
                Assert.NotNull(dlg.AcceptButton);
            });
        }

        [Fact]
        public void PresetManagerForm_loads_the_selection_on_enter()
        {
            StaRunner.Run(() =>
            {
                var settings = new AppSettings();
                using var dlg = new PresetManagerForm(settings);

                var load = FindControl(dlg, "btnLoad");
                Assert.Same(load, dlg.AcceptButton);
            });
        }

        /// <summary>
        /// A Label raises no UI Automation event when its text is replaced, so the details pane
        /// was rebuilt on every selection change and never announced.
        /// </summary>
        [Fact]
        public void PresetManagerForm_details_pane_is_a_readable_text_box()
        {
            StaRunner.Run(() =>
            {
                var settings = new AppSettings();
                using var dlg = new PresetManagerForm(settings);

                var details = FindControl(dlg, "lblDetails");
                var box = Assert.IsType<TextBox>(details);
                Assert.True(box.ReadOnly);
                Assert.True(box.Multiline);
            });
        }

        [Fact]
        public void About_and_help_report_cancel_when_dismissed()
        {
            StaRunner.Run(() =>
            {
                using (var about = new AboutForm())
                {
                    Assert.NotNull(about.CancelButton);
                    Assert.Equal(DialogResult.Cancel, about.CancelButton!.DialogResult);
                }

                using (var help = new HelpForm())
                {
                    Assert.NotNull(help.CancelButton);
                    Assert.Equal(DialogResult.Cancel, help.CancelButton!.DialogResult);
                }
            });
        }

        // ---------------------------------------------------------------- across every form

        /// <summary>
        /// Builds one of each form. Done reflectively-by-hand rather than by scanning the assembly
        /// so that a form needing constructor arguments is supplied them explicitly and a new form
        /// added later fails loudly here rather than being silently skipped.
        /// </summary>
        /// <summary>
        /// Exposes <see cref="EveryForm"/> to the theme smoke test, so both suites build the same
        /// list and adding a form updates both at once.
        /// </summary>
        internal static IEnumerable<Form> EveryFormForTheming() => EveryForm();

        private static IEnumerable<Form> EveryForm()
        {
            yield return new MainForm();
            yield return new AboutForm();
            yield return new HelpForm();
            yield return new OptionsForm(new AppSettings());
            yield return new PromptDialog("t", "Value:", "");
            yield return new PasswordDialog();
            yield return new PresetManagerForm(new AppSettings());
            yield return new ExtensionCountsForm(new FileContentService());
            yield return new FolderTreePickerForm(System.IO.Path.GetTempPath());
            yield return new DiffViewerForm(new List<FilePlan>());
            yield return new FindReplaceForm(new RichTextBox(), new List<string>());

            // WS5's dialogs. Listed here deliberately: every structural rule the ten before them
            // are held to — one scale metric, no duplicate tab index within a container, an
            // Accept and a Cancel button — applies to these too.
            yield return new PasteResponseForm(System.IO.Path.GetTempPath());
            yield return new SecretWarningForm(new List<Filters.SecretMatch>(), redactByDefault: true);
            yield return new ExclusionRuleEditorForm(new List<string>(), "", new List<string>());
            yield return new PromptComposerForm(new AppSettings(), "");
            yield return new TokenBreakdownForm("", TokenBudget.Claude, 200_000);
        }

        /// <summary>
        /// Tab order is only meaningful within a container, so duplicates are checked per parent.
        /// A tie is resolved by z-order, which means the declared order and the actual order can
        /// disagree — exactly the kind of thing that works until someone reorders the Designer.
        /// </summary>
        [Fact]
        public void No_container_holds_two_controls_with_the_same_tab_index()
        {
            StaRunner.Run(() =>
            {
                var offenders = new List<string>();

                foreach (var form in EveryForm())
                {
                    using (form) CheckContainer(form, form.GetType().Name, offenders);
                }

                Assert.True(offenders.Count == 0,
                    "duplicate TabIndex within a container:\n  " + string.Join("\n  ", offenders));
            });

            static void CheckContainer(Control parent, string path, List<string> offenders)
            {
                var seen = new Dictionary<int, string>();
                foreach (Control child in parent.Controls)
                {
                    // Controls that cannot take focus are not part of the tab sequence, so a
                    // shared index on them is not a defect.
                    if (child.TabStop && child.CanSelect)
                    {
                        if (seen.TryGetValue(child.TabIndex, out var other))
                            offenders.Add($"{path}.{parent.Name}: {other} and {child.Name} both at TabIndex {child.TabIndex}");
                        else
                            seen[child.TabIndex] = child.Name;
                    }

                    CheckContainer(child, path, offenders);
                }
            }
        }

        /// <summary>
        /// Escape must dismiss every dialog. MainForm is exempt and is asserted to be exempt
        /// deliberately: it is the application window, and binding Escape there would close the
        /// program on a stray keystroke.
        /// </summary>
        [Fact]
        public void Every_dialog_can_be_dismissed_with_escape()
        {
            StaRunner.Run(() =>
            {
                var missing = new List<string>();

                foreach (var form in EveryForm())
                {
                    using (form)
                    {
                        if (form is MainForm)
                        {
                            Assert.Null(form.CancelButton);
                            continue;
                        }
                        if (form.CancelButton == null) missing.Add(form.GetType().Name);
                    }
                }

                Assert.True(missing.Count == 0, "no CancelButton on: " + string.Join(", ", missing));
            });
        }

        /// <summary>
        /// Every dialog should have a default action, with one deliberate exception: the diff
        /// viewer, whose only affirmative action writes over the user's files.
        /// </summary>
        [Fact]
        public void Every_dialog_has_a_default_button_except_the_destructive_one()
        {
            StaRunner.Run(() =>
            {
                var missing = new List<string>();

                foreach (var form in EveryForm())
                {
                    using (form)
                    {
                        if (form is MainForm or DiffViewerForm) continue;
                        if (form.AcceptButton == null) missing.Add(form.GetType().Name);
                    }
                }

                Assert.True(missing.Count == 0, "no AcceptButton on: " + string.Join(", ", missing));
            });
        }

        /// <summary>
        /// The scaling normalisation is only honest while every form declares the same metric,
        /// and opening the Visual Studio designer silently resaves it at the developer's local
        /// DPI. This is the cheap permanent guard against that.
        /// </summary>
        [Fact]
        public void Every_form_declares_the_same_auto_scale_metric()
        {
            StaRunner.Run(() =>
            {
                var wrong = new List<string>();

                foreach (var form in EveryForm())
                {
                    using (form)
                    {
                        if (form.AutoScaleDimensions != new SizeF(7F, 15F))
                            wrong.Add($"{form.GetType().Name}: {form.AutoScaleDimensions}");
                    }
                }

                Assert.True(wrong.Count == 0, "expected (7, 15) but found:\n  " + string.Join("\n  ", wrong));
            });
        }

        /// <summary>
        /// The old constraint demanded roughly 1840x1304 physical pixels at 200% scaling — taller
        /// than a 1080p screen, so the window could not be positioned at all.
        /// </summary>
        [Fact]
        public void MainForm_fits_on_a_small_screen()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();

                Assert.True(form.MinimumSize.Width <= 900,
                    $"MinimumSize.Width is {form.MinimumSize.Width}");
                Assert.True(form.MinimumSize.Height <= 600,
                    $"MinimumSize.Height is {form.MinimumSize.Height}");
            });
        }

        /// <summary>
        /// The panes were fixed relative to each other; neither could be widened.
        /// </summary>
        /// <remarks>
        /// The panes are <c>railHost</c> and <c>paneHost</c> since the restructure. They were
        /// <c>pnlLeft</c> and <c>pnlRight</c>, which the layout dissolved: the rail is now three
        /// labelled sections rather than two group boxes, and the right pane carries the budget
        /// strip and the round-trip strip as well as the output.
        /// </remarks>
        [Fact]
        public void MainForm_panes_are_separated_by_a_splitter()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                var split = Assert.IsType<SplitContainer>(FindControl(form, "splitMain"));

                Assert.Contains(FindControl(form, "railHost"), split.Panel1.Controls.Cast<Control>());
                Assert.Contains(FindControl(form, "paneHost"), split.Panel2.Controls.Cast<Control>());
                Assert.True(split.Panel1MinSize > 0 && split.Panel2MinSize > 0);
            });
        }

        /// <summary>
        /// The rail reads as the pipeline it is. Guards the restructure against a later change
        /// that reintroduces control-shaped section names.
        /// </summary>
        [Fact]
        public void Rail_is_ordered_source_then_filters_then_files()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    var rail = FindControl(form, "railHost");

                    // Ordered by where they land on screen, not by index in the Controls
                    // collection: docked children are held in reverse of their visual order, so
                    // asserting on collection order would assert the opposite of what a user sees.
                    var titles = Descendants(rail)
                        .OfType<CodeShuttle.Controls.SectionHeader>()
                        .OrderBy(h => h.PointToScreen(Point.Empty).Y)
                        .Select(h => h.Title)
                        .ToList();

                    Assert.Equal(new[] { "Source", "Filters", "Files" }, titles);
                }
                finally { form.Hide(); }
            });
        }

        /// <summary>
        /// Producing a pack must reveal it.
        /// </summary>
        /// <remarks>
        /// The empty state covers the output pane, and its visibility was refreshed only from
        /// <c>SyncUIWithService</c> — which reacts to the file and extension model, and which
        /// Generate does not call. So Generate filled a pane that was still hidden behind "No pack
        /// yet": the pack existed, was counted in the statistics and the token gauge, and could be
        /// copied, but the window showed no code at all.
        /// </remarks>
        [Fact]
        public void Generated_output_replaces_the_empty_state()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    var output = (RichTextBox)FindControl(form, "rtbOutput");
                    var empty = FindControl(form, "emptyOutput");

                    // Only the empty state is toggled. The output pane stays visible underneath
                    // and is simply covered — hiding it would leave the RichTextBox without a
                    // window handle, which is a separate and much nastier bug.
                    Assert.True(output.Visible);
                    Assert.True(empty.Visible);

                    output.Text = ">>>> file: Program.cs\nclass P { }\n<<<< end file";

                    Assert.False(empty.Visible,
                        "the empty state is still covering the pack");
                    Assert.True(output.Visible,
                        "the output pane must stay visible and handle-backed at all times");
                    Assert.True(FindControl(form, "btnCopyOutput").Enabled,
                        "there is a pack to copy, but Copy is still disabled");

                    output.Text = "";

                    Assert.True(empty.Visible, "clearing the pack did not restore the empty state");
                }
                finally { form.Hide(); }
            });
        }

        /// <summary>
        /// Closing the window after generating a pack must not throw.
        /// </summary>
        /// <remarks>
        /// Disposing a <see cref="RichTextBox"/> raises <c>TextChanged</c> as its contents go
        /// away. The empty-state handler hung off that event sets <c>Visible</c> on the pane and
        /// the empty state, and assigning <c>Visible</c> forces handle creation — so closing the
        /// window mid-teardown threw "Dispose() cannot be called while doing CreateHandle()" and
        /// put a .NET crash dialog in front of the user on the way out.
        /// </remarks>
        [Fact]
        public void Closing_the_window_with_a_pack_open_does_not_throw()
        {
            StaRunner.Run(() =>
            {
                var form = new MainForm();
                form.Show();
                ((RichTextBox)FindControl(form, "rtbOutput")).Text =
                    ">>>> file: Program.cs\nclass P { }\n<<<< end file";

                // Close then Dispose, in that order, is what the window manager does.
                form.Close();
                form.Dispose();
            });
        }

        /// <summary>
        /// Round-trip is half the product and used to sit behind a dismissible banner, so it was
        /// invisible to anyone who had not generated a pack and permanently gone for anyone who
        /// had once hidden it.
        /// </summary>
        [Fact]
        public void Round_trip_strip_is_always_visible()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    var strip = FindControl(form, "pnlRecreateInfo");
                    Assert.True(strip.Visible, "The round-trip strip is hidden on an empty output pane.");
                    Assert.True(FindControl(form, "btnPasteResponse").Enabled,
                        "Pasting a reply does not need a pack, so it must not be gated on one.");
                }
                finally { form.Hide(); }
            });
        }

        /// <summary>
        /// Cancellation already worked internally and was only ever triggered by one scan
        /// superseding another. There was no way for a user to stop anything.
        /// </summary>
        [Fact]
        public void Status_strip_carries_progress_and_a_cancel_button()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                var strip = (StatusStrip)FindControl(form, "statusBar");
                var items = strip.Items.Cast<ToolStripItem>().ToList();

                Assert.Contains(items, i => i is ToolStripProgressBar);
                Assert.Contains(items, i => i is ToolStripButton b && b.Name == "sbCancel");
                Assert.Contains(items, i => i.Name == "sbSkipped");
            });
        }

        // ---------------------------------------------------------------- helpers

        private static Control FindControl(Control root, string name)
        {
            var found = FindControlOrNull(root, name);
            Assert.True(found != null, $"control '{name}' not found under {root.GetType().Name}");
            return found!;
        }

        /// <summary>
        /// Every descendant, in top-down document order — so a test can assert the order controls
        /// appear in, not merely that they exist.
        /// </summary>
        private static IEnumerable<Control> Descendants(Control root)
        {
            foreach (Control child in root.Controls)
            {
                yield return child;
                foreach (var deeper in Descendants(child)) yield return deeper;
            }
        }

        private static Control? FindControlOrNull(Control root, string name)
        {
            foreach (Control child in root.Controls)
            {
                if (child.Name == name) return child;
                var deeper = FindControlOrNull(child, name);
                if (deeper != null) return deeper;
            }
            return null;
        }

        private static int IndexOfChild(Control parent, string name)
        {
            for (int i = 0; i < parent.Controls.Count; i++)
                if (parent.Controls[i].Name == name) return i;
            return -1;
        }
    }
}
