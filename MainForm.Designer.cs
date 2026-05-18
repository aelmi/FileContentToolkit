using System;
using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlRecreateInfo;
        private System.Windows.Forms.TableLayoutPanel tblRecreateInfo;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.GroupBox grpFiles;
        private System.Windows.Forms.ListBox lstFiles;
        private System.Windows.Forms.Panel pnlFileButtons;
        private System.Windows.Forms.Button btnAddMultipleFiles;
        private System.Windows.Forms.Button btnRemoveFile;
        private System.Windows.Forms.Label lblFileCount;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.GroupBox grpExtensions;
        private System.Windows.Forms.Label lblExtension;
        private System.Windows.Forms.ComboBox cmbExtension;
        private FileContentToolkit.UI.SplitButton btnAdd;
        private System.Windows.Forms.ListBox lstExtensions;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.CheckBox chkIncludeSubfolders;
        private System.Windows.Forms.Button btnRefreshExtensions;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Button btnCopyOutput;
        private System.Windows.Forms.Button btnEditOutput;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip cmsAddDropdown;
        private System.Windows.Forms.ToolStripMenuItem miShowExtensionSummary;
        private System.Windows.Forms.ContextMenuStrip ctxFiles;
        private System.Windows.Forms.ToolStripMenuItem miSortByName;
        private System.Windows.Forms.ToolStripMenuItem miSortByExtension;
        private System.Windows.Forms.Panel pnlOutput;
        private System.Windows.Forms.Button btnExportOutput;
        private System.Windows.Forms.Panel pnlOutputHeader;
        private System.Windows.Forms.Panel pnlCompressionTools;
        private System.Windows.Forms.Button btnDecompressEnc;
        private System.Windows.Forms.Button btnCompressEnc;
        private System.Windows.Forms.Button btnDecompress;
        private System.Windows.Forms.Button btnCompress;
        private System.Windows.Forms.Label lblCompression;
        private System.Windows.Forms.Panel pnlSeparator;
        private System.Windows.Forms.TextBox txtIgnorePatterns;
        private System.Windows.Forms.Label lblIgnorePatterns;
        private System.Windows.Forms.TextBox txtSearchFiles;
        private System.Windows.Forms.Button btnSearchFiles;
        private System.Windows.Forms.Label lblSearchFiles;
        private System.Windows.Forms.ComboBox cmbEncoding;
        private System.Windows.Forms.Label lblEncoding;
        private System.Windows.Forms.Label lblOutputStats;
        private System.Windows.Forms.Label lblRecreateInfo;
        private System.Windows.Forms.Button btnRecreateFiles;

        // --- new toolbar row inside pnlTop ---
        private System.Windows.Forms.Button btnTree;
        private System.Windows.Forms.Button btnRecentFolders;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnSavePreset;
        private System.Windows.Forms.Button btnLoadPreset;
        private System.Windows.Forms.CheckBox chkWatch;

        // --- search row enhancements inside grpFiles ---
        private System.Windows.Forms.Button btnSearchRecents;
        private System.Windows.Forms.CheckBox chkCase;
        private System.Windows.Forms.CheckBox chkWord;
        private System.Windows.Forms.CheckBox chkRegex;
        private System.Windows.Forms.Button btnFindReplace;
        private System.Windows.Forms.Label lblSearchMatches;

        // --- main menu strip ---
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpShortcuts;
        private System.Windows.Forms.ToolStripSeparator mnuHelpSep1;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;

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
            cmbEncoding = new ComboBox();
            lblEncoding = new Label();
            lblPath = new Label();
            txtFolderPath = new TextBox();
            btnBrowse = new Button();
            btnTree = new Button();
            btnRecentFolders = new Button();
            btnOptions = new Button();
            btnSavePreset = new Button();
            btnLoadPreset = new Button();
            chkWatch = new CheckBox();
            pnlLeft = new Panel();
            grpFiles = new GroupBox();
            txtSearchFiles = new TextBox();
            btnSearchFiles = new Button();
            btnSearchRecents = new Button();
            chkCase = new CheckBox();
            chkWord = new CheckBox();
            chkRegex = new CheckBox();
            btnFindReplace = new Button();
            lblSearchMatches = new Label();
            menuMain = new MenuStrip();
            mnuHelp = new ToolStripMenuItem();
            mnuHelpShortcuts = new ToolStripMenuItem();
            mnuHelpSep1 = new ToolStripSeparator();
            mnuHelpAbout = new ToolStripMenuItem();
            lblSearchFiles = new Label();
            lstFiles = new ListBox();
            ctxFiles = new ContextMenuStrip(components);
            miSortByName = new ToolStripMenuItem();
            miSortByExtension = new ToolStripMenuItem();
            pnlFileButtons = new Panel();
            lblFileCount = new Label();
            btnAddMultipleFiles = new Button();
            btnRemoveFile = new Button();
            btnMoveUp = new Button();
            btnMoveDown = new Button();
            grpExtensions = new GroupBox();
            txtIgnorePatterns = new TextBox();
            lblIgnorePatterns = new Label();
            lblExtension = new Label();
            cmbExtension = new ComboBox();
            btnAdd = new FileContentToolkit.UI.SplitButton();
            cmsAddDropdown = new ContextMenuStrip(components);
            miShowExtensionSummary = new ToolStripMenuItem();
            lstExtensions = new ListBox();
            btnRemove = new Button();
            chkIncludeSubfolders = new CheckBox();
            btnRefreshExtensions = new Button();
            pnlBottom = new Panel();
            progressBar = new ProgressBar();
            btnGenerate = new Button();
            pnlRight = new Panel();
            rtbOutput = new RichTextBox();
            pnlRecreateInfo = new Panel();
            tblRecreateInfo = new TableLayoutPanel();
            lblRecreateInfo = new Label();
            btnRecreateFiles = new Button();
            pnlOutput = new Panel();
            lblOutputStats = new Label();
            pnlOutputHeader = new Panel();
            btnExportOutput = new Button();
            btnEditOutput = new Button();
            btnCopyOutput = new Button();
            lblOutput = new Label();
            pnlCompressionTools = new Panel();
            btnDecompressEnc = new Button();
            btnCompressEnc = new Button();
            btnDecompress = new Button();
            btnCompress = new Button();
            lblCompression = new Label();
            pnlSeparator = new Panel();
            toolTip1 = new ToolTip(components);
            menuMain.SuspendLayout();
            pnlTop.SuspendLayout();
            pnlLeft.SuspendLayout();
            grpFiles.SuspendLayout();
            ctxFiles.SuspendLayout();
            pnlFileButtons.SuspendLayout();
            grpExtensions.SuspendLayout();
            cmsAddDropdown.SuspendLayout();
            pnlBottom.SuspendLayout();
            pnlRight.SuspendLayout();
            pnlRecreateInfo.SuspendLayout();
            tblRecreateInfo.SuspendLayout();
            pnlOutput.SuspendLayout();
            pnlOutputHeader.SuspendLayout();
            pnlCompressionTools.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.FromArgb(0, 102, 204);
            pnlTop.Controls.Add(cmbEncoding);
            pnlTop.Controls.Add(lblEncoding);
            pnlTop.Controls.Add(lblPath);
            pnlTop.Controls.Add(txtFolderPath);
            pnlTop.Controls.Add(btnBrowse);
            pnlTop.Controls.Add(btnTree);
            pnlTop.Controls.Add(btnRecentFolders);
            pnlTop.Controls.Add(btnOptions);
            pnlTop.Controls.Add(btnSavePreset);
            pnlTop.Controls.Add(btnLoadPreset);
            pnlTop.Controls.Add(chkWatch);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(20, 20, 20, 10);
            pnlTop.Size = new Size(1640, 165);
            pnlTop.TabIndex = 0;
            // 
            // cmbEncoding
            // 
            cmbEncoding.BackColor = Color.White;
            cmbEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEncoding.FlatStyle = FlatStyle.Flat;
            cmbEncoding.Font = new Font("Segoe UI", 10F);
            cmbEncoding.Location = new Point(1180, 58);
            cmbEncoding.Name = "cmbEncoding";
            cmbEncoding.Size = new Size(150, 36);
            cmbEncoding.TabIndex = 4;
            cmbEncoding.SelectedIndexChanged += CmbEncoding_SelectedIndexChanged;
            // 
            // lblEncoding
            // 
            lblEncoding.AutoSize = true;
            lblEncoding.Font = new Font("Segoe UI", 10F);
            lblEncoding.ForeColor = Color.White;
            lblEncoding.Location = new Point(1180, 20);
            lblEncoding.Name = "lblEncoding";
            lblEncoding.Size = new Size(98, 28);
            lblEncoding.TabIndex = 3;
            lblEncoding.Text = "Encoding:";
            // 
            // lblPath
            // 
            lblPath.AutoSize = true;
            lblPath.Font = new Font("Segoe UI", 10F);
            lblPath.ForeColor = Color.White;
            lblPath.Location = new Point(20, 20);
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
            txtFolderPath.Location = new Point(20, 58);
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.Size = new Size(1130, 34);
            txtFolderPath.TabIndex = 1;
            txtFolderPath.TextChanged += TxtFolderPath_TextChanged;
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.BackColor = Color.FromArgb(51, 122, 183);
            btnBrowse.Cursor = Cursors.Hand;
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnBrowse.ForeColor = Color.White;
            btnBrowse.Location = new Point(1350, 56);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(67, 40);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "...";
            toolTip1.SetToolTip(btnBrowse, "Browse for a folder");
            btnBrowse.UseVisualStyleBackColor = false;
            btnBrowse.Click += BtnBrowse_Click;
            // 
            // btnTree
            // 
            btnTree.BackColor = Color.FromArgb(51, 122, 183);
            btnTree.Cursor = Cursors.Hand;
            btnTree.FlatAppearance.BorderSize = 0;
            btnTree.FlatStyle = FlatStyle.Flat;
            btnTree.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnTree.ForeColor = Color.White;
            btnTree.Location = new Point(20, 110);
            btnTree.Name = "btnTree";
            btnTree.Size = new Size(90, 34);
            btnTree.TabIndex = 5;
            btnTree.Text = "Tree";
            toolTip1.SetToolTip(btnTree, "Pick files and folders from a tree view");
            btnTree.UseVisualStyleBackColor = false;
            btnTree.Click += BtnTree_Click;
            // 
            // btnRecentFolders
            // 
            btnRecentFolders.BackColor = Color.FromArgb(51, 122, 183);
            btnRecentFolders.Cursor = Cursors.Hand;
            btnRecentFolders.FlatAppearance.BorderSize = 0;
            btnRecentFolders.FlatStyle = FlatStyle.Flat;
            btnRecentFolders.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnRecentFolders.ForeColor = Color.White;
            btnRecentFolders.Location = new Point(116, 110);
            btnRecentFolders.Name = "btnRecentFolders";
            btnRecentFolders.Size = new Size(110, 34);
            btnRecentFolders.TabIndex = 6;
            btnRecentFolders.Text = "Recent ▾";
            toolTip1.SetToolTip(btnRecentFolders, "Recently used folders");
            btnRecentFolders.UseVisualStyleBackColor = false;
            btnRecentFolders.Click += BtnRecentFolders_Click;
            // 
            // btnOptions
            // 
            btnOptions.BackColor = Color.FromArgb(108, 117, 125);
            btnOptions.Cursor = Cursors.Hand;
            btnOptions.FlatAppearance.BorderSize = 0;
            btnOptions.FlatStyle = FlatStyle.Flat;
            btnOptions.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnOptions.ForeColor = Color.White;
            btnOptions.Location = new Point(232, 110);
            btnOptions.Name = "btnOptions";
            btnOptions.Size = new Size(110, 34);
            btnOptions.TabIndex = 7;
            btnOptions.Text = "Options";
            toolTip1.SetToolTip(btnOptions, "Configure filters, encoding, watcher");
            btnOptions.UseVisualStyleBackColor = false;
            btnOptions.Click += BtnOptions_Click;
            // 
            // btnSavePreset
            // 
            btnSavePreset.BackColor = Color.FromArgb(40, 167, 69);
            btnSavePreset.Cursor = Cursors.Hand;
            btnSavePreset.FlatAppearance.BorderSize = 0;
            btnSavePreset.FlatStyle = FlatStyle.Flat;
            btnSavePreset.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSavePreset.ForeColor = Color.White;
            btnSavePreset.Location = new Point(348, 110);
            btnSavePreset.Name = "btnSavePreset";
            btnSavePreset.Size = new Size(150, 34);
            btnSavePreset.TabIndex = 8;
            btnSavePreset.Text = "Save preset";
            toolTip1.SetToolTip(btnSavePreset, "Save current folder + extensions as a preset");
            btnSavePreset.UseVisualStyleBackColor = false;
            btnSavePreset.Click += BtnSavePreset_Click;
            // 
            // btnLoadPreset
            // 
            btnLoadPreset.BackColor = Color.FromArgb(40, 167, 69);
            btnLoadPreset.Cursor = Cursors.Hand;
            btnLoadPreset.FlatAppearance.BorderSize = 0;
            btnLoadPreset.FlatStyle = FlatStyle.Flat;
            btnLoadPreset.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnLoadPreset.ForeColor = Color.White;
            btnLoadPreset.Location = new Point(504, 110);
            btnLoadPreset.Name = "btnLoadPreset";
            btnLoadPreset.Size = new Size(130, 34);
            btnLoadPreset.TabIndex = 9;
            btnLoadPreset.Text = "Presets ▾";
            toolTip1.SetToolTip(btnLoadPreset, "Load or manage saved presets");
            btnLoadPreset.UseVisualStyleBackColor = false;
            btnLoadPreset.Click += BtnLoadPreset_Click;
            // 
            // chkWatch
            // 
            chkWatch.AutoSize = true;
            chkWatch.BackColor = Color.FromArgb(0, 102, 204);
            chkWatch.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            chkWatch.ForeColor = Color.White;
            chkWatch.Location = new Point(648, 116);
            chkWatch.Name = "chkWatch";
            chkWatch.Size = new Size(152, 29);
            chkWatch.TabIndex = 10;
            chkWatch.Text = "Watch folder";
            toolTip1.SetToolTip(chkWatch, "Auto-refresh when files change on disk");
            chkWatch.UseVisualStyleBackColor = false;
            chkWatch.CheckedChanged += ChkWatch_CheckedChanged;
            // 
            // pnlLeft
            // 
            pnlLeft.BackColor = Color.FromArgb(245, 247, 250);
            pnlLeft.Controls.Add(grpFiles);
            pnlLeft.Controls.Add(grpExtensions);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 165);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Padding = new Padding(20);
            pnlLeft.Size = new Size(497, 945);
            pnlLeft.TabIndex = 1;
            // 
            // grpFiles
            // 
            grpFiles.BackColor = Color.White;
            grpFiles.Controls.Add(txtSearchFiles);
            grpFiles.Controls.Add(btnSearchFiles);
            grpFiles.Controls.Add(btnSearchRecents);
            grpFiles.Controls.Add(chkCase);
            grpFiles.Controls.Add(chkWord);
            grpFiles.Controls.Add(chkRegex);
            grpFiles.Controls.Add(btnFindReplace);
            grpFiles.Controls.Add(lblSearchMatches);
            grpFiles.Controls.Add(lblSearchFiles);
            grpFiles.Controls.Add(lstFiles);
            grpFiles.Controls.Add(pnlFileButtons);
            grpFiles.Dock = DockStyle.Fill;
            grpFiles.Font = new Font("Segoe UI", 10F);
            grpFiles.ForeColor = Color.FromArgb(0, 102, 204);
            grpFiles.Location = new Point(20, 480);
            grpFiles.Name = "grpFiles";
            grpFiles.Padding = new Padding(10);
            grpFiles.Size = new Size(457, 445);
            grpFiles.TabIndex = 1;
            grpFiles.TabStop = false;
            grpFiles.Text = "Selected Files";
            // 
            // txtSearchFiles
            // 
            txtSearchFiles.BorderStyle = BorderStyle.FixedSingle;
            txtSearchFiles.Location = new Point(10, 55);
            txtSearchFiles.Name = "txtSearchFiles";
            txtSearchFiles.Size = new Size(300, 34);
            txtSearchFiles.TabIndex = 5;
            // 
            // btnSearchFiles
            // 
            btnSearchFiles.BackColor = Color.FromArgb(108, 117, 125);
            btnSearchFiles.FlatStyle = FlatStyle.Flat;
            btnSearchFiles.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSearchFiles.ForeColor = Color.White;
            btnSearchFiles.Location = new Point(320, 55);
            btnSearchFiles.Name = "btnSearchFiles";
            btnSearchFiles.Size = new Size(100, 34);
            btnSearchFiles.TabIndex = 6;
            btnSearchFiles.Text = "Search";
            btnSearchFiles.UseVisualStyleBackColor = false;
            btnSearchFiles.Click += BtnSearchFiles_Click;
            // 
            // btnSearchRecents
            // 
            btnSearchRecents.BackColor = Color.FromArgb(108, 117, 125);
            btnSearchRecents.Cursor = Cursors.Hand;
            btnSearchRecents.FlatAppearance.BorderSize = 0;
            btnSearchRecents.FlatStyle = FlatStyle.Flat;
            btnSearchRecents.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSearchRecents.ForeColor = Color.White;
            btnSearchRecents.Location = new Point(424, 55);
            btnSearchRecents.Name = "btnSearchRecents";
            btnSearchRecents.Size = new Size(30, 34);
            btnSearchRecents.TabIndex = 7;
            btnSearchRecents.Text = "▾";
            toolTip1.SetToolTip(btnSearchRecents, "Recent searches");
            btnSearchRecents.UseVisualStyleBackColor = false;
            btnSearchRecents.Click += BtnSearchRecents_Click;
            // 
            // chkCase
            // 
            chkCase.AutoSize = true;
            chkCase.Font = new Font("Segoe UI", 9.5F);
            chkCase.ForeColor = Color.FromArgb(33, 37, 41);
            chkCase.Location = new Point(10, 102);
            chkCase.Name = "chkCase";
            chkCase.Size = new Size(60, 29);
            chkCase.TabIndex = 8;
            chkCase.Text = "Aa";
            toolTip1.SetToolTip(chkCase, "Match case");
            chkCase.UseVisualStyleBackColor = true;
            // 
            // chkWord
            // 
            chkWord.AutoSize = true;
            chkWord.Font = new Font("Segoe UI", 9.5F);
            chkWord.ForeColor = Color.FromArgb(33, 37, 41);
            chkWord.Location = new Point(75, 102);
            chkWord.Name = "chkWord";
            chkWord.Size = new Size(84, 29);
            chkWord.TabIndex = 9;
            chkWord.Text = "Word";
            toolTip1.SetToolTip(chkWord, "Whole word");
            chkWord.UseVisualStyleBackColor = true;
            // 
            // chkRegex
            // 
            chkRegex.AutoSize = true;
            chkRegex.Font = new Font("Segoe UI", 9.5F);
            chkRegex.ForeColor = Color.FromArgb(33, 37, 41);
            chkRegex.Location = new Point(169, 102);
            chkRegex.Name = "chkRegex";
            chkRegex.Size = new Size(50, 29);
            chkRegex.TabIndex = 10;
            chkRegex.Text = ".*";
            toolTip1.SetToolTip(chkRegex, "Regular expression");
            chkRegex.UseVisualStyleBackColor = true;
            // 
            // btnFindReplace
            // 
            btnFindReplace.BackColor = Color.FromArgb(13, 110, 253);
            btnFindReplace.Cursor = Cursors.Hand;
            btnFindReplace.FlatAppearance.BorderSize = 0;
            btnFindReplace.FlatStyle = FlatStyle.Flat;
            btnFindReplace.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnFindReplace.ForeColor = Color.White;
            btnFindReplace.Location = new Point(294, 102);
            btnFindReplace.Name = "btnFindReplace";
            btnFindReplace.Size = new Size(150, 34);
            btnFindReplace.TabIndex = 11;
            btnFindReplace.Text = "Find / Replace";
            toolTip1.SetToolTip(btnFindReplace, "Find & Replace in the output pane (Ctrl+F)");
            btnFindReplace.UseVisualStyleBackColor = false;
            btnFindReplace.Click += BtnFindReplace_Click;
            // 
            // lblSearchMatches
            // 
            lblSearchMatches.AutoSize = true;
            lblSearchMatches.Font = new Font("Segoe UI", 9F);
            lblSearchMatches.ForeColor = Color.FromArgb(108, 117, 125);
            lblSearchMatches.Location = new Point(10, 142);
            lblSearchMatches.Name = "lblSearchMatches";
            lblSearchMatches.Size = new Size(0, 25);
            lblSearchMatches.TabIndex = 12;
            // 
            // lblSearchFiles
            // 
            lblSearchFiles.AutoSize = true;
            lblSearchFiles.Font = new Font("Segoe UI", 9F);
            lblSearchFiles.Location = new Point(10, 30);
            lblSearchFiles.Name = "lblSearchFiles";
            lblSearchFiles.Size = new Size(126, 25);
            lblSearchFiles.TabIndex = 4;
            lblSearchFiles.Text = "Search in Files:";
            // 
            // lstFiles
            // 
            lstFiles.AllowDrop = true;
            lstFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstFiles.ContextMenuStrip = ctxFiles;
            lstFiles.Font = new Font("Segoe UI", 9F);
            lstFiles.FormattingEnabled = true;
            lstFiles.ItemHeight = 25;
            lstFiles.Location = new Point(10, 170);
            lstFiles.Name = "lstFiles";
            lstFiles.SelectionMode = SelectionMode.MultiExtended;
            lstFiles.Size = new Size(437, 179);
            lstFiles.TabIndex = 0;
            lstFiles.DragDrop += LstFiles_DragDrop;
            lstFiles.DragEnter += LstFiles_DragEnter;
            lstFiles.KeyDown += LstFiles_KeyDown;
            // 
            // ctxFiles
            // 
            ctxFiles.ImageScalingSize = new Size(24, 24);
            ctxFiles.Items.AddRange(new ToolStripItem[] { miSortByName, miSortByExtension });
            ctxFiles.Name = "ctxFiles";
            ctxFiles.Size = new Size(223, 68);
            // 
            // miSortByName
            // 
            miSortByName.Name = "miSortByName";
            miSortByName.Size = new Size(222, 32);
            miSortByName.Text = "Sort by Name";
            miSortByName.Click += MiSortByName_Click;
            // 
            // miSortByExtension
            // 
            miSortByExtension.Name = "miSortByExtension";
            miSortByExtension.Size = new Size(222, 32);
            miSortByExtension.Text = "Sort by Extension";
            miSortByExtension.Click += MiSortByExtension_Click;
            // 
            // pnlFileButtons
            // 
            pnlFileButtons.Controls.Add(lblFileCount);
            pnlFileButtons.Controls.Add(btnAddMultipleFiles);
            pnlFileButtons.Controls.Add(btnRemoveFile);
            pnlFileButtons.Controls.Add(btnMoveUp);
            pnlFileButtons.Controls.Add(btnMoveDown);
            pnlFileButtons.Dock = DockStyle.Bottom;
            pnlFileButtons.Location = new Point(10, 387);
            pnlFileButtons.Name = "pnlFileButtons";
            pnlFileButtons.Size = new Size(437, 48);
            pnlFileButtons.TabIndex = 1;
            // 
            // lblFileCount
            // 
            lblFileCount.AutoSize = true;
            lblFileCount.Location = new Point(5, 12);
            lblFileCount.Name = "lblFileCount";
            lblFileCount.Size = new Size(70, 28);
            lblFileCount.TabIndex = 0;
            lblFileCount.Text = "Files: 0";
            // 
            // btnAddMultipleFiles
            // 
            btnAddMultipleFiles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddMultipleFiles.BackColor = Color.FromArgb(40, 167, 69);
            btnAddMultipleFiles.FlatStyle = FlatStyle.Flat;
            btnAddMultipleFiles.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAddMultipleFiles.ForeColor = Color.White;
            btnAddMultipleFiles.Location = new Point(130, 5);
            btnAddMultipleFiles.Name = "btnAddMultipleFiles";
            btnAddMultipleFiles.Size = new Size(100, 38);
            btnAddMultipleFiles.TabIndex = 1;
            btnAddMultipleFiles.Text = "Add Files";
            btnAddMultipleFiles.UseVisualStyleBackColor = false;
            btnAddMultipleFiles.Click += BtnAddMultipleFiles_Click;
            // 
            // btnRemoveFile
            // 
            btnRemoveFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRemoveFile.BackColor = Color.FromArgb(220, 53, 69);
            btnRemoveFile.FlatStyle = FlatStyle.Flat;
            btnRemoveFile.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRemoveFile.ForeColor = Color.White;
            btnRemoveFile.Location = new Point(240, 5);
            btnRemoveFile.Name = "btnRemoveFile";
            btnRemoveFile.Size = new Size(90, 38);
            btnRemoveFile.TabIndex = 2;
            btnRemoveFile.Text = "Remove";
            btnRemoveFile.UseVisualStyleBackColor = false;
            btnRemoveFile.Click += BtnRemoveFile_Click;
            // 
            // btnMoveUp
            // 
            btnMoveUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoveUp.BackColor = Color.FromArgb(233, 236, 239);
            btnMoveUp.FlatAppearance.BorderColor = Color.FromArgb(206, 212, 218);
            btnMoveUp.FlatStyle = FlatStyle.Flat;
            btnMoveUp.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnMoveUp.ForeColor = Color.FromArgb(73, 80, 87);
            btnMoveUp.Location = new Point(340, 5);
            btnMoveUp.Name = "btnMoveUp";
            btnMoveUp.Size = new Size(40, 38);
            btnMoveUp.TabIndex = 3;
            btnMoveUp.Text = "▲";
            btnMoveUp.UseVisualStyleBackColor = false;
            btnMoveUp.Click += BtnMoveUp_Click;
            // 
            // btnMoveDown
            // 
            btnMoveDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoveDown.BackColor = Color.FromArgb(233, 236, 239);
            btnMoveDown.FlatAppearance.BorderColor = Color.FromArgb(206, 212, 218);
            btnMoveDown.FlatStyle = FlatStyle.Flat;
            btnMoveDown.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnMoveDown.ForeColor = Color.FromArgb(73, 80, 87);
            btnMoveDown.Location = new Point(385, 5);
            btnMoveDown.Name = "btnMoveDown";
            btnMoveDown.Size = new Size(40, 38);
            btnMoveDown.TabIndex = 4;
            btnMoveDown.Text = "▼";
            btnMoveDown.UseVisualStyleBackColor = false;
            btnMoveDown.Click += BtnMoveDown_Click;
            // 
            // grpExtensions
            // 
            grpExtensions.BackColor = Color.White;
            grpExtensions.Controls.Add(txtIgnorePatterns);
            grpExtensions.Controls.Add(lblIgnorePatterns);
            grpExtensions.Controls.Add(lblExtension);
            grpExtensions.Controls.Add(cmbExtension);
            grpExtensions.Controls.Add(btnAdd);
            grpExtensions.Controls.Add(lstExtensions);
            grpExtensions.Controls.Add(btnRemove);
            grpExtensions.Controls.Add(chkIncludeSubfolders);
            grpExtensions.Controls.Add(btnRefreshExtensions);
            grpExtensions.Dock = DockStyle.Top;
            grpExtensions.Font = new Font("Segoe UI", 10F);
            grpExtensions.ForeColor = Color.FromArgb(0, 102, 204);
            grpExtensions.Location = new Point(20, 20);
            grpExtensions.Name = "grpExtensions";
            grpExtensions.Padding = new Padding(10);
            grpExtensions.Size = new Size(457, 460);
            grpExtensions.TabIndex = 0;
            grpExtensions.TabStop = false;
            grpExtensions.Text = "File Extensions";
            // 
            // txtIgnorePatterns
            // 
            txtIgnorePatterns.BorderStyle = BorderStyle.FixedSingle;
            txtIgnorePatterns.Location = new Point(10, 410);
            txtIgnorePatterns.Name = "txtIgnorePatterns";
            txtIgnorePatterns.Size = new Size(430, 34);
            txtIgnorePatterns.TabIndex = 7;
            txtIgnorePatterns.TextChanged += TxtIgnorePatterns_TextChanged;
            // 
            // lblIgnorePatterns
            // 
            lblIgnorePatterns.AutoSize = true;
            lblIgnorePatterns.Font = new Font("Segoe UI", 9F);
            lblIgnorePatterns.Location = new Point(10, 382);
            lblIgnorePatterns.Name = "lblIgnorePatterns";
            lblIgnorePatterns.Size = new Size(136, 25);
            lblIgnorePatterns.TabIndex = 6;
            lblIgnorePatterns.Text = "Ignore Patterns:";
            // 
            // lblExtension
            // 
            lblExtension.AutoSize = true;
            lblExtension.Font = new Font("Segoe UI", 9F);
            lblExtension.Location = new Point(10, 30);
            lblExtension.Name = "lblExtension";
            lblExtension.Size = new Size(161, 25);
            lblExtension.TabIndex = 0;
            lblExtension.Text = "Add File Extension:";
            // 
            // cmbExtension
            // 
            cmbExtension.Location = new Point(10, 60);
            cmbExtension.Name = "cmbExtension";
            cmbExtension.Size = new Size(249, 36);
            cmbExtension.TabIndex = 1;
            cmbExtension.KeyPress += CmbExtension_KeyPress;
            // 
            // btnAdd
            // 
            btnAdd.BackColor = Color.FromArgb(51, 122, 183);
            btnAdd.DropDownMenu = cmsAddDropdown;
            btnAdd.DropDownWidth = 22;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(270, 60);
            btnAdd.Name = "btnAdd";
            btnAdd.ShowSplit = true;
            btnAdd.Size = new Size(133, 38);
            btnAdd.TabIndex = 2;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += BtnAdd_Click;
            // 
            // cmsAddDropdown
            // 
            cmsAddDropdown.ImageScalingSize = new Size(24, 24);
            cmsAddDropdown.Items.AddRange(new ToolStripItem[] { miShowExtensionSummary });
            cmsAddDropdown.Name = "cmsAddDropdown";
            cmsAddDropdown.Size = new Size(301, 36);
            // 
            // miShowExtensionSummary
            // 
            miShowExtensionSummary.Name = "miShowExtensionSummary";
            miShowExtensionSummary.Size = new Size(300, 32);
            miShowExtensionSummary.Text = "Show extension summary…";
            miShowExtensionSummary.Click += MiShowExtensionSummary_Click;
            // 
            // lstExtensions
            // 
            lstExtensions.FormattingEnabled = true;
            lstExtensions.ItemHeight = 28;
            lstExtensions.Location = new Point(10, 105);
            lstExtensions.Name = "lstExtensions";
            lstExtensions.Size = new Size(249, 228);
            lstExtensions.TabIndex = 3;
            // 
            // btnRemove
            // 
            btnRemove.BackColor = Color.FromArgb(220, 53, 69);
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnRemove.ForeColor = Color.White;
            btnRemove.Location = new Point(270, 105);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(133, 38);
            btnRemove.TabIndex = 4;
            btnRemove.Text = "Remove";
            btnRemove.UseVisualStyleBackColor = false;
            btnRemove.Click += BtnRemove_Click;
            // 
            // chkIncludeSubfolders
            // 
            chkIncludeSubfolders.AutoSize = true;
            chkIncludeSubfolders.Checked = true;
            chkIncludeSubfolders.CheckState = CheckState.Checked;
            chkIncludeSubfolders.Location = new Point(10, 345);
            chkIncludeSubfolders.Name = "chkIncludeSubfolders";
            chkIncludeSubfolders.Size = new Size(209, 32);
            chkIncludeSubfolders.TabIndex = 5;
            chkIncludeSubfolders.Text = "Include subfolder(s)";
            chkIncludeSubfolders.UseVisualStyleBackColor = true;
            chkIncludeSubfolders.CheckedChanged += ChkIncludeSubfolders_CheckedChanged;
            // 
            // btnRefreshExtensions
            // 
            btnRefreshExtensions.BackColor = Color.FromArgb(51, 122, 183);
            btnRefreshExtensions.FlatStyle = FlatStyle.Flat;
            btnRefreshExtensions.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnRefreshExtensions.ForeColor = Color.White;
            btnRefreshExtensions.Location = new Point(270, 150);
            btnRefreshExtensions.Name = "btnRefreshExtensions";
            btnRefreshExtensions.Size = new Size(133, 38);
            btnRefreshExtensions.TabIndex = 6;
            btnRefreshExtensions.Text = "Refresh";
            btnRefreshExtensions.UseVisualStyleBackColor = false;
            btnRefreshExtensions.Click += BtnRefreshExtensions_Click;
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.FromArgb(245, 247, 250);
            pnlBottom.Controls.Add(progressBar);
            pnlBottom.Controls.Add(btnGenerate);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 1110);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(1640, 115);
            pnlBottom.TabIndex = 3;
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(20, 80);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(1600, 25);
            progressBar.TabIndex = 1;
            progressBar.Visible = false;
            // 
            // btnGenerate
            // 
            btnGenerate.Anchor = AnchorStyles.None;
            btnGenerate.BackColor = Color.FromArgb(0, 123, 255);
            btnGenerate.FlatStyle = FlatStyle.Flat;
            btnGenerate.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnGenerate.ForeColor = Color.White;
            btnGenerate.Location = new Point(695, 20);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(250, 60);
            btnGenerate.TabIndex = 0;
            btnGenerate.Text = "▶  GENERATE";
            btnGenerate.UseVisualStyleBackColor = false;
            btnGenerate.Click += BtnGenerate_Click;
            // 
            // pnlRight
            // 
            pnlRight.BackColor = Color.FromArgb(245, 247, 250);
            pnlRight.Controls.Add(rtbOutput);
            pnlRight.Controls.Add(pnlRecreateInfo);
            pnlRight.Controls.Add(pnlOutput);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Location = new Point(497, 165);
            pnlRight.Name = "pnlRight";
            pnlRight.Padding = new Padding(20);
            pnlRight.Size = new Size(1143, 945);
            pnlRight.TabIndex = 2;
            // 
            // rtbOutput
            // 
            rtbOutput.BackColor = Color.White;
            rtbOutput.Dock = DockStyle.Fill;
            rtbOutput.Font = new Font("Consolas", 10F);
            rtbOutput.Location = new Point(20, 240);
            rtbOutput.Name = "rtbOutput";
            rtbOutput.ReadOnly = true;
            rtbOutput.Size = new Size(1103, 685);
            rtbOutput.TabIndex = 1;
            rtbOutput.Text = "";
            rtbOutput.WordWrap = false;
            // 
            // pnlRecreateInfo
            // 
            pnlRecreateInfo.BackColor = Color.FromArgb(255, 255, 224);
            pnlRecreateInfo.Controls.Add(tblRecreateInfo);
            pnlRecreateInfo.Dock = DockStyle.Top;
            pnlRecreateInfo.Location = new Point(20, 160);
            pnlRecreateInfo.Name = "pnlRecreateInfo";
            pnlRecreateInfo.Size = new Size(1103, 80);
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
            tblRecreateInfo.Size = new Size(1103, 80);
            tblRecreateInfo.TabIndex = 0;
            // 
            // lblRecreateInfo
            // 
            lblRecreateInfo.Dock = DockStyle.Fill;
            lblRecreateInfo.Font = new Font("Segoe UI", 9.5F);
            lblRecreateInfo.ForeColor = Color.FromArgb(120, 80, 0);
            lblRecreateInfo.Location = new Point(3, 0);
            lblRecreateInfo.Name = "lblRecreateInfo";
            lblRecreateInfo.Size = new Size(951, 80);
            lblRecreateInfo.TabIndex = 0;
            lblRecreateInfo.Text = "🗂️ Recreate Files: Restore files and folders from the output below.";
            lblRecreateInfo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnRecreateFiles
            // 
            btnRecreateFiles.BackColor = Color.FromArgb(40, 167, 69);
            btnRecreateFiles.FlatStyle = FlatStyle.Flat;
            btnRecreateFiles.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnRecreateFiles.ForeColor = Color.White;
            btnRecreateFiles.Location = new Point(960, 3);
            btnRecreateFiles.Name = "btnRecreateFiles";
            btnRecreateFiles.Size = new Size(140, 40);
            btnRecreateFiles.TabIndex = 1;
            btnRecreateFiles.Text = "Recreate Files";
            btnRecreateFiles.UseVisualStyleBackColor = false;
            btnRecreateFiles.Click += BtnRecreateFiles_Click;
            // 
            // pnlOutput
            // 
            pnlOutput.BackColor = Color.White;
            pnlOutput.Controls.Add(lblOutputStats);
            pnlOutput.Controls.Add(pnlOutputHeader);
            pnlOutput.Controls.Add(pnlCompressionTools);
            pnlOutput.Controls.Add(pnlSeparator);
            pnlOutput.Dock = DockStyle.Top;
            pnlOutput.Location = new Point(20, 20);
            pnlOutput.Name = "pnlOutput";
            pnlOutput.Size = new Size(1103, 140);
            pnlOutput.TabIndex = 5;
            // 
            // lblOutputStats
            // 
            lblOutputStats.AutoSize = true;
            lblOutputStats.ForeColor = Color.FromArgb(108, 117, 125);
            lblOutputStats.Location = new Point(10, 110);
            lblOutputStats.Name = "lblOutputStats";
            lblOutputStats.Size = new Size(259, 25);
            lblOutputStats.TabIndex = 4;
            lblOutputStats.Text = "Chars: 0 | Lines: 0 | Size: 0 bytes";
            // 
            // pnlOutputHeader
            // 
            pnlOutputHeader.Controls.Add(btnExportOutput);
            pnlOutputHeader.Controls.Add(btnEditOutput);
            pnlOutputHeader.Controls.Add(btnCopyOutput);
            pnlOutputHeader.Controls.Add(lblOutput);
            pnlOutputHeader.Dock = DockStyle.Top;
            pnlOutputHeader.Location = new Point(0, 55);
            pnlOutputHeader.Name = "pnlOutputHeader";
            pnlOutputHeader.Size = new Size(1103, 50);
            pnlOutputHeader.TabIndex = 0;
            // 
            // btnExportOutput
            // 
            btnExportOutput.BackColor = Color.FromArgb(248, 249, 250);
            btnExportOutput.Cursor = Cursors.Hand;
            btnExportOutput.Dock = DockStyle.Right;
            btnExportOutput.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            btnExportOutput.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            btnExportOutput.FlatStyle = FlatStyle.Flat;
            btnExportOutput.Font = new Font("Segoe UI", 9F);
            btnExportOutput.ForeColor = Color.FromArgb(73, 80, 87);
            btnExportOutput.Location = new Point(798, 0);
            btnExportOutput.Name = "btnExportOutput";
            btnExportOutput.Size = new Size(105, 50);
            btnExportOutput.TabIndex = 4;
            btnExportOutput.Text = "💾 Export";
            toolTip1.SetToolTip(btnExportOutput, "Export output to file");
            btnExportOutput.UseVisualStyleBackColor = false;
            btnExportOutput.Click += BtnExportOutput_Click;
            // 
            // btnEditOutput
            // 
            btnEditOutput.BackColor = Color.FromArgb(248, 249, 250);
            btnEditOutput.Cursor = Cursors.Hand;
            btnEditOutput.Dock = DockStyle.Right;
            btnEditOutput.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            btnEditOutput.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            btnEditOutput.FlatStyle = FlatStyle.Flat;
            btnEditOutput.Font = new Font("Segoe UI", 9F);
            btnEditOutput.ForeColor = Color.FromArgb(73, 80, 87);
            btnEditOutput.Location = new Point(903, 0);
            btnEditOutput.Name = "btnEditOutput";
            btnEditOutput.Size = new Size(100, 50);
            btnEditOutput.TabIndex = 3;
            btnEditOutput.Text = "✏️ Edit";
            toolTip1.SetToolTip(btnEditOutput, "Edit the output");
            btnEditOutput.UseVisualStyleBackColor = false;
            btnEditOutput.Click += BtnEditOutput_Click;
            // 
            // btnCopyOutput
            // 
            btnCopyOutput.BackColor = Color.FromArgb(248, 249, 250);
            btnCopyOutput.Cursor = Cursors.Hand;
            btnCopyOutput.Dock = DockStyle.Right;
            btnCopyOutput.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            btnCopyOutput.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            btnCopyOutput.FlatStyle = FlatStyle.Flat;
            btnCopyOutput.Font = new Font("Segoe UI", 9F);
            btnCopyOutput.ForeColor = Color.FromArgb(73, 80, 87);
            btnCopyOutput.Location = new Point(1003, 0);
            btnCopyOutput.Name = "btnCopyOutput";
            btnCopyOutput.Size = new Size(100, 50);
            btnCopyOutput.TabIndex = 2;
            btnCopyOutput.Text = "📋 Copy";
            toolTip1.SetToolTip(btnCopyOutput, "Copy output to clipboard");
            btnCopyOutput.UseVisualStyleBackColor = false;
            btnCopyOutput.Click += BtnCopyOutput_Click;
            // 
            // lblOutput
            // 
            lblOutput.AutoSize = true;
            lblOutput.Font = new Font("Segoe UI Semibold", 11F);
            lblOutput.Location = new Point(10, 10);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new Size(85, 30);
            lblOutput.TabIndex = 5;
            lblOutput.Text = "Output";
            // 
            // pnlCompressionTools
            // 
            pnlCompressionTools.Controls.Add(btnDecompressEnc);
            pnlCompressionTools.Controls.Add(btnCompressEnc);
            pnlCompressionTools.Controls.Add(btnDecompress);
            pnlCompressionTools.Controls.Add(btnCompress);
            pnlCompressionTools.Controls.Add(lblCompression);
            pnlCompressionTools.Dock = DockStyle.Top;
            pnlCompressionTools.Location = new Point(0, 1);
            pnlCompressionTools.Name = "pnlCompressionTools";
            pnlCompressionTools.Size = new Size(1103, 54);
            pnlCompressionTools.TabIndex = 2;
            // 
            // btnDecompressEnc
            // 
            btnDecompressEnc.BackColor = Color.FromArgb(220, 53, 69);
            btnDecompressEnc.Dock = DockStyle.Right;
            btnDecompressEnc.ForeColor = Color.White;
            btnDecompressEnc.Location = new Point(323, 0);
            btnDecompressEnc.Name = "btnDecompressEnc";
            btnDecompressEnc.Size = new Size(240, 54);
            btnDecompressEnc.TabIndex = 0;
            btnDecompressEnc.Text = "🔓 Decrypt + Decompress";
            btnDecompressEnc.UseVisualStyleBackColor = false;
            btnDecompressEnc.Click += BtnDecompressEnc_Click;
            // 
            // btnCompressEnc
            // 
            btnCompressEnc.BackColor = Color.FromArgb(25, 135, 84);
            btnCompressEnc.Dock = DockStyle.Right;
            btnCompressEnc.ForeColor = Color.White;
            btnCompressEnc.Location = new Point(563, 0);
            btnCompressEnc.Name = "btnCompressEnc";
            btnCompressEnc.Size = new Size(240, 54);
            btnCompressEnc.TabIndex = 1;
            btnCompressEnc.Text = "🔒 Compress + Encrypt";
            btnCompressEnc.UseVisualStyleBackColor = false;
            btnCompressEnc.Click += BtnCompressEnc_Click;
            // 
            // btnDecompress
            // 
            btnDecompress.BackColor = Color.FromArgb(108, 117, 125);
            btnDecompress.Dock = DockStyle.Right;
            btnDecompress.ForeColor = Color.White;
            btnDecompress.Location = new Point(803, 0);
            btnDecompress.Name = "btnDecompress";
            btnDecompress.Size = new Size(150, 54);
            btnDecompress.TabIndex = 2;
            btnDecompress.Text = "Decompress";
            btnDecompress.UseVisualStyleBackColor = false;
            btnDecompress.Click += BtnDecompress_Click;
            // 
            // btnCompress
            // 
            btnCompress.BackColor = Color.FromArgb(13, 110, 253);
            btnCompress.Dock = DockStyle.Right;
            btnCompress.ForeColor = Color.White;
            btnCompress.Location = new Point(953, 0);
            btnCompress.Name = "btnCompress";
            btnCompress.Size = new Size(150, 54);
            btnCompress.TabIndex = 3;
            btnCompress.Text = "Compress";
            btnCompress.UseVisualStyleBackColor = false;
            btnCompress.Click += BtnCompress_Click;
            // 
            // lblCompression
            // 
            lblCompression.AutoSize = true;
            lblCompression.Location = new Point(10, 15);
            lblCompression.Name = "lblCompression";
            lblCompression.Size = new Size(167, 25);
            lblCompression.TabIndex = 0;
            lblCompression.Text = "Compression Tools:";
            // 
            // pnlSeparator
            // 
            pnlSeparator.BackColor = Color.FromArgb(220, 220, 220);
            pnlSeparator.Dock = DockStyle.Top;
            pnlSeparator.Location = new Point(0, 0);
            pnlSeparator.Name = "pnlSeparator";
            pnlSeparator.Size = new Size(1103, 1);
            pnlSeparator.TabIndex = 1;
            //
            // menuMain
            //
            menuMain.BackColor = Color.FromArgb(245, 247, 250);
            menuMain.Font = new Font("Segoe UI", 9.5F);
            menuMain.ImageScalingSize = new Size(20, 20);
            menuMain.Items.AddRange(new ToolStripItem[] { mnuHelp });
            menuMain.Location = new Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Padding = new Padding(6, 2, 0, 2);
            menuMain.Size = new Size(1640, 28);
            menuMain.TabIndex = 11;
            //
            // mnuHelp
            //
            mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { mnuHelpShortcuts, mnuHelpSep1, mnuHelpAbout });
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new Size(56, 24);
            mnuHelp.Text = "&Help";
            //
            // mnuHelpShortcuts
            //
            mnuHelpShortcuts.Name = "mnuHelpShortcuts";
            mnuHelpShortcuts.ShortcutKeys = Keys.F1;
            mnuHelpShortcuts.Size = new Size(220, 26);
            mnuHelpShortcuts.Text = "&Keyboard Shortcuts…";
            mnuHelpShortcuts.Click += MnuHelpShortcuts_Click;
            //
            // mnuHelpSep1
            //
            mnuHelpSep1.Name = "mnuHelpSep1";
            mnuHelpSep1.Size = new Size(217, 6);
            //
            // mnuHelpAbout
            //
            mnuHelpAbout.Name = "mnuHelpAbout";
            mnuHelpAbout.Size = new Size(220, 26);
            mnuHelpAbout.Text = "&About…";
            mnuHelpAbout.Click += MnuHelpAbout_Click;
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1640, 1225);
            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            Controls.Add(menuMain);
            MainMenuStrip = menuMain;
            MinimumSize = new Size(1315, 1087);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "File Content Toolkit";
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlLeft.ResumeLayout(false);
            grpFiles.ResumeLayout(false);
            grpFiles.PerformLayout();
            ctxFiles.ResumeLayout(false);
            pnlFileButtons.ResumeLayout(false);
            pnlFileButtons.PerformLayout();
            grpExtensions.ResumeLayout(false);
            grpExtensions.PerformLayout();
            cmsAddDropdown.ResumeLayout(false);
            pnlBottom.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            pnlRecreateInfo.ResumeLayout(false);
            tblRecreateInfo.ResumeLayout(false);
            pnlOutput.ResumeLayout(false);
            pnlOutput.PerformLayout();
            pnlOutputHeader.ResumeLayout(false);
            pnlOutputHeader.PerformLayout();
            pnlCompressionTools.ResumeLayout(false);
            pnlCompressionTools.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}