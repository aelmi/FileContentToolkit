using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class TokenBreakdownForm
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
            lblFiles = new Label();
            lstFiles = new ListView();
            colFile = new ColumnHeader();
            colTokens = new ColumnHeader();
            colShare = new ColumnHeader();

            pnlSuggestion = new Panel();
            lblSuggestion = new Label();

            pnlFooter = new TableLayoutPanel();
            lblCaveat = new Label();
            pnlButtons = new FlowLayoutPanel();
            btnClose = new Button();
            btnHelp = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            pnlSuggestion.SuspendLayout();
            pnlFooter.SuspendLayout();
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
            lblHeaderTitle.Text = "Token breakdown";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Dock = DockStyle.Top;
            lblHeaderSubtitle.Margin = new Padding(0, 4, 0, 0);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text = "";
            //
            // pnlBody
            //
            pnlBody.ColumnCount = 1;
            pnlBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlBody.Controls.Add(lblFiles, 0, 0);
            pnlBody.Controls.Add(lstFiles, 0, 1);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(16, 4, 16, 4);
            pnlBody.RowCount = 2;
            pnlBody.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlBody.TabIndex = 1;
            //
            // lblFiles
            //
            lblFiles.AutoSize = true;
            lblFiles.Margin = new Padding(0, 0, 0, 4);
            lblFiles.Name = "lblFiles";
            lblFiles.Text = "&Files, largest first:";
            lblFiles.TabIndex = 0;
            //
            // lstFiles
            //
            lstFiles.Columns.AddRange(new ColumnHeader[] { colFile, colTokens, colShare });
            lstFiles.Dock = DockStyle.Fill;
            lstFiles.FullRowSelect = true;
            lstFiles.HideSelection = false;
            lstFiles.Margin = new Padding(0);
            lstFiles.Name = "lstFiles";
            lstFiles.TabIndex = 1;
            lstFiles.UseCompatibleStateImageBehavior = false;
            lstFiles.View = View.Details;
            lstFiles.AccessibleName = "Per-file token estimate";
            //
            // colFile
            //
            colFile.Text = "File";
            colFile.Width = 400;
            //
            // colTokens
            //
            colTokens.Text = "Tokens";
            colTokens.TextAlign = HorizontalAlignment.Right;
            colTokens.Width = 100;
            //
            // colShare
            //
            colShare.Text = "Share";
            colShare.TextAlign = HorizontalAlignment.Right;
            colShare.Width = 80;
            //
            // pnlSuggestion
            //
            pnlSuggestion.AutoSize = true;
            pnlSuggestion.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlSuggestion.Controls.Add(lblSuggestion);
            pnlSuggestion.Dock = DockStyle.Bottom;
            pnlSuggestion.Name = "pnlSuggestion";
            pnlSuggestion.Padding = new Padding(16, 8, 16, 8);
            pnlSuggestion.TabIndex = 2;
            pnlSuggestion.Visible = false;
            //
            // lblSuggestion
            //
            lblSuggestion.AutoSize = true;
            lblSuggestion.Dock = DockStyle.Top;
            lblSuggestion.Name = "lblSuggestion";
            lblSuggestion.Text = "";
            lblSuggestion.AccessibleName = "Trim suggestion";
            //
            // pnlFooter
            //
            pnlFooter.AutoSize = true;
            pnlFooter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlFooter.ColumnCount = 2;
            pnlFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlFooter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlFooter.Controls.Add(lblCaveat, 0, 0);
            pnlFooter.Controls.Add(pnlButtons, 1, 0);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Padding = new Padding(16, 4, 16, 10);
            pnlFooter.RowCount = 1;
            pnlFooter.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlFooter.TabIndex = 3;
            //
            // lblCaveat
            //
            // The estimate is labelled as an estimate wherever it appears, by rule.
            lblCaveat.Anchor = AnchorStyles.Left;
            lblCaveat.AutoSize = true;
            lblCaveat.Name = "lblCaveat";
            lblCaveat.Text = "";
            lblCaveat.TabIndex = 0;
            lblCaveat.AccessibleName = "Estimate note";
            //
            // pnlButtons
            //
            pnlButtons.AutoSize = true;
            pnlButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlButtons.Controls.Add(btnClose);
            pnlButtons.Controls.Add(btnHelp);
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
            // Dismiss-only, so both keys report Cancel rather than implying consent to something.
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
            btnHelp.TabIndex = 1;
            btnHelp.Text = "?";
            btnHelp.AccessibleName = "Help for this dialog";
            btnHelp.UseVisualStyleBackColor = false;
            btnHelp.Click += BtnHelp_Click;
            //
            // TokenBreakdownForm
            //
            AcceptButton = btnClose;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new Size(720, 480);
            Controls.Add(pnlBody);
            Controls.Add(pnlFooter);
            Controls.Add(pnlSuggestion);
            Controls.Add(pnlHeader);
            MinimizeBox = false;
            MinimumSize = new Size(560, 380);
            Name = "TokenBreakdownForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Token breakdown";

            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ThemeRoles.Set(lblHeaderTitle, ThemeRole.HeaderTitle, FontRole.Title);
            ThemeRoles.Set(lblHeaderSubtitle, ThemeRole.HeaderSubtitle, FontRole.Small);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlSuggestion, ThemeRole.Banner);
            ThemeRoles.Set(lblSuggestion, ThemeRole.BannerText, FontRole.Body);
            ThemeRoles.Set(pnlFooter, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(lblCaveat, ThemeRole.TextSecondary, FontRole.Small);
            ThemeRoles.Set(pnlButtons, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(btnClose, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(btnHelp, ThemeRole.ButtonSubtle, FontRole.BodyBold);

            HelpTopics.Set(this, HelpTopics.BuildingThePack);

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            pnlSuggestion.ResumeLayout(false);
            pnlSuggestion.PerformLayout();
            pnlFooter.ResumeLayout(false);
            pnlFooter.PerformLayout();
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
        private Label lblFiles;
        private ListView lstFiles;
        private ColumnHeader colFile;
        private ColumnHeader colTokens;
        private ColumnHeader colShare;
        private Panel pnlSuggestion;
        private Label lblSuggestion;
        private TableLayoutPanel pnlFooter;
        private Label lblCaveat;
        private FlowLayoutPanel pnlButtons;
        private Button btnClose;
        private Button btnHelp;
    }
}
