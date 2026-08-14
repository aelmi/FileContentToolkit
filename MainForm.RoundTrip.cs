using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeShuttle.Controls;
using CodeShuttle.Dialogs;
using CodeShuttle.Help;
using CodeShuttle.Settings;
using CodeShuttle.Theming;
using CodeShuttle.UI;

namespace CodeShuttle
{
    /// <summary>
    /// The round-trip promotion and the trust features: token budgeting, the exclusion rule
    /// editor, settings transfer, and contextual help.
    /// </summary>
    public partial class MainForm
    {
        // ------------------------------------------------------------------ token budget

        /// <summary>The model the gauge and the over-budget warning measure against.</summary>
        private TokenModel CurrentTokenModel =>
            cmbBudgetModel.SelectedItem as TokenModel ?? TokenBudget.Resolve(_settings.TokenModelId);

        /// <summary>Fills the model dropdown and restores the persisted choice.</summary>
        private void InitTokenBudget()
        {
            foreach (var model in TokenBudget.All) cmbBudgetModel.Items.Add(model);

            var saved = TokenBudget.Resolve(_settings.TokenModelId);
            cmbBudgetModel.SelectedItem = TokenBudget.All.First(m => m.Id == saved.Id);

            toolTip1.SetToolTip(cmbBudgetModel,
                "Which context window to measure the pack against. " + TokenBudget.EstimateCaveat);

            UpdateTokenBudget();
        }

