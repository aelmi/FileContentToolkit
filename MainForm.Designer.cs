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
        private FileContentToolkit.UI.SplitButton btnCopyOutput;
        private System.Windows.Forms.Button btnEditOutput;
        private System.Windows.Forms.ContextMenuStrip cmsCopyAs;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyPlain;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyMarkdown;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyXml;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyJson;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip cmsAddDropdown;
        private System.Windows.Forms.ToolStripMenuItem miShowExtensionSummary;
        private System.Windows.Forms.ToolStripMenuItem mnuAddLangPresets;
        private System.Windows.Forms.ToolStripSeparator mnuAddSep1;
        private System.Windows.Forms.ToolStripMenuItem mnuLangCs;
        private System.Windows.Forms.ToolStripMenuItem mnuLangCpp;
        private System.Windows.Forms.ToolStripMenuItem mnuLangWeb;
        private System.Windows.Forms.ToolStripMenuItem mnuLangTs;
        private System.Windows.Forms.ToolStripMenuItem mnuLangNode;
        private System.Windows.Forms.ToolStripMenuItem mnuLangPy;
        private System.Windows.Forms.ToolStripMenuItem mnuLangJava;
        private System.Windows.Forms.ToolStripMenuItem mnuLangKotlin;
        private System.Windows.Forms.ToolStripMenuItem mnuLangGo;
        private System.Windows.Forms.ToolStripMenuItem mnuLangRust;
        private System.Windows.Forms.ToolStripMenuItem mnuLangRuby;
        private System.Windows.Forms.ToolStripMenuItem mnuLangPhp;
        private System.Windows.Forms.ToolStripMenuItem mnuLangSwift;
        private System.Windows.Forms.ToolStripMenuItem mnuLangShell;
        private System.Windows.Forms.ToolStripMenuItem mnuLangDocs;
        private System.Windows.Forms.ToolStripMenuItem mnuLangConfig;

        // Designer-owned popups for the Recent / Searches / Presets buttons.
        // Item slots are persisted here; at runtime only Text / Tag / Visible are updated.
        private System.Windows.Forms.ContextMenuStrip cmsRecentFolders;
        private System.Windows.Forms.ToolStripMenuItem mnuRf01;
        private System.Windows.Forms.ToolStripMenuItem mnuRf02;
        private System.Windows.Forms.ToolStripMenuItem mnuRf03;
        private System.Windows.Forms.ToolStripMenuItem mnuRf04;
        private System.Windows.Forms.ToolStripMenuItem mnuRf05;
        private System.Windows.Forms.ToolStripMenuItem mnuRf06;
        private System.Windows.Forms.ToolStripMenuItem mnuRf07;
        private System.Windows.Forms.ToolStripMenuItem mnuRf08;
        private System.Windows.Forms.ToolStripMenuItem mnuRf09;
        private System.Windows.Forms.ToolStripMenuItem mnuRf10;
        private System.Windows.Forms.ToolStripMenuItem mnuRf11;
        private System.Windows.Forms.ToolStripMenuItem mnuRf12;
        private System.Windows.Forms.ToolStripMenuItem mnuRf13;
        private System.Windows.Forms.ToolStripMenuItem mnuRf14;
        private System.Windows.Forms.ToolStripMenuItem mnuRf15;
        private System.Windows.Forms.ToolStripMenuItem mnuRfEmpty;
        private System.Windows.Forms.ToolStripSeparator mnuRfSep;
        private System.Windows.Forms.ToolStripMenuItem mnuRfClear;

        private System.Windows.Forms.ContextMenuStrip cmsRecentSearches;
        private System.Windows.Forms.ToolStripMenuItem mnuRs01;
        private System.Windows.Forms.ToolStripMenuItem mnuRs02;
        private System.Windows.Forms.ToolStripMenuItem mnuRs03;
        private System.Windows.Forms.ToolStripMenuItem mnuRs04;
        private System.Windows.Forms.ToolStripMenuItem mnuRs05;
        private System.Windows.Forms.ToolStripMenuItem mnuRs06;
        private System.Windows.Forms.ToolStripMenuItem mnuRs07;
        private System.Windows.Forms.ToolStripMenuItem mnuRs08;
        private System.Windows.Forms.ToolStripMenuItem mnuRs09;
        private System.Windows.Forms.ToolStripMenuItem mnuRs10;
        private System.Windows.Forms.ToolStripMenuItem mnuRs11;
        private System.Windows.Forms.ToolStripMenuItem mnuRs12;
        private System.Windows.Forms.ToolStripMenuItem mnuRs13;
        private System.Windows.Forms.ToolStripMenuItem mnuRs14;
        private System.Windows.Forms.ToolStripMenuItem mnuRs15;
        private System.Windows.Forms.ToolStripMenuItem mnuRsEmpty;
        private System.Windows.Forms.ToolStripSeparator mnuRsSep;
        private System.Windows.Forms.ToolStripMenuItem mnuRsClear;

        private System.Windows.Forms.ContextMenuStrip cmsPresets;
        private System.Windows.Forms.ToolStripMenuItem mnuPs01;
        private System.Windows.Forms.ToolStripMenuItem mnuPs02;
        private System.Windows.Forms.ToolStripMenuItem mnuPs03;
        private System.Windows.Forms.ToolStripMenuItem mnuPs04;
        private System.Windows.Forms.ToolStripMenuItem mnuPs05;
        private System.Windows.Forms.ToolStripMenuItem mnuPs06;
        private System.Windows.Forms.ToolStripMenuItem mnuPs07;
        private System.Windows.Forms.ToolStripMenuItem mnuPs08;
        private System.Windows.Forms.ToolStripMenuItem mnuPs09;
        private System.Windows.Forms.ToolStripMenuItem mnuPs10;
        private System.Windows.Forms.ToolStripMenuItem mnuPs11;
        private System.Windows.Forms.ToolStripMenuItem mnuPs12;
        private System.Windows.Forms.ToolStripMenuItem mnuPs13;
        private System.Windows.Forms.ToolStripMenuItem mnuPs14;
        private System.Windows.Forms.ToolStripMenuItem mnuPs15;
        private System.Windows.Forms.ToolStripMenuItem mnuPs16;
        private System.Windows.Forms.ToolStripMenuItem mnuPs17;
        private System.Windows.Forms.ToolStripMenuItem mnuPs18;
        private System.Windows.Forms.ToolStripMenuItem mnuPs19;
        private System.Windows.Forms.ToolStripMenuItem mnuPs20;
        private System.Windows.Forms.ToolStripMenuItem mnuPs21;
        private System.Windows.Forms.ToolStripMenuItem mnuPs22;
        private System.Windows.Forms.ToolStripMenuItem mnuPs23;
        private System.Windows.Forms.ToolStripMenuItem mnuPs24;
        private System.Windows.Forms.ToolStripMenuItem mnuPs25;
        private System.Windows.Forms.ToolStripMenuItem mnuPsEmpty;
        private System.Windows.Forms.ToolStripSeparator mnuPsSep;
        private System.Windows.Forms.ToolStripMenuItem mnuPsManage;

        private System.Windows.Forms.ContextMenuStrip ctxFiles;
        private System.Windows.Forms.ToolStripMenuItem miSortByName;
        private System.Windows.Forms.ToolStripMenuItem miSortByExtension;
        private System.Windows.Forms.ToolStripSeparator miFilesSep1;
        private System.Windows.Forms.ToolStripMenuItem miOpenFile;
        private System.Windows.Forms.ToolStripMenuItem miRevealInExplorer;
        private System.Windows.Forms.ToolStripMenuItem miCopyPath;
        private System.Windows.Forms.ToolStripMenuItem miOpenContainingFolder;
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
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuViewDarkMode;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpShortcuts;
        private System.Windows.Forms.ToolStripSeparator mnuHelpSep1;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpCheckUpdates;

        // --- status bar ---
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel sbFileCount;
        private System.Windows.Forms.ToolStripStatusLabel sbTotalSize;
        private System.Windows.Forms.ToolStripStatusLabel sbSpring;
        private System.Windows.Forms.ToolStripStatusLabel sbScanStatus;
        private System.Windows.Forms.ToolStripStatusLabel sbUpdateNotice;

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
            lblSearchFiles = new Label();
            lstFiles = new ListBox();
            ctxFiles = new ContextMenuStrip(components);
            miOpenFile = new ToolStripMenuItem();
            miRevealInExplorer = new ToolStripMenuItem();
            miOpenContainingFolder = new ToolStripMenuItem();
            miCopyPath = new ToolStripMenuItem();
            miFilesSep1 = new ToolStripSeparator();
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
            mnuAddLangPresets = new ToolStripMenuItem();
            mnuLangCs = new ToolStripMenuItem();
            mnuLangCpp = new ToolStripMenuItem();
            mnuLangWeb = new ToolStripMenuItem();
            mnuLangTs = new ToolStripMenuItem();
            mnuLangNode = new ToolStripMenuItem();
            mnuLangPy = new ToolStripMenuItem();
            mnuLangJava = new ToolStripMenuItem();
            mnuLangKotlin = new ToolStripMenuItem();
            mnuLangGo = new ToolStripMenuItem();
            mnuLangRust = new ToolStripMenuItem();
            mnuLangRuby = new ToolStripMenuItem();
            mnuLangPhp = new ToolStripMenuItem();
            mnuLangSwift = new ToolStripMenuItem();
            mnuLangShell = new ToolStripMenuItem();
            mnuLangDocs = new ToolStripMenuItem();
            mnuLangConfig = new ToolStripMenuItem();
            mnuAddSep1 = new ToolStripSeparator();
            miShowExtensionSummary = new ToolStripMenuItem();
            lstExtensions = new ListBox();
            btnRemove = new Button();
            chkIncludeSubfolders = new CheckBox();
            btnRefreshExtensions = new Button();
            cmsRecentFolders = new ContextMenuStrip(components);
            mnuRfEmpty = new ToolStripMenuItem();
            mnuRf01 = new ToolStripMenuItem();
            mnuRf02 = new ToolStripMenuItem();
            mnuRf03 = new ToolStripMenuItem();
            mnuRf04 = new ToolStripMenuItem();
            mnuRf05 = new ToolStripMenuItem();
            mnuRf06 = new ToolStripMenuItem();
            mnuRf07 = new ToolStripMenuItem();
            mnuRf08 = new ToolStripMenuItem();
            mnuRf09 = new ToolStripMenuItem();
            mnuRf10 = new ToolStripMenuItem();
            mnuRf11 = new ToolStripMenuItem();
            mnuRf12 = new ToolStripMenuItem();
            mnuRf13 = new ToolStripMenuItem();
            mnuRf14 = new ToolStripMenuItem();
            mnuRf15 = new ToolStripMenuItem();
            mnuRfSep = new ToolStripSeparator();
            mnuRfClear = new ToolStripMenuItem();
            cmsRecentSearches = new ContextMenuStrip(components);
            mnuRsEmpty = new ToolStripMenuItem();
            mnuRs01 = new ToolStripMenuItem();
            mnuRs02 = new ToolStripMenuItem();
            mnuRs03 = new ToolStripMenuItem();
            mnuRs04 = new ToolStripMenuItem();
            mnuRs05 = new ToolStripMenuItem();
            mnuRs06 = new ToolStripMenuItem();
            mnuRs07 = new ToolStripMenuItem();
            mnuRs08 = new ToolStripMenuItem();
            mnuRs09 = new ToolStripMenuItem();
            mnuRs10 = new ToolStripMenuItem();
            mnuRs11 = new ToolStripMenuItem();
            mnuRs12 = new ToolStripMenuItem();
            mnuRs13 = new ToolStripMenuItem();
            mnuRs14 = new ToolStripMenuItem();
            mnuRs15 = new ToolStripMenuItem();
            mnuRsSep = new ToolStripSeparator();
            mnuRsClear = new ToolStripMenuItem();
            cmsPresets = new ContextMenuStrip(components);
            mnuPsEmpty = new ToolStripMenuItem();
            mnuPs01 = new ToolStripMenuItem();
            mnuPs02 = new ToolStripMenuItem();
            mnuPs03 = new ToolStripMenuItem();
            mnuPs04 = new ToolStripMenuItem();
            mnuPs05 = new ToolStripMenuItem();
            mnuPs06 = new ToolStripMenuItem();
            mnuPs07 = new ToolStripMenuItem();
            mnuPs08 = new ToolStripMenuItem();
            mnuPs09 = new ToolStripMenuItem();
            mnuPs10 = new ToolStripMenuItem();
            mnuPs11 = new ToolStripMenuItem();
            mnuPs12 = new ToolStripMenuItem();
            mnuPs13 = new ToolStripMenuItem();
            mnuPs14 = new ToolStripMenuItem();
            mnuPs15 = new ToolStripMenuItem();
            mnuPs16 = new ToolStripMenuItem();
            mnuPs17 = new ToolStripMenuItem();
            mnuPs18 = new ToolStripMenuItem();
            mnuPs19 = new ToolStripMenuItem();
            mnuPs20 = new ToolStripMenuItem();
            mnuPs21 = new ToolStripMenuItem();
            mnuPs22 = new ToolStripMenuItem();
            mnuPs23 = new ToolStripMenuItem();
            mnuPs24 = new ToolStripMenuItem();
            mnuPs25 = new ToolStripMenuItem();
            mnuPsSep = new ToolStripSeparator();
            mnuPsManage = new ToolStripMenuItem();
            menuMain = new MenuStrip();
            mnuView = new ToolStripMenuItem();
            mnuViewDarkMode = new ToolStripMenuItem();
            mnuHelp = new ToolStripMenuItem();
            mnuHelpShortcuts = new ToolStripMenuItem();
            mnuHelpCheckUpdates = new ToolStripMenuItem();
            mnuHelpSep1 = new ToolStripSeparator();
            mnuHelpAbout = new ToolStripMenuItem();
            statusBar = new StatusStrip();
            sbFileCount = new ToolStripStatusLabel();
            sbTotalSize = new ToolStripStatusLabel();
            sbSpring = new ToolStripStatusLabel();
            sbScanStatus = new ToolStripStatusLabel();
            sbUpdateNotice = new ToolStripStatusLabel();
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
            btnCopyOutput = new FileContentToolkit.UI.SplitButton();
            cmsCopyAs = new ContextMenuStrip(components);
            mnuCopyPlain = new ToolStripMenuItem();
            mnuCopyMarkdown = new ToolStripMenuItem();
            mnuCopyXml = new ToolStripMenuItem();
            mnuCopyJson = new ToolStripMenuItem();
            lblOutput = new Label();
            pnlCompressionTools = new Panel();
            btnDecompressEnc = new Button();
            btnCompressEnc = new Button();
            btnDecompress = new Button();
            btnCompress = new Button();
            lblCompression = new Label();
            pnlSeparator = new Panel();
            toolTip1 = new ToolTip(components);
            pnlTop.SuspendLayout();
            pnlLeft.SuspendLayout();
            grpFiles.SuspendLayout();
            ctxFiles.SuspendLayout();
            pnlFileButtons.SuspendLayout();
            grpExtensions.SuspendLayout();
            cmsAddDropdown.SuspendLayout();
            cmsRecentFolders.SuspendLayout();
            cmsRecentSearches.SuspendLayout();
            cmsPresets.SuspendLayout();
            menuMain.SuspendLayout();
            statusBar.SuspendLayout();
            pnlBottom.SuspendLayout();
            pnlRight.SuspendLayout();
            pnlRecreateInfo.SuspendLayout();
            tblRecreateInfo.SuspendLayout();
            pnlOutput.SuspendLayout();
            pnlOutputHeader.SuspendLayout();
            cmsCopyAs.SuspendLayout();
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
            pnlTop.Location = new Point(0, 33);
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
            pnlLeft.Location = new Point(0, 198);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Padding = new Padding(20);
            pnlLeft.Size = new Size(497, 940);
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
            grpFiles.Location = new Point(20, 410);
            grpFiles.Name = "grpFiles";
            grpFiles.Padding = new Padding(10);
            grpFiles.Size = new Size(457, 510);
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
            lstFiles.Location = new Point(10, 142);
            lstFiles.Name = "lstFiles";
            lstFiles.SelectionMode = SelectionMode.MultiExtended;
            lstFiles.Size = new Size(437, 254);
            lstFiles.TabIndex = 0;
            lstFiles.DragDrop += LstFiles_DragDrop;
            lstFiles.DragEnter += LstFiles_DragEnter;
            lstFiles.DragOver += LstFiles_DragOver;
            lstFiles.KeyDown += LstFiles_KeyDown;
            lstFiles.MouseDown += LstFiles_MouseDown;
            lstFiles.MouseMove += LstFiles_MouseMove;
            // 
            // ctxFiles
            // 
            ctxFiles.ImageScalingSize = new Size(24, 24);
            ctxFiles.Items.AddRange(new ToolStripItem[] { miOpenFile, miRevealInExplorer, miOpenContainingFolder, miCopyPath, miFilesSep1, miSortByName, miSortByExtension });
            ctxFiles.Name = "ctxFiles";
            ctxFiles.Size = new Size(269, 202);
            // 
            // miOpenFile
            // 
            miOpenFile.Name = "miOpenFile";
            miOpenFile.Size = new Size(268, 32);
            miOpenFile.Text = "Open";
            miOpenFile.Click += MiOpenFile_Click;
            // 
            // miRevealInExplorer
            // 
            miRevealInExplorer.Name = "miRevealInExplorer";
            miRevealInExplorer.Size = new Size(268, 32);
            miRevealInExplorer.Text = "Reveal in Explorer";
            miRevealInExplorer.Click += MiRevealInExplorer_Click;
            // 
            // miOpenContainingFolder
            // 
            miOpenContainingFolder.Name = "miOpenContainingFolder";
            miOpenContainingFolder.Size = new Size(268, 32);
            miOpenContainingFolder.Text = "Open containing folder";
            miOpenContainingFolder.Click += MiOpenContainingFolder_Click;
            // 
            // miCopyPath
            // 
            miCopyPath.Name = "miCopyPath";
            miCopyPath.Size = new Size(268, 32);
            miCopyPath.Text = "Copy path";
            miCopyPath.Click += MiCopyPath_Click;
            // 
            // miFilesSep1
            // 
            miFilesSep1.Name = "miFilesSep1";
            miFilesSep1.Size = new Size(265, 6);
            // 
            // miSortByName
            // 
            miSortByName.Name = "miSortByName";
            miSortByName.Size = new Size(268, 32);
            miSortByName.Text = "Sort by Name";
            miSortByName.Click += MiSortByName_Click;
            // 
            // miSortByExtension
            // 
            miSortByExtension.Name = "miSortByExtension";
            miSortByExtension.Size = new Size(268, 32);
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
            pnlFileButtons.Location = new Point(10, 452);
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
            btnAddMultipleFiles.Location = new Point(119, 5);
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
            btnRemoveFile.Location = new Point(229, 5);
            btnRemoveFile.Name = "btnRemoveFile";
            btnRemoveFile.Size = new Size(94, 38);
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
            grpExtensions.Size = new Size(457, 390);
            grpExtensions.TabIndex = 0;
            grpExtensions.TabStop = false;
            grpExtensions.Text = "File Extensions";
            // 
            // txtIgnorePatterns
            // 
            txtIgnorePatterns.BorderStyle = BorderStyle.FixedSingle;
            txtIgnorePatterns.Location = new Point(10, 344);
            txtIgnorePatterns.Name = "txtIgnorePatterns";
            txtIgnorePatterns.Size = new Size(430, 34);
            txtIgnorePatterns.TabIndex = 7;
            txtIgnorePatterns.TextChanged += TxtIgnorePatterns_TextChanged;
            // 
            // lblIgnorePatterns
            // 
            lblIgnorePatterns.AutoSize = true;
            lblIgnorePatterns.Font = new Font("Segoe UI", 9F);
            lblIgnorePatterns.Location = new Point(10, 316);
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
            cmsAddDropdown.Items.AddRange(new ToolStripItem[] { mnuAddLangPresets, mnuAddSep1, miShowExtensionSummary });
            cmsAddDropdown.Name = "cmsAddDropdown";
            cmsAddDropdown.Size = new Size(301, 74);
            // 
            // mnuAddLangPresets
            // 
            mnuAddLangPresets.DropDownItems.AddRange(new ToolStripItem[] { mnuLangCs, mnuLangCpp, mnuLangWeb, mnuLangTs, mnuLangNode, mnuLangPy, mnuLangJava, mnuLangKotlin, mnuLangGo, mnuLangRust, mnuLangRuby, mnuLangPhp, mnuLangSwift, mnuLangShell, mnuLangDocs, mnuLangConfig });
            mnuAddLangPresets.Name = "mnuAddLangPresets";
            mnuAddLangPresets.Size = new Size(300, 32);
            mnuAddLangPresets.Text = "Add language preset";
            // 
            // mnuLangCs
            // 
            mnuLangCs.Name = "mnuLangCs";
            mnuLangCs.Size = new Size(272, 34);
            mnuLangCs.Text = "C# project";
            mnuLangCs.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangCpp
            // 
            mnuLangCpp.Name = "mnuLangCpp";
            mnuLangCpp.Size = new Size(272, 34);
            mnuLangCpp.Text = "C / C++";
            mnuLangCpp.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangWeb
            // 
            mnuLangWeb.Name = "mnuLangWeb";
            mnuLangWeb.Size = new Size(272, 34);
            mnuLangWeb.Text = "Web (HTML/CSS/JS)";
            mnuLangWeb.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangTs
            // 
            mnuLangTs.Name = "mnuLangTs";
            mnuLangTs.Size = new Size(272, 34);
            mnuLangTs.Text = "TypeScript / React";
            mnuLangTs.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangNode
            // 
            mnuLangNode.Name = "mnuLangNode";
            mnuLangNode.Size = new Size(272, 34);
            mnuLangNode.Text = "Node.js";
            mnuLangNode.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangPy
            // 
            mnuLangPy.Name = "mnuLangPy";
            mnuLangPy.Size = new Size(272, 34);
            mnuLangPy.Text = "Python";
            mnuLangPy.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangJava
            // 
            mnuLangJava.Name = "mnuLangJava";
            mnuLangJava.Size = new Size(272, 34);
            mnuLangJava.Text = "Java";
            mnuLangJava.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangKotlin
            // 
            mnuLangKotlin.Name = "mnuLangKotlin";
            mnuLangKotlin.Size = new Size(272, 34);
            mnuLangKotlin.Text = "Kotlin";
            mnuLangKotlin.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangGo
            // 
            mnuLangGo.Name = "mnuLangGo";
            mnuLangGo.Size = new Size(272, 34);
            mnuLangGo.Text = "Go";
            mnuLangGo.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangRust
            // 
            mnuLangRust.Name = "mnuLangRust";
            mnuLangRust.Size = new Size(272, 34);
            mnuLangRust.Text = "Rust";
            mnuLangRust.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangRuby
            // 
            mnuLangRuby.Name = "mnuLangRuby";
            mnuLangRuby.Size = new Size(272, 34);
            mnuLangRuby.Text = "Ruby";
            mnuLangRuby.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangPhp
            // 
            mnuLangPhp.Name = "mnuLangPhp";
            mnuLangPhp.Size = new Size(272, 34);
            mnuLangPhp.Text = "PHP";
            mnuLangPhp.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangSwift
            // 
            mnuLangSwift.Name = "mnuLangSwift";
            mnuLangSwift.Size = new Size(272, 34);
            mnuLangSwift.Text = "Swift";
            mnuLangSwift.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangShell
            // 
            mnuLangShell.Name = "mnuLangShell";
            mnuLangShell.Size = new Size(272, 34);
            mnuLangShell.Text = "Shell / Scripts";
            mnuLangShell.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangDocs
            // 
            mnuLangDocs.Name = "mnuLangDocs";
            mnuLangDocs.Size = new Size(272, 34);
            mnuLangDocs.Text = "Docs / Markup";
            mnuLangDocs.Click += MnuLanguagePreset_Click;
            // 
            // mnuLangConfig
            // 
            mnuLangConfig.Name = "mnuLangConfig";
            mnuLangConfig.Size = new Size(272, 34);
            mnuLangConfig.Text = "Config files";
            mnuLangConfig.Click += MnuLanguagePreset_Click;
            // 
            // mnuAddSep1
            // 
            mnuAddSep1.Name = "mnuAddSep1";
            mnuAddSep1.Size = new Size(297, 6);
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
            lstExtensions.ContextMenuStrip = cmsAddDropdown;
            lstExtensions.FormattingEnabled = true;
            lstExtensions.ItemHeight = 28;
            lstExtensions.Location = new Point(10, 105);
            lstExtensions.Name = "lstExtensions";
            lstExtensions.Size = new Size(249, 172);
            lstExtensions.TabIndex = 3;
            lstExtensions.KeyDown += LstExtensions_KeyDown;
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
            chkIncludeSubfolders.Location = new Point(10, 284);
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
            // cmsRecentFolders
            // 
            cmsRecentFolders.ImageScalingSize = new Size(20, 20);
            cmsRecentFolders.Items.AddRange(new ToolStripItem[] { mnuRfEmpty, mnuRf01, mnuRf02, mnuRf03, mnuRf04, mnuRf05, mnuRf06, mnuRf07, mnuRf08, mnuRf09, mnuRf10, mnuRf11, mnuRf12, mnuRf13, mnuRf14, mnuRf15, mnuRfSep, mnuRfClear });
            cmsRecentFolders.Name = "cmsRecentFolders";
            cmsRecentFolders.Size = new Size(183, 554);
            // 
            // mnuRfEmpty
            // 
            mnuRfEmpty.Enabled = false;
            mnuRfEmpty.Name = "mnuRfEmpty";
            mnuRfEmpty.Size = new Size(182, 32);
            mnuRfEmpty.Text = "(empty)";
            // 
            // mnuRf01
            // 
            mnuRf01.Name = "mnuRf01";
            mnuRf01.Size = new Size(182, 32);
            // 
            // mnuRf02
            // 
            mnuRf02.Name = "mnuRf02";
            mnuRf02.Size = new Size(182, 32);
            // 
            // mnuRf03
            // 
            mnuRf03.Name = "mnuRf03";
            mnuRf03.Size = new Size(182, 32);
            // 
            // mnuRf04
            // 
            mnuRf04.Name = "mnuRf04";
            mnuRf04.Size = new Size(182, 32);
            // 
            // mnuRf05
            // 
            mnuRf05.Name = "mnuRf05";
            mnuRf05.Size = new Size(182, 32);
            // 
            // mnuRf06
            // 
            mnuRf06.Name = "mnuRf06";
            mnuRf06.Size = new Size(182, 32);
            // 
            // mnuRf07
            // 
            mnuRf07.Name = "mnuRf07";
            mnuRf07.Size = new Size(182, 32);
            // 
            // mnuRf08
            // 
            mnuRf08.Name = "mnuRf08";
            mnuRf08.Size = new Size(182, 32);
            // 
            // mnuRf09
            // 
            mnuRf09.Name = "mnuRf09";
            mnuRf09.Size = new Size(182, 32);
            // 
            // mnuRf10
            // 
            mnuRf10.Name = "mnuRf10";
            mnuRf10.Size = new Size(182, 32);
            // 
            // mnuRf11
            // 
            mnuRf11.Name = "mnuRf11";
            mnuRf11.Size = new Size(182, 32);
            // 
            // mnuRf12
            // 
            mnuRf12.Name = "mnuRf12";
            mnuRf12.Size = new Size(182, 32);
            // 
            // mnuRf13
            // 
            mnuRf13.Name = "mnuRf13";
            mnuRf13.Size = new Size(182, 32);
            // 
            // mnuRf14
            // 
            mnuRf14.Name = "mnuRf14";
            mnuRf14.Size = new Size(182, 32);
            // 
            // mnuRf15
            // 
            mnuRf15.Name = "mnuRf15";
            mnuRf15.Size = new Size(182, 32);
            // 
            // mnuRfSep
            // 
            mnuRfSep.Name = "mnuRfSep";
            mnuRfSep.Size = new Size(179, 6);
            // 
            // mnuRfClear
            // 
            mnuRfClear.Name = "mnuRfClear";
            mnuRfClear.Size = new Size(182, 32);
            mnuRfClear.Text = "Clear history";
            mnuRfClear.Click += MnuRfClear_Click;
            // 
            // cmsRecentSearches
            // 
            cmsRecentSearches.ImageScalingSize = new Size(20, 20);
            cmsRecentSearches.Items.AddRange(new ToolStripItem[] { mnuRsEmpty, mnuRs01, mnuRs02, mnuRs03, mnuRs04, mnuRs05, mnuRs06, mnuRs07, mnuRs08, mnuRs09, mnuRs10, mnuRs11, mnuRs12, mnuRs13, mnuRs14, mnuRs15, mnuRsSep, mnuRsClear });
            cmsRecentSearches.Name = "cmsRecentSearches";
            cmsRecentSearches.Size = new Size(183, 554);
            // 
            // mnuRsEmpty
            // 
            mnuRsEmpty.Enabled = false;
            mnuRsEmpty.Name = "mnuRsEmpty";
            mnuRsEmpty.Size = new Size(182, 32);
            mnuRsEmpty.Text = "(empty)";
            // 
            // mnuRs01
            // 
            mnuRs01.Name = "mnuRs01";
            mnuRs01.Size = new Size(182, 32);
            // 
            // mnuRs02
            // 
            mnuRs02.Name = "mnuRs02";
            mnuRs02.Size = new Size(182, 32);
            // 
            // mnuRs03
            // 
            mnuRs03.Name = "mnuRs03";
            mnuRs03.Size = new Size(182, 32);
            // 
            // mnuRs04
            // 
            mnuRs04.Name = "mnuRs04";
            mnuRs04.Size = new Size(182, 32);
            // 
            // mnuRs05
            // 
            mnuRs05.Name = "mnuRs05";
            mnuRs05.Size = new Size(182, 32);
            // 
            // mnuRs06
            // 
            mnuRs06.Name = "mnuRs06";
            mnuRs06.Size = new Size(182, 32);
            // 
            // mnuRs07
            // 
            mnuRs07.Name = "mnuRs07";
            mnuRs07.Size = new Size(182, 32);
            // 
            // mnuRs08
            // 
            mnuRs08.Name = "mnuRs08";
            mnuRs08.Size = new Size(182, 32);
            // 
            // mnuRs09
            // 
            mnuRs09.Name = "mnuRs09";
            mnuRs09.Size = new Size(182, 32);
            // 
            // mnuRs10
            // 
            mnuRs10.Name = "mnuRs10";
            mnuRs10.Size = new Size(182, 32);
            // 
            // mnuRs11
            // 
            mnuRs11.Name = "mnuRs11";
            mnuRs11.Size = new Size(182, 32);
            // 
            // mnuRs12
            // 
            mnuRs12.Name = "mnuRs12";
            mnuRs12.Size = new Size(182, 32);
            // 
            // mnuRs13
            // 
            mnuRs13.Name = "mnuRs13";
            mnuRs13.Size = new Size(182, 32);
            // 
            // mnuRs14
            // 
            mnuRs14.Name = "mnuRs14";
            mnuRs14.Size = new Size(182, 32);
            // 
            // mnuRs15
            // 
            mnuRs15.Name = "mnuRs15";
            mnuRs15.Size = new Size(182, 32);
            // 
            // mnuRsSep
            // 
            mnuRsSep.Name = "mnuRsSep";
            mnuRsSep.Size = new Size(179, 6);
            // 
            // mnuRsClear
            // 
            mnuRsClear.Name = "mnuRsClear";
            mnuRsClear.Size = new Size(182, 32);
            mnuRsClear.Text = "Clear history";
            mnuRsClear.Click += MnuRsClear_Click;
            // 
            // cmsPresets
            // 
            cmsPresets.ImageScalingSize = new Size(20, 20);
            cmsPresets.Items.AddRange(new ToolStripItem[] { mnuPsEmpty, mnuPs01, mnuPs02, mnuPs03, mnuPs04, mnuPs05, mnuPs06, mnuPs07, mnuPs08, mnuPs09, mnuPs10, mnuPs11, mnuPs12, mnuPs13, mnuPs14, mnuPs15, mnuPs16, mnuPs17, mnuPs18, mnuPs19, mnuPs20, mnuPs21, mnuPs22, mnuPs23, mnuPs24, mnuPs25, mnuPsSep, mnuPsManage });
            cmsPresets.Name = "cmsPresets";
            cmsPresets.Size = new Size(224, 874);
            // 
            // mnuPsEmpty
            // 
            mnuPsEmpty.Enabled = false;
            mnuPsEmpty.Name = "mnuPsEmpty";
            mnuPsEmpty.Size = new Size(223, 32);
            mnuPsEmpty.Text = "(no presets)";
            // 
            // mnuPs01
            // 
            mnuPs01.Name = "mnuPs01";
            mnuPs01.Size = new Size(223, 32);
            // 
            // mnuPs02
            // 
            mnuPs02.Name = "mnuPs02";
            mnuPs02.Size = new Size(223, 32);
            // 
            // mnuPs03
            // 
            mnuPs03.Name = "mnuPs03";
            mnuPs03.Size = new Size(223, 32);
            // 
            // mnuPs04
            // 
            mnuPs04.Name = "mnuPs04";
            mnuPs04.Size = new Size(223, 32);
            // 
            // mnuPs05
            // 
            mnuPs05.Name = "mnuPs05";
            mnuPs05.Size = new Size(223, 32);
            // 
            // mnuPs06
            // 
            mnuPs06.Name = "mnuPs06";
            mnuPs06.Size = new Size(223, 32);
            // 
            // mnuPs07
            // 
            mnuPs07.Name = "mnuPs07";
            mnuPs07.Size = new Size(223, 32);
            // 
            // mnuPs08
            // 
            mnuPs08.Name = "mnuPs08";
            mnuPs08.Size = new Size(223, 32);
            // 
            // mnuPs09
            // 
            mnuPs09.Name = "mnuPs09";
            mnuPs09.Size = new Size(223, 32);
            // 
            // mnuPs10
            // 
            mnuPs10.Name = "mnuPs10";
            mnuPs10.Size = new Size(223, 32);
            // 
            // mnuPs11
            // 
            mnuPs11.Name = "mnuPs11";
            mnuPs11.Size = new Size(223, 32);
            // 
            // mnuPs12
            // 
            mnuPs12.Name = "mnuPs12";
            mnuPs12.Size = new Size(223, 32);
            // 
            // mnuPs13
            // 
            mnuPs13.Name = "mnuPs13";
            mnuPs13.Size = new Size(223, 32);
            // 
            // mnuPs14
            // 
            mnuPs14.Name = "mnuPs14";
            mnuPs14.Size = new Size(223, 32);
            // 
            // mnuPs15
            // 
            mnuPs15.Name = "mnuPs15";
            mnuPs15.Size = new Size(223, 32);
            // 
            // mnuPs16
            // 
            mnuPs16.Name = "mnuPs16";
            mnuPs16.Size = new Size(223, 32);
            // 
            // mnuPs17
            // 
            mnuPs17.Name = "mnuPs17";
            mnuPs17.Size = new Size(223, 32);
            // 
            // mnuPs18
            // 
            mnuPs18.Name = "mnuPs18";
            mnuPs18.Size = new Size(223, 32);
            // 
            // mnuPs19
            // 
            mnuPs19.Name = "mnuPs19";
            mnuPs19.Size = new Size(223, 32);
            // 
            // mnuPs20
            // 
            mnuPs20.Name = "mnuPs20";
            mnuPs20.Size = new Size(223, 32);
            // 
            // mnuPs21
            // 
            mnuPs21.Name = "mnuPs21";
            mnuPs21.Size = new Size(223, 32);
            // 
            // mnuPs22
            // 
            mnuPs22.Name = "mnuPs22";
            mnuPs22.Size = new Size(223, 32);
            // 
            // mnuPs23
            // 
            mnuPs23.Name = "mnuPs23";
            mnuPs23.Size = new Size(223, 32);
            // 
            // mnuPs24
            // 
            mnuPs24.Name = "mnuPs24";
            mnuPs24.Size = new Size(223, 32);
            // 
            // mnuPs25
            // 
            mnuPs25.Name = "mnuPs25";
            mnuPs25.Size = new Size(223, 32);
            // 
            // mnuPsSep
            // 
            mnuPsSep.Name = "mnuPsSep";
            mnuPsSep.Size = new Size(220, 6);
            // 
            // mnuPsManage
            // 
            mnuPsManage.Name = "mnuPsManage";
            mnuPsManage.Size = new Size(223, 32);
            mnuPsManage.Text = "Manage presets…";
            mnuPsManage.Click += MnuPsManage_Click;
            // 
            // menuMain
            // 
            menuMain.BackColor = Color.FromArgb(245, 247, 250);
            menuMain.Font = new Font("Segoe UI", 9.5F);
            menuMain.ImageScalingSize = new Size(20, 20);
            menuMain.Items.AddRange(new ToolStripItem[] { mnuView, mnuHelp });
            menuMain.Location = new Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Size = new Size(1640, 33);
            menuMain.TabIndex = 11;
            // 
            // mnuView
            // 
            mnuView.DropDownItems.AddRange(new ToolStripItem[] { mnuViewDarkMode });
            mnuView.Name = "mnuView";
            mnuView.Size = new Size(69, 29);
            mnuView.Text = "&View";
            // 
            // mnuViewDarkMode
            // 
            mnuViewDarkMode.CheckOnClick = true;
            mnuViewDarkMode.Name = "mnuViewDarkMode";
            mnuViewDarkMode.Size = new Size(206, 34);
            mnuViewDarkMode.Text = "&Dark mode";
            mnuViewDarkMode.CheckedChanged += MnuViewDarkMode_CheckedChanged;
            // 
            // mnuHelp
            // 
            mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { mnuHelpShortcuts, mnuHelpCheckUpdates, mnuHelpSep1, mnuHelpAbout });
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new Size(67, 29);
            mnuHelp.Text = "&Help";
            // 
            // mnuHelpShortcuts
            // 
            mnuHelpShortcuts.Name = "mnuHelpShortcuts";
            mnuHelpShortcuts.ShortcutKeys = Keys.F1;
            mnuHelpShortcuts.Size = new Size(323, 34);
            mnuHelpShortcuts.Text = "&Keyboard Shortcuts…";
            mnuHelpShortcuts.Click += MnuHelpShortcuts_Click;
            // 
            // mnuHelpCheckUpdates
            // 
            mnuHelpCheckUpdates.Name = "mnuHelpCheckUpdates";
            mnuHelpCheckUpdates.Size = new Size(323, 34);
            mnuHelpCheckUpdates.Text = "Check for &updates…";
            mnuHelpCheckUpdates.Click += MnuHelpCheckUpdates_Click;
            // 
            // mnuHelpSep1
            // 
            mnuHelpSep1.Name = "mnuHelpSep1";
            mnuHelpSep1.Size = new Size(320, 6);
            // 
            // mnuHelpAbout
            // 
            mnuHelpAbout.Name = "mnuHelpAbout";
            mnuHelpAbout.Size = new Size(323, 34);
            mnuHelpAbout.Text = "&About…";
            mnuHelpAbout.Click += MnuHelpAbout_Click;
            // 
            // statusBar
            // 
            statusBar.BackColor = Color.FromArgb(233, 236, 239);
            statusBar.Font = new Font("Segoe UI", 9F);
            statusBar.ImageScalingSize = new Size(20, 20);
            statusBar.Items.AddRange(new ToolStripItem[] { sbFileCount, sbTotalSize, sbSpring, sbScanStatus, sbUpdateNotice });
            statusBar.Location = new Point(0, 1189);
            statusBar.Name = "statusBar";
            statusBar.Padding = new Padding(8, 2, 8, 2);
            statusBar.Size = new Size(1640, 36);
            statusBar.SizingGrip = false;
            statusBar.TabIndex = 12;
            // 
            // sbFileCount
            // 
            sbFileCount.ForeColor = Color.FromArgb(33, 37, 41);
            sbFileCount.Name = "sbFileCount";
            sbFileCount.Size = new Size(65, 25);
            sbFileCount.Text = "Files: 0";
            // 
            // sbTotalSize
            // 
            sbTotalSize.ForeColor = Color.FromArgb(33, 37, 41);
            sbTotalSize.Margin = new Padding(20, 3, 0, 2);
            sbTotalSize.Name = "sbTotalSize";
            sbTotalSize.Size = new Size(77, 27);
            sbTotalSize.Text = "Size: 0 B";
            // 
            // sbSpring
            // 
            sbSpring.Name = "sbSpring";
            sbSpring.Size = new Size(1462, 25);
            sbSpring.Spring = true;
            // 
            // sbScanStatus
            // 
            sbScanStatus.ForeColor = Color.FromArgb(108, 117, 125);
            sbScanStatus.Name = "sbScanStatus";
            sbScanStatus.Size = new Size(0, 25);
            // 
            // sbUpdateNotice
            // 
            sbUpdateNotice.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            sbUpdateNotice.ForeColor = Color.FromArgb(13, 110, 253);
            sbUpdateNotice.IsLink = true;
            sbUpdateNotice.Margin = new Padding(20, 3, 0, 2);
            sbUpdateNotice.Name = "sbUpdateNotice";
            sbUpdateNotice.Size = new Size(0, 27);
            sbUpdateNotice.Visible = false;
            sbUpdateNotice.Click += SbUpdateNotice_Click;
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.FromArgb(245, 247, 250);
            pnlBottom.Controls.Add(progressBar);
            pnlBottom.Controls.Add(btnGenerate);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 1138);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(1640, 51);
            pnlBottom.TabIndex = 3;
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(955, 18);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(655, 16);
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
            btnGenerate.Location = new Point(695, 0);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(240, 49);
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
            pnlRight.Location = new Point(497, 198);
            pnlRight.Name = "pnlRight";
            pnlRight.Padding = new Padding(20);
            pnlRight.Size = new Size(1143, 940);
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
            rtbOutput.Size = new Size(1103, 680);
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
            btnExportOutput.Location = new Point(778, 0);
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
            btnEditOutput.Location = new Point(883, 0);
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
            btnCopyOutput.DropDownMenu = cmsCopyAs;
            btnCopyOutput.DropDownWidth = 22;
            btnCopyOutput.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            btnCopyOutput.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            btnCopyOutput.FlatStyle = FlatStyle.Flat;
            btnCopyOutput.Font = new Font("Segoe UI", 9F);
            btnCopyOutput.ForeColor = Color.FromArgb(73, 80, 87);
            btnCopyOutput.Location = new Point(983, 0);
            btnCopyOutput.Name = "btnCopyOutput";
            btnCopyOutput.ShowSplit = true;
            btnCopyOutput.Size = new Size(120, 50);
            btnCopyOutput.TabIndex = 2;
            btnCopyOutput.Text = "📋 Copy";
            toolTip1.SetToolTip(btnCopyOutput, "Copy output to clipboard (click ▾ for formats)");
            btnCopyOutput.UseVisualStyleBackColor = false;
            btnCopyOutput.Click += BtnCopyOutput_Click;
            // 
            // cmsCopyAs
            // 
            cmsCopyAs.ImageScalingSize = new Size(20, 20);
            cmsCopyAs.Items.AddRange(new ToolStripItem[] { mnuCopyPlain, mnuCopyMarkdown, mnuCopyXml, mnuCopyJson });
            cmsCopyAs.Name = "cmsCopyAs";
            cmsCopyAs.Size = new Size(282, 132);
            // 
            // mnuCopyPlain
            // 
            mnuCopyPlain.Name = "mnuCopyPlain";
            mnuCopyPlain.Size = new Size(281, 32);
            mnuCopyPlain.Text = "Plain text";
            mnuCopyPlain.Click += MnuCopyPlain_Click;
            // 
            // mnuCopyMarkdown
            // 
            mnuCopyMarkdown.Name = "mnuCopyMarkdown";
            mnuCopyMarkdown.Size = new Size(281, 32);
            mnuCopyMarkdown.Text = "Markdown (fenced code)";
            mnuCopyMarkdown.Click += MnuCopyMarkdown_Click;
            // 
            // mnuCopyXml
            // 
            mnuCopyXml.Name = "mnuCopyXml";
            mnuCopyXml.Size = new Size(281, 32);
            mnuCopyXml.Text = "XML (Claude-friendly)";
            mnuCopyXml.Click += MnuCopyXml_Click;
            // 
            // mnuCopyJson
            // 
            mnuCopyJson.Name = "mnuCopyJson";
            mnuCopyJson.Size = new Size(281, 32);
            mnuCopyJson.Text = "JSON array";
            mnuCopyJson.Click += MnuCopyJson_Click;
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
            Controls.Add(statusBar);
            MainMenuStrip = menuMain;
            MinimumSize = new Size(1315, 1087);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "File Content Toolkit";
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
            cmsRecentFolders.ResumeLayout(false);
            cmsRecentSearches.ResumeLayout(false);
            cmsPresets.ResumeLayout(false);
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            statusBar.ResumeLayout(false);
            statusBar.PerformLayout();
            pnlBottom.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            pnlRecreateInfo.ResumeLayout(false);
            tblRecreateInfo.ResumeLayout(false);
            pnlOutput.ResumeLayout(false);
            pnlOutput.PerformLayout();
            pnlOutputHeader.ResumeLayout(false);
            pnlOutputHeader.PerformLayout();
            cmsCopyAs.ResumeLayout(false);
            pnlCompressionTools.ResumeLayout(false);
            pnlCompressionTools.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}