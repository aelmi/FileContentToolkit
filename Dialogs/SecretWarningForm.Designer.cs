using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class SecretWarningForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Hand-written to the project conventions: docked rows, no absolute coordinates,
        /// <c>AutoScaleDimensions (7F, 15F)</c>, colours from theme roles only. Do not open in
        /// the Visual Studio designer.
        /// </summary>
        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();
            lblHeaderSubtitle = new Label();

            pnlBody = new TableLayoutPanel();
            lblMatches = new Label();
            lstMatches = new ListView();
            colFile = new ColumnHeader();
            colLine = new ColumnHeader();
            colKind = new ColumnHeader();
            colPreview = new ColumnHeader();

            pnlFooter = new TableLayoutPanel();
            pnlBulk = new FlowLayoutPanel();
            btnRedactAll = new Button();
            btnKeepAll = new Button();
            pnlButtons = new FlowLayoutPanel();
            btnContinue = new Button();
            btnCancel = new Button();
            btnHelp = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            pnlFooter.SuspendLayout();
            pnlBulk.SuspendLayout();
            pnlButtons.SuspendLayout();
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
            pnlHeader.Padding = new Padding(16, 12, 16, 12);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Dock = DockStyle.Top;
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "Possible credentials in this pack";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Dock = DockStyle.Top;
            lblHeaderSubtitle.Margin = new Padding(0, 4, 0, 0);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text =
                "Ticked entries are replaced with a redaction marker before the pack leaves this " +
                "window. Untick anything that is a test fixture or a placeholder.";
            //
            // pnlBody
            //
            pnlBody.ColumnCount = 1;
            pnlBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlBody.Controls.Add(lblMatches, 0, 0);
            pnlBody.Controls.Add(lstMatches, 0, 1);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(16, 4, 16, 8);
            pnlBody.RowCount = 2;
            pnlBody.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlBody.TabIndex = 1;
            //
            // lblMatches
            //
            lblMatches.AutoSize = true;
            lblMatches.Margin = new Padding(0, 0, 0, 4);
            lblMatches.Name = "lblMatches";
            lblMatches.Text = "&Detected values:";
            lblMatches.TabIndex = 0;
            //
            // lstMatches
            //
            // The value itself is never shown in full — only a four-character prefix and a length.
            // A dialog whose purpose is to stop credentials being pasted somewhere should not
            // start by printing them on screen.
            lstMatches.CheckBoxes = true;
            lstMatches.Columns.AddRange(new ColumnHeader[] { colFile, colLine, colKind, colPreview });
            lstMatches.Dock = DockStyle.Fill;
            lstMatches.FullRowSelect = true;
            lstMatches.HideSelection = false;
            lstMatches.Margin = new Padding(0);
            lstMatches.Name = "lstMatches";
            lstMatches.TabIndex = 1;
            lstMatches.UseCompatibleStateImageBehavior = false;
            lstMatches.View = View.Details;
            lstMatches.AccessibleName = "Detected credentials";
            lstMatches.AccessibleDescription =
                "Tick an entry to redact it. Values are shown masked.";
            //
            // colFile
            //
            colFile.Text = "File";
            colFile.Width = 260;
            //
            // colLine
            //
            colLine.Text = "Line";
            colLine.TextAlign = HorizontalAlignment.Right;
            colLine.Width = 56;
            //
            // colKind
            //
            colKind.Text = "Kind";
            colKind.Width = 180;
            //
            // colPreview
            //
            colPreview.Text = "Value (masked)";
            colPreview.Width = 160;
            //
            // pnlFooter
            //
            pnlFooter.AutoSize = true;
            pnlFooter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlFooter.ColumnCount = 2;
            pnlFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlFooter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlFooter.Controls.Add(pnlBulk, 0, 0);
            pnlFooter.Controls.Add(pnlButtons, 1, 0);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Padding = new Padding(16, 4, 16, 10);
            pnlFooter.RowCount = 1;
            pnlFooter.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlFooter.TabIndex = 2;
            //
            // pnlBulk
            //
            pnlBulk.AutoSize = true;
            pnlBulk.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlBulk.Controls.Add(btnRedactAll);
            pnlBulk.Controls.Add(btnKeepAll);
            pnlBulk.Dock = DockStyle.Left;
            pnlBulk.FlowDirection = FlowDirection.LeftToRight;
            pnlBulk.Margin = new Padding(0);
            pnlBulk.Name = "pnlBulk";
            pnlBulk.TabIndex = 0;
            pnlBulk.WrapContents = false;
            //
            // btnRedactAll
            //
            btnRedactAll.AutoSize = true;
            btnRedactAll.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnRedactAll.Cursor = Cursors.Hand;
            btnRedactAll.FlatAppearance.BorderSize = 0;
            btnRedactAll.FlatStyle = FlatStyle.Flat;
            btnRedactAll.Margin = new Padding(0, 0, 8, 0);
            btnRedactAll.MinimumSize = new Size(88, 30);
            btnRedactAll.Name = "btnRedactAll";
            btnRedactAll.Padding = new Padding(10, 4, 10, 4);
            btnRedactAll.TabIndex = 0;
            btnRedactAll.Text = "Redact &all";
            btnRedactAll.AccessibleName = "Redact all detected values";
            btnRedactAll.UseVisualStyleBackColor = false;
            btnRedactAll.Click += BtnRedactAll_Click;
            //
            // btnKeepAll
            //
            btnKeepAll.AutoSize = true;
            btnKeepAll.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnKeepAll.Cursor = Cursors.Hand;
            btnKeepAll.FlatAppearance.BorderSize = 0;
            btnKeepAll.FlatStyle = FlatStyle.Flat;
            btnKeepAll.Margin = new Padding(0);
            btnKeepAll.MinimumSize = new Size(88, 30);
            btnKeepAll.Name = "btnKeepAll";
            btnKeepAll.Padding = new Padding(10, 4, 10, 4);
            btnKeepAll.TabIndex = 1;
            btnKeepAll.Text = "&Keep all";
            btnKeepAll.AccessibleName = "Keep all detected values unredacted";
            btnKeepAll.UseVisualStyleBackColor = false;
            btnKeepAll.Click += BtnKeepAll_Click;
            //
            // pnlButtons
            //
            pnlButtons.AutoSize = true;
            pnlButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlButtons.Controls.Add(btnContinue);
            pnlButtons.Controls.Add(btnCancel);
            pnlButtons.Controls.Add(btnHelp);
            pnlButtons.Dock = DockStyle.Right;
            pnlButtons.FlowDirection = FlowDirection.RightToLeft;
            pnlButtons.Margin = new Padding(0);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.TabIndex = 1;
            pnlButtons.WrapContents = false;
            //
            // btnContinue
            //
            btnContinue.AutoSize = true;
            btnContinue.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnContinue.Cursor = Cursors.Hand;
            btnContinue.DialogResult = DialogResult.OK;
            btnContinue.FlatAppearance.BorderSize = 0;
            btnContinue.FlatStyle = FlatStyle.Flat;
            btnContinue.Margin = new Padding(8, 0, 0, 0);
            btnContinue.MinimumSize = new Size(104, 30);
            btnContinue.Name = "btnContinue";
            btnContinue.Padding = new Padding(10, 4, 10, 4);
            btnContinue.TabIndex = 0;
            btnContinue.Text = "C&ontinue";
            btnContinue.AccessibleName = "Continue with the chosen redactions";
            btnContinue.UseVisualStyleBackColor = false;
            //
            // btnCancel
            //
            btnCancel.AutoSize = true;
            btnCancel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Margin = new Padding(8, 0, 0, 0);
            btnCancel.MinimumSize = new Size(88, 30);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(10, 4, 10, 4);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.AccessibleName = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            //
            // btnHelp
            //
            btnHelp.AutoSize = true;
            btnHelp.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnHelp.Cursor = Cursors.Hand;
            btnHelp.FlatAppearance.BorderSize = 0;
            btnHelp.FlatStyle = FlatStyle.Flat;
            btnHelp.Margin = new Padding(8, 0, 0, 0);
            btnHelp.MinimumSize = new Size(34, 30);
            btnHelp.Name = "btnHelp";
            btnHelp.TabIndex = 2;
            btnHelp.Text = "?";
            btnHelp.AccessibleName = "Help for this dialog";
            btnHelp.UseVisualStyleBackColor = false;
            btnHelp.Click += BtnHelp_Click;
            //
            // SecretWarningForm
            //
            AcceptButton = btnContinue;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(760, 440);
            Controls.Add(pnlBody);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            MinimizeBox = false;
            MinimumSize = new Size(600, 360);
            Name = "SecretWarningForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Possible credentials";

            ThemeRoles.Set(pnlHeader, ThemeRole.Banner);
            ThemeRoles.Set(lblHeaderTitle, ThemeRole.BannerText, FontRole.Title);
            ThemeRoles.Set(lblHeaderSubtitle, ThemeRole.BannerText, FontRole.Small);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlFooter, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlBulk, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlButtons, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(btnRedactAll, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnKeepAll, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnContinue, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(btnCancel, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnHelp, ThemeRole.ButtonSubtle, FontRole.BodyBold);

            HelpTopics.Set(this, HelpTopics.BuildingThePack);

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            pnlFooter.ResumeLayout(false);
            pnlFooter.PerformLayout();
            pnlBulk.ResumeLayout(false);
            pnlBulk.PerformLayout();
            pnlButtons.ResumeLayout(false);
            pnlButtons.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;
        private TableLayoutPanel pnlBody;
        private Label lblMatches;
        private ListView lstMatches;
        private ColumnHeader colFile;
        private ColumnHeader colLine;
        private ColumnHeader colKind;
        private ColumnHeader colPreview;
        private TableLayoutPanel pnlFooter;
        private FlowLayoutPanel pnlBulk;
        private Button btnRedactAll;
        private Button btnKeepAll;
        private FlowLayoutPanel pnlButtons;
        private Button btnContinue;
        private Button btnCancel;
        private Button btnHelp;
    }
}
