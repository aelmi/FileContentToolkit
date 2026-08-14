using System;
using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle
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
        private System.Windows.Forms.Button btnAddFolder;
        /// <summary>
        /// Replaces the fixed-width left panel. The file list and the output pane were previously
        /// unresizable relative to each other.
        /// </summary>
        private System.Windows.Forms.SplitContainer splitMain;
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
        private CodeShuttle.UI.SplitButton btnAdd;
        private System.Windows.Forms.ListBox lstExtensions;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.CheckBox chkIncludeSubfolders;
        private System.Windows.Forms.Button btnRefreshExtensions;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private CodeShuttle.UI.SplitButton btnCopyOutput;
        private System.Windows.Forms.Button btnEditOutput;
        private System.Windows.Forms.ContextMenuStrip cmsCopyAs;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyPlain;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyMarkdown;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyXml;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyJson;
        private System.Windows.Forms.ToolStripSeparator mnuCopyAsPromptSep;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyAsPrompt;
        private CodeShuttle.UI.SplitButton btnProtect;
        private System.Windows.Forms.ContextMenuStrip cmsProtect;
        private System.Windows.Forms.ToolStripMenuItem mnuProtectEncrypt;
        private System.Windows.Forms.ToolStripMenuItem mnuProtectDecrypt;
        private System.Windows.Forms.ToolStripSeparator mnuProtectSep;
        private System.Windows.Forms.ToolStripMenuItem mnuProtectCompress;
        private System.Windows.Forms.ToolStripMenuItem mnuProtectDecompress;
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
        private System.Windows.Forms.Panel pnlSeparator;
        private System.Windows.Forms.TextBox txtIgnorePatterns;
        private System.Windows.Forms.Label lblIgnorePatterns;
        private System.Windows.Forms.Button btnEditRules;
        /// <summary>
        /// Replaces txtSearchFiles, btnSearchFiles, btnSearchRecents, chkCase, chkWord, chkRegex,
        /// lblSearchFiles and lblSearchMatches, which were eight separate fields at absolute
        /// coordinates with no accessible names between them.
        /// </summary>
        private CodeShuttle.Controls.SearchBox searchBox;
        private System.Windows.Forms.ComboBox cmbEncoding;
        private System.Windows.Forms.Label lblEncoding;
        private System.Windows.Forms.Label lblOutputStats;
        private System.Windows.Forms.Panel pnlBudget;
        private System.Windows.Forms.Label lblBudgetModel;
        private System.Windows.Forms.ComboBox cmbBudgetModel;
        private System.Windows.Forms.ProgressBar barBudget;
        private System.Windows.Forms.Label lblBudgetText;
        private System.Windows.Forms.Button btnBudgetBreakdown;
        private System.Windows.Forms.Label lblRecreateInfo;
        private System.Windows.Forms.Button btnApplyAiChanges;
        private System.Windows.Forms.Button btnPasteResponse;

        /// <summary>
        /// Dismisses the round-trip strip for the session. The strip was permanent, taking a slab
        /// of vertical space from the output pane whether or not there was anything to apply.
        /// </summary>
        private System.Windows.Forms.Button btnHideRecreateInfo;

        // --- new toolbar row inside pnlTop ---
        private System.Windows.Forms.Button btnTree;
        private System.Windows.Forms.Button btnRecentFolders;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnSavePreset;
        private System.Windows.Forms.Button btnLoadPreset;
        private System.Windows.Forms.CheckBox chkWatch;

        // --- search row enhancements inside grpFiles ---
        private System.Windows.Forms.Button btnFindReplace;

        // --- main menu strip ---
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuViewDarkMode;
        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsPasteResponse;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsPromptTemplates;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsExclusionRules;
        private System.Windows.Forms.ToolStripSeparator mnuToolsSep1;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsImportSettings;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsExportSettings;
        private System.Windows.Forms.ToolStripSeparator mnuToolsSep2;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsCompression;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsCompress;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsDecompress;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsCompressEnc;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsDecompressEnc;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpShortcuts;
        private System.Windows.Forms.ToolStripSeparator mnuHelpSep1;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpContents;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpCheckUpdates;

        // --- status bar ---
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel sbFileCount;
        private System.Windows.Forms.ToolStripStatusLabel sbTotalSize;
        private System.Windows.Forms.ToolStripStatusLabel sbSpring;
        private System.Windows.Forms.ToolStripStatusLabel sbScanStatus;
        private System.Windows.Forms.ToolStripStatusLabel sbUpdateNotice;

        /// <summary>
        /// Clickable "N files skipped" item. The skip data has existed since the scan robustness
        /// work; nothing surfaced it, so a bundle could quietly omit files the user believed were
        /// included.
        /// </summary>
        private System.Windows.Forms.ToolStripStatusLabel sbSkipped;

        /// <summary>
        /// Progress moves into the status strip so it is present for scan, generate and apply
        /// rather than floating in the bottom panel for scan alone.
        /// </summary>
        private System.Windows.Forms.ToolStripProgressBar sbProgress;

        /// <summary>
        /// The cancel affordance the product never had. Cancellation itself already worked — it
        /// was only ever triggered by one scan superseding another, never by the user.
        /// </summary>
        private System.Windows.Forms.ToolStripButton sbCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                DisposeOwnedResources();
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
            btnAddFolder = new Button();
            btnTree = new Button();
            btnRecentFolders = new Button();
            btnOptions = new Button();
            btnSavePreset = new Button();
            btnLoadPreset = new Button();
            chkWatch = new CheckBox();
            splitMain = new SplitContainer();
            pnlLeft = new Panel();
            grpFiles = new GroupBox();
            searchBox = new CodeShuttle.Controls.SearchBox();
            btnFindReplace = new Button();
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
            btnEditRules = new Button();
            lblExtension = new Label();
            cmbExtension = new ComboBox();
            btnAdd = new CodeShuttle.UI.SplitButton();
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
            mnuTools = new ToolStripMenuItem();
            mnuToolsPasteResponse = new ToolStripMenuItem();
            mnuToolsPromptTemplates = new ToolStripMenuItem();
            mnuToolsExclusionRules = new ToolStripMenuItem();
            mnuToolsSep1 = new ToolStripSeparator();
            mnuToolsImportSettings = new ToolStripMenuItem();
            mnuToolsExportSettings = new ToolStripMenuItem();
            mnuToolsSep2 = new ToolStripSeparator();
            mnuToolsCompression = new ToolStripMenuItem();
            mnuToolsCompress = new ToolStripMenuItem();
            mnuToolsDecompress = new ToolStripMenuItem();
            mnuToolsCompressEnc = new ToolStripMenuItem();
            mnuToolsDecompressEnc = new ToolStripMenuItem();
            mnuHelpContents = new ToolStripMenuItem();
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
            sbSkipped = new ToolStripStatusLabel();
            sbProgress = new ToolStripProgressBar();
            sbCancel = new ToolStripButton();
            pnlBottom = new Panel();
            btnGenerate = new Button();
            pnlRight = new Panel();
            rtbOutput = new RichTextBox();
            pnlRecreateInfo = new Panel();
            tblRecreateInfo = new TableLayoutPanel();
            lblRecreateInfo = new Label();
            btnApplyAiChanges = new Button();
            btnPasteResponse = new Button();
            btnHideRecreateInfo = new Button();
            pnlOutput = new Panel();
            lblOutputStats = new Label();
            pnlBudget = new Panel();
            lblBudgetModel = new Label();
            cmbBudgetModel = new ComboBox();
            barBudget = new ProgressBar();
            lblBudgetText = new Label();
            btnBudgetBreakdown = new Button();
            pnlOutputHeader = new Panel();
            btnExportOutput = new Button();
            btnEditOutput = new Button();
            btnCopyOutput = new CodeShuttle.UI.SplitButton();
            cmsCopyAs = new ContextMenuStrip(components);
            mnuCopyPlain = new ToolStripMenuItem();
            mnuCopyMarkdown = new ToolStripMenuItem();
            mnuCopyXml = new ToolStripMenuItem();
            mnuCopyJson = new ToolStripMenuItem();
            mnuCopyAsPromptSep = new ToolStripSeparator();
            mnuCopyAsPrompt = new ToolStripMenuItem();
            btnProtect = new CodeShuttle.UI.SplitButton();
            cmsProtect = new ContextMenuStrip(components);
            mnuProtectEncrypt = new ToolStripMenuItem();
            mnuProtectDecrypt = new ToolStripMenuItem();
            mnuProtectSep = new ToolStripSeparator();
            mnuProtectCompress = new ToolStripMenuItem();
            mnuProtectDecompress = new ToolStripMenuItem();
            lblOutput = new Label();
            pnlSeparator = new Panel();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
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
            pnlBudget.SuspendLayout();
            pnlOutputHeader.SuspendLayout();
            cmsCopyAs.SuspendLayout();
            cmsProtect.SuspendLayout();
            SuspendLayout();
            //
            // pnlTop
            //
            pnlTop.Controls.Add(cmbEncoding);
            pnlTop.Controls.Add(lblEncoding);
            pnlTop.Controls.Add(lblPath);
            pnlTop.Controls.Add(txtFolderPath);
            pnlTop.Controls.Add(btnBrowse);
            pnlTop.Controls.Add(btnAddFolder);
            pnlTop.Controls.Add(btnTree);
            pnlTop.Controls.Add(btnRecentFolders);
            pnlTop.Controls.Add(btnOptions);
            pnlTop.Controls.Add(btnSavePreset);
            pnlTop.Controls.Add(btnLoadPreset);
            pnlTop.Controls.Add(chkWatch);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 20);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(14, 12, 14, 6);
            pnlTop.Size = new Size(1148, 99);
            pnlTop.TabIndex = 0;
            //
            // cmbEncoding
            //
            cmbEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEncoding.FlatStyle = FlatStyle.Flat;
            cmbEncoding.Location = new Point(826, 35);
            cmbEncoding.Name = "cmbEncoding";
            cmbEncoding.Size = new Size(105, 21);
            cmbEncoding.TabIndex = 5;
            cmbEncoding.AccessibleName = "Output encoding";
            cmbEncoding.SelectedIndexChanged += CmbEncoding_SelectedIndexChanged;
            //
            // lblEncoding
            //
            lblEncoding.AutoSize = true;
            lblEncoding.Location = new Point(826, 12);
            lblEncoding.Name = "lblEncoding";
            lblEncoding.Size = new Size(69, 17);
            lblEncoding.TabIndex = 4;
            lblEncoding.Text = "Encoding:";
            //
            // lblPath
            //
            lblPath.AutoSize = true;
            lblPath.Location = new Point(14, 12);
            lblPath.Name = "lblPath";
            lblPath.Size = new Size(80, 17);
            lblPath.TabIndex = 0;
            lblPath.Text = "Folder Path:";
            //
            // txtFolderPath
            //
            txtFolderPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFolderPath.BorderStyle = BorderStyle.FixedSingle;
            txtFolderPath.Location = new Point(14, 35);
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.Size = new Size(791, 20);
            txtFolderPath.TabIndex = 1;
            txtFolderPath.AccessibleName = "Folder path";
            txtFolderPath.TextChanged += TxtFolderPath_TextChanged;
            //
            // btnBrowse
            //
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Cursor = Cursors.Hand;
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.Location = new Point(945, 34);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(47, 24);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "...";
            // Announced as "dot dot dot button" without this.
            btnBrowse.AccessibleName = "Browse for folder";
            btnBrowse.AccessibleDescription = "Choose the folder to scan.";
            toolTip1.SetToolTip(btnBrowse, "Browse for a folder");
            btnBrowse.UseVisualStyleBackColor = false;
            btnBrowse.Click += BtnBrowse_Click;
            //
            // btnAddFolder
            //
            btnAddFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddFolder.Cursor = Cursors.Hand;
            btnAddFolder.FlatAppearance.BorderSize = 0;
            btnAddFolder.FlatStyle = FlatStyle.Flat;
            btnAddFolder.Location = new Point(896, 66);
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Size = new Size(98, 23);
            btnAddFolder.TabIndex = 3;
            btnAddFolder.Text = "+ Add &Folder";
            toolTip1.SetToolTip(btnAddFolder, "Scan another folder for the configured extensions and append its files to the list");
            btnAddFolder.UseVisualStyleBackColor = false;
            btnAddFolder.Click += BtnAddFolder_Click;
            //
            // btnTree
            //
            btnTree.Cursor = Cursors.Hand;
            btnTree.FlatAppearance.BorderSize = 0;
            btnTree.FlatStyle = FlatStyle.Flat;
            btnTree.Location = new Point(14, 66);
            btnTree.Name = "btnTree";
            btnTree.Size = new Size(63, 23);
            btnTree.TabIndex = 6;
            btnTree.Text = "&Tree";
            toolTip1.SetToolTip(btnTree, "Pick files and folders from a tree view");
            btnTree.UseVisualStyleBackColor = false;
            btnTree.Click += BtnTree_Click;
            //
            // btnRecentFolders
            //
            btnRecentFolders.Cursor = Cursors.Hand;
            btnRecentFolders.FlatAppearance.BorderSize = 0;
            btnRecentFolders.FlatStyle = FlatStyle.Flat;
            btnRecentFolders.Location = new Point(81, 66);
            btnRecentFolders.Name = "btnRecentFolders";
            btnRecentFolders.Size = new Size(77, 23);
            btnRecentFolders.TabIndex = 7;
            btnRecentFolders.Text = "&Recent ▾";
            btnRecentFolders.AccessibleName = "Recent folders";
            btnRecentFolders.AccessibleRole = AccessibleRole.ButtonDropDown;
            toolTip1.SetToolTip(btnRecentFolders, "Recently used folders");
            btnRecentFolders.UseVisualStyleBackColor = false;
            btnRecentFolders.Click += BtnRecentFolders_Click;
            //
            // btnOptions
            //
            btnOptions.Cursor = Cursors.Hand;
            btnOptions.FlatAppearance.BorderSize = 0;
            btnOptions.FlatStyle = FlatStyle.Flat;
            btnOptions.Location = new Point(162, 66);
            btnOptions.Name = "btnOptions";
            btnOptions.Size = new Size(77, 23);
            btnOptions.TabIndex = 8;
            btnOptions.Text = "&Options";
            toolTip1.SetToolTip(btnOptions, "Configure filters, encoding, watcher");
            btnOptions.UseVisualStyleBackColor = false;
            btnOptions.Click += BtnOptions_Click;
            //
            // btnSavePreset
            //
            btnSavePreset.Cursor = Cursors.Hand;
            btnSavePreset.FlatAppearance.BorderSize = 0;
            btnSavePreset.FlatStyle = FlatStyle.Flat;
            btnSavePreset.Location = new Point(244, 66);
            btnSavePreset.Name = "btnSavePreset";
            btnSavePreset.Size = new Size(105, 23);
            btnSavePreset.TabIndex = 9;
            btnSavePreset.Text = "&Save preset";
            toolTip1.SetToolTip(btnSavePreset, "Save current folder + extensions as a preset");
            btnSavePreset.UseVisualStyleBackColor = false;
            btnSavePreset.Click += BtnSavePreset_Click;
            //
            // btnLoadPreset
            //
            btnLoadPreset.Cursor = Cursors.Hand;
            btnLoadPreset.FlatAppearance.BorderSize = 0;
            btnLoadPreset.FlatStyle = FlatStyle.Flat;
            btnLoadPreset.Location = new Point(353, 66);
            btnLoadPreset.Name = "btnLoadPreset";
            btnLoadPreset.Size = new Size(91, 23);
            btnLoadPreset.TabIndex = 10;
            btnLoadPreset.Text = "&Presets ▾";
            btnLoadPreset.AccessibleName = "Saved presets";
            btnLoadPreset.AccessibleRole = AccessibleRole.ButtonDropDown;
            toolTip1.SetToolTip(btnLoadPreset, "Load or manage saved presets");
            btnLoadPreset.UseVisualStyleBackColor = false;
            btnLoadPreset.Click += BtnLoadPreset_Click;
            //
            // chkWatch
            //
            chkWatch.AutoSize = true;
            chkWatch.Location = new Point(454, 70);
            chkWatch.Name = "chkWatch";
            chkWatch.Size = new Size(106, 17);
            chkWatch.TabIndex = 11;
            chkWatch.Text = "&Watch folder";
            toolTip1.SetToolTip(chkWatch, "Auto-refresh when files change on disk");
            chkWatch.UseVisualStyleBackColor = false;
            chkWatch.CheckedChanged += ChkWatch_CheckedChanged;
            //
            // splitMain
            //
            // The left pane was fixed width, so the file list and the output pane could never be
            // resized against each other. SplitterDistance is restored from settings at load.
            splitMain.Dock = DockStyle.Fill;
            splitMain.FixedPanel = FixedPanel.None;
            splitMain.Location = new Point(0, 119);
            splitMain.Name = "splitMain";
            splitMain.Orientation = Orientation.Vertical;
            splitMain.Panel1.Controls.Add(pnlLeft);
            splitMain.Panel1MinSize = 240;
            splitMain.Panel2.Controls.Add(pnlRight);
            splitMain.Panel2MinSize = 320;
            splitMain.Size = new Size(1148, 564);
            splitMain.SplitterDistance = 348;
            splitMain.SplitterWidth = 6;
            splitMain.TabIndex = 1;
            splitMain.AccessibleName = "Main splitter";
            splitMain.AccessibleDescription = "Adjusts the width of the file list against the output pane.";
            //
            // pnlLeft
            //
            pnlLeft.Controls.Add(grpFiles);
            pnlLeft.Controls.Add(grpExtensions);
            pnlLeft.Dock = DockStyle.Fill;
            pnlLeft.Location = new Point(0, 0);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Padding = new Padding(14, 12, 14, 12);
            pnlLeft.Size = new Size(348, 564);
            pnlLeft.TabIndex = 0;
            //
            // grpFiles
            //
            // Docked children are resolved from the end of the collection backwards, so this
            // order gives searchBox the top edge, pnlFileButtons the bottom, and lstFiles the
            // remainder. btnFindReplace has moved to pnlOutputHeader: it acts on the output pane,
            // not on the file list it used to sit in.
            grpFiles.Controls.Add(lstFiles);
            grpFiles.Controls.Add(pnlFileButtons);
            grpFiles.Controls.Add(searchBox);
            grpFiles.Dock = DockStyle.Fill;
            grpFiles.Location = new Point(14, 246);
            grpFiles.Name = "grpFiles";
            grpFiles.Padding = new Padding(7, 6, 7, 6);
            grpFiles.Size = new Size(320, 306);
            grpFiles.TabIndex = 1;
            grpFiles.TabStop = false;
            grpFiles.Text = "Selected Files";
            //
            // searchBox
            //
            // The composite owns its own internal layout, accessible names and access keys.
            searchBox.Dock = DockStyle.Top;
            searchBox.Name = "searchBox";
            searchBox.TabIndex = 2;
            searchBox.SearchRequested += BtnSearchFiles_Click;
            searchBox.RecentsRequested += BtnSearchRecents_Click;
            //
            // btnFindReplace
            //
            btnFindReplace.Cursor = Cursors.Hand;
            btnFindReplace.Dock = DockStyle.Right;
            btnFindReplace.FlatAppearance.BorderSize = 0;
            btnFindReplace.FlatStyle = FlatStyle.Flat;
            btnFindReplace.Location = new Point(440, 0);
            btnFindReplace.Name = "btnFindReplace";
            btnFindReplace.Size = new Size(105, 30);
            btnFindReplace.TabIndex = 1;
            btnFindReplace.Text = "Find / Rep&lace";
            btnFindReplace.AccessibleName = "Find and replace in output";
            toolTip1.SetToolTip(btnFindReplace, "Find & Replace in the output pane (Ctrl+F)");
            btnFindReplace.UseVisualStyleBackColor = false;
            btnFindReplace.Click += BtnFindReplace_Click;
            //
            // lstFiles
            //
            lstFiles.AllowDrop = true;
            lstFiles.ContextMenuStrip = ctxFiles;
            lstFiles.Dock = DockStyle.Fill;
            lstFiles.FormattingEnabled = true;
            lstFiles.IntegralHeight = false;
            lstFiles.Location = new Point(7, 85);
            lstFiles.Name = "lstFiles";
            lstFiles.SelectionMode = SelectionMode.MultiExtended;
            lstFiles.Size = new Size(306, 153);
            lstFiles.TabIndex = 0;
            lstFiles.AccessibleName = "Selected files";
            lstFiles.AccessibleDescription = "The files that will be included in the generated output.";
            lstFiles.DragDrop += LstFiles_DragDrop;
            lstFiles.DragEnter += LstFiles_DragEnter;
            lstFiles.DragOver += LstFiles_DragOver;
            lstFiles.KeyDown += LstFiles_KeyDown;
            lstFiles.MouseDown += LstFiles_MouseDown;
            lstFiles.MouseMove += LstFiles_MouseMove;
            //
            // ctxFiles
            //
            ctxFiles.ImageScalingSize = new Size(17, 14);
            ctxFiles.Items.AddRange(new ToolStripItem[] { miOpenFile, miRevealInExplorer, miOpenContainingFolder, miCopyPath, miFilesSep1, miSortByName, miSortByExtension });
            ctxFiles.Name = "ctxFiles";
            ctxFiles.Size = new Size(188, 121);
            //
            // miOpenFile
            //
            miOpenFile.Name = "miOpenFile";
            miOpenFile.Size = new Size(188, 19);
            miOpenFile.Text = "Open";
            miOpenFile.Click += MiOpenFile_Click;
            //
            // miRevealInExplorer
            //
            miRevealInExplorer.Name = "miRevealInExplorer";
            miRevealInExplorer.Size = new Size(188, 19);
            miRevealInExplorer.Text = "Reveal in Explorer";
            miRevealInExplorer.Click += MiRevealInExplorer_Click;
            //
            // miOpenContainingFolder
            //
            miOpenContainingFolder.Name = "miOpenContainingFolder";
            miOpenContainingFolder.Size = new Size(188, 19);
            miOpenContainingFolder.Text = "Open containing folder";
            miOpenContainingFolder.Click += MiOpenContainingFolder_Click;
            //
            // miCopyPath
            //
            miCopyPath.Name = "miCopyPath";
            miCopyPath.Size = new Size(188, 19);
            miCopyPath.Text = "Copy path";
            miCopyPath.Click += MiCopyPath_Click;
            //
            // miFilesSep1
            //
            miFilesSep1.Name = "miFilesSep1";
            miFilesSep1.Size = new Size(186, 4);
            //
            // miSortByName
            //
            miSortByName.Name = "miSortByName";
            miSortByName.Size = new Size(188, 19);
            miSortByName.Text = "Sort by Name";
            miSortByName.Click += MiSortByName_Click;
            //
            // miSortByExtension
            //
            miSortByExtension.Name = "miSortByExtension";
            miSortByExtension.Size = new Size(188, 19);
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
            pnlFileButtons.Location = new Point(7, 271);
            pnlFileButtons.Name = "pnlFileButtons";
            pnlFileButtons.Size = new Size(306, 29);
            pnlFileButtons.TabIndex = 1;
            //
            // lblFileCount
            //
            lblFileCount.AutoSize = true;
            lblFileCount.Location = new Point(4, 7);
            lblFileCount.Name = "lblFileCount";
            lblFileCount.Size = new Size(48, 17);
            lblFileCount.TabIndex = 0;
            lblFileCount.Text = "Files: 0";
            //
            // btnAddMultipleFiles
            //
            btnAddMultipleFiles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddMultipleFiles.FlatStyle = FlatStyle.Flat;
            btnAddMultipleFiles.Location = new Point(83, 3);
            btnAddMultipleFiles.Name = "btnAddMultipleFiles";
            btnAddMultipleFiles.Size = new Size(70, 23);
            btnAddMultipleFiles.TabIndex = 1;
            btnAddMultipleFiles.Text = "Add Files";
            btnAddMultipleFiles.UseVisualStyleBackColor = false;
            btnAddMultipleFiles.Click += BtnAddMultipleFiles_Click;
            //
            // btnRemoveFile
            //
            btnRemoveFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRemoveFile.FlatStyle = FlatStyle.Flat;
            btnRemoveFile.Location = new Point(160, 3);
            btnRemoveFile.Name = "btnRemoveFile";
            btnRemoveFile.Size = new Size(66, 23);
            btnRemoveFile.TabIndex = 2;
            btnRemoveFile.Text = "Remove";
            btnRemoveFile.UseVisualStyleBackColor = false;
            btnRemoveFile.Click += BtnRemoveFile_Click;
            //
            // btnMoveUp
            //
            btnMoveUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoveUp.FlatStyle = FlatStyle.Flat;
            btnMoveUp.Location = new Point(238, 3);
            btnMoveUp.Name = "btnMoveUp";
            btnMoveUp.Size = new Size(28, 23);
            btnMoveUp.TabIndex = 3;
            btnMoveUp.Text = "▲";
            btnMoveUp.AccessibleName = "Move file up";
            btnMoveUp.UseVisualStyleBackColor = false;
            btnMoveUp.Click += BtnMoveUp_Click;
            //
            // btnMoveDown
            //
            btnMoveDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoveDown.FlatStyle = FlatStyle.Flat;
            btnMoveDown.Location = new Point(270, 3);
            btnMoveDown.Name = "btnMoveDown";
            btnMoveDown.Size = new Size(28, 23);
            btnMoveDown.TabIndex = 4;
            btnMoveDown.Text = "▼";
            btnMoveDown.AccessibleName = "Move file down";
            btnMoveDown.UseVisualStyleBackColor = false;
            btnMoveDown.Click += BtnMoveDown_Click;
            //
            // grpExtensions
            //
            grpExtensions.Controls.Add(btnEditRules);
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
            grpExtensions.Location = new Point(14, 12);
            grpExtensions.Name = "grpExtensions";
            grpExtensions.Padding = new Padding(7, 6, 7, 6);
            grpExtensions.Size = new Size(320, 234);
            grpExtensions.TabIndex = 0;
            grpExtensions.TabStop = false;
            grpExtensions.Text = "File Extensions";
            //
            // txtIgnorePatterns
            //
            txtIgnorePatterns.BorderStyle = BorderStyle.FixedSingle;
            txtIgnorePatterns.Location = new Point(7, 206);
            txtIgnorePatterns.Name = "txtIgnorePatterns";
            txtIgnorePatterns.Size = new Size(301, 21);
            txtIgnorePatterns.TabIndex = 8;
            txtIgnorePatterns.AccessibleName = "Ignore patterns";
            txtIgnorePatterns.AccessibleDescription = "Comma-separated patterns for files to exclude.";
            txtIgnorePatterns.TextChanged += TxtIgnorePatterns_TextChanged;
            //
            // lblIgnorePatterns
            //
            lblIgnorePatterns.AutoSize = true;
            lblIgnorePatterns.Location = new Point(7, 190);
            lblIgnorePatterns.Name = "lblIgnorePatterns";
            lblIgnorePatterns.Size = new Size(95, 15);
            lblIgnorePatterns.TabIndex = 7;
            lblIgnorePatterns.Text = "Ignore Patterns:";
            //
            // btnEditRules
            //
            // The comma-separated box stays as a quick edit; this opens the row-per-rule
            // editor, which is the only place the effect of a rule is actually visible.
            btnEditRules.Cursor = Cursors.Hand;
            btnEditRules.FlatAppearance.BorderSize = 0;
            btnEditRules.FlatStyle = FlatStyle.Flat;
            btnEditRules.Location = new Point(240, 186);
            btnEditRules.Name = "btnEditRules";
            btnEditRules.Size = new Size(68, 20);
            btnEditRules.TabIndex = 9;
            btnEditRules.Text = "Edit &rules";
            btnEditRules.AccessibleName = "Edit exclusion rules";
            btnEditRules.AccessibleDescription =
                "Open the rule editor, showing how many files each rule removes.";
            btnEditRules.UseVisualStyleBackColor = false;
            btnEditRules.Click += BtnEditRules_Click;
            //
            // lblExtension
            //
            lblExtension.AutoSize = true;
            lblExtension.Location = new Point(7, 18);
            lblExtension.Name = "lblExtension";
            lblExtension.Size = new Size(113, 15);
            lblExtension.TabIndex = 0;
            lblExtension.Text = "Add File Extension:";
            //
            // cmbExtension
            //
            cmbExtension.Location = new Point(7, 36);
            cmbExtension.Name = "cmbExtension";
            cmbExtension.Size = new Size(174, 22);
            cmbExtension.TabIndex = 1;
            cmbExtension.AccessibleName = "Add file extension";
            cmbExtension.KeyPress += CmbExtension_KeyPress;
            //
            // btnAdd
            //
            btnAdd.DropDownMenu = cmsAddDropdown;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Location = new Point(189, 36);
            btnAdd.Name = "btnAdd";
            btnAdd.ShowSplit = true;
            btnAdd.Size = new Size(93, 26);
            btnAdd.TabIndex = 2;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += BtnAdd_Click;
            //
            // cmsAddDropdown
            //
            cmsAddDropdown.ImageScalingSize = new Size(17, 14);
            cmsAddDropdown.Items.AddRange(new ToolStripItem[] { mnuAddLangPresets, mnuAddSep1, miShowExtensionSummary });
            cmsAddDropdown.Name = "cmsAddDropdown";
            cmsAddDropdown.Size = new Size(211, 44);
            //
            // mnuAddLangPresets
            //
            mnuAddLangPresets.DropDownItems.AddRange(new ToolStripItem[] { mnuLangCs, mnuLangCpp, mnuLangWeb, mnuLangTs, mnuLangNode, mnuLangPy, mnuLangJava, mnuLangKotlin, mnuLangGo, mnuLangRust, mnuLangRuby, mnuLangPhp, mnuLangSwift, mnuLangShell, mnuLangDocs, mnuLangConfig });
            mnuAddLangPresets.Name = "mnuAddLangPresets";
            mnuAddLangPresets.Size = new Size(210, 19);
            mnuAddLangPresets.Text = "Add language preset";
            //
            // mnuLangCs
            //
            mnuLangCs.Name = "mnuLangCs";
            mnuLangCs.Size = new Size(190, 20);
            mnuLangCs.Text = "C# project";
            mnuLangCs.Click += MnuLanguagePreset_Click;
            //
            // mnuLangCpp
            //
            mnuLangCpp.Name = "mnuLangCpp";
            mnuLangCpp.Size = new Size(190, 20);
            mnuLangCpp.Text = "C / C++";
            mnuLangCpp.Click += MnuLanguagePreset_Click;
            //
            // mnuLangWeb
            //
            mnuLangWeb.Name = "mnuLangWeb";
            mnuLangWeb.Size = new Size(190, 20);
            mnuLangWeb.Text = "Web (HTML/CSS/JS)";
            mnuLangWeb.Click += MnuLanguagePreset_Click;
            //
            // mnuLangTs
            //
            mnuLangTs.Name = "mnuLangTs";
            mnuLangTs.Size = new Size(190, 20);
            mnuLangTs.Text = "TypeScript / React";
            mnuLangTs.Click += MnuLanguagePreset_Click;
            //
            // mnuLangNode
            //
            mnuLangNode.Name = "mnuLangNode";
            mnuLangNode.Size = new Size(190, 20);
            mnuLangNode.Text = "Node.js";
            mnuLangNode.Click += MnuLanguagePreset_Click;
            //
            // mnuLangPy
            //
            mnuLangPy.Name = "mnuLangPy";
            mnuLangPy.Size = new Size(190, 20);
            mnuLangPy.Text = "Python";
            mnuLangPy.Click += MnuLanguagePreset_Click;
            //
            // mnuLangJava
            //
            mnuLangJava.Name = "mnuLangJava";
            mnuLangJava.Size = new Size(190, 20);
            mnuLangJava.Text = "Java";
            mnuLangJava.Click += MnuLanguagePreset_Click;
            //
            // mnuLangKotlin
            //
            mnuLangKotlin.Name = "mnuLangKotlin";
            mnuLangKotlin.Size = new Size(190, 20);
            mnuLangKotlin.Text = "Kotlin";
            mnuLangKotlin.Click += MnuLanguagePreset_Click;
            //
            // mnuLangGo
            //
            mnuLangGo.Name = "mnuLangGo";
            mnuLangGo.Size = new Size(190, 20);
            mnuLangGo.Text = "Go";
            mnuLangGo.Click += MnuLanguagePreset_Click;
            //
            // mnuLangRust
            //
            mnuLangRust.Name = "mnuLangRust";
            mnuLangRust.Size = new Size(190, 20);
            mnuLangRust.Text = "Rust";
            mnuLangRust.Click += MnuLanguagePreset_Click;
            //
            // mnuLangRuby
            //
            mnuLangRuby.Name = "mnuLangRuby";
            mnuLangRuby.Size = new Size(190, 20);
            mnuLangRuby.Text = "Ruby";
            mnuLangRuby.Click += MnuLanguagePreset_Click;
            //
            // mnuLangPhp
            //
            mnuLangPhp.Name = "mnuLangPhp";
            mnuLangPhp.Size = new Size(190, 20);
            mnuLangPhp.Text = "PHP";
            mnuLangPhp.Click += MnuLanguagePreset_Click;
            //
            // mnuLangSwift
            //
            mnuLangSwift.Name = "mnuLangSwift";
            mnuLangSwift.Size = new Size(190, 20);
            mnuLangSwift.Text = "Swift";
            mnuLangSwift.Click += MnuLanguagePreset_Click;
            //
            // mnuLangShell
            //
            mnuLangShell.Name = "mnuLangShell";
            mnuLangShell.Size = new Size(190, 20);
            mnuLangShell.Text = "Shell / Scripts";
            mnuLangShell.Click += MnuLanguagePreset_Click;
            //
            // mnuLangDocs
            //
            mnuLangDocs.Name = "mnuLangDocs";
            mnuLangDocs.Size = new Size(190, 20);
            mnuLangDocs.Text = "Docs / Markup";
            mnuLangDocs.Click += MnuLanguagePreset_Click;
            //
            // mnuLangConfig
            //
            mnuLangConfig.Name = "mnuLangConfig";
            mnuLangConfig.Size = new Size(190, 20);
            mnuLangConfig.Text = "Config files";
            mnuLangConfig.Click += MnuLanguagePreset_Click;
            //
            // mnuAddSep1
            //
            mnuAddSep1.Name = "mnuAddSep1";
            mnuAddSep1.Size = new Size(208, 4);
            //
            // miShowExtensionSummary
            //
            miShowExtensionSummary.Name = "miShowExtensionSummary";
            miShowExtensionSummary.Size = new Size(210, 19);
            miShowExtensionSummary.Text = "Show extension summary…";
            miShowExtensionSummary.Click += MiShowExtensionSummary_Click;
            //
            // lstExtensions
            //
            lstExtensions.ContextMenuStrip = cmsAddDropdown;
            lstExtensions.FormattingEnabled = true;
            lstExtensions.Location = new Point(7, 63);
            lstExtensions.Name = "lstExtensions";
            lstExtensions.SelectionMode = SelectionMode.MultiExtended;
            lstExtensions.Size = new Size(174, 103);
            lstExtensions.TabIndex = 3;
            lstExtensions.AccessibleName = "File extensions";
            lstExtensions.AccessibleDescription = "Only files with these extensions are collected.";
            lstExtensions.KeyDown += LstExtensions_KeyDown;
            //
            // btnRemove
            //
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.Location = new Point(189, 69);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(93, 26);
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
            chkIncludeSubfolders.Location = new Point(7, 170);
            chkIncludeSubfolders.Name = "chkIncludeSubfolders";
            chkIncludeSubfolders.Size = new Size(146, 20);
            chkIncludeSubfolders.TabIndex = 5;
            chkIncludeSubfolders.Text = "Include s&ubfolder(s)";
            chkIncludeSubfolders.UseVisualStyleBackColor = true;
            chkIncludeSubfolders.CheckedChanged += ChkIncludeSubfolders_CheckedChanged;
            //
            // btnRefreshExtensions
            //
            btnRefreshExtensions.FlatStyle = FlatStyle.Flat;
            btnRefreshExtensions.Location = new Point(189, 101);
            btnRefreshExtensions.Name = "btnRefreshExtensions";
            btnRefreshExtensions.Size = new Size(93, 26);
            btnRefreshExtensions.TabIndex = 6;
            btnRefreshExtensions.Text = "Refresh";
            btnRefreshExtensions.UseVisualStyleBackColor = false;
            btnRefreshExtensions.Click += BtnRefreshExtensions_Click;
            //
            // cmsRecentFolders
            //
            cmsRecentFolders.ImageScalingSize = new Size(14, 12);
            cmsRecentFolders.Items.AddRange(new ToolStripItem[] { mnuRfEmpty, mnuRf01, mnuRf02, mnuRf03, mnuRf04, mnuRf05, mnuRf06, mnuRf07, mnuRf08, mnuRf09, mnuRf10, mnuRf11, mnuRf12, mnuRf13, mnuRf14, mnuRf15, mnuRfSep, mnuRfClear });
            cmsRecentFolders.Name = "cmsRecentFolders";
            cmsRecentFolders.Size = new Size(128, 332);
            //
            // mnuRfEmpty
            //
            mnuRfEmpty.Enabled = false;
            mnuRfEmpty.Name = "mnuRfEmpty";
            mnuRfEmpty.Size = new Size(127, 19);
            mnuRfEmpty.Text = "(empty)";
            //
            // mnuRf01
            //
            mnuRf01.Name = "mnuRf01";
            mnuRf01.Size = new Size(127, 19);
            //
            // mnuRf02
            //
            mnuRf02.Name = "mnuRf02";
            mnuRf02.Size = new Size(127, 19);
            //
            // mnuRf03
            //
            mnuRf03.Name = "mnuRf03";
            mnuRf03.Size = new Size(127, 19);
            //
            // mnuRf04
            //
            mnuRf04.Name = "mnuRf04";
            mnuRf04.Size = new Size(127, 19);
            //
            // mnuRf05
            //
            mnuRf05.Name = "mnuRf05";
            mnuRf05.Size = new Size(127, 19);
            //
            // mnuRf06
            //
            mnuRf06.Name = "mnuRf06";
            mnuRf06.Size = new Size(127, 19);
            //
            // mnuRf07
            //
            mnuRf07.Name = "mnuRf07";
            mnuRf07.Size = new Size(127, 19);
            //
            // mnuRf08
            //
            mnuRf08.Name = "mnuRf08";
            mnuRf08.Size = new Size(127, 19);
            //
            // mnuRf09
            //
            mnuRf09.Name = "mnuRf09";
            mnuRf09.Size = new Size(127, 19);
            //
            // mnuRf10
            //
            mnuRf10.Name = "mnuRf10";
            mnuRf10.Size = new Size(127, 19);
            //
            // mnuRf11
            //
            mnuRf11.Name = "mnuRf11";
            mnuRf11.Size = new Size(127, 19);
            //
            // mnuRf12
            //
            mnuRf12.Name = "mnuRf12";
            mnuRf12.Size = new Size(127, 19);
            //
            // mnuRf13
            //
            mnuRf13.Name = "mnuRf13";
            mnuRf13.Size = new Size(127, 19);
            //
            // mnuRf14
            //
            mnuRf14.Name = "mnuRf14";
            mnuRf14.Size = new Size(127, 19);
            //
            // mnuRf15
            //
            mnuRf15.Name = "mnuRf15";
            mnuRf15.Size = new Size(127, 19);
            //
            // mnuRfSep
            //
            mnuRfSep.Name = "mnuRfSep";
            mnuRfSep.Size = new Size(125, 4);
            //
            // mnuRfClear
            //
            mnuRfClear.Name = "mnuRfClear";
            mnuRfClear.Size = new Size(127, 19);
            mnuRfClear.Text = "Clear history";
            mnuRfClear.Click += MnuRfClear_Click;
            //
            // cmsRecentSearches
            //
            cmsRecentSearches.ImageScalingSize = new Size(14, 12);
            cmsRecentSearches.Items.AddRange(new ToolStripItem[] { mnuRsEmpty, mnuRs01, mnuRs02, mnuRs03, mnuRs04, mnuRs05, mnuRs06, mnuRs07, mnuRs08, mnuRs09, mnuRs10, mnuRs11, mnuRs12, mnuRs13, mnuRs14, mnuRs15, mnuRsSep, mnuRsClear });
            cmsRecentSearches.Name = "cmsRecentSearches";
            cmsRecentSearches.Size = new Size(128, 332);
            //
            // mnuRsEmpty
            //
            mnuRsEmpty.Enabled = false;
            mnuRsEmpty.Name = "mnuRsEmpty";
            mnuRsEmpty.Size = new Size(127, 19);
            mnuRsEmpty.Text = "(empty)";
            //
            // mnuRs01
            //
            mnuRs01.Name = "mnuRs01";
            mnuRs01.Size = new Size(127, 19);
            //
            // mnuRs02
            //
            mnuRs02.Name = "mnuRs02";
            mnuRs02.Size = new Size(127, 19);
            //
            // mnuRs03
            //
            mnuRs03.Name = "mnuRs03";
            mnuRs03.Size = new Size(127, 19);
            //
            // mnuRs04
            //
            mnuRs04.Name = "mnuRs04";
            mnuRs04.Size = new Size(127, 19);
            //
            // mnuRs05
            //
            mnuRs05.Name = "mnuRs05";
            mnuRs05.Size = new Size(127, 19);
            //
            // mnuRs06
            //
            mnuRs06.Name = "mnuRs06";
            mnuRs06.Size = new Size(127, 19);
            //
            // mnuRs07
            //
            mnuRs07.Name = "mnuRs07";
            mnuRs07.Size = new Size(127, 19);
            //
            // mnuRs08
            //
            mnuRs08.Name = "mnuRs08";
            mnuRs08.Size = new Size(127, 19);
            //
            // mnuRs09
            //
            mnuRs09.Name = "mnuRs09";
            mnuRs09.Size = new Size(127, 19);
            //
            // mnuRs10
            //
            mnuRs10.Name = "mnuRs10";
            mnuRs10.Size = new Size(127, 19);
            //
            // mnuRs11
            //
            mnuRs11.Name = "mnuRs11";
            mnuRs11.Size = new Size(127, 19);
            //
            // mnuRs12
            //
            mnuRs12.Name = "mnuRs12";
            mnuRs12.Size = new Size(127, 19);
            //
            // mnuRs13
            //
            mnuRs13.Name = "mnuRs13";
            mnuRs13.Size = new Size(127, 19);
            //
            // mnuRs14
            //
            mnuRs14.Name = "mnuRs14";
            mnuRs14.Size = new Size(127, 19);
            //
            // mnuRs15
            //
            mnuRs15.Name = "mnuRs15";
            mnuRs15.Size = new Size(127, 19);
            //
            // mnuRsSep
            //
            mnuRsSep.Name = "mnuRsSep";
            mnuRsSep.Size = new Size(125, 4);
            //
            // mnuRsClear
            //
            mnuRsClear.Name = "mnuRsClear";
            mnuRsClear.Size = new Size(127, 19);
            mnuRsClear.Text = "Clear history";
            mnuRsClear.Click += MnuRsClear_Click;
            //
            // cmsPresets
            //
            cmsPresets.ImageScalingSize = new Size(14, 12);
            cmsPresets.Items.AddRange(new ToolStripItem[] { mnuPsEmpty, mnuPs01, mnuPs02, mnuPs03, mnuPs04, mnuPs05, mnuPs06, mnuPs07, mnuPs08, mnuPs09, mnuPs10, mnuPs11, mnuPs12, mnuPs13, mnuPs14, mnuPs15, mnuPs16, mnuPs17, mnuPs18, mnuPs19, mnuPs20, mnuPs21, mnuPs22, mnuPs23, mnuPs24, mnuPs25, mnuPsSep, mnuPsManage });
            cmsPresets.Name = "cmsPresets";
            cmsPresets.Size = new Size(157, 524);
            //
            // mnuPsEmpty
            //
            mnuPsEmpty.Enabled = false;
            mnuPsEmpty.Name = "mnuPsEmpty";
            mnuPsEmpty.Size = new Size(156, 19);
            mnuPsEmpty.Text = "(no presets)";
            //
            // mnuPs01
            //
            mnuPs01.Name = "mnuPs01";
            mnuPs01.Size = new Size(156, 19);
            //
            // mnuPs02
            //
            mnuPs02.Name = "mnuPs02";
            mnuPs02.Size = new Size(156, 19);
            //
            // mnuPs03
            //
            mnuPs03.Name = "mnuPs03";
            mnuPs03.Size = new Size(156, 19);
            //
            // mnuPs04
            //
            mnuPs04.Name = "mnuPs04";
            mnuPs04.Size = new Size(156, 19);
            //
            // mnuPs05
            //
            mnuPs05.Name = "mnuPs05";
            mnuPs05.Size = new Size(156, 19);
            //
            // mnuPs06
            //
            mnuPs06.Name = "mnuPs06";
            mnuPs06.Size = new Size(156, 19);
            //
            // mnuPs07
            //
            mnuPs07.Name = "mnuPs07";
            mnuPs07.Size = new Size(156, 19);
            //
            // mnuPs08
            //
            mnuPs08.Name = "mnuPs08";
            mnuPs08.Size = new Size(156, 19);
            //
            // mnuPs09
            //
            mnuPs09.Name = "mnuPs09";
            mnuPs09.Size = new Size(156, 19);
            //
            // mnuPs10
            //
            mnuPs10.Name = "mnuPs10";
            mnuPs10.Size = new Size(156, 19);
            //
            // mnuPs11
            //
            mnuPs11.Name = "mnuPs11";
            mnuPs11.Size = new Size(156, 19);
            //
            // mnuPs12
            //
            mnuPs12.Name = "mnuPs12";
            mnuPs12.Size = new Size(156, 19);
            //
            // mnuPs13
            //
            mnuPs13.Name = "mnuPs13";
            mnuPs13.Size = new Size(156, 19);
            //
            // mnuPs14
            //
            mnuPs14.Name = "mnuPs14";
            mnuPs14.Size = new Size(156, 19);
            //
            // mnuPs15
            //
            mnuPs15.Name = "mnuPs15";
            mnuPs15.Size = new Size(156, 19);
            //
            // mnuPs16
            //
            mnuPs16.Name = "mnuPs16";
            mnuPs16.Size = new Size(156, 19);
            //
            // mnuPs17
            //
            mnuPs17.Name = "mnuPs17";
            mnuPs17.Size = new Size(156, 19);
            //
            // mnuPs18
            //
            mnuPs18.Name = "mnuPs18";
            mnuPs18.Size = new Size(156, 19);
            //
            // mnuPs19
            //
            mnuPs19.Name = "mnuPs19";
            mnuPs19.Size = new Size(156, 19);
            //
            // mnuPs20
            //
            mnuPs20.Name = "mnuPs20";
            mnuPs20.Size = new Size(156, 19);
            //
            // mnuPs21
            //
            mnuPs21.Name = "mnuPs21";
            mnuPs21.Size = new Size(156, 19);
            //
            // mnuPs22
            //
            mnuPs22.Name = "mnuPs22";
            mnuPs22.Size = new Size(156, 19);
            //
            // mnuPs23
            //
            mnuPs23.Name = "mnuPs23";
            mnuPs23.Size = new Size(156, 19);
            //
            // mnuPs24
            //
            mnuPs24.Name = "mnuPs24";
            mnuPs24.Size = new Size(156, 19);
            //
            // mnuPs25
            //
            mnuPs25.Name = "mnuPs25";
            mnuPs25.Size = new Size(156, 19);
            //
            // mnuPsSep
            //
            mnuPsSep.Name = "mnuPsSep";
            mnuPsSep.Size = new Size(154, 4);
            //
            // mnuPsManage
            //
            mnuPsManage.Name = "mnuPsManage";
            mnuPsManage.Size = new Size(156, 19);
            mnuPsManage.Text = "Manage presets…";
            mnuPsManage.Click += MnuPsManage_Click;
            //
            // menuMain
            //
            menuMain.ImageScalingSize = new Size(14, 12);
            menuMain.Items.AddRange(new ToolStripItem[] { mnuView, mnuTools, mnuHelp });
            menuMain.Location = new Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Size = new Size(1148, 20);
            menuMain.TabIndex = 11;
            //
            // mnuView
            //
            mnuView.DropDownItems.AddRange(new ToolStripItem[] { mnuViewDarkMode });
            mnuView.Name = "mnuView";
            mnuView.Size = new Size(48, 17);
            mnuView.Text = "&View";
            //
            // mnuViewDarkMode
            //
            mnuViewDarkMode.CheckOnClick = true;
            mnuViewDarkMode.Name = "mnuViewDarkMode";
            mnuViewDarkMode.Size = new Size(144, 20);
            mnuViewDarkMode.Text = "&Dark mode";
            mnuViewDarkMode.CheckedChanged += MnuViewDarkMode_CheckedChanged;
            //
            // mnuTools
            //
            // Where the compression and encryption utilities went. They are not part of the
            // round trip the product is sold on, and on the main surface they competed for
            // attention with Generate and Apply. The functionality itself is unchanged.
            mnuTools.DropDownItems.AddRange(new ToolStripItem[] {
                mnuToolsPasteResponse, mnuToolsPromptTemplates, mnuToolsExclusionRules,
                mnuToolsSep1, mnuToolsImportSettings, mnuToolsExportSettings,
                mnuToolsSep2, mnuToolsCompression });
            mnuTools.Name = "mnuTools";
            mnuTools.Size = new Size(52, 17);
            mnuTools.Text = "&Tools";
            //
            // mnuToolsPasteResponse
            //
            mnuToolsPasteResponse.Name = "mnuToolsPasteResponse";
            mnuToolsPasteResponse.ShortcutKeys = Keys.Control | Keys.Shift | Keys.V;
            mnuToolsPasteResponse.Size = new Size(240, 20);
            mnuToolsPasteResponse.Text = "&Paste AI response...";
            mnuToolsPasteResponse.Click += BtnPasteResponse_Click;
            //
            // mnuToolsPromptTemplates
            //
            mnuToolsPromptTemplates.Name = "mnuToolsPromptTemplates";
            mnuToolsPromptTemplates.Size = new Size(240, 20);
            mnuToolsPromptTemplates.Text = "Prompt &templates...";
            mnuToolsPromptTemplates.Click += MnuCopyAsPrompt_Click;
            //
            // mnuToolsExclusionRules
            //
            mnuToolsExclusionRules.Name = "mnuToolsExclusionRules";
            mnuToolsExclusionRules.Size = new Size(240, 20);
            mnuToolsExclusionRules.Text = "&Exclusion rules...";
            mnuToolsExclusionRules.Click += BtnEditRules_Click;
            //
            // mnuToolsSep1
            //
            mnuToolsSep1.Name = "mnuToolsSep1";
            mnuToolsSep1.Size = new Size(238, 4);
            //
            // mnuToolsImportSettings
            //
            mnuToolsImportSettings.Name = "mnuToolsImportSettings";
            mnuToolsImportSettings.Size = new Size(240, 20);
            mnuToolsImportSettings.Text = "&Import settings...";
            mnuToolsImportSettings.Click += MnuImportSettings_Click;
            //
            // mnuToolsExportSettings
            //
            mnuToolsExportSettings.Name = "mnuToolsExportSettings";
            mnuToolsExportSettings.Size = new Size(240, 20);
            mnuToolsExportSettings.Text = "E&xport settings...";
            mnuToolsExportSettings.Click += MnuExportSettings_Click;
            //
            // mnuToolsSep2
            //
            mnuToolsSep2.Name = "mnuToolsSep2";
            mnuToolsSep2.Size = new Size(238, 4);
            //
            // mnuToolsCompression
            //
            mnuToolsCompression.DropDownItems.AddRange(new ToolStripItem[] {
                mnuToolsCompress, mnuToolsDecompress, mnuToolsCompressEnc, mnuToolsDecompressEnc });
            mnuToolsCompression.Name = "mnuToolsCompression";
            mnuToolsCompression.Size = new Size(240, 20);
            mnuToolsCompression.Text = "&Compression and encryption";
            //
            // mnuToolsCompress
            //
            mnuToolsCompress.Name = "mnuToolsCompress";
            mnuToolsCompress.Size = new Size(228, 20);
            mnuToolsCompress.Text = "Co&mpress output";
            mnuToolsCompress.Click += BtnCompress_Click;
            //
            // mnuToolsDecompress
            //
            mnuToolsDecompress.Name = "mnuToolsDecompress";
            mnuToolsDecompress.Size = new Size(228, 20);
            mnuToolsDecompress.Text = "&Decompress output";
            mnuToolsDecompress.Click += BtnDecompress_Click;
            //
            // mnuToolsCompressEnc
            //
            mnuToolsCompressEnc.Name = "mnuToolsCompressEnc";
            mnuToolsCompressEnc.Size = new Size(228, 20);
            mnuToolsCompressEnc.Text = "Compress and &encrypt...";
            mnuToolsCompressEnc.Click += BtnCompressEnc_Click;
            //
            // mnuToolsDecompressEnc
            //
            mnuToolsDecompressEnc.Name = "mnuToolsDecompressEnc";
            mnuToolsDecompressEnc.Size = new Size(228, 20);
            mnuToolsDecompressEnc.Text = "Decr&ypt and decompress...";
            mnuToolsDecompressEnc.Click += BtnDecompressEnc_Click;
            //
            // mnuHelp
            //
            mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { mnuHelpContents, mnuHelpShortcuts, mnuHelpCheckUpdates, mnuHelpSep1, mnuHelpAbout });
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new Size(47, 17);
            mnuHelp.Text = "&Help";
            //
            // mnuHelpShortcuts
            //
            // F1 is handled in ProcessCmdKey so that it opens the topic for whatever pane
            // has focus. Declaring it here as well would consume the key before that runs.
            mnuHelpShortcuts.Name = "mnuHelpShortcuts";
            mnuHelpShortcuts.Size = new Size(226, 20);
            mnuHelpShortcuts.Text = "&Keyboard Shortcuts…";
            mnuHelpShortcuts.Click += MnuHelpShortcuts_Click;
            //
            // mnuHelpContents
            //
            mnuHelpContents.Name = "mnuHelpContents";
            mnuHelpContents.ShortcutKeys = Keys.Shift | Keys.F1;
            mnuHelpContents.Size = new Size(226, 20);
            mnuHelpContents.Text = "&Help contents...";
            mnuHelpContents.Click += MnuHelpContents_Click;
            //
            // mnuHelpCheckUpdates
            //
            mnuHelpCheckUpdates.Name = "mnuHelpCheckUpdates";
            mnuHelpCheckUpdates.Size = new Size(226, 20);
            mnuHelpCheckUpdates.Text = "Check for &updates…";
            mnuHelpCheckUpdates.Click += MnuHelpCheckUpdates_Click;
            //
            // mnuHelpSep1
            //
            mnuHelpSep1.Name = "mnuHelpSep1";
            mnuHelpSep1.Size = new Size(224, 4);
            //
            // mnuHelpAbout
            //
            mnuHelpAbout.Name = "mnuHelpAbout";
            mnuHelpAbout.Size = new Size(226, 20);
            mnuHelpAbout.Text = "&About…";
            mnuHelpAbout.Click += MnuHelpAbout_Click;
            //
            // statusBar
            //
            statusBar.ImageScalingSize = new Size(14, 12);
            statusBar.Items.AddRange(new ToolStripItem[] { sbFileCount, sbTotalSize, sbSkipped, sbSpring, sbProgress, sbCancel, sbScanStatus, sbUpdateNotice });
            statusBar.Location = new Point(0, 713);
            statusBar.Name = "statusBar";
            statusBar.Padding = new Padding(6, 1, 6, 1);
            statusBar.Size = new Size(1148, 22);
            statusBar.SizingGrip = false;
            statusBar.TabIndex = 12;
            //
            // sbFileCount
            //
            sbFileCount.Name = "sbFileCount";
            sbFileCount.Size = new Size(46, 15);
            sbFileCount.Text = "Files: 0";
            //
            // sbTotalSize
            //
            sbTotalSize.Margin = new Padding(14, 2, 0, 1);
            sbTotalSize.Name = "sbTotalSize";
            sbTotalSize.Size = new Size(54, 16);
            sbTotalSize.Text = "Size: 0 B";
            //
            // sbSpring
            //
            sbSpring.Name = "sbSpring";
            sbSpring.Size = new Size(1023, 15);
            sbSpring.Spring = true;
            //
            // sbScanStatus
            //
            sbScanStatus.Name = "sbScanStatus";
            sbScanStatus.Size = new Size(0, 15);
            //
            // sbUpdateNotice
            //
            sbUpdateNotice.IsLink = true;
            sbUpdateNotice.Margin = new Padding(14, 2, 0, 1);
            sbUpdateNotice.Name = "sbUpdateNotice";
            sbUpdateNotice.Size = new Size(0, 16);
            sbUpdateNotice.Visible = false;
            sbUpdateNotice.Click += SbUpdateNotice_Click;
            //
            // sbSkipped
            //
            sbSkipped.IsLink = true;
            sbSkipped.Margin = new Padding(14, 2, 0, 1);
            sbSkipped.Name = "sbSkipped";
            sbSkipped.Size = new Size(0, 16);
            sbSkipped.Visible = false;
            sbSkipped.AccessibleName = "Skipped files";
            sbSkipped.ToolTipText = "Click to see which files were left out and why";
            sbSkipped.Click += SbSkipped_Click;
            //
            // sbProgress
            //
            sbProgress.Name = "sbProgress";
            sbProgress.Size = new Size(140, 14);
            sbProgress.Visible = false;
            sbProgress.AccessibleName = "Progress";
            //
            // sbCancel
            //
            // The cancellation tokens already existed and were only ever triggered by one scan
            // superseding another. This is the first user-facing way to stop anything.
            sbCancel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            sbCancel.Name = "sbCancel";
            sbCancel.Size = new Size(48, 20);
            sbCancel.Text = "Cancel";
            sbCancel.Visible = false;
            sbCancel.AccessibleName = "Cancel the running operation";
            sbCancel.ToolTipText = "Cancel (Esc)";
            sbCancel.Click += SbCancel_Click;
            //
            // pnlBottom
            //
            pnlBottom.Controls.Add(btnGenerate);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 683);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(1148, 30);
            pnlBottom.TabIndex = 3;
            // The free-floating ProgressBar that used to sit here is gone: progress now lives in
            // the status strip (sbProgress), where it is shared by scan, generate, search and
            // apply and sits beside the Cancel button rather than a pane away from it.
            //
            // btnGenerate
            //
            btnGenerate.Anchor = AnchorStyles.None;
            btnGenerate.FlatStyle = FlatStyle.Flat;
            btnGenerate.Location = new Point(486, 0);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(168, 29);
            btnGenerate.TabIndex = 0;
            btnGenerate.Text = "&GENERATE";
            btnGenerate.UseVisualStyleBackColor = false;
            btnGenerate.Click += BtnGenerate_Click;
            //
            // pnlRight
            //
            pnlRight.Controls.Add(rtbOutput);
            pnlRight.Controls.Add(pnlRecreateInfo);
            pnlRight.Controls.Add(pnlOutput);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Location = new Point(348, 119);
            pnlRight.Name = "pnlRight";
            pnlRight.Padding = new Padding(14, 12, 14, 12);
            pnlRight.Size = new Size(800, 564);
            pnlRight.TabIndex = 2;
            //
            // rtbOutput
            //
            rtbOutput.Dock = DockStyle.Fill;
            rtbOutput.Location = new Point(14, 144);
            rtbOutput.Name = "rtbOutput";
            rtbOutput.ReadOnly = true;
            rtbOutput.Size = new Size(772, 408);
            rtbOutput.TabIndex = 1;
            rtbOutput.Text = "";
            rtbOutput.WordWrap = false;
            rtbOutput.AccessibleName = "Generated output";
            rtbOutput.AccessibleDescription = "The assembled pack of file contents.";
            //
            // pnlRecreateInfo
            //
            pnlRecreateInfo.Controls.Add(tblRecreateInfo);
            pnlRecreateInfo.Dock = DockStyle.Top;
            pnlRecreateInfo.Location = new Point(14, 96);
            pnlRecreateInfo.Name = "pnlRecreateInfo";
            pnlRecreateInfo.Size = new Size(772, 48);
            pnlRecreateInfo.TabIndex = 4;
            // Hidden until there is output to apply, rather than occupying the pane permanently.
            pnlRecreateInfo.Visible = false;
            //
            // tblRecreateInfo
            //
            tblRecreateInfo.ColumnCount = 4;
            tblRecreateInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblRecreateInfo.ColumnStyles.Add(new ColumnStyle());
            tblRecreateInfo.ColumnStyles.Add(new ColumnStyle());
            tblRecreateInfo.ColumnStyles.Add(new ColumnStyle());
            tblRecreateInfo.Controls.Add(lblRecreateInfo, 0, 0);
            tblRecreateInfo.Controls.Add(btnPasteResponse, 1, 0);
            tblRecreateInfo.Controls.Add(btnApplyAiChanges, 2, 0);
            tblRecreateInfo.Controls.Add(btnHideRecreateInfo, 3, 0);
            tblRecreateInfo.Dock = DockStyle.Fill;
            tblRecreateInfo.Location = new Point(0, 0);
            tblRecreateInfo.Name = "tblRecreateInfo";
            tblRecreateInfo.RowCount = 1;
            tblRecreateInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblRecreateInfo.Size = new Size(772, 48);
            tblRecreateInfo.TabIndex = 0;
            //
            // lblRecreateInfo
            //
            lblRecreateInfo.Dock = DockStyle.Fill;
            lblRecreateInfo.Location = new Point(2, 0);
            lblRecreateInfo.Name = "lblRecreateInfo";
            lblRecreateInfo.Size = new Size(666, 48);
            lblRecreateInfo.TabIndex = 0;
            // The strip is the round-trip's entry point, not a nag about a utility function.
            lblRecreateInfo.Text =
                "Round trip: send this pack to your AI, then bring the reply back here to diff and apply it.";
            lblRecreateInfo.TextAlign = ContentAlignment.MiddleLeft;
            //
            // btnPasteResponse
            //
            // The inbound half. Before this the only route in was pasting into the output pane —
            // a control that is read-only and is being used as an output.
            btnPasteResponse.Cursor = Cursors.Hand;
            btnPasteResponse.FlatStyle = FlatStyle.Flat;
            btnPasteResponse.Location = new Point(540, 2);
            btnPasteResponse.Name = "btnPasteResponse";
            btnPasteResponse.Size = new Size(126, 24);
            btnPasteResponse.TabIndex = 1;
            btnPasteResponse.Text = "&Paste AI response…";
            btnPasteResponse.AccessibleName = "Paste AI response";
            btnPasteResponse.AccessibleDescription =
                "Paste a reply from an AI chat and review it against a folder.";
            toolTip1.SetToolTip(btnPasteResponse,
                "Paste the AI's reply and review it against a folder (Ctrl+Shift+V). No Generate needed first.");
            btnPasteResponse.UseVisualStyleBackColor = false;
            btnPasteResponse.Click += BtnPasteResponse_Click;
            //
            // btnApplyAiChanges
            //
            btnApplyAiChanges.Cursor = Cursors.Hand;
            btnApplyAiChanges.FlatStyle = FlatStyle.Flat;
            btnApplyAiChanges.Location = new Point(672, 2);
            btnApplyAiChanges.Name = "btnApplyAiChanges";
            btnApplyAiChanges.Size = new Size(126, 24);
            btnApplyAiChanges.TabIndex = 2;
            btnApplyAiChanges.Text = "&Apply AI Changes…";
            btnApplyAiChanges.AccessibleName = "Apply AI changes";
            btnApplyAiChanges.AccessibleDescription =
                "Diff the output pane against a folder and write the changes you accept.";
            btnApplyAiChanges.UseVisualStyleBackColor = false;
            btnApplyAiChanges.Click += BtnApplyAiChanges_Click;
            //
            // btnHideRecreateInfo
            //
            btnHideRecreateInfo.Cursor = Cursors.Hand;
            btnHideRecreateInfo.FlatAppearance.BorderSize = 0;
            btnHideRecreateInfo.FlatStyle = FlatStyle.Flat;
            btnHideRecreateInfo.Location = new Point(2, 2);
            btnHideRecreateInfo.Margin = new Padding(6, 2, 2, 2);
            btnHideRecreateInfo.Name = "btnHideRecreateInfo";
            btnHideRecreateInfo.Size = new Size(52, 24);
            btnHideRecreateInfo.TabIndex = 3;
            btnHideRecreateInfo.Text = "Hide";
            btnHideRecreateInfo.AccessibleName = "Hide the round-trip strip";
            toolTip1.SetToolTip(btnHideRecreateInfo, "Hide this strip for now");
            btnHideRecreateInfo.UseVisualStyleBackColor = false;
            btnHideRecreateInfo.Click += BtnHideRecreateInfo_Click;
            //
            // pnlOutput
            //
            pnlOutput.Controls.Add(pnlBudget);
            pnlOutput.Controls.Add(lblOutputStats);
            pnlOutput.Controls.Add(pnlOutputHeader);
            pnlOutput.Controls.Add(pnlSeparator);
            pnlOutput.Dock = DockStyle.Top;
            pnlOutput.Location = new Point(14, 12);
            pnlOutput.Name = "pnlOutput";
            pnlOutput.Size = new Size(772, 112);
            pnlOutput.TabIndex = 5;
            //
            // lblOutputStats
            //
            lblOutputStats.AutoSize = true;
            lblOutputStats.Location = new Point(7, 66);
            lblOutputStats.Name = "lblOutputStats";
            lblOutputStats.Size = new Size(181, 15);
            lblOutputStats.TabIndex = 4;
            lblOutputStats.Text = "Chars: 0 | Lines: 0 | Size: 0 bytes";
            //
            // pnlBudget
            //
            // The token count used to be one number on the end of a stats line, with nothing
            // to measure it against. It only answers "will this fit" once there is a window
            // beside it.
            pnlBudget.Controls.Add(btnBudgetBreakdown);
            pnlBudget.Controls.Add(lblBudgetText);
            pnlBudget.Controls.Add(barBudget);
            pnlBudget.Controls.Add(cmbBudgetModel);
            pnlBudget.Controls.Add(lblBudgetModel);
            pnlBudget.Dock = DockStyle.Bottom;
            pnlBudget.Location = new Point(0, 63);
            pnlBudget.Name = "pnlBudget";
            pnlBudget.Size = new Size(772, 26);
            pnlBudget.TabIndex = 5;
            //
            // lblBudgetModel
            //
            lblBudgetModel.AutoSize = true;
            lblBudgetModel.Location = new Point(7, 6);
            lblBudgetModel.Name = "lblBudgetModel";
            lblBudgetModel.Size = new Size(44, 15);
            lblBudgetModel.TabIndex = 0;
            lblBudgetModel.Text = "Fits in:";
            //
            // cmbBudgetModel
            //
            cmbBudgetModel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBudgetModel.Location = new Point(56, 2);
            cmbBudgetModel.Name = "cmbBudgetModel";
            cmbBudgetModel.Size = new Size(132, 23);
            cmbBudgetModel.TabIndex = 1;
            cmbBudgetModel.AccessibleName = "Target model context window";
            cmbBudgetModel.SelectedIndexChanged += CmbBudgetModel_SelectedIndexChanged;
            //
            // barBudget
            //
            barBudget.Location = new Point(196, 5);
            barBudget.Name = "barBudget";
            barBudget.Size = new Size(150, 16);
            barBudget.TabIndex = 2;
            barBudget.TabStop = false;
            barBudget.AccessibleName = "Context window used";
            //
            // lblBudgetText
            //
            lblBudgetText.AutoSize = true;
            lblBudgetText.Location = new Point(354, 6);
            lblBudgetText.Name = "lblBudgetText";
            lblBudgetText.Size = new Size(90, 15);
            lblBudgetText.TabIndex = 3;
            lblBudgetText.Text = "";
            lblBudgetText.AccessibleName = "Token budget";
            //
            // btnBudgetBreakdown
            //
            btnBudgetBreakdown.Cursor = Cursors.Hand;
            btnBudgetBreakdown.Dock = DockStyle.Right;
            btnBudgetBreakdown.FlatAppearance.BorderSize = 0;
            btnBudgetBreakdown.FlatStyle = FlatStyle.Flat;
            btnBudgetBreakdown.Location = new Point(672, 0);
            btnBudgetBreakdown.Name = "btnBudgetBreakdown";
            btnBudgetBreakdown.Size = new Size(100, 26);
            btnBudgetBreakdown.TabIndex = 4;
            btnBudgetBreakdown.Text = "&Breakdown...";
            btnBudgetBreakdown.AccessibleName = "Token breakdown";
            btnBudgetBreakdown.AccessibleDescription =
                "Show which files use the most tokens, and what to remove to fit.";
            btnBudgetBreakdown.UseVisualStyleBackColor = false;
            btnBudgetBreakdown.Click += BtnBudgetBreakdown_Click;
            //
            // pnlOutputHeader
            //
            // Index 0 docks last, so btnFindReplace sits at the left of the right-docked group.
            pnlOutputHeader.Controls.Add(btnFindReplace);
            pnlOutputHeader.Controls.Add(btnExportOutput);
            pnlOutputHeader.Controls.Add(btnEditOutput);
            pnlOutputHeader.Controls.Add(btnCopyOutput);
            pnlOutputHeader.Controls.Add(lblOutput);
            pnlOutputHeader.Dock = DockStyle.Top;
            pnlOutputHeader.Location = new Point(0, 33);
            pnlOutputHeader.Name = "pnlOutputHeader";
            pnlOutputHeader.Size = new Size(772, 30);
            pnlOutputHeader.TabIndex = 0;
            //
            // btnExportOutput
            //
            btnExportOutput.Cursor = Cursors.Hand;
            btnExportOutput.Dock = DockStyle.Right;
            btnExportOutput.FlatStyle = FlatStyle.Flat;
            btnExportOutput.Location = new Point(545, 0);
            btnExportOutput.Name = "btnExportOutput";
            btnExportOutput.Size = new Size(73, 30);
            btnExportOutput.TabIndex = 2;
            btnExportOutput.Text = "&Export";
            toolTip1.SetToolTip(btnExportOutput, "Export output to file");
            btnExportOutput.UseVisualStyleBackColor = false;
            btnExportOutput.Click += BtnExportOutput_Click;
            //
            // btnEditOutput
            //
            btnEditOutput.Cursor = Cursors.Hand;
            btnEditOutput.Dock = DockStyle.Right;
            btnEditOutput.FlatStyle = FlatStyle.Flat;
            btnEditOutput.Location = new Point(618, 0);
            btnEditOutput.Name = "btnEditOutput";
            btnEditOutput.Size = new Size(70, 30);
            btnEditOutput.TabIndex = 3;
            btnEditOutput.Text = "Ed&it";
            toolTip1.SetToolTip(btnEditOutput, "Edit the output");
            btnEditOutput.UseVisualStyleBackColor = false;
            btnEditOutput.Click += BtnEditOutput_Click;
            //
            // btnCopyOutput
            //
            btnCopyOutput.Cursor = Cursors.Hand;
            btnCopyOutput.Dock = DockStyle.Right;
            btnCopyOutput.DropDownMenu = cmsCopyAs;
            btnCopyOutput.FlatStyle = FlatStyle.Flat;
            btnCopyOutput.Location = new Point(688, 0);
            btnCopyOutput.Name = "btnCopyOutput";
            btnCopyOutput.ShowSplit = true;
            btnCopyOutput.Size = new Size(84, 30);
            btnCopyOutput.TabIndex = 4;
            btnCopyOutput.Text = "&Copy";
            toolTip1.SetToolTip(btnCopyOutput, "Copy output to clipboard (Ctrl+C); the arrow lists formats");
            btnCopyOutput.UseVisualStyleBackColor = false;
            btnCopyOutput.Click += BtnCopyOutput_Click;
            //
            // btnProtect
            //
            // The four compression and encryption actions, back on the main surface. They were moved
            // into Tools because four buttons competed with Generate for attention; one button that
            // opens a four-item menu keeps the capability visible without re-creating that fight.
            // The whole face opens the menu — see MainForm.Layout.cs — so no click can start an
            // encryption the user did not choose.
            btnProtect.Cursor = Cursors.Hand;
            btnProtect.Dock = DockStyle.Right;
            btnProtect.DropDownMenu = cmsProtect;
            btnProtect.FlatStyle = FlatStyle.Flat;
            btnProtect.Name = "btnProtect";
            btnProtect.ShowSplit = true;
            btnProtect.Size = new Size(84, 30);
            btnProtect.TabIndex = 5;
            btnProtect.Text = "Protect";
            toolTip1.SetToolTip(btnProtect, "Compress or encrypt the pack before it leaves this machine");
            btnProtect.UseVisualStyleBackColor = false;
            btnProtect.Click += BtnProtect_Click;
            //
            // cmsProtect
            //
            cmsProtect.ImageScalingSize = new Size(14, 12);
            cmsProtect.Items.AddRange(new ToolStripItem[] {
                mnuProtectEncrypt, mnuProtectDecrypt, mnuProtectSep, mnuProtectCompress, mnuProtectDecompress });
            cmsProtect.Name = "cmsProtect";
            cmsProtect.Size = new Size(240, 98);
            cmsProtect.Opening += CmsProtect_Opening;
            //
            // mnuProtectEncrypt
            //
            mnuProtectEncrypt.Name = "mnuProtectEncrypt";
            mnuProtectEncrypt.Size = new Size(240, 19);
            mnuProtectEncrypt.Text = "&Encrypt with password...";
            mnuProtectEncrypt.Click += BtnCompressEnc_Click;
            //
            // mnuProtectDecrypt
            //
            mnuProtectDecrypt.Name = "mnuProtectDecrypt";
            mnuProtectDecrypt.Size = new Size(240, 19);
            mnuProtectDecrypt.Text = "&Decrypt with password...";
            mnuProtectDecrypt.Click += BtnDecompressEnc_Click;
            //
            // mnuProtectSep
            //
            mnuProtectSep.Name = "mnuProtectSep";
            mnuProtectSep.Size = new Size(237, 6);
            //
            // mnuProtectCompress
            //
            mnuProtectCompress.Name = "mnuProtectCompress";
            mnuProtectCompress.Size = new Size(240, 19);
            mnuProtectCompress.Text = "Co&mpress (no password)";
            mnuProtectCompress.Click += BtnCompress_Click;
            //
            // mnuProtectDecompress
            //
            mnuProtectDecompress.Name = "mnuProtectDecompress";
            mnuProtectDecompress.Size = new Size(240, 19);
            mnuProtectDecompress.Text = "Decom&press";
            mnuProtectDecompress.Click += BtnDecompress_Click;
            //
            // cmsCopyAs
            //
            cmsCopyAs.ImageScalingSize = new Size(14, 12);
            cmsCopyAs.Items.AddRange(new ToolStripItem[] { mnuCopyPlain, mnuCopyMarkdown, mnuCopyXml, mnuCopyJson, mnuCopyAsPromptSep, mnuCopyAsPrompt });
            cmsCopyAs.Name = "cmsCopyAs";
            cmsCopyAs.Size = new Size(197, 79);
            //
            // mnuCopyPlain
            //
            mnuCopyPlain.Name = "mnuCopyPlain";
            mnuCopyPlain.Size = new Size(197, 19);
            mnuCopyPlain.Text = "Plain text";
            mnuCopyPlain.Click += MnuCopyPlain_Click;
            //
            // mnuCopyMarkdown
            //
            mnuCopyMarkdown.Name = "mnuCopyMarkdown";
            mnuCopyMarkdown.Size = new Size(197, 19);
            mnuCopyMarkdown.Text = "Markdown (fenced code)";
            mnuCopyMarkdown.Click += MnuCopyMarkdown_Click;
            //
            // mnuCopyXml
            //
            mnuCopyXml.Name = "mnuCopyXml";
            mnuCopyXml.Size = new Size(197, 19);
            mnuCopyXml.Text = "XML (Claude-friendly)";
            mnuCopyXml.Click += MnuCopyXml_Click;
            //
            // mnuCopyJson
            //
            mnuCopyJson.Name = "mnuCopyJson";
            mnuCopyJson.Size = new Size(197, 19);
            mnuCopyJson.Text = "JSON array";
            mnuCopyJson.Click += MnuCopyJson_Click;
            //
            // mnuCopyAsPromptSep
            //
            mnuCopyAsPromptSep.Name = "mnuCopyAsPromptSep";
            mnuCopyAsPromptSep.Size = new Size(195, 4);
            //
            // mnuCopyAsPrompt
            //
            // The entry point for the two prompt builders that were written, correct, and
            // reachable from nowhere in the product.
            mnuCopyAsPrompt.Name = "mnuCopyAsPrompt";
            mnuCopyAsPrompt.Size = new Size(197, 19);
            mnuCopyAsPrompt.Text = "As prompt...";
            mnuCopyAsPrompt.Click += MnuCopyAsPrompt_Click;
            //
            // lblOutput
            //
            lblOutput.AutoSize = true;
            lblOutput.Location = new Point(7, 6);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new Size(59, 18);
            lblOutput.TabIndex = 0;
            lblOutput.Text = "Output";
            //
            // pnlSeparator
            //
            pnlSeparator.Dock = DockStyle.Top;
            pnlSeparator.Location = new Point(0, 0);
            pnlSeparator.Name = "pnlSeparator";
            pnlSeparator.Size = new Size(772, 1);
            pnlSeparator.TabIndex = 1;
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1148, 735);
            Controls.Add(splitMain);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            Controls.Add(menuMain);
            Controls.Add(statusBar);
            MainMenuStrip = menuMain;
            // The previous constraint was 1315x1087 as authored, which the scaling normalisation
            // restated as 920x652 at 100%. Either way it demanded roughly 1840x1304 physical
            // pixels at 200% scaling — taller than a 1080p screen, so the window could not be
            // positioned at all. The panes now have their own minimums via Panel1MinSize and
            // Panel2MinSize, which is where a floor of this kind belongs.
            MinimumSize = new Size(760, 520);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CodeShuttle";
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
            pnlBudget.ResumeLayout(false);
            pnlBudget.PerformLayout();
            pnlOutput.ResumeLayout(false);
            pnlOutput.PerformLayout();
            pnlOutputHeader.ResumeLayout(false);
            pnlOutputHeader.PerformLayout();
            cmsCopyAs.ResumeLayout(false);
            cmsProtect.ResumeLayout(false);
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);

            // Theme roles. Colours and fonts are resolved from ThemeTokens /
            // ThemeFonts at runtime; anything not listed here takes the default
            // for its control type.
            ThemeRoles.Set(btnAdd, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(btnAddFolder, ThemeRole.ButtonSuccess, FontRole.BodyBold);
            ThemeRoles.Set(btnAddMultipleFiles, ThemeRole.ButtonSuccess, FontRole.SmallBold);
            ThemeRoles.Set(btnBrowse, ThemeRole.ButtonAccent, FontRole.MediumBold);
            ThemeRoles.Set(btnCopyOutput, ThemeRole.ButtonSubtle, FontRole.Small);
            ThemeRoles.Set(btnEditOutput, ThemeRole.ButtonSubtle, FontRole.Small);
            ThemeRoles.Set(btnExportOutput, ThemeRole.ButtonSubtle, FontRole.Small);
            ThemeRoles.Set(btnFindReplace, ThemeRole.ButtonAccent, FontRole.SmallBold);
            ThemeRoles.Set(btnGenerate, ThemeRole.ButtonAccent, FontRole.Title);
            ThemeRoles.Set(btnLoadPreset, ThemeRole.ButtonSuccess, FontRole.BodyBold);
            ThemeRoles.Set(btnProtect, ThemeRole.ButtonSubtle, FontRole.Small);
            ThemeRoles.Set(btnMoveDown, ThemeRole.ButtonSubtle, FontRole.MediumBold);
            ThemeRoles.Set(btnMoveUp, ThemeRole.ButtonSubtle, FontRole.MediumBold);
            ThemeRoles.Set(btnOptions, ThemeRole.ButtonSecondary, FontRole.BodyBold);
            ThemeRoles.Set(btnRecentFolders, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(btnApplyAiChanges, ThemeRole.ButtonSuccess, FontRole.BodyBold);
            ThemeRoles.Set(btnPasteResponse, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnHideRecreateInfo, ThemeRole.ButtonSubtle, FontRole.Small);
            ThemeRoles.Set(btnRefreshExtensions, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(btnRemove, ThemeRole.ButtonDanger, FontRole.BodyBold);
            ThemeRoles.Set(btnRemoveFile, ThemeRole.ButtonDanger, FontRole.SmallBold);
            ThemeRoles.Set(btnSavePreset, ThemeRole.ButtonSuccess, FontRole.BodyBold);
            ThemeRoles.Set(btnTree, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(chkWatch, ThemeRole.Header, FontRole.BodyBold);
            ThemeRoles.Set(cmbEncoding, FontRole.Medium);
            ThemeRoles.Set(grpExtensions, ThemeRole.SurfaceAlt, FontRole.Medium);
            ThemeRoles.SetText(grpExtensions, ThemeRole.Heading);
            ThemeRoles.Set(grpFiles, ThemeRole.SurfaceAlt, FontRole.Medium);
            ThemeRoles.SetText(grpFiles, ThemeRole.Heading);
            ThemeRoles.Set(lblEncoding, FontRole.Medium);
            ThemeRoles.Set(lblExtension, FontRole.Small);
            ThemeRoles.Set(lblIgnorePatterns, FontRole.Small);
            ThemeRoles.Set(lblBudgetModel, ThemeRole.TextSecondary, FontRole.Small);
            ThemeRoles.Set(cmbBudgetModel, FontRole.Small);
            ThemeRoles.Set(lblBudgetText, ThemeRole.TextSecondary, FontRole.Small);
            ThemeRoles.Set(btnBudgetBreakdown, ThemeRole.ButtonSubtle, FontRole.Small);
            ThemeRoles.Set(btnEditRules, ThemeRole.ButtonSubtle, FontRole.Small);
            ThemeRoles.Set(lblOutput, FontRole.Heading);
            ThemeRoles.Set(lblOutputStats, ThemeRole.TextSecondary);
            ThemeRoles.Set(lblPath, FontRole.Medium);
            ThemeRoles.Set(lblRecreateInfo, ThemeRole.BannerText, FontRole.Body);
            ThemeRoles.Set(lstFiles, FontRole.Small);
            ThemeRoles.Set(pnlBottom, ThemeRole.Surface);
            ThemeRoles.Set(pnlLeft, ThemeRole.Surface);
            ThemeRoles.Set(pnlOutput, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlRecreateInfo, ThemeRole.Banner);
            ThemeRoles.Set(pnlRight, ThemeRole.Surface);
            ThemeRoles.Set(pnlSeparator, ThemeRole.Separator);
            ThemeRoles.Set(pnlTop, ThemeRole.Header);
            ThemeRoles.Set(rtbOutput, FontRole.Mono);
            ThemeRoles.Set(sbScanStatus, ThemeRole.TextSecondary);
            ThemeRoles.Set(sbUpdateNotice, ThemeRole.Heading);
            ThemeRoles.Set(sbSkipped, ThemeRole.Heading);
            ThemeRoles.Set(statusBar, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(txtFolderPath, FontRole.Medium);
            // splitMain itself is a container and inherits; its two panels take the surface of
            // the panel each hosts, so nothing further is needed here.
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}