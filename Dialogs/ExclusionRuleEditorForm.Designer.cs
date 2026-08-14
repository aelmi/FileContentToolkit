using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class ExclusionRuleEditorForm
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
            lblRules = new Label();
            lstRules = new ListView();
            colRule = new ColumnHeader();
            colExcludes = new ColumnHeader();

            pnlEdit = new TableLayoutPanel();
            lblNewRule = new Label();
            pnlEditRow = new TableLayoutPanel();
            txtNewRule = new TextBox();
            btnAddRule = new Button();
            btnRemoveRule = new Button();

            pnlTest = new TableLayoutPanel();
            lblTest = new Label();
            txtTest = new TextBox();
            lblTestResult = new Label();

            pnlFooter = new TableLayoutPanel();
            lblSummary = new Label();
            pnlButtons = new FlowLayoutPanel();
            btnOk = new Button();
            btnCancel = new Button();
            btnHelp = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            pnlEdit.SuspendLayout();
            pnlEditRow.SuspendLayout();
            pnlTest.SuspendLayout();
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
            lblHeaderTitle.Text = "Exclusion rules";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Dock = DockStyle.Top;
            lblHeaderSubtitle.Margin = new Padding(0, 4, 0, 0);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text =
                "One rule per row, with what each removes from the current scan. Globs: *.tmp, bin/, " +
                "docs/notes.md, **/generated/*.";
            //
            // pnlBody
            //
            pnlBody.ColumnCount = 1;
            pnlBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlBody.Controls.Add(lblRules, 0, 0);
            pnlBody.Controls.Add(lstRules, 0, 1);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(16, 4, 16, 4);
            pnlBody.RowCount = 2;
            pnlBody.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlBody.TabIndex = 1;
            //
            // lblRules
            //
            lblRules.AutoSize = true;
            lblRules.Margin = new Padding(0, 0, 0, 4);
            lblRules.Name = "lblRules";
            lblRules.Text = "&Rules:";
            lblRules.TabIndex = 0;
            //
            // lstRules
            //
            lstRules.Columns.AddRange(new ColumnHeader[] { colRule, colExcludes });
            lstRules.Dock = DockStyle.Fill;
            lstRules.FullRowSelect = true;
            lstRules.HideSelection = false;
            lstRules.Margin = new Padding(0);
            lstRules.MultiSelect = false;
            lstRules.Name = "lstRules";
            lstRules.TabIndex = 1;
            lstRules.UseCompatibleStateImageBehavior = false;
            lstRules.View = View.Details;
            lstRules.AccessibleName = "Exclusion rules";
            lstRules.AccessibleDescription =
                "Each row shows a rule and how many of the current candidate files it removes.";
            lstRules.SelectedIndexChanged += LstRules_SelectedIndexChanged;
            //
            // colRule
            //
            colRule.Text = "Rule";
            colRule.Width = 380;
            //
            // colExcludes
            //
            // The whole reason this editor exists: the matching semantics changed, so a user
            // whose patterns were written against substring matching needs to see what they now
            // do rather than discover it as a missing file.
            colExcludes.Text = "Excludes";
            colExcludes.Width = 200;
            //
            // pnlEdit
            //
            pnlEdit.AutoSize = true;
            pnlEdit.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlEdit.ColumnCount = 1;
            pnlEdit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlEdit.Controls.Add(lblNewRule, 0, 0);
            pnlEdit.Controls.Add(pnlEditRow, 0, 1);
            pnlEdit.Dock = DockStyle.Bottom;
            pnlEdit.Name = "pnlEdit";
            pnlEdit.Padding = new Padding(16, 4, 16, 4);
            pnlEdit.RowCount = 2;
            pnlEdit.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlEdit.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlEdit.TabIndex = 2;
            //
            // lblNewRule
            //
            lblNewRule.AutoSize = true;
            lblNewRule.Margin = new Padding(0, 0, 0, 4);
            lblNewRule.Name = "lblNewRule";
            lblNewRule.Text = "&New rule:";
            lblNewRule.TabIndex = 0;
            //
            // pnlEditRow
            //
            pnlEditRow.AutoSize = true;
            pnlEditRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlEditRow.ColumnCount = 3;
            pnlEditRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlEditRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlEditRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlEditRow.Controls.Add(txtNewRule, 0, 0);
            pnlEditRow.Controls.Add(btnAddRule, 1, 0);
            pnlEditRow.Controls.Add(btnRemoveRule, 2, 0);
            pnlEditRow.Dock = DockStyle.Top;
            pnlEditRow.Margin = new Padding(0);
            pnlEditRow.Name = "pnlEditRow";
            pnlEditRow.RowCount = 1;
            pnlEditRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlEditRow.TabIndex = 1;
            //
            // txtNewRule
            //
            txtNewRule.BorderStyle = BorderStyle.FixedSingle;
            txtNewRule.Dock = DockStyle.Fill;
            txtNewRule.Margin = new Padding(0, 0, 8, 0);
            txtNewRule.Name = "txtNewRule";
            txtNewRule.TabIndex = 0;
            txtNewRule.AccessibleName = "New rule";
            txtNewRule.KeyDown += TxtNewRule_KeyDown;
            //
            // btnAddRule
            //
            btnAddRule.AutoSize = true;
            btnAddRule.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnAddRule.Cursor = Cursors.Hand;
            btnAddRule.FlatAppearance.BorderSize = 0;
            btnAddRule.FlatStyle = FlatStyle.Flat;
            btnAddRule.Margin = new Padding(0, 0, 8, 0);
            btnAddRule.MinimumSize = new Size(88, 26);
            btnAddRule.Name = "btnAddRule";
            btnAddRule.Padding = new Padding(10, 3, 10, 3);
            btnAddRule.TabIndex = 1;
            btnAddRule.Text = "A&dd";
            btnAddRule.AccessibleName = "Add this rule";
            btnAddRule.UseVisualStyleBackColor = false;
            btnAddRule.Click += BtnAddRule_Click;
            //
            // btnRemoveRule
            //
            btnRemoveRule.AutoSize = true;
            btnRemoveRule.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnRemoveRule.Cursor = Cursors.Hand;
            btnRemoveRule.Enabled = false;
            btnRemoveRule.FlatAppearance.BorderSize = 0;
            btnRemoveRule.FlatStyle = FlatStyle.Flat;
            btnRemoveRule.Margin = new Padding(0);
            btnRemoveRule.MinimumSize = new Size(88, 26);
            btnRemoveRule.Name = "btnRemoveRule";
            btnRemoveRule.Padding = new Padding(10, 3, 10, 3);
            btnRemoveRule.TabIndex = 2;
            btnRemoveRule.Text = "Re&move";
            btnRemoveRule.AccessibleName = "Remove the selected rule";
            btnRemoveRule.UseVisualStyleBackColor = false;
            btnRemoveRule.Click += BtnRemoveRule_Click;
            //
            // pnlTest
            //
            pnlTest.AutoSize = true;
            pnlTest.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlTest.ColumnCount = 1;
            pnlTest.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlTest.Controls.Add(lblTest, 0, 0);
            pnlTest.Controls.Add(txtTest, 0, 1);
            pnlTest.Controls.Add(lblTestResult, 0, 2);
            pnlTest.Dock = DockStyle.Bottom;
            pnlTest.Name = "pnlTest";
            pnlTest.Padding = new Padding(16, 4, 16, 4);
            pnlTest.RowCount = 3;
            pnlTest.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlTest.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlTest.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlTest.TabIndex = 3;
            //
            // lblTest
            //
            lblTest.AutoSize = true;
            lblTest.Margin = new Padding(0, 0, 0, 4);
            lblTest.Name = "lblTest";
            lblTest.Text = "&Test a path against these rules:";
            lblTest.TabIndex = 0;
            //
            // txtTest
            //
            txtTest.BorderStyle = BorderStyle.FixedSingle;
            txtTest.Dock = DockStyle.Fill;
            txtTest.Margin = new Padding(0, 0, 0, 4);
            txtTest.Name = "txtTest";
            txtTest.TabIndex = 1;
            txtTest.AccessibleName = "Test a path";
            txtTest.AccessibleDescription =
                "Type a path relative to the scan root to see whether the rules exclude it.";
            txtTest.TextChanged += TxtTest_TextChanged;
            //
            // lblTestResult
            //
            lblTestResult.AutoSize = true;
            lblTestResult.Margin = new Padding(0);
            lblTestResult.Name = "lblTestResult";
            lblTestResult.Text = "";
            lblTestResult.TabIndex = 2;
            lblTestResult.AccessibleName = "Test result";
            //
            // pnlFooter
            //
            pnlFooter.AutoSize = true;
            pnlFooter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlFooter.ColumnCount = 2;
            pnlFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlFooter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlFooter.Controls.Add(lblSummary, 0, 0);
            pnlFooter.Controls.Add(pnlButtons, 1, 0);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Padding = new Padding(16, 4, 16, 10);
            pnlFooter.RowCount = 1;
            pnlFooter.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlFooter.TabIndex = 4;
            //
            // lblSummary
            //
            lblSummary.AutoSize = true;
            lblSummary.Anchor = AnchorStyles.Left;
            lblSummary.Name = "lblSummary";
            lblSummary.Text = "";
            lblSummary.TabIndex = 0;
            lblSummary.AccessibleName = "Rule summary";
            //
            // pnlButtons
            //
            pnlButtons.AutoSize = true;
            pnlButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlButtons.Controls.Add(btnOk);
            pnlButtons.Controls.Add(btnCancel);
            pnlButtons.Controls.Add(btnHelp);
            pnlButtons.Dock = DockStyle.Right;
            pnlButtons.FlowDirection = FlowDirection.RightToLeft;
            pnlButtons.Margin = new Padding(0);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.TabIndex = 1;
            pnlButtons.WrapContents = false;
            //
            // btnOk
            //
            btnOk.AutoSize = true;
            btnOk.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnOk.Cursor = Cursors.Hand;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Margin = new Padding(8, 0, 0, 0);
            btnOk.MinimumSize = new Size(88, 30);
            btnOk.Name = "btnOk";
            btnOk.Padding = new Padding(10, 4, 10, 4);
            btnOk.TabIndex = 0;
            btnOk.Text = "OK";
            btnOk.AccessibleName = "Apply these rules";
            btnOk.UseVisualStyleBackColor = false;
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
            // ExclusionRuleEditorForm
            //
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(720, 560);
            Controls.Add(pnlBody);
            Controls.Add(pnlFooter);
            Controls.Add(pnlTest);
            Controls.Add(pnlEdit);
            Controls.Add(pnlHeader);
            MinimizeBox = false;
            MinimumSize = new Size(600, 460);
            Name = "ExclusionRuleEditorForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Exclusion rules";

            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ThemeRoles.Set(lblHeaderTitle, ThemeRole.HeaderTitle, FontRole.Title);
            ThemeRoles.Set(lblHeaderSubtitle, ThemeRole.HeaderSubtitle, FontRole.Small);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlEdit, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlEditRow, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlTest, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlFooter, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlButtons, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(txtNewRule, FontRole.Mono);
            ThemeRoles.Set(txtTest, FontRole.Mono);
            ThemeRoles.Set(lblTestResult, ThemeRole.TextSecondary, FontRole.Small);
            ThemeRoles.Set(lblSummary, ThemeRole.TextSecondary, FontRole.Small);
            ThemeRoles.Set(btnAddRule, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnRemoveRule, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnOk, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(btnCancel, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnHelp, ThemeRole.ButtonSubtle, FontRole.BodyBold);

            HelpTopics.Set(this, HelpTopics.SelectingFiles);

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            pnlEdit.ResumeLayout(false);
            pnlEdit.PerformLayout();
            pnlEditRow.ResumeLayout(false);
            pnlEditRow.PerformLayout();
            pnlTest.ResumeLayout(false);
            pnlTest.PerformLayout();
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
        private Label lblRules;
        private ListView lstRules;
        private ColumnHeader colRule;
        private ColumnHeader colExcludes;
        private TableLayoutPanel pnlEdit;
        private Label lblNewRule;
        private TableLayoutPanel pnlEditRow;
        private TextBox txtNewRule;
        private Button btnAddRule;
        private Button btnRemoveRule;
        private TableLayoutPanel pnlTest;
        private Label lblTest;
        private TextBox txtTest;
        private Label lblTestResult;
        private TableLayoutPanel pnlFooter;
        private Label lblSummary;
        private FlowLayoutPanel pnlButtons;
        private Button btnOk;
        private Button btnCancel;
        private Button btnHelp;
    }
}
