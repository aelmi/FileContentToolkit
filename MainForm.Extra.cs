using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileContentToolkit.Diagnostics;
using FileContentToolkit.Dialogs;
using FileContentToolkit.Settings;
using FileContentToolkit.UI;
using FileContentToolkit.Watcher;

namespace FileContentToolkit
{
    // All extra UI elements live in MainForm.Designer.cs now.
    // This partial only holds the cross-cutting plumbing: settings persistence,
    // folder watcher, hover effects, and the event handlers wired by the designer.
    public partial class MainForm
    {
        private AppSettings _settings = AppSettings.Load();
        private readonly FolderWatcher _folderWatcher = new();
        private FindReplaceForm? _findReplaceForm;
        private UpdateInfo? _latestUpdate;

        private void InitExtraFeatures()
        {
            // Push settings into service so the first scan uses them
            ApplySettingsToService();

            // Restore UI state from settings
            chkRegex.Checked = _settings.RegexSearch;
            chkCase.Checked = _settings.CaseSensitiveSearch;
            chkWord.Checked = _settings.WholeWordSearch;
            chkWatch.Checked = _settings.WatchFolderForChanges;

            // Hover effects on the new toolbar buttons
            Theme.AttachHover(btnTree, btnTree.BackColor);
            Theme.AttachHover(btnRecentFolders, btnRecentFolders.BackColor);
            Theme.AttachHover(btnOptions, btnOptions.BackColor);
            Theme.AttachHover(btnSavePreset, btnSavePreset.BackColor);
            Theme.AttachHover(btnLoadPreset, btnLoadPreset.BackColor);
            Theme.AttachHover(btnSearchRecents, btnSearchRecents.BackColor);
            Theme.AttachHover(btnFindReplace, btnFindReplace.BackColor);

            // Restore last folder
            if (string.IsNullOrEmpty(txtFolderPath.Text) && _settings.RecentFolders.Count > 0)
            {
                var first = _settings.RecentFolders[0];
                if (Directory.Exists(first))
                    txtFolderPath.Text = first;
            }

            // Folder watcher fires on the watcher thread; marshal to UI thread.
            _folderWatcher.Changed += () => BeginInvoke(new Action(() => _ = RefreshFilesInBackground()));
            RestartWatcherIfEnabled();

            // Open Find/Replace from the output editor with Ctrl+F or Ctrl+H
            rtbOutput.KeyDown += (s, e) =>
            {
                if (e.Control && (e.KeyCode == Keys.F || e.KeyCode == Keys.H))
                {
                    BtnFindReplace_Click(s, e);
                    e.Handled = true;
                }
            };

            // Save on close
            FormClosing += (s, e) =>
            {
                _settings.RegexSearch = chkRegex.Checked;
                _settings.CaseSensitiveSearch = chkCase.Checked;
                _settings.WholeWordSearch = chkWord.Checked;
                _settings.WatchFolderForChanges = chkWatch.Checked;
                _settings.Save();
                _folderWatcher.Dispose();
            };

            // Apply saved dark-mode preference (the menu CheckedChanged will fire and re-apply
            // through MnuViewDarkMode_CheckedChanged — that's the single code path).
            mnuViewDarkMode.Checked = _settings.DarkMode;
            if (_settings.DarkMode) Theme.Apply(this, true);

            // Background update check (non-blocking; never throws).
            _ = CheckForUpdatesAsync(silentIfNone: true);
        }

        private void MnuViewDarkMode_CheckedChanged(object? sender, EventArgs e)
        {
            _settings.DarkMode = mnuViewDarkMode.Checked;
            Theme.Apply(this, mnuViewDarkMode.Checked);
            _settings.Save();
        }

