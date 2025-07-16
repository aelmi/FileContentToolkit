using System;
using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // New fields for the Recreate Files panel
        private System.Windows.Forms.Panel pnlRecreateInfo;
        private System.Windows.Forms.TableLayoutPanel tblRecreateInfo;
        private System.Windows.Forms.Label lblRecreateInfo;
        private System.Windows.Forms.Button btnRecreateFiles;

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnBrowse;

        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.GroupBox grpExtensions;
        private System.Windows.Forms.Label lblExtension;
        private System.Windows.Forms.TextBox txtExtension;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox lstExtensions;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.CheckBox chkIncludeSubfolders;
        private System.Windows.Forms.Button btnRefreshExtensions;

        private System.Windows.Forms.GroupBox grpFiles;
        private System.Windows.Forms.ListBox lstFiles;
        private System.Windows.Forms.Panel pnlFileButtons;
        private System.Windows.Forms.Button btnAddFile;
        private System.Windows.Forms.Button btnRemoveFile;
        private System.Windows.Forms.Label lblFileCount;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;

        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnGenerate;

        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Button btnCopyOutput;
        private System.Windows.Forms.Button btnEditOutput;

        private System.Windows.Forms.ToolTip toolTip1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlTop = new Panel();
            lblPath = new Label();
            txtFolderPath = new TextBox();
            btnBrowse = new Button();
            pnlLeft = new Panel();
            grpFiles = new GroupBox();
            lstFiles = new ListBox();
            pnlFileButtons = new Panel();
            lblFileCount = new Label();
            btnAddFile = new Button();
            btnRemoveFile = new Button();
            btnMoveUp = new Button();
            btnMoveDown = new Button();
            grpExtensions = new GroupBox();
            lblExtension = new Label();
            txtExtension = new TextBox();
            btnAdd = new Button();
            lstExtensions = new ListBox();
            btnRemove = new Button();
            chkIncludeSubfolders = new CheckBox();
            btnRefreshExtensions = new Button();
            pnlBottom = new Panel();
            btnGenerate = new Button();
            pnlRight = new Panel();
            rtbOutput = new RichTextBox();
            pnlRecreateInfo = new Panel();
            tblRecreateInfo = new TableLayoutPanel();
            lblRecreateInfo = new Label();
            btnRecreateFiles = new Button();
            lblOutput = new Label();
            btnCopyOutput = new Button();
            btnEditOutput = new Button();
            toolTip1 = new ToolTip(components);
            pnlTop.SuspendLayout();
            pnlLeft.SuspendLayout();
            grpFiles.SuspendLayout();
            pnlFileButtons.SuspendLayout();
            grpExtensions.SuspendLayout();
            pnlBottom.SuspendLayout();
            pnlRight.SuspendLayout();
            pnlRecreateInfo.SuspendLayout();
            tblRecreateInfo.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(lblPath);
            pnlTop.Controls.Add(txtFolderPath);
            pnlTop.Controls.Add(btnBrowse);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(17, 19, 17, 19);
            pnlTop.Size = new Size(1640, 115);
            pnlTop.TabIndex = 0;
            // 
            // lblPath
            // 
            lblPath.AutoSize = true;
            lblPath.Font = new Font("Segoe UI", 10F);
            lblPath.ForeColor = Color.FromArgb(0, 102, 204);
            lblPath.Location = new Point(17, 19);
            lblPath.Name = "lblPath";
            lblPath.Size = new Size(115, 28);
            lblPath.TabIndex = 0;
            lblPath.Text = "Folder Path:";
            // 
            // txtFolderPath
            // 
            txtFolderPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFolderPath.BackColor = Color.White;
            txtFolderPath.BorderStyle = BorderStyle.FixedSingle;
            txtFolderPath.Font = new Font("Segoe UI", 10F);
            txtFolderPath.Location = new Point(17, 58);
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.Size = new Size(1532, 34);
            txtFolderPath.TabIndex = 1;
            txtFolderPath.TextChanged += TxtFolderPath_TextChanged;
            // 
            // btnBrowse
            // 
            btnBrowse.BackColor = Color.FromArgb(51, 122, 183);
            btnBrowse.Cursor = Cursors.Hand;
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnBrowse.ForeColor = Color.White;
            btnBrowse.Location = new Point(1560, 56);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(67, 52);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "...";
            btnBrowse.UseVisualStyleBackColor = false;
            btnBrowse.Click += BtnBrowse_Click;
            // 
            // pnlLeft
            // 
            pnlLeft.Controls.Add(grpFiles);
            pnlLeft.Controls.Add(grpExtensions);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 115);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Padding = new Padding(17, 0, 17, 0);
            pnlLeft.Size = new Size(496, 964);
            pnlLeft.TabIndex = 1;
            // 
            // grpFiles
            // 
            grpFiles.Controls.Add(lstFiles);
            grpFiles.Controls.Add(pnlFileButtons);
            grpFiles.Dock = DockStyle.Fill;
            grpFiles.Font = new Font("Segoe UI", 10F);
            grpFiles.ForeColor = Color.FromArgb(0, 102, 204);
            grpFiles.Location = new Point(17, 385);
            grpFiles.Name = "grpFiles";
            grpFiles.Padding = new Padding(5, 6, 5, 6);
            grpFiles.Size = new Size(462, 579);
            grpFiles.TabIndex = 1;
            grpFiles.TabStop = false;
            grpFiles.Text = "Selected Files";
            // 
            // lstFiles
            // 
            lstFiles.BackColor = Color.White;
            lstFiles.BorderStyle = BorderStyle.FixedSingle;
            lstFiles.Dock = DockStyle.Fill;
            lstFiles.Font = new Font("Segoe UI", 9F);
            lstFiles.ForeColor = Color.Black;
            lstFiles.FormattingEnabled = true;
            lstFiles.HorizontalScrollbar = true;
            lstFiles.ItemHeight = 25;
            lstFiles.Location = new Point(5, 33);
            lstFiles.Name = "lstFiles";
            lstFiles.Size = new Size(452, 463);
            lstFiles.TabIndex = 0;
            lstFiles.KeyDown += LstFiles_KeyDown;
            // 
            // pnlFileButtons
            // 
            pnlFileButtons.Controls.Add(lblFileCount);
            pnlFileButtons.Controls.Add(btnAddFile);
            pnlFileButtons.Controls.Add(btnRemoveFile);
            pnlFileButtons.Controls.Add(btnMoveUp);
            pnlFileButtons.Controls.Add(btnMoveDown);
            pnlFileButtons.Dock = DockStyle.Bottom;
            pnlFileButtons.Location = new Point(5, 496);
            pnlFileButtons.Name = "pnlFileButtons";
            pnlFileButtons.Size = new Size(452, 77);
            pnlFileButtons.TabIndex = 1;
            // 
            // lblFileCount
            // 
            lblFileCount.AutoSize = true;
            lblFileCount.Font = new Font("Segoe UI", 9F);
            lblFileCount.ForeColor = Color.FromArgb(64, 64, 64);
            lblFileCount.Location = new Point(8, 23);
            lblFileCount.Name = "lblFileCount";
            lblFileCount.Size = new Size(65, 25);
            lblFileCount.TabIndex = 0;
            lblFileCount.Text = "Files: 0";
            // 
            // btnAddFile
            // 
            btnAddFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddFile.BackColor = Color.FromArgb(40, 167, 69);
            btnAddFile.Cursor = Cursors.Hand;
            btnAddFile.FlatAppearance.BorderSize = 0;
            btnAddFile.FlatStyle = FlatStyle.Flat;
            btnAddFile.Font = new Font("Segoe UI", 9F);
            btnAddFile.ForeColor = Color.White;
            btnAddFile.Location = new Point(120, 12);
            btnAddFile.Name = "btnAddFile";
            btnAddFile.Size = new Size(90, 52);
            btnAddFile.TabIndex = 1;
            btnAddFile.Text = "Add File";
            btnAddFile.UseVisualStyleBackColor = false;
            btnAddFile.Click += BtnAddFile_Click;
            // 
            // btnRemoveFile
            // 
            btnRemoveFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRemoveFile.BackColor = Color.FromArgb(220, 53, 69);
            btnRemoveFile.Cursor = Cursors.Hand;
            btnRemoveFile.FlatAppearance.BorderSize = 0;
            btnRemoveFile.FlatStyle = FlatStyle.Flat;
            btnRemoveFile.Font = new Font("Segoe UI", 9F);
            btnRemoveFile.ForeColor = Color.White;
            btnRemoveFile.Location = new Point(215, 12);
            btnRemoveFile.Name = "btnRemoveFile";
            btnRemoveFile.Size = new Size(90, 52);
            btnRemoveFile.TabIndex = 2;
            btnRemoveFile.Text = "Remove";
            btnRemoveFile.UseVisualStyleBackColor = false;
            btnRemoveFile.Click += BtnRemoveFile_Click;
            // 
            // btnMoveUp
            // 
            btnMoveUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoveUp.BackColor = Color.LightGray;
            btnMoveUp.Cursor = Cursors.Hand;
            btnMoveUp.FlatAppearance.BorderSize = 0;
            btnMoveUp.FlatStyle = FlatStyle.Flat;
            btnMoveUp.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnMoveUp.ForeColor = Color.Black;
            btnMoveUp.Location = new Point(310, 12);
            btnMoveUp.Name = "btnMoveUp";
            btnMoveUp.Size = new Size(40, 40);
            btnMoveUp.TabIndex = 3;
            btnMoveUp.Text = "▲";
            btnMoveUp.UseVisualStyleBackColor = false;
            btnMoveUp.Click += BtnMoveUp_Click;
            // 
            // btnMoveDown
            // 
            btnMoveDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoveDown.BackColor = Color.LightGray;
            btnMoveDown.Cursor = Cursors.Hand;
            btnMoveDown.FlatAppearance.BorderSize = 0;
            btnMoveDown.FlatStyle = FlatStyle.Flat;
            btnMoveDown.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnMoveDown.ForeColor = Color.Black;
            btnMoveDown.Location = new Point(355, 12);
            btnMoveDown.Name = "btnMoveDown";
            btnMoveDown.Size = new Size(40, 40);
            btnMoveDown.TabIndex = 4;
            btnMoveDown.Text = "▼";
            btnMoveDown.UseVisualStyleBackColor = false;
            btnMoveDown.Click += BtnMoveDown_Click;
            // 
            // grpExtensions
            // 
            grpExtensions.Controls.Add(lblExtension);
            grpExtensions.Controls.Add(txtExtension);
            grpExtensions.Controls.Add(btnAdd);
            grpExtensions.Controls.Add(lstExtensions);
            grpExtensions.Controls.Add(btnRemove);
            grpExtensions.Controls.Add(chkIncludeSubfolders);
            grpExtensions.Controls.Add(btnRefreshExtensions);
            grpExtensions.Dock = DockStyle.Top;
            grpExtensions.Font = new Font("Segoe UI", 10F);
            grpExtensions.ForeColor = Color.FromArgb(0, 102, 204);
            grpExtensions.Location = new Point(17, 0);
            grpExtensions.Name = "grpExtensions";
            grpExtensions.Padding = new Padding(5, 6, 5, 6);
            grpExtensions.Size = new Size(462, 385);
            grpExtensions.TabIndex = 0;
            grpExtensions.TabStop = false;
            grpExtensions.Text = "File Extensions";
            // 
            // lblExtension
            // 
            lblExtension.AutoSize = true;
            lblExtension.Font = new Font("Segoe UI", 9F);
            lblExtension.Location = new Point(17, 48);
            lblExtension.Name = "lblExtension";
            lblExtension.Size = new Size(161, 25);
            lblExtension.TabIndex = 0;
            lblExtension.Text = "Add File Extension:";
            // 
            // txtExtension
            // 
            txtExtension.BackColor = Color.White;
            txtExtension.BorderStyle = BorderStyle.FixedSingle;
            txtExtension.Font = new Font("Segoe UI", 10F);
            txtExtension.Location = new Point(17, 87);
            txtExtension.Name = "txtExtension";
            txtExtension.Size = new Size(249, 34);
            txtExtension.TabIndex = 1;
            txtExtension.KeyPress += TxtExtension_KeyPress;
            // 
            // btnAdd
            // 
            btnAdd.BackColor = Color.FromArgb(51, 122, 183);
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Segoe UI", 9F);
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(283, 85);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(133, 52);
            btnAdd.TabIndex = 2;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += BtnAdd_Click;
            // 
            // lstExtensions
            // 
            lstExtensions.BackColor = Color.White;
            lstExtensions.BorderStyle = BorderStyle.FixedSingle;
            lstExtensions.Font = new Font("Segoe UI", 10F);
            lstExtensions.ForeColor = Color.Black;
            lstExtensions.FormattingEnabled = true;
            lstExtensions.ItemHeight = 28;
            lstExtensions.Location = new Point(17, 154);
            lstExtensions.Name = "lstExtensions";
            lstExtensions.Size = new Size(249, 198);
            lstExtensions.TabIndex = 3;
            // 
            // btnRemove
            // 
            btnRemove.BackColor = Color.FromArgb(220, 53, 69);
            btnRemove.Cursor = Cursors.Hand;
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.Font = new Font("Segoe UI", 9F);
            btnRemove.ForeColor = Color.White;
            btnRemove.Location = new Point(283, 154);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(125, 52);
            btnRemove.TabIndex = 4;
            btnRemove.Text = "Remove";
            btnRemove.UseVisualStyleBackColor = false;
            btnRemove.Click += BtnRemove_Click;
            // 
            // chkIncludeSubfolders
            // 
            chkIncludeSubfolders.AutoSize = true;
            chkIncludeSubfolders.Font = new Font("Segoe UI", 9F);
            chkIncludeSubfolders.Location = new Point(17, 360);
            chkIncludeSubfolders.Name = "chkIncludeSubfolders";
            chkIncludeSubfolders.Size = new Size(194, 29);
            chkIncludeSubfolders.TabIndex = 5;
            chkIncludeSubfolders.Text = "Include subfolder(s)";
            chkIncludeSubfolders.UseVisualStyleBackColor = true;
            chkIncludeSubfolders.CheckedChanged += ChkIncludeSubfolders_CheckedChanged;
            // 
            // btnRefreshExtensions
            // 
            btnRefreshExtensions.BackColor = Color.FromArgb(51, 122, 183);
            btnRefreshExtensions.Cursor = Cursors.Hand;
            btnRefreshExtensions.FlatAppearance.BorderSize = 0;
            btnRefreshExtensions.FlatStyle = FlatStyle.Flat;
            btnRefreshExtensions.Font = new Font("Segoe UI", 9F);
            btnRefreshExtensions.ForeColor = Color.White;
            btnRefreshExtensions.Location = new Point(283, 214);
            btnRefreshExtensions.Name = "btnRefreshExtensions";
            btnRefreshExtensions.Size = new Size(125, 52);
            btnRefreshExtensions.TabIndex = 6;
            btnRefreshExtensions.Text = "Refresh";
            btnRefreshExtensions.UseVisualStyleBackColor = false;
            btnRefreshExtensions.Click += BtnRefreshExtensions_Click;
            // 
            // pnlBottom
            // 
            pnlBottom.Controls.Add(btnGenerate);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 1079);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(1640, 115);
            pnlBottom.TabIndex = 3;
            // 
            // btnGenerate
            // 
            btnGenerate.Anchor = AnchorStyles.None;
            btnGenerate.BackColor = Color.FromArgb(0, 123, 255);
            btnGenerate.Cursor = Cursors.Hand;
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.FlatStyle = FlatStyle.Flat;
            btnGenerate.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnGenerate.ForeColor = Color.White;
            btnGenerate.Location = new Point(695, 19);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(250, 77);
            btnGenerate.TabIndex = 0;
            btnGenerate.Text = "GENERATE";
            btnGenerate.UseVisualStyleBackColor = false;
            btnGenerate.Click += BtnGenerate_Click;
            // 
            // pnlRight
            // 
            pnlRight.Controls.Add(rtbOutput);
            pnlRight.Controls.Add(pnlRecreateInfo);
            pnlRight.Controls.Add(lblOutput);
            pnlRight.Controls.Add(btnCopyOutput);
            pnlRight.Controls.Add(btnEditOutput);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Location = new Point(496, 115);
            pnlRight.Name = "pnlRight";
            pnlRight.Padding = new Padding(17, 0, 17, 0);
            pnlRight.Size = new Size(1144, 964);
            pnlRight.TabIndex = 2;
            // 
            // rtbOutput
            // 
            rtbOutput.BackColor = Color.White;
            rtbOutput.BorderStyle = BorderStyle.FixedSingle;
            rtbOutput.Dock = DockStyle.Fill;
            rtbOutput.Font = new Font("Consolas", 10F);
            rtbOutput.Location = new Point(17, 118);
            rtbOutput.Name = "rtbOutput";
            rtbOutput.ReadOnly = true;
            rtbOutput.Size = new Size(1110, 846);
            rtbOutput.TabIndex = 1;
            rtbOutput.Text = "";
            rtbOutput.WordWrap = false;
            // 
            // pnlRecreateInfo
            // 
            pnlRecreateInfo.BackColor = Color.FromArgb(255, 255, 224);
            pnlRecreateInfo.Controls.Add(tblRecreateInfo);
            pnlRecreateInfo.Dock = DockStyle.Top;
            pnlRecreateInfo.Location = new Point(17, 38);
            pnlRecreateInfo.Name = "pnlRecreateInfo";
            pnlRecreateInfo.Size = new Size(1110, 80);
            pnlRecreateInfo.TabIndex = 4;
            // 
            // tblRecreateInfo
            // 
            tblRecreateInfo.ColumnCount = 2;
            tblRecreateInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblRecreateInfo.ColumnStyles.Add(new ColumnStyle());
            tblRecreateInfo.Controls.Add(lblRecreateInfo, 0, 0);
            tblRecreateInfo.Controls.Add(btnRecreateFiles, 1, 0);
            tblRecreateInfo.Dock = DockStyle.Fill;
            tblRecreateInfo.Location = new Point(0, 0);
            tblRecreateInfo.Name = "tblRecreateInfo";
            tblRecreateInfo.RowCount = 1;
            tblRecreateInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblRecreateInfo.Size = new Size(1110, 80);
            tblRecreateInfo.TabIndex = 0;
            // 
            // lblRecreateInfo
            // 
            lblRecreateInfo.Dock = DockStyle.Fill;
            lblRecreateInfo.Font = new Font("Segoe UI", 9.5F);
            lblRecreateInfo.ForeColor = Color.FromArgb(120, 80, 0);
            lblRecreateInfo.Location = new Point(3, 0);
            lblRecreateInfo.Name = "lblRecreateInfo";
            lblRecreateInfo.Size = new Size(934, 80);
            lblRecreateInfo.TabIndex = 0;
            lblRecreateInfo.Text = "🗂️  Recreate Files: This feature lets you restore files and folders from the output below.\nPaste or load the output, then click 'Recreate Files' to generate them in a folder of your choice.";
            lblRecreateInfo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnRecreateFiles
            // 
            btnRecreateFiles.BackColor = Color.FromArgb(40, 167, 69);
            btnRecreateFiles.FlatAppearance.BorderSize = 0;
            btnRecreateFiles.FlatStyle = FlatStyle.Flat;
            btnRecreateFiles.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnRecreateFiles.ForeColor = Color.White;
            btnRecreateFiles.Location = new Point(950, 20);
            btnRecreateFiles.Margin = new Padding(10, 20, 20, 20);
            btnRecreateFiles.Name = "btnRecreateFiles";
            btnRecreateFiles.Size = new Size(140, 40);
            btnRecreateFiles.TabIndex = 1;
            btnRecreateFiles.Text = "Recreate Files";
            btnRecreateFiles.UseVisualStyleBackColor = false;
            btnRecreateFiles.Click += BtnRecreateFiles_Click;
            // 
            // lblOutput
            // 
            lblOutput.AutoSize = true;
            lblOutput.Dock = DockStyle.Top;
            lblOutput.Font = new Font("Segoe UI", 10F);
            lblOutput.ForeColor = Color.FromArgb(0, 102, 204);
            lblOutput.Location = new Point(17, 0);
            lblOutput.Name = "lblOutput";
            lblOutput.Padding = new Padding(0, 0, 0, 10);
            lblOutput.Size = new Size(79, 38);
            lblOutput.TabIndex = 0;
            lblOutput.Text = "Output:";
            // 
            // btnCopyOutput
            // 
            btnCopyOutput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopyOutput.BackColor = Color.Transparent;
            btnCopyOutput.Cursor = Cursors.Hand;
            btnCopyOutput.FlatAppearance.BorderSize = 0;
            btnCopyOutput.FlatStyle = FlatStyle.Flat;
            btnCopyOutput.Font = new Font("Segoe UI Emoji", 12F);
            btnCopyOutput.Location = new Point(1060, 0);
            btnCopyOutput.Name = "btnCopyOutput";
            btnCopyOutput.Size = new Size(40, 38);
            btnCopyOutput.TabIndex = 2;
            btnCopyOutput.Text = "📋";
            btnCopyOutput.UseVisualStyleBackColor = false;
            btnCopyOutput.Click += BtnCopyOutput_Click;
            // 
            // btnEditOutput
            // 
            btnEditOutput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnEditOutput.BackColor = Color.Transparent;
            btnEditOutput.Cursor = Cursors.Hand;
            btnEditOutput.FlatAppearance.BorderSize = 0;
            btnEditOutput.FlatStyle = FlatStyle.Flat;
            btnEditOutput.Font = new Font("Segoe UI Emoji", 12F);
            btnEditOutput.Location = new Point(1015, 0);
            btnEditOutput.Name = "btnEditOutput";
            btnEditOutput.Size = new Size(40, 38);
            btnEditOutput.TabIndex = 3;
            btnEditOutput.Text = "✏️";
            btnEditOutput.UseVisualStyleBackColor = false;
            btnEditOutput.Click += BtnEditOutput_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(1640, 1194);
            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            MinimumSize = new Size(1319, 1102);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "File Content Toolkit";
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlLeft.ResumeLayout(false);
            grpFiles.ResumeLayout(false);
            pnlFileButtons.ResumeLayout(false);
            pnlFileButtons.PerformLayout();
            grpExtensions.ResumeLayout(false);
            grpExtensions.PerformLayout();
            pnlBottom.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            pnlRight.PerformLayout();
            pnlRecreateInfo.ResumeLayout(false);
            tblRecreateInfo.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
    }
}