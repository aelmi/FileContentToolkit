using FileContentToolkit.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace FileContentToolkit
{
    public partial class MainForm : Form
    {
        private FileContentService fileService = new FileContentService();
        private BackgroundWorker bgWorker; // For background scan
        private Encoding selectedEncoding = Encoding.UTF8; // Default UTF-8

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

            // New: Setup background worker for scan
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += BgWorker_DoWork;
            bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
            bgWorker.ProgressChanged += BgWorker_ProgressChanged;
            bgWorker.WorkerReportsProgress = true;

            // New: Populate extension suggestions
            cmbExtension.Items.AddRange(new string[] { ".cs", ".txt", ".xml", ".json", ".md", ".html", ".css", ".js", ".py", ".java", ".cpp" });
            cmbExtension.DropDownStyle = ComboBoxStyle.DropDown;

            // New: Populate encodings
            cmbEncoding.Items.AddRange(new object[] { "UTF-8", "ASCII", "UTF-16", "UTF-32", "ISO-8859-1" });
            cmbEncoding.SelectedIndex = 0; // Default UTF-8
        }

        #region UI Polishing (Hover Effects)

        private void SetupHoverEffects()
        {
            AddHoverEffect(btnBrowse, Color.FromArgb(51, 122, 183));
            AddHoverEffect(btnAdd, Color.FromArgb(51, 122, 183));
            AddHoverEffect(btnRemove, Color.FromArgb(220, 53, 69));
            AddHoverEffect(btnAddMultipleFiles, Color.FromArgb(40, 167, 69));
            AddHoverEffect(btnRemoveFile, Color.FromArgb(220, 53, 69));
            AddHoverEffect(btnGenerate, Color.FromArgb(0, 123, 255));
            AddHoverEffect(btnRefreshExtensions, Color.FromArgb(51, 122, 183));
            AddHoverEffect(btnMoveUp, Color.LightGray);
            AddHoverEffect(btnMoveDown, Color.LightGray);
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

        private string PromptForPassword(string title)
        {
            using var f = new Form()
            {
                Width = 380,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterParent
            };
            var lbl = new Label() { Left = 15, Top = 12, Width = 340, Text = "Password:" };
            var tb = new System.Windows.Forms.TextBox() { Left = 15, Top = 35, Width = 340, UseSystemPasswordChar = true };
            var ok = new System.Windows.Forms.Button() { Text = "OK", Left = 190, Width = 75, Top = 70, DialogResult = DialogResult.OK };
            var cancel = new System.Windows.Forms.Button() { Text = "Cancel", Left = 280, Width = 75, Top = 70, DialogResult = DialogResult.Cancel };
            f.Controls.AddRange(new Control[] { lbl, tb, ok, cancel });
            f.AcceptButton = ok; f.CancelButton = cancel;

            return f.ShowDialog(this) == DialogResult.OK ? tb.Text : null;
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
                }
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
            RefreshFilesInBackground();
        }

        private void ChkIncludeSubfolders_CheckedChanged(object sender, EventArgs e)
        {
            fileService.SetIncludeSubfolders(chkIncludeSubfolders.Checked);
            RefreshFilesInBackground();
        }

        private void BtnRefreshExtensions_Click(object sender, EventArgs e)
        {
            RefreshFilesInBackground();
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
            BtnRefreshExtensions_Click(null, null);
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (lstExtensions.SelectedIndex != -1)
            {
                string removedExtension = lstExtensions.SelectedItem.ToString();
                fileService.RemoveExtension(removedExtension);
                SyncUIWithService();
            }
            else
            {
                MessageBox.Show("Please select an extension to remove.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            BtnRefreshExtensions_Click(null, null);
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
            if (!string.IsNullOrEmpty(rtbOutput.Text))
                Clipboard.SetText(rtbOutput.Text);
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            if (fileService.SelectedFiles.Count == 0)
            {
                MessageBox.Show("Please add at least one file to process.", "Files Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ProcessFiles();
        }

        private void LstFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                BtnRemoveFile_Click(sender, e);
            }
        }

        private void BtnRecreateFiles_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder to recreate files in";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string baseFolder = folderDialog.SelectedPath;
                    try
                    {
                        int count = FileRecreator.RecreateFilesFromOutput(
                            rtbOutput.Text,
                            baseFolder,
                            fileService.FolderPath);

                        MessageBox.Show($"{count} file(s) created successfully.",
                            "Recreate Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message,
                            "Recreate Files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
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

        // New: Search in Selected Files
        private void BtnSearchFiles_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearchFiles.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Enter a search term.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            lstFiles.ClearSelected();
            for (int i = 0; i < fileService.SelectedFiles.Count; i++)
            {
                try
                {
                    string content = File.ReadAllText(fileService.SelectedFiles[i], selectedEncoding);
                    if (content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    {
                        lstFiles.SetSelected(i, true);
                    }
                }
                catch { } // Skip errors
            }
        }

        // New: Update Ignore Patterns
        private void TxtIgnorePatterns_TextChanged(object sender, EventArgs e)
        {
            fileService.IgnorePatterns.Clear();
            fileService.IgnorePatterns.AddRange(txtIgnorePatterns.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));
            RefreshFilesInBackground();
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

        // New: Background Refresh
        private void RefreshFilesInBackground()
        {
            if (bgWorker.IsBusy) return;
            progressBar.Visible = true;
            btnRefreshExtensions.Enabled = false;
            bgWorker.RunWorkerAsync();
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var progress = new Progress<int>(percent => bgWorker.ReportProgress(percent));
            fileService.RefreshFilesAsync(progress).Wait();
        }

        private void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Visible = false;
            btnRefreshExtensions.Enabled = true;
            SyncUIWithService();
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

        private void ProcessFiles()
        {
            rtbOutput.Clear();

            try
            {
                for (int i = 0; i < fileService.SelectedFiles.Count; i++)
                {
                    string displayPath = fileService.SelectedFiles[i];

                    // Header styling
                    rtbOutput.SelectionStart = rtbOutput.TextLength;
                    rtbOutput.SelectionLength = 0;
                    rtbOutput.SelectionColor = Color.FromArgb(0, 102, 204);
                    rtbOutput.SelectionFont = new Font(rtbOutput.Font, FontStyle.Bold);
                    rtbOutput.AppendText(displayPath + ":");
                    rtbOutput.SelectionColor = Color.Black;
                    rtbOutput.SelectionFont = new Font(rtbOutput.Font, FontStyle.Regular);
                    rtbOutput.AppendText("\n");

                    try
                    {
                        string content = File.ReadAllText(fileService.SelectedFiles[i], selectedEncoding); // Use selected encoding
                        rtbOutput.AppendText(content);
                    }
                    catch (Exception ex)
                    {
                        rtbOutput.AppendText($"[Error reading file: {ex.Message}]");
                    }

                    if (i < fileService.SelectedFiles.Count - 1)
                    {
                        rtbOutput.AppendText("\n\n\n\n");
                    }
                }

                rtbOutput.SelectionStart = 0;
                rtbOutput.ScrollToCaret();

                // New: Update statistics
                UpdateOutputStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // New: Update Output Statistics
        private void UpdateOutputStatistics()
        {
            string text = rtbOutput.Text;
            int charCount = text.Length;
            int lineCount = text.Split('\n').Length;
            int byteSize = Encoding.UTF8.GetByteCount(text);
            lblOutputStats.Text = $"Chars: {charCount:N0} | Lines: {lineCount:N0} | Size: {byteSize:N0} bytes";
        }

        private void SyncUIWithService()
        {
            lstExtensions.Items.Clear();
            foreach (var ext in fileService.Extensions)
                lstExtensions.Items.Add(ext);

            lstFiles.Items.Clear();
            foreach (var file in fileService.SelectedFiles)
            {
                string relativePath = string.IsNullOrEmpty(fileService.FolderPath)
                    ? file
                    : GetRelativePath(fileService.FolderPath, file);

                lstFiles.Items.Add(relativePath);
            }

            lblFileCount.Text = $"Files: {fileService.SelectedFiles.Count}";
            chkIncludeSubfolders.Checked = fileService.IncludeSubfolders;
            txtIgnorePatterns.Text = string.Join(", ", fileService.IgnorePatterns); // New
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
            Uri baseUri = new Uri(AppendDirectorySeparatorChar(basePath));
            Uri fullUri = new Uri(fullPath);
            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);
            return Uri.UnescapeDataString(relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return path + Path.DirectorySeparatorChar;
            return path;
        }

        #endregion
    }
}