using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;

        private TableLayoutPanel pnlBody;
        private PictureBox picIcon;
        private TableLayoutPanel pnlIdentity;
        private Label lblAppName;
        private Label lblEdition;
        private Label lblVersion;
        private Label lblDescription;
        private Label lblCopyright;
        private FlowLayoutPanel pnlLinks;
        private LinkLabel lnkWebsite;
        private LinkLabel lnkDocs;
        private LinkLabel lnkReleaseNotes;
        private LinkLabel lnkReportBug;
        private LinkLabel lnkProject;

        private TableLayoutPanel pnlNotices;
        private Label lblNoticesCaption;
        private TextBox txtNotices;

        private TableLayoutPanel pnlBottom;
        private FlowLayoutPanel pnlSupport;
        private Button btnCopyDiagnostics;
        private Button btnOpenSettings;
        private Button btnOpenLogs;
        private FlowLayoutPanel pnlButtons;
        private Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Rebuilt to carry what an About box is actually required to carry.
        /// </summary>
        /// <remarks>
        /// It previously showed a version that could never change, a bare "© 2026" with no
        /// holder — which is not a copyright notice — and no third-party attributions at all,
        /// which is a legal requirement rather than a nicety. The absolute coordinates are gone
        /// with it; the body is a table so the content can grow without a coordinate going stale.
        /// Do not open in the Visual Studio designer.
        /// </remarks>
        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();
            lblHeaderSubtitle = new Label();

            pnlBody = new TableLayoutPanel();
            picIcon = new PictureBox();
            pnlIdentity = new TableLayoutPanel();
            lblAppName = new Label();
            lblEdition = new Label();
            lblVersion = new Label();
            lblDescription = new Label();
            lblCopyright = new Label();
            pnlLinks = new FlowLayoutPanel();
            lnkWebsite = new LinkLabel();
            lnkDocs = new LinkLabel();
            lnkReleaseNotes = new LinkLabel();
            lnkReportBug = new LinkLabel();
            lnkProject = new LinkLabel();

            pnlNotices = new TableLayoutPanel();
            lblNoticesCaption = new Label();
            txtNotices = new TextBox();

            pnlBottom = new TableLayoutPanel();
            pnlSupport = new FlowLayoutPanel();
            btnCopyDiagnostics = new Button();
            btnOpenSettings = new Button();
            btnOpenLogs = new Button();
            pnlButtons = new FlowLayoutPanel();
            btnClose = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            pnlIdentity.SuspendLayout();
            pnlLinks.SuspendLayout();
            pnlNotices.SuspendLayout();
            pnlBottom.SuspendLayout();
            pnlSupport.SuspendLayout();
            pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picIcon).BeginInit();
            SuspendLayout();
            //
            // pnlHeader
            //
            pnlHeader.AutoSize = true;
            pnlHeader.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlHeader.Controls.Add(lblHeaderSubtitle);
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(20, 14, 20, 12);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Dock = DockStyle.Top;
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "About CodeShuttle";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Dock = DockStyle.Top;
            lblHeaderSubtitle.Margin = new Padding(0, 4, 0, 0);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text = "Send your code to AI. Bring the answers back.";
            //
            // pnlBody
            //
            pnlBody.AutoSize = true;
            pnlBody.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlBody.ColumnCount = 2;
            pnlBody.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlBody.Controls.Add(picIcon, 0, 0);
            pnlBody.Controls.Add(pnlIdentity, 1, 0);
            pnlBody.Dock = DockStyle.Top;
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(20, 16, 20, 8);
            pnlBody.RowCount = 1;
            pnlBody.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlBody.TabIndex = 1;
            //
            // picIcon
            //
            picIcon.Margin = new Padding(0, 0, 16, 0);
            picIcon.Name = "picIcon";
            picIcon.Size = new Size(64, 64);
            picIcon.SizeMode = PictureBoxSizeMode.Zoom;
            picIcon.TabIndex = 0;
            picIcon.TabStop = false;
            picIcon.AccessibleName = "CodeShuttle icon";
            //
            // pnlIdentity
            //
            pnlIdentity.AutoSize = true;
            pnlIdentity.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlIdentity.ColumnCount = 1;
            pnlIdentity.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlIdentity.Controls.Add(lblAppName, 0, 0);
            pnlIdentity.Controls.Add(lblEdition, 0, 1);
            pnlIdentity.Controls.Add(lblVersion, 0, 2);
            pnlIdentity.Controls.Add(lblDescription, 0, 3);
            pnlIdentity.Controls.Add(lblCopyright, 0, 4);
            pnlIdentity.Controls.Add(pnlLinks, 0, 5);
            pnlIdentity.Dock = DockStyle.Fill;
            pnlIdentity.Margin = new Padding(0);
            pnlIdentity.Name = "pnlIdentity";
            pnlIdentity.RowCount = 6;
            for (int i = 0; i < 6; i++) pnlIdentity.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlIdentity.TabIndex = 1;
            //
            // lblAppName
            //
            lblAppName.AutoSize = true;
            lblAppName.Margin = new Padding(0);
            lblAppName.Name = "lblAppName";
            lblAppName.Text = "CodeShuttle";
            //
            // lblEdition
            //
            lblEdition.AutoSize = true;
            lblEdition.Margin = new Padding(0, 2, 0, 0);
            lblEdition.Name = "lblEdition";
            lblEdition.Text = "Standard edition";
            //
            // lblVersion
            //
            // Version, build and commit SHA all come from AppVersion, which reads the
            // informational version. The three places that used to read AssemblyVersion were
            // pinned at 1.0.0.0 forever.
            lblVersion.AutoSize = true;
            lblVersion.Margin = new Padding(0, 6, 0, 0);
            lblVersion.Name = "lblVersion";
            lblVersion.Text = "Version 0.0.0";
            //
            // lblDescription
            //
            lblDescription.AutoSize = true;
            lblDescription.Margin = new Padding(0, 10, 0, 0);
            lblDescription.MaximumSize = new Size(400, 0);
            lblDescription.Name = "lblDescription";
            lblDescription.Text =
                "Pack a codebase for an AI chat, then paste the reply back, diff it against disk and " +
                "apply what you accept. Filtering, presets, secret redaction and token budgeting " +
                "along the way.";
            //
            // lblCopyright
            //
            lblCopyright.AutoSize = true;
            lblCopyright.Margin = new Padding(0, 12, 0, 0);
            lblCopyright.Name = "lblCopyright";
            lblCopyright.Text = "© 2026";
            //
            // pnlLinks
            //
            pnlLinks.AutoSize = true;
            pnlLinks.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlLinks.Controls.Add(lnkWebsite);
            pnlLinks.Controls.Add(lnkDocs);
            pnlLinks.Controls.Add(lnkReleaseNotes);
            pnlLinks.Controls.Add(lnkReportBug);
            pnlLinks.Controls.Add(lnkProject);
            pnlLinks.Dock = DockStyle.Fill;
            pnlLinks.FlowDirection = FlowDirection.LeftToRight;
            pnlLinks.Margin = new Padding(0, 10, 0, 0);
            pnlLinks.MaximumSize = new Size(400, 0);
            pnlLinks.Name = "pnlLinks";
            pnlLinks.TabIndex = 0;
            pnlLinks.WrapContents = true;
            //
            // lnkWebsite
            //
            lnkWebsite.AutoSize = true;
            lnkWebsite.Margin = new Padding(0, 0, 14, 4);
            lnkWebsite.Name = "lnkWebsite";
            lnkWebsite.TabIndex = 0;
            lnkWebsite.TabStop = true;
            lnkWebsite.Text = "Website";
            lnkWebsite.AccessibleName = "Website";
            lnkWebsite.LinkClicked += LnkWebsite_LinkClicked;
            //
            // lnkDocs
            //
            lnkDocs.AutoSize = true;
            lnkDocs.Margin = new Padding(0, 0, 14, 4);
            lnkDocs.Name = "lnkDocs";
            lnkDocs.TabIndex = 1;
            lnkDocs.TabStop = true;
            lnkDocs.Text = "Documentation";
            lnkDocs.AccessibleName = "Documentation";
            lnkDocs.LinkClicked += LnkDocs_LinkClicked;
            //
            // lnkReleaseNotes
            //
            lnkReleaseNotes.AutoSize = true;
            lnkReleaseNotes.Margin = new Padding(0, 0, 14, 4);
            lnkReleaseNotes.Name = "lnkReleaseNotes";
            lnkReleaseNotes.TabIndex = 2;
            lnkReleaseNotes.TabStop = true;
            lnkReleaseNotes.Text = "Release notes";
            lnkReleaseNotes.AccessibleName = "Release notes";
            lnkReleaseNotes.LinkClicked += LnkReleaseNotes_LinkClicked;
            //
            // lnkReportBug
            //
            lnkReportBug.AutoSize = true;
            lnkReportBug.Margin = new Padding(0, 0, 14, 4);
            lnkReportBug.Name = "lnkReportBug";
            lnkReportBug.TabIndex = 3;
            lnkReportBug.TabStop = true;
            lnkReportBug.Text = "Report a bug";
            lnkReportBug.AccessibleName = "Report a bug";
            lnkReportBug.LinkClicked += LnkReportBug_LinkClicked;
            //
            // lnkProject
            //
            lnkProject.AutoSize = true;
            lnkProject.Margin = new Padding(0, 0, 0, 4);
            lnkProject.Name = "lnkProject";
            lnkProject.TabIndex = 4;
            lnkProject.TabStop = true;
            lnkProject.Text = "Licence";
            lnkProject.AccessibleName = "Licence";
            lnkProject.LinkClicked += LnkProject_LinkClicked;
            //
            // pnlNotices
            //
            pnlNotices.ColumnCount = 1;
            pnlNotices.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlNotices.Controls.Add(lblNoticesCaption, 0, 0);
            pnlNotices.Controls.Add(txtNotices, 0, 1);
            pnlNotices.Dock = DockStyle.Fill;
            pnlNotices.Name = "pnlNotices";
            pnlNotices.Padding = new Padding(20, 4, 20, 8);
            pnlNotices.RowCount = 2;
            pnlNotices.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlNotices.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlNotices.TabIndex = 2;
            //
            // lblNoticesCaption
            //
            lblNoticesCaption.AutoSize = true;
            lblNoticesCaption.Margin = new Padding(0, 0, 0, 4);
            lblNoticesCaption.Name = "lblNoticesCaption";
            lblNoticesCaption.Text = "&Third-party notices:";
            lblNoticesCaption.TabIndex = 0;
            //
            // txtNotices
            //
            // A read-only text box rather than a Label: attributions have to be selectable and
            // copyable to be of any use, and a Label announces nothing when its text changes.
            txtNotices.BorderStyle = BorderStyle.FixedSingle;
            txtNotices.Dock = DockStyle.Fill;
            txtNotices.Margin = new Padding(0);
            txtNotices.Multiline = true;
            txtNotices.Name = "txtNotices";
            txtNotices.ReadOnly = true;
            txtNotices.ScrollBars = ScrollBars.Vertical;
            txtNotices.TabIndex = 1;
            txtNotices.AccessibleName = "Third-party notices";
            //
            // pnlBottom
            //
            pnlBottom.AutoSize = true;
            pnlBottom.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlBottom.ColumnCount = 2;
            pnlBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlBottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlBottom.Controls.Add(pnlSupport, 0, 0);
            pnlBottom.Controls.Add(pnlButtons, 1, 0);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 6, 20, 12);
            pnlBottom.RowCount = 1;
            pnlBottom.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlBottom.TabIndex = 3;
            //
            // pnlSupport
            //
            pnlSupport.AutoSize = true;
            pnlSupport.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlSupport.Controls.Add(btnCopyDiagnostics);
            pnlSupport.Controls.Add(btnOpenSettings);
            pnlSupport.Controls.Add(btnOpenLogs);
            pnlSupport.Dock = DockStyle.Left;
            pnlSupport.FlowDirection = FlowDirection.LeftToRight;
            pnlSupport.Margin = new Padding(0);
            pnlSupport.Name = "pnlSupport";
            pnlSupport.TabIndex = 0;
            pnlSupport.WrapContents = false;
            //
            // btnCopyDiagnostics
            //
            // Environment details only. It must never carry a scanned path or any file content —
            // the natural next step after copying this is pasting it into a public issue.
            btnCopyDiagnostics.AutoSize = true;
            btnCopyDiagnostics.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnCopyDiagnostics.Cursor = Cursors.Hand;
            btnCopyDiagnostics.FlatAppearance.BorderSize = 0;
            btnCopyDiagnostics.FlatStyle = FlatStyle.Flat;
            btnCopyDiagnostics.Margin = new Padding(0, 0, 8, 0);
            btnCopyDiagnostics.MinimumSize = new Size(112, 30);
            btnCopyDiagnostics.Name = "btnCopyDiagnostics";
            btnCopyDiagnostics.Padding = new Padding(10, 4, 10, 4);
            btnCopyDiagnostics.TabIndex = 0;
            btnCopyDiagnostics.Text = "Copy &diagnostics";
            btnCopyDiagnostics.AccessibleName = "Copy diagnostics";
            btnCopyDiagnostics.AccessibleDescription =
                "Copies operating system, .NET, DPI and culture details for a support request.";
            btnCopyDiagnostics.UseVisualStyleBackColor = false;
            btnCopyDiagnostics.Click += BtnCopyDiagnostics_Click;
            //
            // btnOpenSettings
            //
            btnOpenSettings.AutoSize = true;
            btnOpenSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnOpenSettings.Cursor = Cursors.Hand;
            btnOpenSettings.FlatAppearance.BorderSize = 0;
            btnOpenSettings.FlatStyle = FlatStyle.Flat;
            btnOpenSettings.Margin = new Padding(0, 0, 8, 0);
            btnOpenSettings.MinimumSize = new Size(104, 30);
            btnOpenSettings.Name = "btnOpenSettings";
            btnOpenSettings.Padding = new Padding(10, 4, 10, 4);
            btnOpenSettings.TabIndex = 1;
            btnOpenSettings.Text = "&Settings folder";
            btnOpenSettings.AccessibleName = "Open the settings folder";
            btnOpenSettings.UseVisualStyleBackColor = false;
            btnOpenSettings.Click += BtnOpenSettings_Click;
            //
            // btnOpenLogs
            //
            btnOpenLogs.AutoSize = true;
            btnOpenLogs.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnOpenLogs.Cursor = Cursors.Hand;
            btnOpenLogs.FlatAppearance.BorderSize = 0;
            btnOpenLogs.FlatStyle = FlatStyle.Flat;
            btnOpenLogs.Margin = new Padding(0);
            btnOpenLogs.MinimumSize = new Size(88, 30);
            btnOpenLogs.Name = "btnOpenLogs";
            btnOpenLogs.Padding = new Padding(10, 4, 10, 4);
            btnOpenLogs.TabIndex = 2;
            btnOpenLogs.Text = "&Log folder";
            btnOpenLogs.AccessibleName = "Open the log folder";
            btnOpenLogs.UseVisualStyleBackColor = false;
            btnOpenLogs.Click += BtnOpenLogs_Click;
            //
            // pnlButtons
            //
            pnlButtons.AutoSize = true;
            pnlButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlButtons.Controls.Add(btnClose);
            pnlButtons.Dock = DockStyle.Right;
            pnlButtons.FlowDirection = FlowDirection.RightToLeft;
            pnlButtons.Margin = new Padding(0);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.TabIndex = 1;
            pnlButtons.WrapContents = false;
            //
            // btnClose
            //
            btnClose.AutoSize = true;
            btnClose.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnClose.Cursor = Cursors.Hand;
            // Escape returned DialogResult.OK, because CancelButton pointed at a button whose
            // DialogResult was OK. Dismissing a dialog is not consenting to it; both keys
            // now report Cancel, which is what a dismiss-only dialog should say.
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Margin = new Padding(8, 0, 0, 0);
            btnClose.MinimumSize = new Size(88, 30);
            btnClose.Name = "btnClose";
            btnClose.Padding = new Padding(10, 4, 10, 4);
            btnClose.TabIndex = 0;
            btnClose.Text = "Close";
            btnClose.AccessibleName = "Close";
            btnClose.UseVisualStyleBackColor = false;
            //
            // AboutForm
            //
            AcceptButton = btnClose;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new Size(560, 620);
            Controls.Add(pnlNotices);
            Controls.Add(pnlBottom);
            Controls.Add(pnlBody);
            Controls.Add(pnlHeader);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(560, 520);
            Name = "AboutForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About CodeShuttle";

            // Theme roles. Colours and fonts are resolved from ThemeTokens / ThemeFonts at
            // runtime; anything not listed here takes the default for its control type.
            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ThemeRoles.Set(lblHeaderTitle, ThemeRole.HeaderTitle, FontRole.Title);
            ThemeRoles.Set(lblHeaderSubtitle, ThemeRole.HeaderSubtitle, FontRole.BodyItalic);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlIdentity, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(lblAppName, FontRole.TitleLarge);
            ThemeRoles.Set(lblEdition, ThemeRole.TextSecondary, FontRole.Small);
            ThemeRoles.Set(lblVersion, ThemeRole.TextSecondary, FontRole.Body);
            ThemeRoles.Set(lblDescription, FontRole.Body);
            ThemeRoles.Set(lblCopyright, ThemeRole.TextSecondary, FontRole.Small);
            ThemeRoles.Set(pnlLinks, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(lnkWebsite, FontRole.Small);
            ThemeRoles.Set(lnkDocs, FontRole.Small);
            ThemeRoles.Set(lnkReleaseNotes, FontRole.Small);
            ThemeRoles.Set(lnkReportBug, FontRole.Small);
            ThemeRoles.Set(lnkProject, FontRole.Small);
            ThemeRoles.Set(pnlNotices, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(lblNoticesCaption, FontRole.SmallBold);
            ThemeRoles.Set(txtNotices, FontRole.MonoSmall);
            ThemeRoles.Set(pnlBottom, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlSupport, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlButtons, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(btnCopyDiagnostics, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnOpenSettings, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnOpenLogs, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnClose, ThemeRole.ButtonAccent, FontRole.BodyBold);

            HelpTopics.Set(this, HelpTopics.Troubleshooting);

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            pnlIdentity.ResumeLayout(false);
            pnlIdentity.PerformLayout();
            pnlLinks.ResumeLayout(false);
            pnlLinks.PerformLayout();
            pnlNotices.ResumeLayout(false);
            pnlNotices.PerformLayout();
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            pnlSupport.ResumeLayout(false);
            pnlSupport.PerformLayout();
            pnlButtons.ResumeLayout(false);
            pnlButtons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picIcon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