        private async Task CheckForUpdatesAsync(bool silentIfNone)
        {
            var info = await UpdateChecker.CheckAsync();
            _latestUpdate = info;

            if (IsDisposed) return;

            if (info?.UpdateAvailable == true)
            {
                BeginInvoke(new Action(() =>
                {
                    sbUpdateNotice.Text = $"Update available — {info.TagName}";
                    sbUpdateNotice.ToolTipText = info.HtmlUrl;
                    sbUpdateNotice.Visible = true;
                }));
            }
            else if (!silentIfNone)
            {
                BeginInvoke(new Action(() =>
                {
                    var msg = info == null
                        ? "Could not contact GitHub. Try again later."
                        : $"You're up to date.\nLatest release: {info.TagName}\nCurrent: {info.Current}";
                    MessageBox.Show(this, msg, "Check for updates",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
        }

        private void MnuHelpCheckUpdates_Click(object? sender, EventArgs e)
        {
            _ = CheckForUpdatesAsync(silentIfNone: false);
        }

        private void SbUpdateNotice_Click(object? sender, EventArgs e)
        {
            if (_latestUpdate == null || string.IsNullOrEmpty(_latestUpdate.HtmlUrl)) return;
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _latestUpdate.HtmlUrl,
                    UseShellExecute = true
                });
            }
            catch { /* ignore */ }
        }

        private void ApplySettingsToService()
        {
            fileService.MaxFileSizeBytes = _settings.MaxFileSizeBytes;
            fileService.SkipBinaryFiles = _settings.SkipBinaryFiles;
            fileService.AutoDetectEncoding = _settings.AutoDetectEncoding;
            fileService.UseGitIgnoreFiles = _settings.UseGitIgnoreFiles;
        }

        private void RestartWatcherIfEnabled()
        {
            _folderWatcher.Stop();
            if (chkWatch.Checked && !string.IsNullOrEmpty(fileService.FolderPath) && Directory.Exists(fileService.FolderPath))
            {
                _folderWatcher.Start(fileService.FolderPath, fileService.IncludeSubfolders);
            }
        }

        // -------------------- Designer-wired event handlers --------------------

        private void BtnTree_Click(object? sender, EventArgs e)
        {
            var folder = txtFolderPath.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                MessageBox.Show(this, "Pick a folder first (use Browse or the path box).", "Tree picker",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var dlg = new FolderTreePickerForm(folder, fileService.Extensions);
            if (dlg.ShowDialog(this) == DialogResult.OK && dlg.SelectedFiles.Count > 0)
            {
                fileService.AddFiles(dlg.SelectedFiles);
                SyncUIWithService();
            }
        }

        // -------------------- Designer-declared slot config + slot click handlers --------------------
        // Called from InitializeComponent (Designer.cs). They wire common state onto the
        // pre-declared menu item slots so the Designer file stays free of repetitive boilerplate.

        private void ConfigureRfSlot(ToolStripMenuItem item, string name)
        {
            item.Name = name;
            item.Visible = false;
            item.Click += MnuRecentFolderSlot_Click;
        }

        private void ConfigureRsSlot(ToolStripMenuItem item, string name)
        {
            item.Name = name;
            item.Visible = false;
            item.Click += MnuRecentSearchSlot_Click;
        }

        private void ConfigurePsSlot(ToolStripMenuItem item, string name)
        {
            item.Name = name;
            item.Visible = false;
            item.Click += MnuPresetSlot_Click;
        }

        // -------------------- Recent / Preset menus (populate the persisted slots) --------------------

        private void BtnRecentFolders_Click(object? sender, EventArgs e)
        {
            PopulateRecentSlots(_settings.RecentFolders, RecentFolderSlots(), mnuRfEmpty, mnuRfSep, mnuRfClear);
            cmsRecentFolders.Show(btnRecentFolders, new Point(0, btnRecentFolders.Height));
        }

        private void BtnSearchRecents_Click(object? sender, EventArgs e)
        {
            PopulateRecentSlots(_settings.RecentSearches, RecentSearchSlots(), mnuRsEmpty, mnuRsSep, mnuRsClear);
            cmsRecentSearches.Show(btnSearchRecents, new Point(0, btnSearchRecents.Height));
        }

        private void BtnLoadPreset_Click(object? sender, EventArgs e)
        {
            PopulatePresetSlots();
            cmsPresets.Show(btnLoadPreset, new Point(0, btnLoadPreset.Height));
        }

        private ToolStripMenuItem[] RecentFolderSlots() => new[] {
            mnuRf01, mnuRf02, mnuRf03, mnuRf04, mnuRf05, mnuRf06, mnuRf07, mnuRf08,
            mnuRf09, mnuRf10, mnuRf11, mnuRf12, mnuRf13, mnuRf14, mnuRf15
        };

        private ToolStripMenuItem[] RecentSearchSlots() => new[] {
            mnuRs01, mnuRs02, mnuRs03, mnuRs04, mnuRs05, mnuRs06, mnuRs07, mnuRs08,
            mnuRs09, mnuRs10, mnuRs11, mnuRs12, mnuRs13, mnuRs14, mnuRs15
        };

        private ToolStripMenuItem[] PresetSlots() => new[] {
            mnuPs01, mnuPs02, mnuPs03, mnuPs04, mnuPs05, mnuPs06, mnuPs07, mnuPs08,
            mnuPs09, mnuPs10, mnuPs11, mnuPs12, mnuPs13, mnuPs14, mnuPs15,
            mnuPs16, mnuPs17, mnuPs18, mnuPs19, mnuPs20, mnuPs21, mnuPs22,
            mnuPs23, mnuPs24, mnuPs25
        };

        private static void PopulateRecentSlots(IReadOnlyList<string> data,
                                                ToolStripMenuItem[] slots,
                                                ToolStripMenuItem emptyItem,
                                                ToolStripSeparator separator,
                                                ToolStripMenuItem clearItem)
        {
            int n = Math.Min(data.Count, slots.Length);
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < n)
                {
                    slots[i].Text = data[i];
                    slots[i].Tag = data[i];
                    slots[i].Visible = true;
                }
                else
                {
                    slots[i].Visible = false;
                    slots[i].Tag = null;
                }
            }
            emptyItem.Visible = n == 0;
            separator.Visible = n > 0;
            clearItem.Visible = n > 0;
        }

        private void PopulatePresetSlots()
        {
            var ordered = _settings.Presets
                .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
            var slots = PresetSlots();
            int n = Math.Min(ordered.Count, slots.Length);
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < n)
                {
                    slots[i].Text = ordered[i].Name;
                    slots[i].Tag = ordered[i];
                    slots[i].Visible = true;
                }
                else
                {
                    slots[i].Visible = false;
                    slots[i].Tag = null;
                }
            }
            mnuPsEmpty.Visible = n == 0;
            mnuPsSep.Visible = true; // separator before Manage… is always shown
        }