        private void CmbBudgetModel_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cmbBudgetModel.SelectedItem is TokenModel model)
            {
                _settings.TokenModelId = model.Id;
                _settings.SaveDebounced();
            }
            UpdateTokenBudget();
        }

        /// <summary>
        /// Repaints the gauge. Green below 80% of the window, amber to 100%, red past it.
        /// </summary>
        /// <remarks>
        /// The colour is not the only signal: the text beside it states the numbers, because a
        /// gauge that communicates purely through hue is unreadable to a colour-blind user and
        /// silent to a screen reader.
        /// </remarks>
        private void UpdateTokenBudget()
        {
            // Reached during teardown: disposing the model combo raises SelectedIndexChanged as
            // its selection collapses, and reading the pane's text from here would re-create the
            // handle of a control that is already being disposed. See OutputReadable.
            if (!OutputReadable) return;

            int tokens = TokenEstimator.Estimate(rtbOutput.Text);
            int window = TokenBudget.WindowFor(CurrentTokenModel, _settings.CustomTokenBudget);
            var level = TokenBudget.Classify(tokens, window);

            barBudget.Value = TokenBudget.PercentOf(tokens, window);
            lblBudgetText.Text = window > 0
                ? TokenBudget.Describe(tokens, window) + level switch
                {
                    BudgetLevel.Over => "  — over budget",
                    BudgetLevel.Near => "  — close to the limit",
                    _ => "",
                }
                : $"~{tokens:N0} tokens (set a custom window in Options)";

            ThemeRoles.SetText(lblBudgetText, level switch
            {
                BudgetLevel.Over => ThemeRole.ButtonDanger,
                BudgetLevel.Near => ThemeRole.ButtonSuccess,
                _ => ThemeRole.TextSecondary,
            });
            ThemeApplier.Apply(pnlBudget, ThemeManager.Tokens, ThemeManager.IsDark);

            btnBudgetBreakdown.Enabled = rtbOutput.TextLength > 0;
        }

        private void BtnBudgetBreakdown_Click(object? sender, EventArgs e)
        {
            if (rtbOutput.TextLength == 0)
            {
                AppMessage.Warning(this, "No pack to measure",
                    "There is nothing in the output pane. Generate a pack first.");
                return;
            }

            int window = TokenBudget.WindowFor(CurrentTokenModel, _settings.CustomTokenBudget);
            using var dlg = new TokenBreakdownForm(rtbOutput.Text, CurrentTokenModel, window);
            dlg.ShowDialog(this);
        }

        // ------------------------------------------------------------------ exclusion rules

        /// <summary>
        /// Opens the rule editor over the current rules, measured against the files the last scan
        /// actually considered.
        /// </summary>
        private void BtnEditRules_Click(object? sender, EventArgs e)
        {
            var candidates = _lastScanCandidates.Count > 0
                ? _lastScanCandidates
                : fileService.SelectedFiles.ToList();

            using var dlg = new ExclusionRuleEditorForm(
                fileService.IgnorePatterns, fileService.FolderPath, candidates);

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            fileService.IgnorePatterns.Clear();
            fileService.IgnorePatterns.AddRange(dlg.Rules);

            // Round-trips through the text box, which is still the quick-edit surface. Assigning
            // it fires TextChanged, which is what pushes the rules back into the service and
            // schedules the rescan, so this must not be skipped as "redundant".
            txtIgnorePatterns.Text = string.Join(", ", fileService.IgnorePatterns);
        }

        /// <summary>
        /// The files the last scan looked at, before rules were applied — the denominator the
        /// rule editor's counts are meaningful against.
        /// </summary>
        private List<string> _lastScanCandidates = new();

        // ------------------------------------------------------------------ settings transfer

        private void MnuExportSettings_Click(object? sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = SettingsTransfer.FileFilter,
                FileName = SettingsTransfer.DefaultFileName,
                Title = "Export settings",
            };
            if (sfd.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                SettingsTransfer.ExportToFile(_settings, sfd.FileName);
                Toast.Show(this, "Settings exported to " + Path.GetFileName(sfd.FileName));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                MessageBox.Show(this,
                    $"The settings could not be exported to:\n{sfd.FileName}\n\n{ex.Message}",
                    "Export settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MnuImportSettings_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = SettingsTransfer.FileFilter,
                Title = "Import settings",
                CheckFileExists = true,
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            // Replaces every preset and template the user currently has, so it is confirmed
            // rather than applied on the strength of a file-picker click.
            var answer = MessageBox.Show(this,
                "Importing replaces your current presets, prompt templates, filters and " +
                "appearance settings.\n\nContinue?",
                "Import settings", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (answer != DialogResult.Yes) return;

            try
            {
                SettingsTransfer.ImportFromFile(_settings, ofd.FileName);
                _settings.FlushPendingSave();

                ApplyImportedSettings();
                Toast.Show(this, "Settings imported.");
            }
            catch (Exception ex) when (ex is InvalidDataException or IOException or UnauthorizedAccessException)
            {
                MessageBox.Show(this,
                    $"The settings could not be imported from:\n{ofd.FileName}\n\n{ex.Message}",
                    "Import settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Pushes freshly imported settings into the live UI and service.</summary>
        private void ApplyImportedSettings()
        {
            ApplySettingsToService();

            searchBox.UseRegex = _settings.RegexSearch;
            searchBox.MatchCase = _settings.CaseSensitiveSearch;
            searchBox.WholeWord = _settings.WholeWordSearch;
            chkWatch.Checked = _settings.WatchFolderForChanges;

            ThemeManager.Mode = _settings.Mode;
            mnuViewDarkMode.Checked = _settings.Mode == ThemeMode.Dark;

            var model = TokenBudget.Resolve(_settings.TokenModelId);
            cmbBudgetModel.SelectedItem = TokenBudget.All.First(m => m.Id == model.Id);

            // The recent and preset menus populate from _settings when they are opened, so there
            // is nothing to refresh here — they will read the imported lists on the next click.
            UpdateTokenBudget();
            SyncUIWithService();

            _ = RefreshFilesInBackground();
        }

        // ------------------------------------------------------------------ contextual help

        /// <summary>
        /// Tags the panes so F1 can resolve a topic from whatever has focus.
        /// </summary>
        /// <remarks>
        /// Tagging containers rather than individual controls is the point: everything inside a
        /// tagged group inherits it, so adding a control does not mean remembering to tag it.
        /// </remarks>
        private void InitHelpTopics()
        {
            HelpTopics.Set(pnlTop, HelpTopics.GettingStarted);
            HelpTopics.Set(grpExtensions, HelpTopics.SelectingFiles);
            HelpTopics.Set(grpFiles, HelpTopics.SelectingFiles);
            HelpTopics.Set(pnlOutput, HelpTopics.BuildingThePack);
            HelpTopics.Set(pnlBudget, HelpTopics.BuildingThePack);
            HelpTopics.Set(rtbOutput, HelpTopics.BuildingThePack);
            HelpTopics.Set(pnlRecreateInfo, HelpTopics.ApplyingAnswersBack);
            HelpTopics.Set(searchBox, HelpTopics.Searching);
            HelpTopics.Set(statusBar, HelpTopics.Troubleshooting);

            // The window itself, so F1 always resolves to something.
            HelpTopics.Set(this, HelpTopics.GettingStarted);
        }

        /// <summary>Opens help at the topic for whatever currently has focus.</summary>
        private void ShowContextualHelp()
        {
            var topic = HelpTopics.ResolveFor(ActiveControl) ?? HelpTopics.Default;
            using var dlg = new HelpForm(topic.Id);
            dlg.ShowDialog(this);
        }

        private void MnuHelpContents_Click(object? sender, EventArgs e)
        {
            using var dlg = new HelpForm(HelpTopics.GettingStarted);
            dlg.ShowDialog(this);
        }
    }
}
