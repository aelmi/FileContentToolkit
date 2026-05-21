using FileContentToolkit.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace FileContentToolkit
{
    public partial class MainForm : Form
    {
        private FileContentService fileService = new FileContentService();
        private Encoding selectedEncoding = Encoding.UTF8; // Default UTF-8

        private CancellationTokenSource? _scanCts;
        private readonly System.Windows.Forms.Timer _refreshDebounce;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x000B;

        public MainForm()
        {
            InitializeComponent();

            // Optional: keep these visual behaviors in code-behind
            SetupHoverEffects();

            // ToolTips are defined in Designer now; remove this if you moved them there:
            // SetupToolTips();

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

            // New: Populate extension suggestions
            cmbExtension.Items.AddRange(new string[] { ".cs", ".txt", ".xml", ".json", ".md", ".html", ".css", ".js", ".py", ".java", ".cpp" });
            cmbExtension.DropDownStyle = ComboBoxStyle.DropDown;

            // New: Populate encodings
            cmbEncoding.Items.AddRange(new object[] { "UTF-8", "ASCII", "UTF-16", "UTF-32", "ISO-8859-1" });
            cmbEncoding.SelectedIndex = 0; // Default UTF-8

            // Feature toolbar (recent folders, options, presets, watch, search toggles, find/replace)
            InitExtraFeatures();

            // Bind each language-preset menu item to its extension list (data lives in LanguagePresets.All).
            WireLanguagePresets();
        }

        private void WireLanguagePresets()
        {
            var items = new[] {
                mnuLangCs, mnuLangCpp, mnuLangWeb, mnuLangTs, mnuLangNode, mnuLangPy,
                mnuLangJava, mnuLangKotlin, mnuLangGo, mnuLangRust, mnuLangRuby,
                mnuLangPhp, mnuLangSwift, mnuLangShell, mnuLangDocs, mnuLangConfig
            };
            for (int i = 0; i < items.Length && i < LanguagePresets.All.Length; i++)
            {
                items[i].Tag = LanguagePresets.All[i].Extensions;
                items[i].ToolTipText = string.Join(" ", LanguagePresets.All[i].Extensions);
            }
        }

        private void MnuLanguagePreset_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem mi && mi.Tag is string[] exts)
            {
                fileService.Extensions.Clear();
                BulkAddExtensions(exts);
            }
        }

        private void BulkAddExtensions(string[] exts)
        {
            if (exts == null || exts.Length == 0) return;

            int added = 0, skipped = 0;
            foreach (var raw in exts)
            {
                var ext = raw.Trim();
                if (string.IsNullOrEmpty(ext)) continue;
                if (!ext.StartsWith(".")) ext = "." + ext;

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

        #region UI Polishing (Hover Effects)

        private void SetupHoverEffects()
        {
            AddHoverEffect(btnBrowse, Color.FromArgb(51, 122, 183));
            AddHoverEffect(btnAddFolder, Color.FromArgb(40, 167, 69));
            AddHoverEffect(btnAdd, Color.FromArgb(51, 122, 183));
            AddHoverEffect(btnRemove, Color.FromArgb(220, 53, 69));
            AddHoverEffect(btnAddMultipleFiles, Color.FromArgb(40, 167, 69));
            AddHoverEffect(btnRemoveFile, Color.FromArgb(220, 53, 69));
            AddHoverEffect(btnGenerate, Color.FromArgb(0, 123, 255));
            AddHoverEffect(btnRefreshExtensions, Color.FromArgb(51, 122, 183));
            AddHoverEffect(btnMoveUp, Color.FromArgb(233, 236, 239));
            AddHoverEffect(btnMoveDown, Color.FromArgb(233, 236, 239));
            AddHoverEffect(btnRecreateFiles, Color.FromArgb(40, 167, 69));
            AddHoverEffect(btnExportOutput, Color.FromArgb(13, 110, 253)); // New
            AddHoverEffect(btnSearchFiles, Color.FromArgb(108, 117, 125)); // New
        }

        private void AddHoverEffect(System.Windows.Forms.Button button, Color originalColor)
        {
            button.MouseEnter += (s, e) => button.BackColor = ControlPaint.Light(originalColor, 0.2f);
            button.MouseLeave += (s, e) => button.BackColor = originalColor;
        }

        // Updated with new buttons
        private void AddHoverEffects()
        {
            // For Copy, Edit, Export buttons
            btnCopyOutput.MouseEnter += (s, e) => btnCopyOutput.BackColor = Color.FromArgb(230, 230, 230);
            btnCopyOutput.MouseLeave += (s, e) => btnCopyOutput.BackColor = Color.FromArgb(248, 249, 250);

            btnEditOutput.MouseEnter += (s, e) => btnEditOutput.BackColor = Color.FromArgb(230, 230, 230);
            btnEditOutput.MouseLeave += (s, e) => btnEditOutput.BackColor = Color.FromArgb(248, 249, 250);

            btnExportOutput.MouseEnter += (s, e) => btnExportOutput.BackColor = Color.FromArgb(230, 230, 230);
            btnExportOutput.MouseLeave += (s, e) => btnExportOutput.BackColor = Color.FromArgb(248, 249, 250);

            // For compression buttons - slightly darker on hover
            btnCompress.MouseEnter += (s, e) => btnCompress.BackColor = Color.FromArgb(10, 88, 202);
            btnCompress.MouseLeave += (s, e) => btnCompress.BackColor = Color.FromArgb(13, 110, 253);

            btnDecompress.MouseEnter += (s, e) => btnDecompress.BackColor = Color.FromArgb(90, 98, 104);
            btnDecompress.MouseLeave += (s, e) => btnDecompress.BackColor = Color.FromArgb(108, 117, 125);

            btnCompressEnc.MouseEnter += (s, e) => btnCompressEnc.BackColor = Color.FromArgb(21, 115, 71);
            btnCompressEnc.MouseLeave += (s, e) => btnCompressEnc.BackColor = Color.FromArgb(25, 135, 84);

            btnDecompressEnc.MouseEnter += (s, e) => btnDecompressEnc.BackColor = Color.FromArgb(187, 45, 59);
            btnDecompressEnc.MouseLeave += (s, e) => btnDecompressEnc.BackColor = Color.FromArgb(220, 53, 69);

            // New: Search button
            btnSearchFiles.MouseEnter += (s, e) => btnSearchFiles.BackColor = Color.FromArgb(90, 98, 104);
            btnSearchFiles.MouseLeave += (s, e) => btnSearchFiles.BackColor = Color.FromArgb(108, 117, 125);
        }

        #endregion

        #region (Optional) ToolTips if kept in code-behind
        // If you moved tooltip strings to Designer, you can delete this method and the call.
        private void SetupToolTips()
        {
            // IMPORTANT: Do NOT instantiate toolTip1 here anymore because the Designer owns it.
            // toolTip1 = new System.Windows.Forms.ToolTip();

            toolTip1.SetToolTip(btnBrowse, "Browse for a folder");
            toolTip1.SetToolTip(btnAdd, "Add a file extension to the list");
            toolTip1.SetToolTip(btnRemove, "Remove the selected file extension");
            toolTip1.SetToolTip(btnRefreshExtensions, "Refresh the file list for the selected extensions");
            toolTip1.SetToolTip(btnAddMultipleFiles, "Add one or more files manually to the selected files list");
            toolTip1.SetToolTip(btnRemoveFile, "Remove the selected file from the list");
            toolTip1.SetToolTip(btnMoveUp, "Move the selected file up");
            toolTip1.SetToolTip(btnMoveDown, "Move the selected file down");
            toolTip1.SetToolTip(btnGenerate, "Read and display the contents of the selected files");
            toolTip1.SetToolTip(btnCopyOutput, "Copy the output to the clipboard");
            toolTip1.SetToolTip(chkIncludeSubfolders, "Include files from subfolders");
            toolTip1.SetToolTip(btnEditOutput, "Edit the output");
            toolTip1.SetToolTip(btnRecreateFiles, "Recreate files and folders from the output below");
            toolTip1.SetToolTip(txtIgnorePatterns, "Comma-separated ignore patterns (e.g., *.tmp, bin/)"); // New
            toolTip1.SetToolTip(btnExportOutput, "Export output to file"); // New
            toolTip1.SetToolTip(txtSearchFiles, "Search term for file contents"); // New
            toolTip1.SetToolTip(btnSearchFiles, "Search in selected files"); // New
            toolTip1.SetToolTip(cmbEncoding, "Select file encoding (default UTF-8)"); // New
        }
        #endregion

        #region Event Handlers (wired from Designer)

        private void BtnEditOutput_Click(object sender, EventArgs e)
        {
            rtbOutput.ReadOnly = !rtbOutput.ReadOnly;

            if (rtbOutput.ReadOnly)
            {
                btnEditOutput.BackColor = Color.Transparent;
                toolTip1.SetToolTip(btnEditOutput, "Edit the output");
            }
            else
            {
                btnEditOutput.BackColor = Color.LightYellow;
                toolTip1.SetToolTip(btnEditOutput, "Click to finish editing");
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

        private void BtnAddFolder_Click(object sender, EventArgs e)
        {
            if (fileService.Extensions.Count == 0)
            {
                MessageBox.Show(
                    "Add at least one file extension before adding a folder, so the scan knows which files to include.",
                    "No Extensions Configured",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select another folder — its matching files will be appended to the list";
                if (folderDialog.ShowDialog() != DialogResult.OK) return;

                var matches = fileService.EnumerateMatchingFiles(folderDialog.SelectedPath);
                if (matches.Count == 0)
                {
                    MessageBox.Show(this,
                        "No files in that folder matched the configured extensions and filters.",
                        "Add Folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void BtnCompress_Click(object sender, EventArgs e)
        {
            try
            {
                var input = rtbOutput.Text ?? string.Empty;
                var compressed = CompressionUtils.CompressToBase64(input);
                rtbOutput.ReadOnly = false;
                rtbOutput.Text = compressed;
                rtbOutput.ReadOnly = true;

                MessageBox.Show(
                    $"Compressed with GZip → Base64.\n\nOriginal: {input.Length:N0} chars\nCompressed: {compressed.Length:N0} chars",
                    "Compression", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Compression failed: " + ex.Message, "Compression",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDecompress_Click(object sender, EventArgs e)
        {
            var base64 = rtbOutput.Text ?? string.Empty;
            if (CompressionUtils.TryDecompressFromBase64(base64, out var text, out var error))
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

        private void BtnCompressEnc_Click(object sender, EventArgs e)
        {
            string pwd;

            using (var dialog = new PasswordDialog("Enter password for encryption"))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    pwd = dialog.Password;
                }
                else
                {
                    return; // cancelled
                }
            }

            if (string.IsNullOrEmpty(pwd)) return; // no password entered

            try
            {
                var input = rtbOutput.Text ?? string.Empty;
                var sealedBase64 = CompressionUtils.CompressAndEncryptToBase64(input, pwd);
                rtbOutput.ReadOnly = false;
                rtbOutput.Text = sealedBase64;
                rtbOutput.ReadOnly = true;

                MessageBox.Show("Compressed and encrypted (AES-GCM) successfully.",
                    "Secure Compression", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Secure compression failed: " + ex.Message, "Secure Compression",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDecompressEnc_Click(object sender, EventArgs e)
        {
            string pwd;

            using (var dialog = new PasswordDialog("Enter password for decryption"))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    pwd = dialog.Password;
                }
                else
                {
                    return; // cancelled
                }
            }

            if (string.IsNullOrEmpty(pwd)) return; // no password entered

            try
            {
                var input = rtbOutput.Text ?? string.Empty;

                if (CompressionUtils.TryDecryptAndDecompressFromBase64(input, pwd, out string decrypted, out string error))
                {
                    rtbOutput.ReadOnly = false;
                    rtbOutput.Text = decrypted;
                    rtbOutput.ReadOnly = true;

                    MessageBox.Show("Decrypted and decompressed (AES-GCM) successfully.",
                        "Secure Decompression", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        private void TxtFolderPath_TextChanged(object sender, EventArgs e)
        {
            fileService.SetFolderPath(txtFolderPath.Text);
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
                MessageBox.Show("Please enter a file extension.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!extension.StartsWith("."))
                extension = "." + extension;

            if (fileService.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show("This extension is already in the list.", "Duplicate Extension",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Please select one or more extensions to remove.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                {
                    fileService.AddFiles(openFileDialog.FileNames);
                    SyncUIWithService();
                }
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
                    string displayedRelativePath = selectedItem.ToString();

                    string actualFullPath = fileService.SelectedFiles.FirstOrDefault(f =>
                        (string.IsNullOrEmpty(fileService.FolderPath) && f.Equals(displayedRelativePath, StringComparison.OrdinalIgnoreCase))
                        ||
                        (!string.IsNullOrEmpty(fileService.FolderPath)
                            && GetRelativePath(fileService.FolderPath, f).Equals(displayedRelativePath, StringComparison.OrdinalIgnoreCase)));

                    if (actualFullPath != null)
                        filesToRemove.Add(actualFullPath);
                }

                if (filesToRemove.Any())
                {
                    fileService.RemoveFiles(filesToRemove);
                    SyncUIWithService();
                }
            }
            else
            {
                MessageBox.Show("Please select one or more files to remove.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            CopyToClipboard(rtbOutput.Text, "Output copied to clipboard");
        }

        // -------------------- Copy as ▾ format menu --------------------

        private void MnuCopyPlain_Click(object? sender, EventArgs e)
            => CopyToClipboard(rtbOutput.Text, "Copied as plain text");

        private void MnuCopyMarkdown_Click(object? sender, EventArgs e)
            => CopyToClipboard(OutputFormatter.ToMarkdown(rtbOutput.Text), "Copied as Markdown");

        private void MnuCopyXml_Click(object? sender, EventArgs e)
            => CopyToClipboard(OutputFormatter.ToXmlClaude(rtbOutput.Text), "Copied as XML");

        private void MnuCopyJson_Click(object? sender, EventArgs e)
            => CopyToClipboard(OutputFormatter.ToJsonArray(rtbOutput.Text), "Copied as JSON");

        private void CopyToClipboard(string text, string statusMessage)
        {
            if (string.IsNullOrEmpty(text)) return;
            try
            {
                Clipboard.SetText(text);
                sbScanStatus.Text = $"{statusMessage} — ~{TokenEstimator.Estimate(text):N0} tokens";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Copy failed: " + ex.Message, "Copy",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnGenerate_Click(object sender, EventArgs e)
        {
            if (fileService.SelectedFiles.Count == 0)
            {
                MessageBox.Show("Please add at least one file to process.", "Files Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void BtnRecreateFiles_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog
            {
                Description = "Select a folder to recreate files in"
            };
            if (folderDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                var plans = FileRecreator.Plan(rtbOutput.Text, folderDialog.SelectedPath);
                if (plans.Count == 0)
                {
                    MessageBox.Show(this,
                        "No file headers found in the output. Generate first, then try again.",
                        "Recreate Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var dlg = new FileContentToolkit.Dialogs.DiffViewerForm(plans);
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                int count = FileRecreator.Execute(dlg.ApprovedPlans);
                MessageBox.Show(this, $"{count} file(s) written successfully.",
                    "Recreate Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error: " + ex.Message,
                    "Recreate Files", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MiShowExtensionSummary_Click(object sender, EventArgs e)
        {
            ShowExtensionCounts(false);
        }

        private void PnlRecreateInfo_Resize(object sender, EventArgs e)
        {
            // Keep the button aligned at the right, vertically centered
            btnRecreateFiles.Left = pnlRecreateInfo.Width - btnRecreateFiles.Width - 20;
            btnRecreateFiles.Top = (pnlRecreateInfo.Height - btnRecreateFiles.Height) / 2;
        }

        // New: Export Output
        private void BtnExportOutput_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                sfd.Title = "Export Output";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, rtbOutput.Text);
                    MessageBox.Show("Output exported successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Async search: supports regex / case / whole-word toggles.
        // Records the search term in recent history and shows a match count.
        private async void BtnSearchFiles_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearchFiles.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Enter a search term.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Regex regex;
            try
            {
                regex = BuildSearchRegex(searchTerm, chkRegex.Checked, chkCase.Checked, chkWord.Checked);
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

            btnSearchFiles.Enabled = false;
            progressBar.Visible = true;
            progressBar.Value = 0;

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
                            int count = regex.Matches(content).Count;
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

                lblSearchMatches.Text = result.Hits.Count == 0
                    ? "No matches"
                    : $"{result.TotalMatches:N0} match{(result.TotalMatches == 1 ? "" : "es")} in {result.Hits.Count:N0} file{(result.Hits.Count == 1 ? "" : "s")}";

                _settings.AddRecentSearch(searchTerm);
                _settings.Save();
            }
            finally
            {
                progressBar.Visible = false;
                btnSearchFiles.Enabled = true;
            }
        }

        private static Regex BuildSearchRegex(string pattern, bool isRegex, bool matchCase, bool wholeWord)
        {
            var options = RegexOptions.Multiline;
            if (!matchCase) options |= RegexOptions.IgnoreCase;
            var rx = isRegex ? pattern : Regex.Escape(pattern);
            if (wholeWord) rx = $@"\b(?:{rx})\b";
            return new Regex(rx, options);
        }

        // New: Update Ignore Patterns
        private void TxtIgnorePatterns_TextChanged(object sender, EventArgs e)
        {
            fileService.IgnorePatterns.Clear();
            fileService.IgnorePatterns.AddRange(txtIgnorePatterns.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));
            DebounceRefresh();
        }

        // New: Encoding Changed
        private void CmbEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbEncoding.SelectedItem.ToString())
            {
                case "UTF-8": selectedEncoding = Encoding.UTF8; break;
                case "ASCII": selectedEncoding = Encoding.ASCII; break;
                case "UTF-16": selectedEncoding = Encoding.Unicode; break;
                case "UTF-32": selectedEncoding = Encoding.UTF32; break;
                case "ISO-8859-1": selectedEncoding = Encoding.GetEncoding("ISO-8859-1"); break;
            }
            // Optionally re-process if needed, but here we leave it for next generate
        }

        // Background refresh: cancels any in-flight scan and starts a new one.
        // Only the latest scan updates the UI on completion.
        private async Task RefreshFilesInBackground()
        {
            _scanCts?.Cancel();
            var cts = new CancellationTokenSource();
            _scanCts = cts;
            var ct = cts.Token;

            progressBar.Visible = true;
            progressBar.Value = 0;
            btnRefreshExtensions.Enabled = false;
            sbScanStatus.Text = "Scanning…";

            var progress = new Progress<int>(p =>
            {
                if (!ct.IsCancellationRequested && !progressBar.IsDisposed)
                {
                    progressBar.Value = Math.Min(100, Math.Max(0, p));
                    sbScanStatus.Text = $"Scanning… {progressBar.Value}%";
                }
            });

            try
            {
                await fileService.RefreshFilesAsync(progress, ct);
            }
            catch (OperationCanceledException) { /* superseded by a newer scan */ }
            catch (Exception ex)
            {
                MessageBox.Show("Scan failed: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (cts == _scanCts)
            {
                if (!ct.IsCancellationRequested)
                    SyncUIWithService();

                progressBar.Visible = false;
                btnRefreshExtensions.Enabled = true;
                sbScanStatus.Text = ct.IsCancellationRequested ? "Scan cancelled" : "Scan complete";
                _scanCts = null;
                cts.Dispose();
            }
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

            btnGenerate.Enabled = false;
            progressBar.Visible = true;
            progressBar.Value = 0;

            try
            {
                // Read files in PARALLEL into an indexed buffer, then assemble the output
                // sequentially so order matches the user's file list. Order matters for headers,
                // RTF styling offsets, and what the user sees in the output pane.
                var service = fileService;
                var built = await Task.Run(async () =>
                {
                    var contents = new string[files.Count];

                    int dop = Math.Min(Math.Max(2, Environment.ProcessorCount), 16);
                    await Parallel.ForEachAsync(
                        Enumerable.Range(0, files.Count),
                        new ParallelOptions { MaxDegreeOfParallelism = dop },
                        (i, _) =>
                        {
                            try
                            {
                                contents[i] = service.ReadFileText(files[i], encoding);
                            }
                            catch (Exception ex)
                            {
                                contents[i] = $"[Error reading file: {ex.Message}]";
                            }
                            return ValueTask.CompletedTask;
                        });

                    var sb = new StringBuilder();
                    var offsets = new List<(int Start, int Length)>(files.Count);
                    for (int i = 0; i < files.Count; i++)
                    {
                        var path = files[i];
                        var header = path + ":";
                        offsets.Add((sb.Length, header.Length));
                        sb.Append(header);
                        sb.Append('\n');
                        sb.Append(contents[i]);
                        if (i < files.Count - 1)
                            sb.Append("\n\n\n\n");
                    }
                    return (Text: sb.ToString(), Offsets: offsets);
                });

                // One big assignment, then style headers under suspended redraw so we only repaint once.
                SuspendDrawing(rtbOutput);
                try
                {
                    rtbOutput.Clear();
                    rtbOutput.Text = built.Text;

                    using var headerFont = new Font(rtbOutput.Font, FontStyle.Bold);
                    var headerColor = Color.FromArgb(0, 102, 204);
                    foreach (var (start, length) in built.Offsets)
                    {
                        rtbOutput.Select(start, length);
                        rtbOutput.SelectionColor = headerColor;
                        rtbOutput.SelectionFont = headerFont;
                    }

                    rtbOutput.Select(0, 0);
                }
                finally
                {
                    ResumeDrawing(rtbOutput);
                }

                rtbOutput.ScrollToCaret();
                UpdateOutputStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Visible = false;
                btnGenerate.Enabled = true;
            }
        }

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
            string text = rtbOutput.Text;
            int charCount = text.Length;
            int lineCount = text.Split('\n').Length;
            int byteSize = Encoding.UTF8.GetByteCount(text);
            int tokens = TokenEstimator.Estimate(text);
            lblOutputStats.Text =
                $"Chars: {charCount:N0} | Lines: {lineCount:N0} | Size: {byteSize:N0} bytes | ~{tokens:N0} tokens";
        }

        private void SyncUIWithService()
        {
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
        }

        private void UpdateStatusBar()
        {
            sbFileCount.Text = $"Files: {fileService.SelectedFiles.Count:N0}";

            long total = 0;
            foreach (var f in fileService.SelectedFiles)
            {
                try { total += new System.IO.FileInfo(f).Length; }
                catch { /* skip unreadable */ }
            }
            sbTotalSize.Text = $"Size: {FormatBytes(total)}";
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
                MessageBox.Show("Please set a valid folder path first.", "Folder Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (e.Data.GetDataPresent(ReorderFormat))
                e.Effect = DragDropEffects.Move;
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void LstFiles_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(ReorderFormat))
                e.Effect = DragDropEffects.Move;
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void LstFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(ReorderFormat))
            {
                var sources = (int[])e.Data.GetData(ReorderFormat);
                if (sources == null || sources.Length == 0) return;

                var clientPt = lstFiles.PointToClient(new Point(e.X, e.Y));
                int targetIndex = lstFiles.IndexFromPoint(clientPt);
                var list = fileService.SelectedFiles;
                if (targetIndex < 0) targetIndex = list.Count;

                ReorderSelectedFiles(sources, targetIndex);
                SyncUIWithService();

                // Reselect the moved items at their new positions
                lstFiles.BeginUpdate();
                try
                {
                    lstFiles.ClearSelected();
                    var movedSet = new HashSet<string>(
                        sources.Where(i => i >= 0 && i < list.Count + sources.Length)
                               .Select(_ => "")); // placeholder; we'll reselect by new index below
                }
                finally { lstFiles.EndUpdate(); }
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    fileService.AddFiles(files);
                    SyncUIWithService();
                }
            }
        }

        private void ReorderSelectedFiles(int[] sourceIndices, int targetIndex)
        {
            var list = fileService.SelectedFiles;
            // Snapshot moved items in their selected order
            var moved = sourceIndices
                .Where(i => i >= 0 && i < list.Count)
                .OrderBy(i => i)
                .Select(i => list[i])
                .ToList();

            // Adjust target so it points to the insertion slot AFTER we remove the moved items
            int removedBefore = sourceIndices.Count(i => i >= 0 && i < targetIndex);
            int insertAt = Math.Max(0, targetIndex - removedBefore);

            // Remove in reverse to keep indices valid
            foreach (var i in sourceIndices.Where(i => i >= 0 && i < list.Count).OrderByDescending(i => i))
                list.RemoveAt(i);

            if (insertAt > list.Count) insertAt = list.Count;
            list.InsertRange(insertAt, moved);
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

        private void MiOpenFile_Click(object sender, EventArgs e)
        {
            foreach (var path in SelectedFullPaths().Take(10)) // hard cap to avoid opening hundreds
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
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{path}\"");
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
            if (!string.IsNullOrEmpty(joined))
                Clipboard.SetText(joined);
        }
    }
}