        private void MnuRecentFolderSlot_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem mi && mi.Tag is string path)
                txtFolderPath.Text = path; // fires the debounced refresh
        }

        private void MnuRecentSearchSlot_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem mi && mi.Tag is string term)
                txtSearchFiles.Text = term;
        }

        private void MnuPresetSlot_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem mi && mi.Tag is Preset preset)
                ApplyPreset(preset);
        }

        private void MnuRfClear_Click(object? sender, EventArgs e)
        {
            _settings.RecentFolders.Clear();
            _settings.Save();
        }

        private void MnuRsClear_Click(object? sender, EventArgs e)
        {
            _settings.RecentSearches.Clear();
            _settings.Save();
        }

        private void MnuPsManage_Click(object? sender, EventArgs e)
        {
            using var dlg = new PresetManagerForm(_settings);
            if (dlg.ShowDialog(this) == DialogResult.OK && dlg.LoadRequested && dlg.SelectedPreset != null)
                ApplyPreset(dlg.SelectedPreset);
            _settings.Save();
        }

        private void BtnOptions_Click(object? sender, EventArgs e)
        {
            using var dlg = new OptionsForm(_settings);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                ApplySettingsToService();
                _settings.Save();
                chkWatch.Checked = _settings.WatchFolderForChanges;
                RestartWatcherIfEnabled();
                _ = RefreshFilesInBackground();
            }
        }

        private void BtnSavePreset_Click(object? sender, EventArgs e)
        {
            string suggestedName()
            {
                var folder = txtFolderPath.Text ?? "";
                return string.IsNullOrEmpty(folder) ? "Preset" : new DirectoryInfo(folder).Name;
            }

            var name = ThemedPrompt.Show(this, "Save preset", "Preset name:", suggestedName());
            if (string.IsNullOrWhiteSpace(name)) return;

            var existing = _settings.Presets.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                if (MessageBox.Show(this, $"Overwrite preset \"{existing.Name}\"?", "Save preset",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                _settings.Presets.Remove(existing);
            }

            _settings.Presets.Add(new Preset
            {
                Name = name,
                FolderPath = txtFolderPath.Text ?? "",
                Extensions = new List<string>(fileService.Extensions),
                IgnorePatterns = new List<string>(fileService.IgnorePatterns),
                IncludeSubfolders = chkIncludeSubfolders.Checked
            });
            _settings.Save();
            MessageBox.Show(this, "Preset saved.", "Save preset", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void ApplyPreset(Preset p)
        {
            fileService.SetExtensions(p.Extensions);
            fileService.IgnorePatterns.Clear();
            fileService.IgnorePatterns.AddRange(p.IgnorePatterns);
            fileService.SetIncludeSubfolders(p.IncludeSubfolders);
            chkIncludeSubfolders.Checked = p.IncludeSubfolders;
            txtFolderPath.Text = p.FolderPath; // triggers debounced refresh
            SyncUIWithService();
        }

        private void ChkWatch_CheckedChanged(object? sender, EventArgs e)
        {
            _settings.WatchFolderForChanges = chkWatch.Checked;
            RestartWatcherIfEnabled();
        }

        private void MnuHelpShortcuts_Click(object? sender, EventArgs e)
        {
            using var dlg = new HelpForm();
            dlg.ShowDialog(this);
        }

        private void MnuHelpAbout_Click(object? sender, EventArgs e)
        {
            using var dlg = new AboutForm();
            dlg.ShowDialog(this);
        }

        private void BtnFindReplace_Click(object? sender, EventArgs e)
        {
            if (_findReplaceForm == null || _findReplaceForm.IsDisposed)
            {
                _findReplaceForm = new FindReplaceForm(
                    rtbOutput,
                    _settings.RecentSearches,
                    initialRegex: chkRegex.Checked,
                    initialCase: chkCase.Checked,
                    initialWord: chkWord.Checked,
                    onSearchUsed: term =>
                    {
                        _settings.AddRecentSearch(term);
                        _settings.Save();
                    });
                _findReplaceForm.Owner = this;
            }
            _findReplaceForm.SetInitialQuery(txtSearchFiles.Text);
            if (!_findReplaceForm.Visible) _findReplaceForm.Show(this);
            else _findReplaceForm.BringToFront();
        }
    }
}
