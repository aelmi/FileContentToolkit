using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class PromptComposerForm
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
            lblTemplates = new Label();
            lstTemplates = new ListBox();
            lblBody = new Label();
            txtBody = new TextBox();

            pnlQuestion = new TableLayoutPanel();
            lblQuestion = new Label();
            txtQuestion = new TextBox();

            pnlFooter = new TableLayoutPanel();
            pnlManage = new FlowLayoutPanel();
            btnNew = new Button();
            btnDelete = new Button();
            btnReset = new Button();
            pnlButtons = new FlowLayoutPanel();
            btnCopy = new Button();
            btnCancel = new Button();
            btnHelp = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            pnlQuestion.SuspendLayout();
            pnlFooter.SuspendLayout();
            pnlManage.SuspendLayout();
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
            lblHeaderTitle.Text = "Copy as prompt";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Dock = DockStyle.Top;
            lblHeaderSubtitle.Margin = new Padding(0, 4, 0, 0);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text =
                "Wraps the pack in a template and copies it. Use {files} and {question} in your own " +
                "templates; leave a built-in's body empty to keep its supplied wording.";
            //
            // pnlBody
            //
            pnlBody.ColumnCount = 2;
            pnlBody.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            pnlBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlBody.Controls.Add(lblTemplates, 0, 0);
            pnlBody.Controls.Add(lblBody, 1, 0);
            pnlBody.Controls.Add(lstTemplates, 0, 1);
            pnlBody.Controls.Add(txtBody, 1, 1);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(16, 4, 16, 4);
            pnlBody.RowCount = 2;
            pnlBody.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlBody.TabIndex = 1;
            //
            // lblTemplates
            //
            lblTemplates.AutoSize = true;
            lblTemplates.Margin = new Padding(0, 0, 8, 4);
            lblTemplates.Name = "lblTemplates";
            lblTemplates.Text = "&Templates:";
            lblTemplates.TabIndex = 0;
            //
            // lstTemplates
            //
            lstTemplates.BorderStyle = BorderStyle.FixedSingle;
            lstTemplates.Dock = DockStyle.Fill;
            lstTemplates.IntegralHeight = false;
            lstTemplates.Margin = new Padding(0, 0, 8, 0);
            lstTemplates.Name = "lstTemplates";
            lstTemplates.TabIndex = 1;
            lstTemplates.AccessibleName = "Templates";
            lstTemplates.SelectedIndexChanged += LstTemplates_SelectedIndexChanged;
            //
            // lblBody
            //
            lblBody.AutoSize = true;
            lblBody.Margin = new Padding(0, 0, 0, 4);
            lblBody.Name = "lblBody";
            lblBody.Text = "Template &body:";
            lblBody.TabIndex = 2;
            //
            // txtBody
            //
            txtBody.AcceptsReturn = true;
            txtBody.BorderStyle = BorderStyle.FixedSingle;
            txtBody.Dock = DockStyle.Fill;
            txtBody.Margin = new Padding(0);
            txtBody.Multiline = true;
            txtBody.Name = "txtBody";
            txtBody.ScrollBars = ScrollBars.Vertical;
            txtBody.TabIndex = 3;
            txtBody.AccessibleName = "Template body";
            txtBody.AccessibleDescription =
                "The prompt wrapper. {files} is replaced with the pack and {question} with your question.";
            txtBody.TextChanged += TxtBody_TextChanged;
            //
            // pnlQuestion
            //
            pnlQuestion.AutoSize = true;
            pnlQuestion.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlQuestion.ColumnCount = 1;
            pnlQuestion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlQuestion.Controls.Add(lblQuestion, 0, 0);
            pnlQuestion.Controls.Add(txtQuestion, 0, 1);
            pnlQuestion.Dock = DockStyle.Bottom;
            pnlQuestion.Name = "pnlQuestion";
            pnlQuestion.Padding = new Padding(16, 4, 16, 4);
            pnlQuestion.RowCount = 2;
            pnlQuestion.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlQuestion.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlQuestion.TabIndex = 2;
            //
            // lblQuestion
            //
            lblQuestion.AutoSize = true;
            lblQuestion.Margin = new Padding(0, 0, 0, 4);
            lblQuestion.Name = "lblQuestion";
            lblQuestion.Text = "Your &question:";
            lblQuestion.TabIndex = 0;
            //
            // txtQuestion
            //
            // The parameter the two built-in prompt builders always accepted and never received.
            txtQuestion.BorderStyle = BorderStyle.FixedSingle;
            txtQuestion.Dock = DockStyle.Fill;
            txtQuestion.Margin = new Padding(0);
            txtQuestion.Name = "txtQuestion";
            txtQuestion.TabIndex = 1;
            txtQuestion.AccessibleName = "Your question";
            txtQuestion.AccessibleDescription =
                "Sent with the files. Left blank, a generic instruction is used instead.";
            //
            // pnlFooter
            //
            pnlFooter.AutoSize = true;
            pnlFooter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlFooter.ColumnCount = 2;
            pnlFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlFooter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlFooter.Controls.Add(pnlManage, 0, 0);
            pnlFooter.Controls.Add(pnlButtons, 1, 0);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Padding = new Padding(16, 4, 16, 10);
            pnlFooter.RowCount = 1;
            pnlFooter.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlFooter.TabIndex = 3;
            //
            // pnlManage
            //
            pnlManage.AutoSize = true;
            pnlManage.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlManage.Controls.Add(btnNew);
            pnlManage.Controls.Add(btnDelete);
            pnlManage.Controls.Add(btnReset);
            pnlManage.Dock = DockStyle.Left;
            pnlManage.FlowDirection = FlowDirection.LeftToRight;
            pnlManage.Margin = new Padding(0);
            pnlManage.Name = "pnlManage";
            pnlManage.TabIndex = 0;
            pnlManage.WrapContents = false;
            //
            // btnNew
            //
            btnNew.AutoSize = true;
            btnNew.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnNew.Cursor = Cursors.Hand;
            btnNew.FlatAppearance.BorderSize = 0;
            btnNew.FlatStyle = FlatStyle.Flat;
            btnNew.Margin = new Padding(0, 0, 8, 0);
            btnNew.MinimumSize = new Size(80, 30);
            btnNew.Name = "btnNew";
            btnNew.Padding = new Padding(10, 4, 10, 4);
            btnNew.TabIndex = 0;
            btnNew.Text = "&New…";
            btnNew.AccessibleName = "New template";
            btnNew.UseVisualStyleBackColor = false;
            btnNew.Click += BtnNew_Click;
            //
            // btnDelete
            //
            btnDelete.AutoSize = true;
            btnDelete.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Margin = new Padding(0, 0, 8, 0);
            btnDelete.MinimumSize = new Size(80, 30);
            btnDelete.Name = "btnDelete";
            btnDelete.Padding = new Padding(10, 4, 10, 4);
            btnDelete.TabIndex = 1;
            btnDelete.Text = "De&lete";
            btnDelete.AccessibleName = "Delete the selected template";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += BtnDelete_Click;
            //
            // btnReset
            //
            btnReset.AutoSize = true;
            btnReset.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnReset.Cursor = Cursors.Hand;
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Margin = new Padding(0);
            btnReset.MinimumSize = new Size(112, 30);
            btnReset.Name = "btnReset";
            btnReset.Padding = new Padding(10, 4, 10, 4);
            btnReset.TabIndex = 2;
            btnReset.Text = "&Restore built-ins";
            btnReset.AccessibleName = "Restore the built-in templates";
            btnReset.UseVisualStyleBackColor = false;
            btnReset.Click += BtnReset_Click;
            //
            // pnlButtons
            //
            pnlButtons.AutoSize = true;
            pnlButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlButtons.Controls.Add(btnCopy);
            pnlButtons.Controls.Add(btnCancel);
            pnlButtons.Controls.Add(btnHelp);
            pnlButtons.Dock = DockStyle.Right;
            pnlButtons.FlowDirection = FlowDirection.RightToLeft;
            pnlButtons.Margin = new Padding(0);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.TabIndex = 1;
            pnlButtons.WrapContents = false;
            //
            // btnCopy
            //
            btnCopy.AutoSize = true;
            btnCopy.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnCopy.Cursor = Cursors.Hand;
            btnCopy.DialogResult = DialogResult.OK;
            btnCopy.FlatAppearance.BorderSize = 0;
            btnCopy.FlatStyle = FlatStyle.Flat;
            btnCopy.Margin = new Padding(8, 0, 0, 0);
            btnCopy.MinimumSize = new Size(96, 30);
            btnCopy.Name = "btnCopy";
            btnCopy.Padding = new Padding(10, 4, 10, 4);
            btnCopy.TabIndex = 0;
            btnCopy.Text = "&Copy";
            btnCopy.AccessibleName = "Copy the composed prompt";
            btnCopy.UseVisualStyleBackColor = false;
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
            // PromptComposerForm
            //
            AcceptButton = btnCopy;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(760, 520);
            Controls.Add(pnlBody);
            Controls.Add(pnlFooter);
            Controls.Add(pnlQuestion);
            Controls.Add(pnlHeader);
            MinimizeBox = false;
            MinimumSize = new Size(620, 420);
            Name = "PromptComposerForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Copy as prompt";

            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ThemeRoles.Set(lblHeaderTitle, ThemeRole.HeaderTitle, FontRole.Title);
            ThemeRoles.Set(lblHeaderSubtitle, ThemeRole.HeaderSubtitle, FontRole.Small);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlQuestion, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlFooter, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlManage, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlButtons, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(txtBody, FontRole.Mono);
            ThemeRoles.Set(btnNew, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnDelete, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnReset, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnCopy, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(btnCancel, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnHelp, ThemeRole.ButtonSubtle, FontRole.BodyBold);

            HelpTopics.Set(this, HelpTopics.BuildingThePack);

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            pnlQuestion.ResumeLayout(false);
            pnlQuestion.PerformLayout();
            pnlFooter.ResumeLayout(false);
            pnlFooter.PerformLayout();
            pnlManage.ResumeLayout(false);
            pnlManage.PerformLayout();
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
        private Label lblTemplates;
        private ListBox lstTemplates;
        private Label lblBody;
        private TextBox txtBody;
        private TableLayoutPanel pnlQuestion;
        private Label lblQuestion;
        private TextBox txtQuestion;
        private TableLayoutPanel pnlFooter;
        private FlowLayoutPanel pnlManage;
        private Button btnNew;
        private Button btnDelete;
        private Button btnReset;
        private FlowLayoutPanel pnlButtons;
        private Button btnCopy;
        private Button btnCancel;
        private Button btnHelp;
    }
}
