using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class PasteResponseForm
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
        /// The inbound half of the round trip.
        /// </summary>
        /// <remarks>
        /// Written by hand against the same conventions as the rest of the product: docked rows
        /// and table panels rather than absolute coordinates, <c>AutoScaleDimensions (7F, 15F)</c>
        /// to match every other form, and colours from theme roles rather than literals. Do not
        /// open this in the Visual Studio designer — it resaves the scale metric at the local DPI
        /// and every coordinate in the file becomes wrong.
        /// </remarks>
        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();
            lblHeaderSubtitle = new Label();

            pnlBody = new TableLayoutPanel();
            lblResponse = new Label();
            txtResponse = new TextBox();

            pnlTarget = new TableLayoutPanel();
            lblTarget = new Label();
            pnlTargetRow = new TableLayoutPanel();
            txtTarget = new TextBox();
            btnBrowse = new Button();

            pnlFooter = new TableLayoutPanel();
            lblStatus = new Label();
            pnlButtons = new FlowLayoutPanel();
            btnReview = new Button();
            btnCancel = new Button();
            btnHelp = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            pnlTarget.SuspendLayout();
            pnlTargetRow.SuspendLayout();
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
            lblHeaderTitle.Text = "Paste AI response";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Dock = DockStyle.Top;
            lblHeaderSubtitle.Margin = new Padding(0, 4, 0, 0);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text =
                "Paste the reply from your AI chat. Every file is diffed against the folder you choose " +
                "before anything is written.";
            //
            // pnlBody
            //
            // Fill is added to the form first so that the docked rows around it resolve outward.
            // Label and input live inside this table so the label can still precede its input in
            // the Controls collection, which is what UI Automation reads the association from.
            pnlBody.ColumnCount = 1;
            pnlBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlBody.Controls.Add(lblResponse, 0, 0);
            pnlBody.Controls.Add(txtResponse, 0, 1);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(16, 4, 16, 8);
            pnlBody.RowCount = 2;
            pnlBody.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlBody.TabIndex = 1;
            //
            // lblResponse
            //
            lblResponse.AutoSize = true;
            lblResponse.Margin = new Padding(0, 0, 0, 4);
            lblResponse.Name = "lblResponse";
            lblResponse.Text = "AI &response:";
            lblResponse.TabIndex = 0;
            //
            // txtResponse
            //
            txtResponse.AcceptsReturn = true;
            txtResponse.AcceptsTab = true;
            txtResponse.BorderStyle = BorderStyle.FixedSingle;
            txtResponse.Dock = DockStyle.Fill;
            txtResponse.Margin = new Padding(0);
            txtResponse.Multiline = true;
            txtResponse.Name = "txtResponse";
            txtResponse.ScrollBars = ScrollBars.Both;
            txtResponse.TabIndex = 1;
            txtResponse.WordWrap = false;
            txtResponse.AccessibleName = "AI response";
            txtResponse.AccessibleDescription =
                "Paste the reply from your AI chat here. It is parsed as a CodeShuttle pack.";
            txtResponse.TextChanged += TxtResponse_TextChanged;
            //
            // pnlTarget
            //
            pnlTarget.AutoSize = true;
            pnlTarget.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlTarget.ColumnCount = 1;
            pnlTarget.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlTarget.Controls.Add(lblTarget, 0, 0);
            pnlTarget.Controls.Add(pnlTargetRow, 0, 1);
            pnlTarget.Dock = DockStyle.Bottom;
            pnlTarget.Name = "pnlTarget";
            pnlTarget.Padding = new Padding(16, 4, 16, 4);
            pnlTarget.RowCount = 2;
            pnlTarget.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlTarget.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlTarget.TabIndex = 2;
            //
            // lblTarget
            //
            lblTarget.AutoSize = true;
            lblTarget.Margin = new Padding(0, 0, 0, 4);
            lblTarget.Name = "lblTarget";
            lblTarget.Text = "Apply to &folder:";
            lblTarget.TabIndex = 0;
            //
            // pnlTargetRow
            //
            pnlTargetRow.AutoSize = true;
            pnlTargetRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlTargetRow.ColumnCount = 2;
            pnlTargetRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlTargetRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlTargetRow.Controls.Add(txtTarget, 0, 0);
            pnlTargetRow.Controls.Add(btnBrowse, 1, 0);
            pnlTargetRow.Dock = DockStyle.Top;
            pnlTargetRow.Margin = new Padding(0);
            pnlTargetRow.Name = "pnlTargetRow";
            pnlTargetRow.RowCount = 1;
            pnlTargetRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlTargetRow.TabIndex = 1;
            //
            // txtTarget
            //
            txtTarget.BorderStyle = BorderStyle.FixedSingle;
            txtTarget.Dock = DockStyle.Fill;
            txtTarget.Margin = new Padding(0, 0, 8, 0);
            txtTarget.Name = "txtTarget";
            txtTarget.TabIndex = 0;
            txtTarget.AccessibleName = "Apply to folder";
            txtTarget.AccessibleDescription =
                "The folder the pasted files are resolved against. Nothing is written outside it.";
            txtTarget.TextChanged += TxtTarget_TextChanged;
            //
            // btnBrowse
            //
            btnBrowse.AutoSize = true;
            btnBrowse.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnBrowse.Cursor = Cursors.Hand;
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.Margin = new Padding(0);
            btnBrowse.MinimumSize = new Size(88, 26);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Padding = new Padding(10, 3, 10, 3);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "&Browse…";
            btnBrowse.AccessibleName = "Browse for the target folder";
            btnBrowse.UseVisualStyleBackColor = false;
            btnBrowse.Click += BtnBrowse_Click;
            //
            // pnlFooter
            //
            pnlFooter.AutoSize = true;
            pnlFooter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlFooter.ColumnCount = 2;
            pnlFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlFooter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlFooter.Controls.Add(lblStatus, 0, 0);
            pnlFooter.Controls.Add(pnlButtons, 1, 0);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Padding = new Padding(16, 4, 16, 10);
            pnlFooter.RowCount = 1;
            pnlFooter.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlFooter.TabIndex = 3;
            //
            // lblStatus
            //
            lblStatus.AutoSize = true;
            lblStatus.Anchor = AnchorStyles.Left;
            lblStatus.Name = "lblStatus";
            lblStatus.Text = "";
            lblStatus.TabIndex = 0;
            lblStatus.AccessibleName = "Status";
            //
            // pnlButtons
            //
            pnlButtons.AutoSize = true;
            pnlButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlButtons.Controls.Add(btnReview);
            pnlButtons.Controls.Add(btnCancel);
            pnlButtons.Controls.Add(btnHelp);
            pnlButtons.Dock = DockStyle.Right;
            pnlButtons.FlowDirection = FlowDirection.RightToLeft;
            pnlButtons.Margin = new Padding(0);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.TabIndex = 1;
            pnlButtons.WrapContents = false;
            //
            // btnReview
            //
            btnReview.AutoSize = true;
            btnReview.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnReview.Cursor = Cursors.Hand;
            btnReview.Enabled = false;
            btnReview.FlatAppearance.BorderSize = 0;
            btnReview.FlatStyle = FlatStyle.Flat;
            btnReview.Margin = new Padding(8, 0, 0, 0);
            btnReview.MinimumSize = new Size(120, 30);
            btnReview.Name = "btnReview";
            btnReview.Padding = new Padding(10, 4, 10, 4);
            btnReview.TabIndex = 0;
            btnReview.Text = "Re&view changes";
            btnReview.AccessibleName = "Review changes";
            btnReview.AccessibleDescription =
                "Parse the response and open the diff. Nothing is written at this point.";
            btnReview.UseVisualStyleBackColor = false;
            btnReview.Click += BtnReview_Click;
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
            // PasteResponseForm
            //
            AcceptButton = btnReview;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(760, 520);
            Controls.Add(pnlBody);
            Controls.Add(pnlFooter);
            Controls.Add(pnlTarget);
            Controls.Add(pnlHeader);
            MinimizeBox = false;
            MinimumSize = new Size(560, 400);
            Name = "PasteResponseForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Paste AI response";

            // Theme roles. No colour literal appears in this file, by rule.
            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ThemeRoles.Set(lblHeaderTitle, ThemeRole.HeaderTitle, FontRole.Title);
            ThemeRoles.Set(lblHeaderSubtitle, ThemeRole.HeaderSubtitle, FontRole.Small);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(txtResponse, FontRole.Mono);
            ThemeRoles.Set(pnlTarget, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlTargetRow, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(btnBrowse, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(pnlFooter, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(lblStatus, ThemeRole.TextSecondary, FontRole.Small);
            ThemeRoles.Set(pnlButtons, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(btnReview, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(btnCancel, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnHelp, ThemeRole.ButtonSubtle, FontRole.BodyBold);

            // F1 anywhere in this window lands on the round-trip topic.
            HelpTopics.Set(this, HelpTopics.ApplyingAnswersBack);

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            pnlTarget.ResumeLayout(false);
            pnlTarget.PerformLayout();
            pnlTargetRow.ResumeLayout(false);
            pnlTargetRow.PerformLayout();
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
        private Label lblResponse;
        private TextBox txtResponse;
        private TableLayoutPanel pnlTarget;
        private Label lblTarget;
        private TableLayoutPanel pnlTargetRow;
        private TextBox txtTarget;
        private Button btnBrowse;
        private TableLayoutPanel pnlFooter;
        private Label lblStatus;
        private FlowLayoutPanel pnlButtons;
        private Button btnReview;
        private Button btnCancel;
        private Button btnHelp;
    }
}
