using CodeShuttle.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeShuttle.Controls;
using CodeShuttle.Filters;
using CodeShuttle.Theming;
using CodeShuttle.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace CodeShuttle
{
    public partial class MainForm : ThemedForm
    {
        // Hoisted out of the constructor: a constant array argument allocates on every call
        // and the analyzer (CA1861) flags it. The contents are unchanged.
        private static readonly string[] ExtensionSuggestions =
            { ".cs", ".txt", ".xml", ".json", ".md", ".html", ".css", ".js", ".py", ".java", ".cpp" };

        private FileContentService fileService = new FileContentService();
        private Encoding selectedEncoding = Encoding.UTF8; // Default UTF-8

        private CancellationTokenSource? _scanCts;
        private CancellationTokenSource? _generateCts;
        private CancellationTokenSource? _applyCts;
        private readonly System.Windows.Forms.Timer _refreshDebounce;

        /// <summary>
        /// Recomputes the output statistics, token budget and round-trip strip after the output
        /// pane changes by any route other than Generate.
        /// </summary>
        /// <remarks>
        /// Debounced because the statistics walk the whole string — length, line count, UTF-8 byte
        /// count and a token estimate — and this pane routinely holds tens of megabytes. Without a
        /// debounce, hand-editing the pane would run four full passes per keystroke.
        /// Before this existed, UpdateOutputStatistics had exactly one caller (ProcessFilesAsync),
        /// so compressing a 200k-token pack left the gauge reading red against the old character
        /// count for a blob a fraction of the size.
        /// </remarks>
        private readonly System.Windows.Forms.Timer _outputStatsDebounce;

        /// <summary>
        /// Hard ceiling on assembled output. Beyond roughly this the RichTextBox needs ~5x the
        /// source size in memory and approaches the 1 G-char string limit.
        /// </summary>
        private const int MaxOutputChars = 60_000_000;

        /// <summary>
        /// Largest pack whose file headers are worth styling. Above this the RTF engine's reflow
        /// cost per styled run makes Generate look hung — see the loop in ProcessFilesAsync.
        /// </summary>
        private const int MaxHighlightChars = 200_000;

        /// <summary>
        /// Called from the designer's Dispose. The debounce timer and the cancellation sources
        /// were never disposed, so every superseded scan leaked one under the typing debounce.
        /// </summary>
        private void DisposeOwnedResources()
        {
            _refreshDebounce?.Dispose();
            _outputStatsDebounce?.Dispose();
            _scanCts?.Dispose();
            _scanCts = null;
            _generateCts?.Dispose();
            _generateCts = null;
            _applyCts?.Dispose();
            _applyCts = null;
            _folderWatcher.Dispose();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x000B;

        public MainForm()
        {
            InitializeComponent();

            // Reassembles the designer's controls into the Source → Filters → Files rail and the
            // Pack pane. Runs before anything reads or writes the UI, because it is what decides
            // which controls are on screen at all.
            BuildLayout();
            WireLayoutMenus();

            // Tooltips were declared, written, and then commented out of the constructor, so
            // every one of them was dead. Restored, and rewritten as outcome-plus-example rather
            // than a restatement of the label beside them.
            SetupToolTips();

            // Ensure UI reflects service state
            SyncUIWithService();

            // Redundant with Designer, but harmless if left:
            lstFiles.SelectionMode = SelectionMode.MultiExtended;

            // Debounce rapid typing in folder path / ignore patterns so we don't kick off a scan per keystroke.
            _refreshDebounce = new System.Windows.Forms.Timer { Interval = 400 };
            _refreshDebounce.Tick += (s, e) =>
            {
                _refreshDebounce.Stop();
                _ = RefreshFilesInBackground();
            };

            // The output pane changes by six routes other than Generate — the four Tools ▸
            // Compression actions, manual editing, and Find/Replace — and none of them used to
            // refresh the statistics, the token gauge or the round-trip strip. Wired here rather
            // than in the Designer so the handler is not lost if that file is ever regenerated.
            _outputStatsDebounce = new System.Windows.Forms.Timer { Interval = 300 };
            _outputStatsDebounce.Tick += (s, e) =>
            {
                _outputStatsDebounce.Stop();
                if (!IsDisposed) UpdateOutputStatistics();
            };
            rtbOutput.TextChanged += (s, e) =>
            {
                _outputStatsDebounce.Stop();
                _outputStatsDebounce.Start();
            };

            // New: Populate extension suggestions
            cmbExtension.Items.AddRange(ExtensionSuggestions);
            cmbExtension.DropDownStyle = ComboBoxStyle.DropDown;

            // New: Populate encodings
            cmbEncoding.Items.AddRange(new object[] { "UTF-8", "ASCII", "UTF-16", "UTF-32", "ISO-8859-1" });
            // DropDownList, not DropDown: free text could leave SelectedIndex at -1, which the
            // SelectedIndexChanged handler then dereferenced.
            cmbEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEncoding.SelectedIndex = 0; // Default UTF-8

            // Feature toolbar (recent folders, options, presets, watch, search toggles, find/replace)
            InitExtraFeatures();

            // Build the project-type menus from the ProjectPresets catalogue.
            WireLanguagePresets();

            // WS5: the round trip's supporting UI. After InitExtraFeatures, which is what loads
            // _settings — the budget dropdown restores from it.
            InitTokenBudget();
            InitHelpTopics();
        }

        /// <summary>
        /// Fills the project-type menus from <see cref="ProjectPresets"/>.
        /// </summary>
        /// <remarks>
        /// The sixteen designer-authored language items are replaced wholesale rather than
        /// re-tagged: the catalogue is now grouped and longer than sixteen, and a fixed set of
        /// designer items would cap it forever. Built in two places from one source so the "+ add"
        /// menu beside the extension chips and the Presets menu can never drift apart.
        /// </remarks>
        private void WireLanguagePresets()
        {
            mnuAddLangPresets.DropDownItems.Clear();
            foreach (var item in BuildProjectPresetItems())
                mnuAddLangPresets.DropDownItems.Add(item);
            mnuAddLangPresets.Text = "Project type";
        }

        /// <summary>One submenu per ecosystem, each holding its project types.</summary>
        private IEnumerable<ToolStripMenuItem> BuildProjectPresetItems()
        {
            foreach (var group in ProjectPresets.ByGroup)
            {
                var parent = new ToolStripMenuItem(group.Key);
                foreach (var preset in group)
                {
                    var item = new ToolStripMenuItem(preset.Name) { Tag = preset };
                    item.ToolTipText = string.Join(" ", preset.Extensions)
                        + (preset.Ignore.Length > 0
                            ? "\nIgnores: " + string.Join(" ", preset.Ignore)
                            : "");
                    item.Click += MnuLanguagePreset_Click;
                    parent.DropDownItems.Add(item);
                }
                yield return parent;
            }
        }

        private void MnuLanguagePreset_Click(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem { Tag: ProjectPreset preset }) return;

            // The extension list is replaced: picking a project type is a statement about what the
            // project *is*, not an addition to it, and this is what the preset menu has always
            // done. The ignore list is merged instead — it is usually hand-tuned, and silently
            // discarding someone's exclusions to add four of ours would be a poor trade.
            fileService.Extensions.Clear();
            int addedIgnores = MergeIgnorePatterns(preset.Ignore);
            BulkAddExtensions(preset.Extensions);

            sbScanStatus.Text = addedIgnores > 0
                ? $"{preset.Name}: {preset.Extensions.Length} extensions, {addedIgnores} ignore rule{(addedIgnores == 1 ? "" : "s")} added"
                : $"{preset.Name}: {preset.Extensions.Length} extensions";
        }

        /// <summary>
        /// Adds the preset's exclusions that are not already present, and returns how many were
        /// new. Writing through the text box rather than the service keeps the one parse of that
        /// field in <c>TxtIgnorePatterns_TextChanged</c>.
        /// </summary>
        private int MergeIgnorePatterns(string[] patterns)
        {
            if (patterns == null || patterns.Length == 0) return 0;

            var current = txtIgnorePatterns.Text
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => p.Length > 0)
                .ToList();

            var added = patterns
                .Where(p => !current.Contains(p, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (added.Count == 0) return 0;

            current.AddRange(added);
            txtIgnorePatterns.Text = string.Join(", ", current);
            return added.Count;
        }

        private void BulkAddExtensions(string[] exts)
        {
            if (exts == null || exts.Length == 0) return;

            int added = 0, skipped = 0;
            foreach (var raw in exts)
            {
                var ext = raw.Trim();
                if (string.IsNullOrEmpty(ext)) continue;
                if (!ext.StartsWith('.')) ext = "." + ext;

                if (fileService.Extensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                    skipped++;
                else
                {
                    fileService.AddExtension(ext);
                    added++;
                }
            }

            SyncUIWithService();
            if (added > 0) _ = RefreshFilesInBackground();

            // Brief feedback in the status bar (sbScanStatus is the rightmost label
            // that the scan flow also uses).
            sbScanStatus.Text = skipped > 0
                ? $"Added {added}, skipped {skipped} already present"
                : $"Added {added} extension{(added == 1 ? "" : "s")}";
        }

        // The hover effects that lived here set BackColor from hardcoded literals, so they
        // fought dark mode and would have had to be maintained per palette. Button hover is now
        // part of the theme: ThemeApplier sets FlatAppearance.MouseOverBackColor and attaches a
        // token-coloured hover to every button it styles.

        /// <summary>
        /// Tooltips, phrased as outcome and example.
        /// </summary>
        /// <remarks>
        /// This method existed and its call was commented out, so none of it reached a user.
        /// Restoring it was also a chance to stop the tips restating the label: "Browse for a
        /// folder" on a button reading Browse tells nobody anything. Each one now says what
        /// happens, and shows an example where the input has a syntax.
        /// </remarks>
        private void SetupToolTips()
        {
            // toolTip1 is owned by the Designer; do not construct it here.

            toolTip1.SetToolTip(btnBrowse, "Pick the folder to scan (Ctrl+O). Its subfolders are included by default.");
            toolTip1.SetToolTip(btnAdd, "Collect files with this extension. Type it with or without the dot: .cs or cs.");
            toolTip1.SetToolTip(btnRemove, "Stop collecting the selected extension. The scan reruns straight away.");
            toolTip1.SetToolTip(btnRefreshExtensions, "Rescan the folder with the current extensions and rules (F5).");
            toolTip1.SetToolTip(btnAddMultipleFiles, "Add specific files that the extension filter would not pick up.");
            toolTip1.SetToolTip(btnRemoveFile, "Drop the selected files from this pack. The files on disk are untouched.");
            toolTip1.SetToolTip(btnMoveUp, "Move earlier in the pack. Order is preserved when the AI reads it.");
            toolTip1.SetToolTip(btnMoveDown, "Move later in the pack.");
            toolTip1.SetToolTip(btnGenerate, "Read every selected file and assemble the pack (Ctrl+G).");
            toolTip1.SetToolTip(btnCopyOutput, "Copy the pack (Ctrl+C). The arrow offers Markdown, XML, JSON and prompt templates.");
            toolTip1.SetToolTip(chkIncludeSubfolders, "Off, only files directly in the chosen folder are collected.");
            toolTip1.SetToolTip(btnEditOutput, "Make the output pane editable, so you can trim the pack before sending it.");
            toolTip1.SetToolTip(btnApplyAiChanges,
                "Diff the pack above against a folder and write what you accept. Backups are taken first.");
            toolTip1.SetToolTip(txtIgnorePatterns,
                "Comma-separated globs, e.g. *.tmp, bin/, **/generated/*. Use Edit rules to see what each one removes.");
            toolTip1.SetToolTip(btnEditRules,
                "One rule per row, with a count of the files each removes and a box to test a path.");
            toolTip1.SetToolTip(btnExportOutput, "Save the pack to a file (Ctrl+E), in the selected encoding.");
            toolTip1.SetToolTip(btnBudgetBreakdown, "See which files use the most tokens, and what to drop to fit.");
            searchBox.SetToolTips(toolTip1);
            toolTip1.SetToolTip(cmbEncoding,
                "Used when auto-detect is off in Options. A file that is not in this encoding is reported, not mangled.");
        }

        #region Event Handlers (wired from Designer)

        /// <summary>
        /// Unlocks the output pane for typing, and locks it again.
        /// </summary>
        /// <remarks>
        /// Works on an empty pane as well as on a generated pack, which is the point: typing or
        /// pasting a bundle in by hand is how you decrypt or decompress one you did not generate
        /// here. That requires taking down the "No pack yet" card, because it is painted over the
        /// output box rather than instead of it — see <see cref="UpdateOutputPresence"/> for why
        /// the box itself must never be hidden.
        /// </remarks>
        private void BtnEditOutput_Click(object sender, EventArgs e)
        {
            _editingOutput = !_editingOutput;
            rtbOutput.ReadOnly = !_editingOutput;

            // Editing is a mode, and the button is the only thing that says so. The "on" look is
            // painted in ApplyOutlineButtons from the accent wash — it used to borrow the warning
            // banner's amber, which read as a caution about a state the user had deliberately
            // chosen, and was the one orange left in a palette that is otherwise blue.
            toolTip1.SetToolTip(btnEditOutput,
                _editingOutput ? "Click to finish editing" : "Edit the output");
            ApplyTheme();

            // Shows or restores the empty state around the edit, and re-reads the protect buttons
            // against whatever the user typed.
            UpdateOutputPresence();

            if (_editingOutput)
            {
                rtbOutput.Focus();
                rtbOutput.SelectionStart = rtbOutput.TextLength;
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder to scan for files";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFolderPath.Text = folderDialog.SelectedPath;
                    _settings.AddRecentFolder(folderDialog.SelectedPath);
                    _settings.Save();
                }
            }
        }

        private async void BtnAddFolder_Click(object sender, EventArgs e)
        {
            if (fileService.Extensions.Count == 0)
            {
                AppMessage.Warning(this, "No extensions configured",
                    "Add at least one file extension first, so the scan knows which files to include.");
                cmbExtension.Focus();
                return;
            }

            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select another folder — its matching files will be appended to the list";
                if (folderDialog.ShowDialog() != DialogResult.OK) return;

                // Enumerating a large tree synchronously on the UI thread froze the app; pointing
                // it at C:\Users could freeze it for minutes.
                List<string> matches;
                try
                {
                    UseWaitCursor = true;
                    var target = folderDialog.SelectedPath;
                    matches = await Task.Run(() => fileService.EnumerateMatchingFiles(target));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not scan that folder: " + ex.Message,
                        "Add Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally { UseWaitCursor = false; }

                if (matches.Count == 0)
                {
                    AppMessage.Info(this, "No matching files",
                        "No files in that folder matched the configured extensions and filters.");
                    return;
                }

                int before = fileService.SelectedFiles.Count;
                fileService.AddFiles(matches);
                int added = fileService.SelectedFiles.Count - before;

                _settings.AddRecentFolder(folderDialog.SelectedPath);
                _settings.Save();

                SyncUIWithService();

                toolTip1.Show($"Added {added} file(s) from {folderDialog.SelectedPath}",
                    btnAddFolder, 0, -24, 2500);
            }
        }

        private void MiSortByName_Click(object sender, EventArgs e)
        {
            SortFilesAndRebind(orderByExtension: false);
        }

        private void MiSortByExtension_Click(object sender, EventArgs e)
        {
            SortFilesAndRebind(orderByExtension: true);
        }

        private async void BtnCompress_Click(object sender, EventArgs e)
        {
            // The gate runs here, against the plain text, because SecretGuard.Scan cannot see
            // inside gzip+base64 — compressing first would hand it a blob it always reports clean.
            // Compressed output is pasted into a chat exactly like uncompressed output is.
            if (string.IsNullOrEmpty(rtbOutput.Text))
            {
                AppMessage.Warning(this, "Nothing to compress",
                    "There is nothing in the output pane to compress.");
                return;
            }

            var input = ApplySecretGate(rtbOutput.Text);
            if (input == null) return;

            try
            {
                UseWaitCursor = true;
                var compressed = await CompressionUtils.CompressToBase64Async(input);
                rtbOutput.ReadOnly = false;
                rtbOutput.Text = compressed;
                rtbOutput.ReadOnly = true;

                Toast.Show(this,
                    $"Compressed: {input.Length:N0} chars to {compressed.Length:N0} chars.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Compression failed: " + ex.Message, "Compression",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { UseWaitCursor = false; }
        }

        private async void BtnDecompress_Click(object sender, EventArgs e)
        {
            var base64 = rtbOutput.Text ?? string.Empty;
            try
            {
                // Off the UI thread and under a hard output budget: a crafted 2 MB payload can
                // expand to gigabytes, which used to hang the app and then exhaust memory.
                UseWaitCursor = true;
                var (ok, text, error) = await CompressionUtils.TryDecompressAsync(base64);
                if (ok)
                {
                    rtbOutput.ReadOnly = false;
                    rtbOutput.Text = text;
                    rtbOutput.ReadOnly = true;
                }
                else
                {
                    MessageBox.Show(error, "Decompression", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally { UseWaitCursor = false; }
        }

        private async void BtnCompressEnc_Click(object sender, EventArgs e)
        {
            // Routed through the validating entry point WITH confirmation: encryption is one-way,
            // so a single mistyped character used to produce a permanently unrecoverable blob.
            var pwd = PasswordDialog.ShowDialogWithValidation(
                this,
                "Enter password for encryption",
                "Enter a password. You will need it to decrypt — it cannot be recovered.",
                requireConfirmation: true);

            if (string.IsNullOrEmpty(pwd)) return; // cancelled

            var input = rtbOutput.Text ?? string.Empty;
            if (input.Length == 0)
            {
                AppMessage.Warning(this, "Nothing to encrypt",
                    "There is nothing in the output pane to encrypt.");
                return;
            }

            try
            {
                UseWaitCursor = true;
                var sealedBase64 = await CompressionUtils.CompressAndEncryptToBase64Async(input, pwd);

                // The output pane is NOT overwritten. It used to be, which destroyed the
                // plaintext at the exact moment the only copy became password-dependent.
                bool copied = TrySetClipboardText(sealedBase64);

                var prompt = "Compressed and encrypted (AES-GCM)."
                           + (copied ? "\n\nThe encrypted text is on the clipboard." : "")
                           + "\n\nThe output pane has been left unchanged.\n\nSave the encrypted text to a file?";

                if (MessageBox.Show(this, prompt, "Secure Compression",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    using var sfd = new SaveFileDialog
                    {
                        Filter = "CodeShuttle encrypted (*.cshtx)|*.cshtx|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                        Title = "Save encrypted output"
                    };
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                        AtomicFile.WriteAllText(sfd.FileName, sealedBase64, new UTF8Encoding(false));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Secure compression failed: " + ex.Message, "Secure Compression",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { UseWaitCursor = false; }
        }

        private async void BtnDecompressEnc_Click(object sender, EventArgs e)
        {
            var pwd = PasswordDialog.ShowDialog(this, "Enter password for decryption");
            if (string.IsNullOrEmpty(pwd)) return; // cancelled

            try
            {
                UseWaitCursor = true;
                var input = rtbOutput.Text ?? string.Empty;
                var (ok, decrypted, error) = await CompressionUtils.TryDecryptAndDecompressAsync(input, pwd);

                if (ok)
                {
                    rtbOutput.ReadOnly = false;
                    rtbOutput.Text = decrypted;
                    rtbOutput.ReadOnly = true;
                }
                else
                {
                    MessageBox.Show($"Secure decompression failed: {error}", "Secure Decompression",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Secure decompression failed: " + ex.Message, "Secure Decompression",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { UseWaitCursor = false; }
        }

        /// <summary>Clipboard.SetText throws ExternalException whenever another process holds the clipboard.</summary>
        private static bool TrySetClipboardText(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            try { Clipboard.SetText(text); return true; }
            catch (System.Runtime.InteropServices.ExternalException) { return false; }
            catch (ArgumentException) { return false; }
        }

        private void TxtFolderPath_TextChanged(object sender, EventArgs e)
        {
            fileService.SetFolderPath(txtFolderPath.Text);
            // Lets the crash logger redact paths under the scan root out of reports the user
            // may email to support.
            CodeShuttle.Diagnostics.CrashLogger.ScanRoot = txtFolderPath.Text;
            RestartWatcherIfEnabled();
            DebounceRefresh();
        }

        private void ChkIncludeSubfolders_CheckedChanged(object sender, EventArgs e)
        {
            fileService.SetIncludeSubfolders(chkIncludeSubfolders.Checked);
            RestartWatcherIfEnabled();
            _ = RefreshFilesInBackground();
        }

        private void BtnRefreshExtensions_Click(object sender, EventArgs e)
        {
            _ = RefreshFilesInBackground();
        }

        private void DebounceRefresh()
        {
            _refreshDebounce.Stop();
            _refreshDebounce.Start();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string extension = cmbExtension.Text.Trim(); // Changed to cmb
            if (string.IsNullOrEmpty(extension))
            {
                AppMessage.Warning(this, "No extension entered", "Enter a file extension first.");
                cmbExtension.Focus();
                return;
            }

            if (!extension.StartsWith('.'))
                extension = "." + extension;

            if (fileService.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                AppMessage.Info(this, "Already added", $"{extension} is already in the list.");
                return;
            }

            fileService.AddExtension(extension);
            SyncUIWithService();
            cmbExtension.Text = "";
            cmbExtension.Focus();
            _ = RefreshFilesInBackground();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (lstExtensions.SelectedItems.Count == 0)
            {
                AppMessage.Warning(this, "Nothing selected", "Select one or more extensions to remove.");
                return;
            }

            var toRemove = lstExtensions.SelectedItems
                .Cast<object>()
                .Select(o => o?.ToString())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            foreach (var ext in toRemove)
                fileService.RemoveExtension(ext!);

            SyncUIWithService();
            _ = RefreshFilesInBackground();
        }

        private void CmbExtension_KeyPress(object sender, KeyPressEventArgs e) // Changed to cmb
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnAdd_Click(sender, e);
                e.Handled = true;
            }
        }

        // Primary "Add Files" button
        /// <summary>
        /// Adds files the user chose explicitly — dropped on the list, or picked from the dialog —
        /// screening out anything that cannot be read as text.
        /// </summary>
        /// <remarks>
        /// The folder scan has always classified candidates and reported what it left out. The two
        /// explicit routes bypassed it entirely and added whatever they were handed, so dropping an
        /// .ico produced a bundle entry whose body was a .NET decoder error. That is worse than it
        /// sounds: the pack is meant to be pasted to an AI and applied back, so an error string
        /// sitting where file content belongs is a path to writing it into the user's actual file.
        ///
        /// An explicitly chosen file keeps its extension privilege — the configured filters are not
        /// applied here, because naming a file is a clearer statement of intent than a filter list.
        /// Being unreadable is a different matter, and is refused.
        /// </remarks>
        private void AddChosenFiles(IReadOnlyCollection<string> paths)
        {
            if (paths == null || paths.Count == 0) return;

            var (accepted, refused) = ScreenChosenFiles(paths);

            if (accepted.Count > 0)
            {
                fileService.AddFiles(accepted);
                SyncUIWithService();
            }

            if (refused.Count == 0) return;

            UpdateSkippedIndicator(refused);

            // Everything the user named was refused, so the window is about to look exactly as it
            // did before they acted. A toast is not enough here: it lasts three seconds, and the
            // only thing left afterwards is a small count in the corner of the status bar — which
            // reads as "the drop didn't work", not as "that file cannot be packed". An explicit
            // action that achieves nothing has to say so explicitly.
            if (accepted.Count == 0)
            {
                AppMessage.Warning(this,
                    refused.Count == 1 ? "That file can't be added" : "Those files can't be added",
                    DescribeRefusals(refused));
                return;
            }

            // Some did land, so the list visibly changed and the status-bar count carries the rest.
            Toast.Show(this,
                refused.Count == 1
                    ? "1 file skipped — not readable text."
                    : $"{refused.Count} files skipped — not readable text.",
                Toast.ToastKind.Info);
        }

        /// <summary>
        /// Decides which explicitly chosen files may be added. Separated from
        /// <see cref="AddChosenFiles"/> so the policy can be tested without a window: the previous
        /// arrangement mixed the decision with a modal dialog, and the only way to test the
        /// all-refused case was to remove the dialog — which is how the feedback got lost.
        /// </summary>
        internal static (List<string> Accepted, List<SkippedFile> Refused) ScreenChosenFiles(
            IEnumerable<string> paths)
        {
            var accepted = new List<string>();
            var refused = new List<SkippedFile>();

            foreach (var path in paths)
            {
                var readability = BinaryFileDetector.Classify(path);
                if (readability == FileReadability.Text)
                {
                    accepted.Add(path);
                    continue;
                }

                refused.Add(new SkippedFile
                {
                    Path = path,
                    Reason = readability switch
                    {
                        FileReadability.Binary => SkipReason.Binary,
                        FileReadability.AccessDenied => SkipReason.AccessDenied,
                        _ => SkipReason.IoError,
                    },
                });
            }

            return (accepted, refused);
        }

        /// <summary>Names each refused file and why, and says what the tool can do instead.</summary>
        internal static string DescribeRefusals(IReadOnlyList<SkippedFile> refused)
        {
            var sb = new StringBuilder();

            foreach (var file in refused.Take(12))
            {
                sb.AppendLine(CultureInfo.CurrentCulture,
                    $"{Path.GetFileName(file.Path)} — {file.Reason switch
                    {
                        SkipReason.Binary => "binary file",
                        SkipReason.AccessDenied => "access denied",
                        _ => "could not be read",
                    }}");
            }

            if (refused.Count > 12)
                sb.AppendLine(CultureInfo.CurrentCulture, $"… and {refused.Count - 12} more");

            sb.AppendLine();
            sb.Append(refused.All(f => f.Reason == SkipReason.Binary)
                ? "A pack is plain text that gets pasted into a chat, so images, icons, "
                  + "executables and other binary files can't go in one."
                : "A pack is plain text, so only files that can be read as text can go in one.");

            return sb.ToString();
        }

        private void BtnAddMultipleFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select files to add";
                openFileDialog.Multiselect = true;

                // Build filter: each entry is "Description|pattern"
                var filters = new List<string>
            {
                "All Files (*.*)|*.*"
            };

                if (fileService.Extensions.Count > 0)
                {
                    string patterns = string.Join("", fileService.Extensions.Select(ext => $"*{ext};"));
                    patterns = patterns.TrimEnd(';');
                    filters.Add($"Configured Extensions|{patterns}");
                }

                // Common code file types
                filters.Add("C# (C-Sharp) (*.cs)|*.cs");
                filters.Add("Text Files (*.txt)|*.txt");
                filters.Add("XML Files (*.xml)|*.xml");
                filters.Add("JSON Files (*.json)|*.json");
                filters.Add("Markdown Files (*.md)|*.md");
                filters.Add("HTML Files (*.html;*.htm)|*.html;*.htm");
                filters.Add("CSS Files (*.css)|*.css");
                filters.Add("JavaScript Files (*.js)|*.js");
                filters.Add("Python Files (*.py)|*.py");
                filters.Add("Java Files (*.java)|*.java");
                filters.Add("C++ Files (*.cpp;*.h)|*.cpp;*.h");

                openFileDialog.Filter = string.Join("|", filters);

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    AddChosenFiles(openFileDialog.FileNames);
            }
        }

        private void BtnRemoveFile_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0)
            {
                // Resolve selected displayed paths (relative) back to full paths
                List<string> filesToRemove = new List<string>();
                foreach (var selectedItem in lstFiles.SelectedItems)
                {
                    string displayedRelativePath = selectedItem.ToString() ?? "";

                    string? actualFullPath = fileService.SelectedFiles.FirstOrDefault(f =>
                        (string.IsNullOrEmpty(fileService.FolderPath) && f.Equals(displayedRelativePath, StringComparison.OrdinalIgnoreCase))
                        ||
                        (!string.IsNullOrEmpty(fileService.FolderPath)
                            && GetRelativePath(fileService.FolderPath, f).Equals(displayedRelativePath, StringComparison.OrdinalIgnoreCase)));

                    if (actualFullPath != null)
                        filesToRemove.Add(actualFullPath);
                }

                if (filesToRemove.Count > 0)
                {
                    fileService.RemoveFiles(filesToRemove);
                    SyncUIWithService();
                }
            }
            else
            {
                AppMessage.Warning(this, "Nothing selected", "Select one or more files to remove.");
            }
        }

        private void BtnMoveUp_Click(object sender, EventArgs e)
        {
            int idx = lstFiles.SelectedIndex;
            if (idx > 0)
            {
                var files = fileService.SelectedFiles;
                var temp = files[idx - 1];
                files[idx - 1] = files[idx];
                files[idx] = temp;

                SyncUIWithService();
                lstFiles.SelectedIndex = idx - 1;
            }
        }

        private void BtnMoveDown_Click(object sender, EventArgs e)
        {
            int idx = lstFiles.SelectedIndex;
            var files = fileService.SelectedFiles;
            if (idx >= 0 && idx < files.Count - 1)
            {
                var temp = files[idx + 1];
                files[idx + 1] = files[idx];
                files[idx] = temp;

                SyncUIWithService();
                lstFiles.SelectedIndex = idx + 1;
            }
        }

        private void BtnCopyOutput_Click(object sender, EventArgs e)
        {
            if (PrepareOutputForRelease("Copy") is not { } pack) return;
            CopyToClipboard(pack, "Output copied to clipboard");
        }

        // -------------------- Copy as ▾ format menu --------------------
        //
        // Every entry gates first and converts second. Gating the converted text instead would
        // mean the scanner looking at Markdown fences or XML escaping rather than at the file
        // contents it was written against, and would need doing four times over.

        private void MnuCopyPlain_Click(object? sender, EventArgs e)
        {
            if (PrepareOutputForRelease("Copy") is not { } pack) return;
            CopyToClipboard(pack, "Copied as plain text");
        }

        private void MnuCopyMarkdown_Click(object? sender, EventArgs e)
        {
            if (PrepareOutputForRelease("Copy") is not { } pack) return;
            CopyToClipboard(OutputFormatter.ToMarkdown(pack), "Copied as Markdown");
        }

        private void MnuCopyXml_Click(object? sender, EventArgs e)
        {
            if (PrepareOutputForRelease("Copy") is not { } pack) return;
            CopyToClipboard(OutputFormatter.ToXmlClaude(pack), "Copied as XML");
        }

        private void MnuCopyJson_Click(object? sender, EventArgs e)
        {
            if (PrepareOutputForRelease("Copy") is not { } pack) return;
            CopyToClipboard(OutputFormatter.ToJsonArray(pack), "Copied as JSON");
        }

        /// <summary>
        /// Wraps the pack in a prompt template and copies the result.
        /// </summary>
        /// <remarks>
        /// The route to <c>OutputFormatter.ForClaudePrompt</c> and <c>ForChatGptPrompt</c>, which
        /// were complete and called from nowhere, and to the question field neither could ever
        /// receive.
        /// </remarks>
        private void MnuCopyAsPrompt_Click(object? sender, EventArgs e)
        {
            if (PrepareOutputForRelease("Copy") is not { } pack) return;

            using var dlg = new CodeShuttle.Dialogs.PromptComposerForm(_settings, pack);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            CopyToClipboard(dlg.ComposedPrompt, "Copied as prompt");
        }

        private void CopyToClipboard(string text, string statusMessage)
        {
            if (string.IsNullOrEmpty(text)) return;
            try
            {
                Clipboard.SetText(text);
                sbScanStatus.Text = $"{statusMessage} — ~{TokenEstimator.Estimate(text):N0} tokens";
            }
            catch (System.Runtime.InteropServices.ExternalException ex)
            {
                MessageBox.Show("Copy failed: " + ex.Message, "Copy",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // -------------------- the gate on everything that leaves --------------------

        /// <summary>
        /// The single checkpoint every copy and export passes through: credential review and the
        /// over-budget warning.
        /// </summary>
        /// <returns>
        /// The pack to release, redacted as the user chose, or <c>null</c> if they backed out.
        /// </returns>
        /// <remarks>
        /// It runs here rather than at Generate because generating is private to this machine and
        /// copying is the moment the content is about to be handed to a third party. Both callers
        /// go through this one method so the check cannot be bypassed by taking the other route —
        /// the failure mode of two implementations is one of them not running.
        /// </remarks>
        private string? PrepareOutputForRelease(string operation)
        {
            var pack = ApplySecretGate(rtbOutput.Text);
            if (pack == null) return null;

            if (!ConfirmOverBudget(pack, operation)) return null;
            return pack;
        }

        /// <summary>
        /// The credential half of the gate on its own: scan, then redact or ask, returning the
        /// content to use or <c>null</c> if the user backed out.
        /// </summary>
        /// <remarks>
        /// Split out of <see cref="PrepareOutputForRelease"/> because the compression path needs
        /// this check but not the token-budget one, and needs it against the <em>pre-compression</em>
        /// text. <see cref="SecretGuard.Scan"/> is a text scan: run against gzip+base64 it matches
        /// nothing and reports a clean pack, so compressing first and gating afterwards let
        /// credentials leave the machine unredacted — through a workflow the product itself
        /// teaches ("compress the pack so it fits the context window, then paste it").
        /// </remarks>
        private string? ApplySecretGate(string? content)
        {
            var pack = content;
            if (string.IsNullOrEmpty(pack)) return null;

            var matches = SecretGuard.Scan(pack);
            switch (SecretGuard.Decide(matches.Count, _settings.WarnOnSecrets, _settings.RedactSecrets))
            {
                case SecretGateAction.RedactSilently:
                    pack = SecretGuard.Redact(pack, matches);
                    Toast.Show(this, $"{matches.Count} credential(s) redacted.", Toast.ToastKind.Info);
                    break;

                case SecretGateAction.Ask:
                {
                    using var dlg = new CodeShuttle.Dialogs.SecretWarningForm(matches, _settings.RedactSecrets);
                    if (dlg.ShowDialog(this) != DialogResult.OK) return null;

                    var redacted = dlg.Redacted;
                    if (redacted.Count > 0) pack = SecretGuard.Redact(pack, redacted);
                    break;
                }

                case SecretGateAction.Pass:
                    break;
            }

            return pack;
        }

        /// <summary>
        /// Warns before releasing a pack larger than the selected context window.
        /// </summary>
        /// <remarks>
        /// Modal rather than a toast: pasting an over-budget pack into a chat wastes the whole
        /// round trip, and the user finds out at the far end. The estimate is labelled as an
        /// estimate here as it is everywhere else.
        /// </remarks>
        private bool ConfirmOverBudget(string pack, string operation)
        {
            int window = TokenBudget.WindowFor(CurrentTokenModel, _settings.CustomTokenBudget);
            int tokens = TokenEstimator.Estimate(pack);

            if (TokenBudget.Classify(tokens, window) != BudgetLevel.Over) return true;

            var answer = MessageBox.Show(this,
                $"This pack is about {tokens:N0} tokens, which is over the " +
                $"{CurrentTokenModel.Display} window of {window:N0}.\n\n" +
                TokenBudget.EstimateCaveat + "\n\n" +
                $"{operation} anyway?",
                "Over the token budget", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            return answer == DialogResult.Yes;
        }

        private async void BtnGenerate_Click(object sender, EventArgs e)
        {
            if (fileService.SelectedFiles.Count == 0)
            {
                AppMessage.Warning(this, "No files to generate",
                    "Add at least one file before generating.");
                return;
            }
            await ProcessFilesAsync();
        }

        private void LstFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                BtnRemoveFile_Click(sender, e);
                e.Handled = true;
            }
        }

        private void LstExtensions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lstExtensions.SelectedItems.Count > 0)
            {
                BtnRemove_Click(sender, e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Applies the output pane's own pack — the outbound-then-back-again case where the user
        /// pasted the AI's reply over the pane, or is re-applying a pack they still have open.
        /// </summary>
        private async void BtnApplyAiChanges_Click(object sender, EventArgs e)
        {
            if (rtbOutput.TextLength == 0)
            {
                AppMessage.Warning(this, "Nothing to apply",
                    "There is nothing in the output pane. Generate a pack, or use Paste AI response.");
                return;
            }

            var text = rtbOutput.Text;

            using var folderDialog = new FolderBrowserDialog
            {
                Description = "Select the folder these files belong to",
                UseDescriptionForTitle = true,
            };
            if (folderDialog.ShowDialog() != DialogResult.OK) return;

            var targetRoot = folderDialog.SelectedPath;

            RecreatePlan plan;
            try
            {
                plan = await Task.Run(() => FileRecreator.Plan(text, targetRoot));
            }
            catch (FormatException ex)
            {
                ReportApplyFailure(ex);
                return;
            }

            if (plan.Count == 0)
            {
                ReportNoFileEntries(text);
                return;
            }

            await ReviewAndApplyAsync(plan, targetRoot);
        }

        /// <summary>
        /// Opens the paste surface and, if it produced a plan, runs it through the same review
        /// and apply path as the output pane.
        /// </summary>
        /// <remarks>
        /// The dialog does the parsing and the containment validation, entirely through
        /// <c>BundleFormat</c> and <c>FileRecreator.Plan</c>. There is deliberately no second
        /// parse here: a new inbound surface that bypassed path containment would reintroduce the
        /// arbitrary-file-write defect exactly.
        /// </remarks>
        private async void BtnPasteResponse_Click(object? sender, EventArgs e)
        {
            RecreatePlan plan;
            string targetRoot;

            using (var dlg = new CodeShuttle.Dialogs.PasteResponseForm(fileService.FolderPath))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK || dlg.Plan == null) return;
                plan = dlg.Plan;
                targetRoot = dlg.TargetRoot;
            }

            await ReviewAndApplyAsync(plan, targetRoot);
        }

        /// <summary>
        /// One place both inbound routes report a failure from, rather than a near-identical
        /// dialog per entry point that would eventually drift apart.
        /// </summary>
        private void ReportApplyFailure(Exception ex) =>
            AppMessage.Error(this, "Apply AI changes", "The pack could not be applied.\n\n" + ex.Message, ex);

        /// <summary>
        /// Explains a plan that parsed cleanly but contained nothing to write.
        /// </summary>
        /// <remarks>
        /// The overwhelmingly common cause is that the pane holds a compressed or encrypted blob
        /// rather than a bundle — the user compressed the output, or pasted a blob back in, and
        /// the parser sees one long base64 line with no file headers in it. The old message said
        /// only "generate first", which is advice for a different problem entirely and sends the
        /// user off to regenerate a pack they already have.
        /// </remarks>
        private void ReportNoFileEntries(string output)
        {
            if (CompressionUtils.LooksLikeCompressedBase64(output))
            {
                AppMessage.Warning(this, "Output is compressed",
                    "The output pane holds a compressed pack, not file entries, so there is nothing to apply yet.\n\n"
                    + "Run Tools ▸ Decompress output first, then apply the changes.");
                return;
            }

            if (CompressionUtils.LooksLikeEncryptedBase64(output))
            {
                AppMessage.Warning(this, "Output is encrypted",
                    "The output pane holds an encrypted pack, not file entries, so there is nothing to apply yet.\n\n"
                    + "Run Tools ▸ Decrypt and decompress first, then apply the changes.");
                return;
            }

            AppMessage.Warning(this, "No file entries found",
                "The output pane does not contain any file entries, so there is nothing to apply.\n\n"
                + "A pack needs the \"" + BundleFormat.FileHeaderPrefix + "\" headers that Generate produces. "
                + "Generate a pack, or use Paste AI response to bring one in.");
        }

        /// <summary>
        /// The one review-and-write path, shared by both inbound routes, including the error
        /// reporting — so the paste surface cannot end up with a different, weaker story about a
        /// failed write than the output pane has.
        /// </summary>
        private async Task ReviewAndApplyAsync(RecreatePlan plan, string targetRoot)
        {
            // Re-checked here because this is the last point before anything is written, and the
            // duplicate-target refusal — two entries resolving to one file — lives behind it.
            if (!plan.CanProceed)
            {
                MessageBox.Show(this,
                    "This pack cannot be applied safely:\n\n" + string.Join("\n\n", plan.Errors),
                    "Apply AI Changes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using var diff = new CodeShuttle.Dialogs.DiffViewerForm(plan.Plans);
            if (diff.ShowDialog(this) != DialogResult.OK) return;

            var approved = diff.ApprovedPlans;
            if (approved.Count == 0) return;

            _applyCts?.Dispose();
            _applyCts = new CancellationTokenSource();

            ApplyReport report;
            btnApplyAiChanges.Enabled = false;
            btnPasteResponse.Enabled = false;
            BeginBusy(BusyOperation.Apply, "Applying changes…");
            try
            {
                var progress = new Progress<int>(p =>
                {
                    if (!IsDisposed) ReportBusyProgress(p);
                });

                // Every file that will be overwritten is copied into a timestamped backup set
                // first, and each write is staged through a temp file in the destination
                // directory — so a failure part-way cannot leave a half-rewritten source tree.
                report = await FileRecreator.ExecuteAsync(approved, targetRoot, progress, ct: _applyCts.Token);

                // The backup set could not be created, so the engine refused to write anything.
                // Ask before overwriting the user's files with no way back — and name the reason,
                // because "backups are off" and "%APPDATA% is full" call for different answers.
                if (report.BackupFailed && report.Results.Count == 0 && !report.Cancelled)
                {
                    if (!ConfirmApplyWithoutBackup(report.BackupError))
                    {
                        EndBusy("Apply cancelled — no backup could be created");
                        return;
                    }

                    report = await FileRecreator.ExecuteAsync(approved, targetRoot, progress,
                        allowUnbackedWrite: true, ct: _applyCts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Cancelling is a routine outcome the user just asked for, not an error.
                EndBusy("Apply cancelled");
                Toast.Show(this, "Apply was cancelled.", Toast.ToastKind.Info);
                return;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                EndBusy("Apply failed");
                ReportApplyFailure(ex);
                return;
            }
            finally
            {
                btnApplyAiChanges.Enabled = true;
                btnPasteResponse.Enabled = true;
            }

            EndBusy("Apply complete");
            ShowApplySummary(report);
        }

        /// <summary>
        /// Asks whether to overwrite files with no backup, after the backup set failed.
        /// </summary>
        /// <remarks>
        /// Modal and defaulting to No, deliberately. This is the one prompt in the product where
        /// "yes" means the writes about to happen cannot be undone, and a toast — which is what
        /// the failure used to produce, reading only "N file(s) written" — cannot carry that.
        /// </remarks>
        private bool ConfirmApplyWithoutBackup(string? reason)
        {
            var message =
                "The backup set could not be created, so nothing this apply overwrites could be restored.\n\n"
                + "Reason: " + (string.IsNullOrWhiteSpace(reason) ? "unknown" : reason) + "\n\n"
                + "Write the files anyway, with no way to undo?";

            return MessageBox.Show(this, message, "Apply AI Changes — no backup",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        /// <summary>
        /// Reports the outcome of an apply.
        /// </summary>
        /// <remarks>
        /// A clean run is a routine success and gets a toast; the backup location goes to the
        /// status bar, where it stays readable rather than vanishing with the toast. Anything
        /// that failed, was cut short, was written unprotected or had files skipped stays modal —
        /// those are the cases where the user has to know which files are in which state, and a
        /// transient notification is the wrong channel for it.
        /// </remarks>
        private void ShowApplySummary(ApplyReport report)
        {
            if (report.Failed == 0 && report.Skipped == 0 && !report.Cancelled && !report.BackupFailed)
            {
                Toast.Show(this, $"{report.Written} file(s) written.");
                sbScanStatus.Text = string.IsNullOrEmpty(report.BackupDirectory)
                    ? $"{report.Written} file(s) written."
                    : $"{report.Written} file(s) written — backups in {report.BackupDirectory}";
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine(CultureInfo.CurrentCulture, $"{report.Written} file(s) written.");
            if (report.Failed > 0) sb.AppendLine(CultureInfo.CurrentCulture, $"{report.Failed} file(s) failed.");
            if (report.Skipped > 0) sb.AppendLine(CultureInfo.CurrentCulture, $"{report.Skipped} file(s) skipped.");
            if (report.Cancelled) sb.AppendLine("The operation was cancelled before all files were written.");

            if (report.BackupFailed)
                sb.AppendLine().AppendLine("NO BACKUP WAS CREATED — these writes cannot be undone.")
                  .AppendLine(report.BackupError ?? "");
            else if (!string.IsNullOrEmpty(report.BackupDirectory))
                sb.AppendLine().AppendLine("Backups of the previous contents:").AppendLine(report.BackupDirectory);

            if (report.Skipped > 0)
            {
                sb.AppendLine().AppendLine("Skipped because they could not be backed up first:");
                foreach (var s in report.Results.Where(r => r.Outcome == ApplyOutcome.Skipped).Take(10))
                    sb.AppendLine(CultureInfo.CurrentCulture, $"  {s.TargetPath}");
            }

            if (report.Failed > 0)
            {
                sb.AppendLine().AppendLine("Failures:");
                foreach (var f in report.Results.Where(r => r.Outcome == ApplyOutcome.Failed).Take(10))
                    sb.AppendLine(CultureInfo.CurrentCulture, $"  {f.TargetPath} — {f.Error}");
            }

            MessageBox.Show(this, sb.ToString(), "Apply AI Changes", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void MiShowExtensionSummary_Click(object sender, EventArgs e)
        {
            ShowExtensionCounts(false);
        }

        // PnlRecreateInfo_Resize used to place the button by hand on every resize. The strip is a
        // TableLayoutPanel now, which positions its own cells, and the handler was already
        // unsubscribed — leaving it would have fought the layout the moment anyone rewired it.

        // New: Export Output
        private void BtnExportOutput_Click(object sender, EventArgs e)
        {
            // Export is the other way content leaves the machine, so it passes the same gate as
            // copy. Guarding only the clipboard would make "Export, then attach it" a bypass.
            if (PrepareOutputForRelease("Export") is not { } pack) return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                sfd.Title = "Export Output";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Unguarded before: with UnhandledExceptionMode.CatchException the failure
                        // was logged and swallowed, and the user believed the export had worked.
                        // Also honours the selected encoding rather than always UTF-8 no-BOM.
                        AtomicFile.WriteAllText(sfd.FileName, pack, selectedEncoding);
                        // A modal box to acknowledge something that already worked.
                        Toast.Show(this, "Output exported to " + System.IO.Path.GetFileName(sfd.FileName));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this,
                            $"The output could not be exported to:\n{sfd.FileName}\n\n{ex.Message}",
                            "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Async search: supports regex / case / whole-word toggles.
        // Records the search term in recent history and shows a match count.
        private async void BtnSearchFiles_Click(object sender, EventArgs e)
        {
            string searchTerm = searchBox.Query.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                AppMessage.Warning(this, "No search term", "Enter a search term.");
                return;
            }

            Regex regex;
            try
            {
                regex = BuildSearchRegex(searchTerm, searchBox.UseRegex, searchBox.MatchCase, searchBox.WholeWord);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("Invalid regular expression: " + ex.Message, "Search",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var files = fileService.SelectedFiles.ToList();
            var encoding = selectedEncoding;
            var service = fileService;

            searchBox.SearchEnabled = false;
            // Indeterminate: the loop below has no denominator to report against, so the staged
            // reveal shows the marquee only once the search is slow enough to be worth mentioning.
            BeginBusy(BusyOperation.Search, "Searching…");
            sbProgress.Style = ProgressBarStyle.Marquee;

            try
            {
                var result = await Task.Run(() =>
                {
                    var hits = new List<int>();
                    int totalMatches = 0;
                    for (int i = 0; i < files.Count; i++)
                    {
                        try
                        {
                            string content = service.ReadFileText(files[i], encoding);
                            int count = regex.Count(content);
                            if (count > 0)
                            {
                                hits.Add(i);
                                totalMatches += count;
                            }
                        }
                        catch { /* skip unreadable files */ }
                    }
                    return (Hits: hits, TotalMatches: totalMatches);
                });

                lstFiles.BeginUpdate();
                try
                {
                    lstFiles.ClearSelected();
                    foreach (var idx in result.Hits)
                    {
                        if (idx >= 0 && idx < lstFiles.Items.Count)
                            lstFiles.SetSelected(idx, true);
                    }
                }
                finally { lstFiles.EndUpdate(); }

                searchBox.MatchesText = result.Hits.Count == 0
                    ? "No matches"
                    : $"{result.TotalMatches:N0} match{(result.TotalMatches == 1 ? "" : "es")} in {result.Hits.Count:N0} file{(result.Hits.Count == 1 ? "" : "s")}";

                _settings.AddRecentSearch(searchTerm);
                _settings.Save();
            }
            finally
            {
                sbProgress.Style = ProgressBarStyle.Blocks;
                EndBusy("Search complete");
                searchBox.SearchEnabled = true;
            }
        }

        /// <summary>
        /// User-supplied regexes are untrusted input. A pattern like <c>(a+)+$</c> backtracks
        /// catastrophically, so literal searches use the non-backtracking engine and every
        /// pattern carries an explicit match timeout.
        /// </summary>
        internal static Regex BuildSearchRegex(string pattern, bool isRegex, bool matchCase, bool wholeWord)
        {
            var options = RegexOptions.Multiline;
            if (!matchCase) options |= RegexOptions.IgnoreCase;
            var rx = isRegex ? pattern : Regex.Escape(pattern);
            if (wholeWord) rx = $@"\b(?:{rx})\b";

            if (!isRegex)
            {
                // Escaped literals never need backtracking, so this can never blow up.
                try { return new Regex(rx, options | RegexOptions.NonBacktracking, RegexMatchTimeout); }
                catch (NotSupportedException) { /* fall through to the backtracking engine */ }
            }

            return new Regex(rx, options, RegexMatchTimeout);
        }

        internal static readonly TimeSpan RegexMatchTimeout = TimeSpan.FromSeconds(2);

        // New: Update Ignore Patterns
        private void TxtIgnorePatterns_TextChanged(object sender, EventArgs e)
        {
            fileService.IgnorePatterns.Clear();
            fileService.IgnorePatterns.AddRange(txtIgnorePatterns.Text.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));
            DebounceRefresh();
        }

        // New: Encoding Changed
        private void CmbEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Null-guarded: the combo could sit at SelectedIndex == -1 and this dereferenced it.
            switch (cmbEncoding.SelectedItem?.ToString())
            {
                case "ASCII": selectedEncoding = Encoding.ASCII; break;
                case "UTF-16": selectedEncoding = Encoding.Unicode; break;
                case "UTF-32": selectedEncoding = Encoding.UTF32; break;
                case "ISO-8859-1": selectedEncoding = Encoding.Latin1; break;
                default: selectedEncoding = Encoding.UTF8; break;
            }
            // Optionally re-process if needed, but here we leave it for next generate
        }

        // Background refresh: cancels any in-flight scan and starts a new one.
        // Only the latest scan updates the UI on completion.
        private async Task RefreshFilesInBackground()
        {
            _scanCts?.Cancel();
            var previous = _scanCts;
            var cts = new CancellationTokenSource();
            _scanCts = cts;
            previous?.Dispose();
            var ct = cts.Token;

            btnRefreshExtensions.Enabled = false;
            BeginBusy(BusyOperation.Scan, "Scanning…");

            var progress = new Progress<int>(p =>
            {
                if (!ct.IsCancellationRequested && !IsDisposed) ReportBusyProgress(p);
            });

            ScanResult? result = null;
            Exception? failure = null;
            try
            {
                result = await fileService.ScanAsync(progress, ct);
            }
            catch (OperationCanceledException) { /* superseded by a newer scan */ }
            catch (Exception ex) { failure = ex; }
            finally
            {
                // Every CTS is disposed, not just the winner's: under the 400 ms typing debounce
                // superseded sources used to accumulate quickly.
                if (cts != _scanCts) cts.Dispose();
            }

            // A superseded scan must neither publish its (older) results nor raise a dialog.
            if (cts != _scanCts || ct.IsCancellationRequested) return;

            if (failure != null)
            {
                MessageBox.Show("Scan failed: " + failure.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (result != null)
            {
                fileService.ApplyScanResult(result);
                SyncUIWithService();

                // The skip list has been produced by the scan all along; this is the first time
                // anything shows it.
                UpdateSkippedIndicator(result.Skipped);

                // What the rule editor measures its per-rule counts against: everything the scan
                // considered, including the files a rule excluded, since those are precisely the
                // ones the counts are about.
                _lastScanCandidates = result.Files
                    .Concat(result.Skipped.Where(s => s.Reason == SkipReason.IgnoredByRule).Select(s => s.Path))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }

            btnRefreshExtensions.Enabled = true;
            EndBusy(result?.RuleWarning ?? (failure != null ? "Scan failed" : "Scan complete"));
            UpdateEmptyStates();

            _scanCts = null;
            cts.Dispose();
        }

        #endregion

        #region Helpers

        private void SortFilesAndRebind(bool orderByExtension)
        {
            // Preserve currently selected items (by relative path as displayed)
            var selectedRel = lstFiles.SelectedItems
                .Cast<string>()
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Sort underlying list of FULL PATHS
            var sorted = orderByExtension
                ? fileService.SelectedFiles
                    .OrderBy(f => Path.GetExtension(f), StringComparer.OrdinalIgnoreCase)
                    .ThenBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
                    .ToList()
                : fileService.SelectedFiles
                    .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
                    .ToList();

            fileService.SelectedFiles.Clear();
            fileService.SelectedFiles.AddRange(sorted);

            // Rebind UI
            SyncUIWithService();

            // Reselect previously selected items by their relative path
            lstFiles.ClearSelected();
            for (int i = 0; i < lstFiles.Items.Count; i++)
            {
                var rel = lstFiles.Items[i]?.ToString() ?? string.Empty;
                if (selectedRel.Contains(rel))
                    lstFiles.SetSelected(i, true);
            }
        }

        private async Task ProcessFilesAsync()
        {
            var files = fileService.SelectedFiles.ToList();
            var encoding = selectedEncoding;

            _generateCts?.Dispose();
            _generateCts = new CancellationTokenSource();
            var ct = _generateCts.Token;

            btnGenerate.Enabled = false;
            BeginBusy(BusyOperation.Generate, "Generating…");

            var progress = new Progress<int>(p =>
            {
                if (!IsDisposed) ReportBusyProgress(p);
            });

            bool cancelled = false;
            bool failed = false;

            try
            {
                // Read files in PARALLEL into an indexed buffer, then assemble the output
                // sequentially so order matches the user's file list. Order matters for headers,
                // RTF styling offsets, and what the user sees in the output pane.
                var service = fileService;
                // Populated from the parallel read, so it has to be thread-safe.
                var unreadable = new System.Collections.Concurrent.ConcurrentBag<SkippedFile>();

                var built = await Task.Run(async () =>
                {
                    var contents = new string?[files.Count];
                    int done = 0;
                    int lastPct = -1;

                    int dop = Math.Min(Math.Max(2, Environment.ProcessorCount), 16);
                    await Parallel.ForEachAsync(
                        Enumerable.Range(0, files.Count),
                        new ParallelOptions { MaxDegreeOfParallelism = dop, CancellationToken = ct },
                        (i, token) =>
                        {
                            token.ThrowIfCancellationRequested();

                            // Screened here as well as when the file was added, because add-time
                            // screening only covers files added by this build: a list restored
                            // from a saved session or a preset, or built by an older version, has
                            // never been checked. Relying on the read throwing is not enough
                            // either — a binary whose bytes happen to decode produces no error and
                            // would go into the pack as garbage. An 8 KB sample per file, in
                            // parallel, against reading the whole file anyway.
                            var readability = BinaryFileDetector.Classify(files[i]);
                            if (readability != FileReadability.Text)
                            {
                                contents[i] = null;
                                unreadable.Add(new SkippedFile
                                {
                                    Path = files[i],
                                    Reason = readability switch
                                    {
                                        FileReadability.Binary => SkipReason.Binary,
                                        FileReadability.AccessDenied => SkipReason.AccessDenied,
                                        _ => SkipReason.IoError,
                                    },
                                });
                                int skippedPct = (int)((long)Interlocked.Increment(ref done) * 100 / Math.Max(1, files.Count));
                                ((IProgress<int>)progress).Report(skippedPct);
                                return ValueTask.CompletedTask;
                            }

                            try
                            {
                                contents[i] = service.ReadFileText(files[i], encoding);
                            }
                            catch (Exception ex)
                            {
                                // Left null so no entry is written for it. This used to store
                                // "[Error reading file: …]" as the file's content, which put a
                                // .NET exception message into the bundle where the source belongs
                                // — and since the bundle is designed to be applied back to disk,
                                // that string was one round trip away from overwriting the file
                                // it failed to read.
                                contents[i] = null;
                                unreadable.Add(new SkippedFile
                                {
                                    Path = files[i],
                                    Reason = ex is UnauthorizedAccessException
                                        ? SkipReason.AccessDenied
                                        : SkipReason.IoError,
                                    Detail = ex.Message,
                                });
                            }

                            // The progress bar was shown and then never updated once.
                            int n = Interlocked.Increment(ref done);
                            int pct = (int)((long)n * 100 / Math.Max(1, files.Count));
                            if (pct != Volatile.Read(ref lastPct))
                            {
                                Volatile.Write(ref lastPct, pct);
                                ((IProgress<int>)progress).Report(pct);
                            }
                            return ValueTask.CompletedTask;
                        });

                    // The framed bundle format states a line count per entry, so a source line
                    // that merely looks like a header can no longer split a file in two.
                    var entries = new List<BundleEntry>(files.Count);
                    for (int i = 0; i < files.Count; i++)
                    {
                        // A file that could not be read contributes no entry at all.
                        if (contents[i] is null) continue;

                        var (content, eol, eolMap, endsWithNewline) = BundleFormat.AnalyzeText(contents[i]!);
                        entries.Add(new BundleEntry
                        {
                            Path = files[i],
                            Content = content,
                            Eol = eol,
                            EolMap = eolMap,
                            EndsWithNewline = endsWithNewline,
                            EncodingToken = BundleFormat.TokenFor(encoding),
                            HasMetadata = true
                        });
                    }

                    var text = BundleFormat.Write(entries);

                    // Offsets are computed against the SAME string that is assigned to the
                    // RichTextBox. Computing them against text containing "\r\n" while the
                    // control's index space omits '\r' made the highlight drift per CRLF file.
                    var offsets = new List<(int Start, int Length)>(files.Count);
                    int search = 0;
                    for (int i = 0; i < entries.Count; i++)
                    {
                        var header = ">>>> file: " + entries[i].Path;
                        int at = text.IndexOf(header, search, StringComparison.Ordinal);
                        if (at < 0) continue;
                        offsets.Add((at, header.Length));
                        search = at + header.Length;
                    }

                    return (Text: text, Offsets: offsets);
                }, ct);

                // The read ran on a worker; by the time it returns the window may be closing. An
                // async continuation that goes on to touch the output pane re-creates the handle
                // of a control that is being disposed, which throws inside Dispose.
                if (_tearingDown || IsDisposed) return;

                if (built.Text.Length > MaxOutputChars)
                {
                    MessageBox.Show(this,
                        $"The assembled output is {built.Text.Length:N0} characters, which is too large to display " +
                        $"(the limit is {MaxOutputChars:N0}).\n\nRemove some files, or export a smaller selection.",
                        "Generate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // One big assignment, then style headers under suspended redraw so we only repaint once.
                //
                // Header styling is skipped above MaxHighlightChars. Each Select/SelectionColor/
                // SelectionFont triple makes the RTF engine reflow from the selection point, and
                // that cost grows with the size of the document — on a 1.1 MB pack the loop took
                // over a minute, during which the text was already on screen and the window looked
                // frozen and broken. Suppressing redraw does not help: the cost is in the engine,
                // not the painting. Bold accent headers are a nicety; a Generate button that
                // appears to do nothing for a minute is not a trade worth making.
                bool highlightHeaders = built.Text.Length <= MaxHighlightChars;

                SuspendDrawing(rtbOutput);
                try
                {
                    rtbOutput.Clear();
                    rtbOutput.Text = built.Text;

                    if (highlightHeaders)
                    {
                        using var headerFont = new Font(rtbOutput.Font, FontStyle.Bold);
                        var headerColor = ThemeManager.Tokens.AccentOnSurface;
                        foreach (var (start, length) in built.Offsets)
                        {
                            rtbOutput.Select(start, length);
                            rtbOutput.SelectionColor = headerColor;
                            rtbOutput.SelectionFont = headerFont;
                        }
                    }

                    rtbOutput.Select(0, 0);
                }
                finally
                {
                    ResumeDrawing(rtbOutput);
                }

                rtbOutput.ScrollToCaret();
                UpdateOutputStatistics();

                // Files left out of the pack are named, not dropped silently: a bundle that is
                // quietly missing files is the failure mode most corrosive to trust in a tool
                // sold on packing up a whole codebase.
                if (!unreadable.IsEmpty) UpdateSkippedIndicator(unreadable.ToList());
            }
            catch (OperationCanceledException)
            {
                cancelled = true;
            }
            catch (Exception ex)
            {
                failed = true;
                MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                EndBusy(cancelled ? "Generate cancelled"
                    : failed ? "Generate failed"
                    : rtbOutput.TextLength > MaxHighlightChars
                        ? "Output generated — headers not highlighted (large pack)"
                        : "Output generated");
                btnGenerate.Enabled = true;
                MarkFirstRunComplete();
            }
        }

        /// <summary>Cancels an in-flight Generate. WS4 wires this to a user-facing Cancel button.</summary>
        internal void CancelGenerate() => _generateCts?.Cancel();

        private static void SuspendDrawing(Control c)
        {
            SendMessage(c.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        }

        private static void ResumeDrawing(Control c)
        {
            SendMessage(c.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
            c.Invalidate();
        }

        // New: Update Output Statistics
        private void UpdateOutputStatistics()
        {
            // Reading Text from a RichTextBox whose handle has gone re-creates it, which during
            // teardown throws inside Dispose. See OutputReadable.
            if (!OutputReadable) return;

            string text = rtbOutput.Text;
            int charCount = text.Length;
            // Counting by allocating every line was wasteful and off by one on trailing newlines.
            int lineCount = CountLines(text);
            int byteSize = Encoding.UTF8.GetByteCount(text);
            int tokens = TokenEstimator.Estimate(text);
            lblOutputStats.Text =
                $"Chars: {charCount:N0} | Lines: {lineCount:N0} | Size: {byteSize:N0} bytes | ~{tokens:N0} tokens (estimate)";
            UpdateRecreateStrip();
            UpdateTokenBudget();
        }

        private void SyncUIWithService()
        {
            // The chips, the section counts and the output pane's empty state are not designer
            // controls, so they are refreshed alongside the ones that are.
            RefreshLayoutState();

            lstExtensions.BeginUpdate();
            try
            {
                lstExtensions.Items.Clear();
                if (fileService.Extensions.Count > 0)
                    lstExtensions.Items.AddRange(fileService.Extensions.Cast<object>().ToArray());
            }
            finally { lstExtensions.EndUpdate(); }

            lstFiles.BeginUpdate();
            try
            {
                lstFiles.Items.Clear();
                var folder = fileService.FolderPath;
                var hasFolder = !string.IsNullOrEmpty(folder);
                var items = new object[fileService.SelectedFiles.Count];
                for (int i = 0; i < fileService.SelectedFiles.Count; i++)
                {
                    var file = fileService.SelectedFiles[i];
                    items[i] = hasFolder ? GetRelativePath(folder, file) : file;
                }
                if (items.Length > 0)
                    lstFiles.Items.AddRange(items);
            }
            finally { lstFiles.EndUpdate(); }

            lblFileCount.Text = $"Files: {fileService.SelectedFiles.Count}";
            chkIncludeSubfolders.Checked = fileService.IncludeSubfolders;
            txtIgnorePatterns.Text = string.Join(", ", fileService.IgnorePatterns); // New

            UpdateStatusBar();
            UpdateEmptyStates();
        }

        internal static int CountLines(string? text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            int lines = 1;
            for (int i = 0; i < text.Length; i++)
                if (text[i] == '\n') lines++;
            // A trailing newline terminates the last line rather than starting a new one.
            if (text[text.Length - 1] == '\n') lines--;
            return lines;
        }

        private void UpdateStatusBar()
        {
            sbFileCount.Text = $"Files: {fileService.SelectedFiles.Count:N0}";

            // Sizes are captured during the scan. Re-stat'ing every file here ran on the UI
            // thread after every scan, add, remove, sort, drag-drop and preset load — minutes of
            // frozen UI on a network share or a OneDrive folder.
            sbTotalSize.Text = $"Size: {FormatBytes(fileService.TotalSelectedBytes)}";

            int skipped = fileService.SkippedFiles.Count;
            sbScanStatus.ToolTipText = skipped == 0
                ? string.Empty
                : $"{skipped:N0} file(s) skipped — " + string.Join(", ",
                    fileService.SkippedFiles.GroupBy(s => s.Reason).Select(g => $"{g.Count()} {g.Key}"));
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            string[] units = { "KB", "MB", "GB", "TB" };
            double size = bytes / 1024.0;
            int unit = 0;
            while (size >= 1024 && unit < units.Length - 1) { size /= 1024; unit++; }
            return $"{size:0.##} {units[unit]}";
        }

        private void ShowExtensionCounts(bool onlyConfigured)
        {
            if (string.IsNullOrEmpty(fileService.FolderPath)
                || !Directory.Exists(fileService.FolderPath))
            {
                AppMessage.Warning(this, "Folder path not set", "Set a valid folder path first.");
                return;
            }

            using (var dlg = new ExtensionCountsForm(fileService))
            {
                // dlg.ConfiguredOnly = onlyConfigured; // if you add such a property in the dialog
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    // Reflect any newly added extension(s)
                    SyncUIWithService();

                    // Optionally reselect added extensions
                    if (dlg.AddedExtensions?.Count > 0)
                    {
                        lstExtensions.ClearSelected();
                        foreach (var ext in dlg.AddedExtensions)
                        {
                            int idx = lstExtensions.Items.IndexOf(ext);
                            if (idx >= 0)
                                lstExtensions.SetSelected(idx, true);
                        }
                    }
                }
            }
        }

        private static string GetRelativePath(string basePath, string fullPath)
        {
            return Path.GetRelativePath(basePath, fullPath);
        }

        #endregion

        // -------------------- Drag-and-drop on lstFiles --------------------
        // Two roles: external FileDrop (add new files) AND internal reorder (move within the list).

        private const string ReorderFormat = "FCTKitFileReorder";

        private void LstFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(ReorderFormat) == true)
                e.Effect = DragDropEffects.Move;
            else if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void LstFiles_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(ReorderFormat) == true)
                e.Effect = DragDropEffects.Move;
            else if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void LstFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(ReorderFormat) == true)
            {
                // The 'is' pattern replaces both the cast and the null check.
                if (e.Data.GetData(ReorderFormat) is not int[] sources || sources.Length == 0) return;

                var clientPt = lstFiles.PointToClient(new Point(e.X, e.Y));
                int targetIndex = lstFiles.IndexFromPoint(clientPt);
                var list = fileService.SelectedFiles;
                if (targetIndex < 0) targetIndex = list.Count;

                int insertedAt = ReorderSelectedFiles(sources, targetIndex);
                SyncUIWithService();

                // Reselect the moved items at their new positions. This used to build a HashSet
                // of empty strings and throw it away, so a reorder silently lost the selection.
                lstFiles.BeginUpdate();
                try
                {
                    lstFiles.ClearSelected();
                    for (int i = insertedAt; i < insertedAt + sources.Length && i < lstFiles.Items.Count; i++)
                        lstFiles.SetSelected(i, true);
                }
                finally { lstFiles.EndUpdate(); }
                return;
            }

            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                // The 'is' pattern replaces both the cast and the null check.
                if (e.Data.GetData(DataFormats.FileDrop) is string[] dropped && dropped.Length > 0)
                {
                    // A dropped DIRECTORY used to be added as a file, producing an entry whose
                    // content read "[Error reading file: …]". Expand them instead.
                    var expanded = new List<string>();
                    foreach (var item in dropped)
                    {
                        try
                        {
                            if (Directory.Exists(item))
                                expanded.AddRange(fileService.EnumerateMatchingFiles(item));
                            else if (File.Exists(item))
                                expanded.Add(item);
                        }
                        catch { /* an unreadable drop item is simply not added */ }
                    }

                    AddChosenFiles(expanded);
                }
            }
        }

        /// <summary>Moves the selected rows to <paramref name="targetIndex"/>; returns where they ended up.</summary>
        private int ReorderSelectedFiles(int[] sourceIndices, int targetIndex)
        {
            var list = fileService.SelectedFiles;
            var valid = sourceIndices.Where(i => i >= 0 && i < list.Count).Distinct().OrderBy(i => i).ToArray();
            if (valid.Length == 0) return Math.Min(targetIndex, list.Count);

            // Snapshot moved items in their list order
            var moved = valid.Select(i => list[i]).ToList();

            // Count of moved items strictly BEFORE the drop point. Using "< targetIndex" counted
            // an item sitting exactly at the drop point, shifting the result by one whenever the
            // drop landed inside the selected block.
            int removedBefore = valid.Count(i => i < targetIndex);
            int insertAt = Math.Max(0, targetIndex - removedBefore);

            // Remove in reverse to keep indices valid
            for (int k = valid.Length - 1; k >= 0; k--)
                list.RemoveAt(valid[k]);

            if (insertAt > list.Count) insertAt = list.Count;
            list.InsertRange(insertAt, moved);
            return insertAt;
        }

        private void LstFiles_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            int idx = lstFiles.IndexFromPoint(e.Location);
            if (idx < 0) return;

            // Click on a non-selected row should select it before any drag starts.
            if (!lstFiles.SelectedIndices.Contains(idx))
            {
                lstFiles.ClearSelected();
                lstFiles.SetSelected(idx, true);
            }
        }

        private void LstFiles_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (lstFiles.SelectedIndices.Count == 0) return;
            var indices = lstFiles.SelectedIndices.Cast<int>().ToArray();
            var data = new DataObject(ReorderFormat, indices);
            lstFiles.DoDragDrop(data, DragDropEffects.Move);
        }

        // -------------------- Right-click context menu actions --------------------

        private IEnumerable<string> SelectedFullPaths()
        {
            var list = fileService.SelectedFiles;
            foreach (int idx in lstFiles.SelectedIndices)
                if (idx >= 0 && idx < list.Count) yield return list[idx];
        }

        private static readonly HashSet<string> ExecutableExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".exe", ".com", ".bat", ".cmd", ".ps1", ".psm1", ".vbs", ".vbe", ".js", ".jse",
            ".wsf", ".wsh", ".msi", ".msp", ".scr", ".cpl", ".hta", ".reg", ".lnk", ".pif", ".jar"
        };

        private void MiOpenFile_Click(object sender, EventArgs e)
        {
            var selected = SelectedFullPaths().Take(10).ToList(); // hard cap to avoid opening hundreds

            // "Open file" shell-executes. With the Shell/Scripts preset loaded, that RUNS the
            // selected .bat/.ps1/.exe rather than opening it, so executables need a confirmation.
            var executables = selected.Where(p => ExecutableExtensions.Contains(Path.GetExtension(p))).ToList();
            if (executables.Count > 0)
            {
                var names = string.Join("\n", executables.Take(5).Select(Path.GetFileName));
                var answer = MessageBox.Show(this,
                    $"{executables.Count} of the selected files can execute code:\n\n{names}\n\n" +
                    "Opening them will RUN them. Continue?",
                    "Open file", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (answer != DialogResult.Yes) return;
            }

            foreach (var path in selected)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not open '{path}':\n{ex.Message}", "Open file",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void MiRevealInExplorer_Click(object sender, EventArgs e)
        {
            foreach (var path in SelectedFullPaths().Take(10))
            {
                try
                {
                    // ArgumentList, not a hand-quoted string: a path containing a quote broke the
                    // command line and could smuggle in extra arguments.
                    var psi = new System.Diagnostics.ProcessStartInfo("explorer.exe") { UseShellExecute = false };
                    psi.ArgumentList.Add("/select,");
                    psi.ArgumentList.Add(path);
                    System.Diagnostics.Process.Start(psi);
                }
                catch { /* ignore */ }
            }
        }

        private void MiOpenContainingFolder_Click(object sender, EventArgs e)
        {
            var folders = SelectedFullPaths()
                .Select(p => System.IO.Path.GetDirectoryName(p))
                .Where(d => !string.IsNullOrEmpty(d))
                .Distinct()
                .Take(5);
            foreach (var folder in folders)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = folder!,
                        UseShellExecute = true
                    });
                }
                catch { /* ignore */ }
            }
        }

        private void MiCopyPath_Click(object sender, EventArgs e)
        {
            var joined = string.Join(Environment.NewLine, SelectedFullPaths());
            if (!string.IsNullOrEmpty(joined) && !TrySetClipboardText(joined))
            {
                MessageBox.Show(this, "The clipboard is in use by another application. Try again in a moment.",
                    "Copy path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}