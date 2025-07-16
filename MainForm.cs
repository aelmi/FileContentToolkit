using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FileContentToolkit
{
    public partial class MainForm : Form
    {
        private FileContentService fileService = new FileContentService();

        public MainForm()
        {
            InitializeComponent();
            SetupHoverEffects();
            SetupToolTips();
            SyncUIWithService();

            // Keep the "Recreate Files" button right-aligned on resize
            pnlRecreateInfo.Resize += (s, e) =>
            {
                btnRecreateFiles.Left = pnlRecreateInfo.Width - btnRecreateFiles.Width - 20;
                btnRecreateFiles.Top = (pnlRecreateInfo.Height - btnRecreateFiles.Height) / 2;
            };
        }

        private void SetupHoverEffects()
        {
            AddHoverEffect(btnBrowse, Color.FromArgb(51, 122, 183));
            AddHoverEffect(btnAdd, Color.FromArgb(51, 122, 183));
            AddHoverEffect(btnRemove, Color.FromArgb(220, 53, 69));
            AddHoverEffect(btnAddFile, Color.FromArgb(40, 167, 69));
            AddHoverEffect(btnRemoveFile, Color.FromArgb(220, 53, 69));
            AddHoverEffect(btnGenerate, Color.FromArgb(0, 123, 255));
            AddHoverEffect(btnRefreshExtensions, Color.FromArgb(51, 122, 183));
            AddHoverEffect(btnMoveUp, Color.LightGray);
            AddHoverEffect(btnMoveDown, Color.LightGray);
            AddHoverEffect(btnRecreateFiles, Color.FromArgb(40, 167, 69));
        }

        private void AddHoverEffect(System.Windows.Forms.Button button, Color originalColor)
        {
            button.MouseEnter += (s, e) => button.BackColor = ControlPaint.Light(originalColor, 0.2f);
            button.MouseLeave += (s, e) => button.BackColor = originalColor;
        }

        private void SetupToolTips()
        {
            toolTip1 = new System.Windows.Forms.ToolTip();
            toolTip1.SetToolTip(btnBrowse, "Browse for a folder");
            toolTip1.SetToolTip(btnAdd, "Add a file extension to the list");
            toolTip1.SetToolTip(btnRemove, "Remove the selected file extension");
            toolTip1.SetToolTip(btnRefreshExtensions, "Refresh the file list for the selected extensions");
            toolTip1.SetToolTip(btnAddFile, "Add files manually to the selected files list");
            toolTip1.SetToolTip(btnRemoveFile, "Remove the selected file from the list");
            toolTip1.SetToolTip(btnMoveUp, "Move the selected file up");
            toolTip1.SetToolTip(btnMoveDown, "Move the selected file down");
            toolTip1.SetToolTip(btnGenerate, "Read and display the contents of the selected files");
            toolTip1.SetToolTip(btnCopyOutput, "Copy the output to the clipboard");
            toolTip1.SetToolTip(chkIncludeSubfolders, "Include files from subfolders");
            toolTip1.SetToolTip(btnEditOutput, "Edit the output");
            toolTip1.SetToolTip(btnRecreateFiles, "Recreate files and folders from the output below");
        }

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

        private void TxtFolderPath_TextChanged(object sender, EventArgs e)
        {
            fileService.SetFolderPath(txtFolderPath.Text);
            SyncUIWithService();
        }

        private void ChkIncludeSubfolders_CheckedChanged(object sender, EventArgs e)
        {
            fileService.SetIncludeSubfolders(chkIncludeSubfolders.Checked);
            SyncUIWithService();
        }

        private void BtnRefreshExtensions_Click(object sender, EventArgs e)
        {
            fileService.RefreshFiles();
            SyncUIWithService();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string extension = txtExtension.Text.Trim();
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
            txtExtension.Clear();
            txtExtension.Focus();
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
        }

        private void TxtExtension_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnAdd_Click(sender, e);
                e.Handled = true;
            }
        }

        private void BtnAddFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select files to add";
                openFileDialog.Multiselect = true;

                if (fileService.Extensions.Count > 0)
                {
                    string filter = "Selected Extensions|";
                    foreach (string ext in fileService.Extensions)
                    {
                        filter += $"*{ext};";
                    }
                    filter = filter.TrimEnd(';');
                    filter += "|All Files|*.*";
                    openFileDialog.Filter = filter;
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileService.AddFiles(openFileDialog.FileNames);
                    SyncUIWithService();
                }
            }
        }

        private void BtnRemoveFile_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedIndex != -1)
            {
                fileService.RemoveFileAt(lstFiles.SelectedIndex);
                SyncUIWithService();
            }
            else
            {
                MessageBox.Show("Please select a file to remove.", "Selection Required",
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

        private void ProcessFiles()
        {
            rtbOutput.Clear();
            try
            {
                for (int i = 0; i < fileService.SelectedFiles.Count; i++)
                {
                    string fileName = Path.GetFileName(fileService.SelectedFiles[i]);
                    string displayPath = fileService.SelectedFiles[i];

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
                        string content = File.ReadAllText(fileService.SelectedFiles[i]);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LstFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                BtnRemoveFile_Click(sender, e);
            }
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
                    : Path.GetRelativePath(fileService.FolderPath, file);
                lstFiles.Items.Add(relativePath);
            }

            lblFileCount.Text = $"Files: {fileService.SelectedFiles.Count}";
            chkIncludeSubfolders.Checked = fileService.IncludeSubfolders;
        }

        // --- New Feature: Recreate Files from Output ---
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
                        int count = FileRecreator.RecreateFilesFromOutput(rtbOutput.Text, baseFolder, fileService.FolderPath);
                        MessageBox.Show($"{count} file(s) created successfully.", "Recreate Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Recreate Files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}