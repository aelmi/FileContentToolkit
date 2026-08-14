using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class OptionsForm
    {
        private System.ComponentModel.IContainer components = null;

        // Header
        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;

        // Body + bordered card
        private Panel pnlBody;
        private TableLayoutPanel pnlCard;

        private Label lblFiltersCaption;
        private TableLayoutPanel pnlMaxKbRow;
        private Label lblMaxKb;
        private NumericUpDown numMaxKb;
        private CheckBox chkSkipBinary;
        private CheckBox chkAutoEncoding;
        private CheckBox chkGitIgnore;
        private CheckBox chkDockerIgnore;
        private CheckBox chkWatch;

        private Label lblTrustCaption;
        private CheckBox chkRedactSecrets;
        private CheckBox chkWarnOnSecrets;

        private Label lblTemplatesCaption;
        private Button btnPromptTemplates;
        private Label lblBudgetCaption;
        private TableLayoutPanel pnlBudgetRow;
        private Label lblModel;
        private ComboBox cmbModel;
        private Label lblCustomBudget;
        private NumericUpDown numCustomBudget;

        // Bottom action bar
        private TableLayoutPanel pnlBottom;
        private FlowLayoutPanel pnlButtons;
        private Button btnOk;
        private Button btnCancel;
        private Button btnHelp;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// The card is a table now rather than a grid of absolute coordinates, because this
        /// workstream adds five controls to it and hand-placing each one is how the previous
        /// scaling defects arose. Rows are AutoSize, so the dialog grows with the font.
        /// Do not open in the Visual Studio designer.
        /// </summary>
        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();
            lblHeaderSubtitle = new Label();
            pnlBody = new Panel();
            pnlCard = new TableLayoutPanel();

            lblFiltersCaption = new Label();
            pnlMaxKbRow = new TableLayoutPanel();
            lblMaxKb = new Label();
            numMaxKb = new NumericUpDown();
            chkSkipBinary = new CheckBox();
            chkAutoEncoding = new CheckBox();
            chkGitIgnore = new CheckBox();
            chkDockerIgnore = new CheckBox();
            chkWatch = new CheckBox();

            lblTrustCaption = new Label();
            chkRedactSecrets = new CheckBox();
            chkWarnOnSecrets = new CheckBox();

            lblTemplatesCaption = new Label();
            btnPromptTemplates = new Button();
            lblBudgetCaption = new Label();
            pnlBudgetRow = new TableLayoutPanel();
            lblModel = new Label();
            cmbModel = new ComboBox();
            lblCustomBudget = new Label();
            numCustomBudget = new NumericUpDown();

            pnlBottom = new TableLayoutPanel();
            pnlButtons = new FlowLayoutPanel();
            btnOk = new Button();
            btnCancel = new Button();
            btnHelp = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            pnlCard.SuspendLayout();
            pnlMaxKbRow.SuspendLayout();
            pnlBudgetRow.SuspendLayout();
            pnlBottom.SuspendLayout();
            pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMaxKb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCustomBudget).BeginInit();
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
            pnlHeader.Padding = new Padding(13, 10, 13, 10);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Dock = DockStyle.Top;
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.TabIndex = 0;
            lblHeaderTitle.Text = "Options";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Dock = DockStyle.Top;
            lblHeaderSubtitle.Margin = new Padding(0, 4, 0, 0);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.TabIndex = 1;
            lblHeaderSubtitle.Text = "How files are read, what leaves the app, and what to measure against.";
            //
            // pnlBody
            //
            pnlBody.AutoScroll = true;
            pnlBody.Controls.Add(pnlCard);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(9, 8, 9, 8);
            pnlBody.TabIndex = 1;
            //
            // pnlCard
            //
            pnlCard.AutoSize = true;
            pnlCard.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlCard.BorderStyle = BorderStyle.FixedSingle;
            pnlCard.ColumnCount = 1;
            pnlCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlCard.Dock = DockStyle.Top;
            pnlCard.Name = "pnlCard";
            pnlCard.Padding = new Padding(14, 12, 14, 12);
            pnlCard.RowCount = 14;
            for (int i = 0; i < 14; i++) pnlCard.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlCard.TabIndex = 0;

            pnlCard.Controls.Add(lblFiltersCaption, 0, 0);
            pnlCard.Controls.Add(pnlMaxKbRow, 0, 1);
            pnlCard.Controls.Add(chkSkipBinary, 0, 2);
            pnlCard.Controls.Add(chkAutoEncoding, 0, 3);
            pnlCard.Controls.Add(chkGitIgnore, 0, 4);
            pnlCard.Controls.Add(chkDockerIgnore, 0, 5);
            pnlCard.Controls.Add(chkWatch, 0, 6);
            pnlCard.Controls.Add(lblTrustCaption, 0, 7);
            pnlCard.Controls.Add(chkRedactSecrets, 0, 8);
            pnlCard.Controls.Add(chkWarnOnSecrets, 0, 9);
            pnlCard.Controls.Add(lblTemplatesCaption, 0, 10);
            pnlCard.Controls.Add(btnPromptTemplates, 0, 11);
            pnlCard.Controls.Add(lblBudgetCaption, 0, 12);
            pnlCard.Controls.Add(pnlBudgetRow, 0, 13);
            //
            // lblFiltersCaption
            //
            lblFiltersCaption.AutoSize = true;
            lblFiltersCaption.Margin = new Padding(0, 0, 0, 6);
            lblFiltersCaption.Name = "lblFiltersCaption";
            lblFiltersCaption.Text = "Filters";
            //
            // pnlMaxKbRow
            //
            pnlMaxKbRow.AutoSize = true;
            pnlMaxKbRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlMaxKbRow.ColumnCount = 2;
            pnlMaxKbRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlMaxKbRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlMaxKbRow.Controls.Add(lblMaxKb, 0, 0);
            pnlMaxKbRow.Controls.Add(numMaxKb, 1, 0);
            pnlMaxKbRow.Dock = DockStyle.Top;
            pnlMaxKbRow.Margin = new Padding(0, 0, 0, 6);
            pnlMaxKbRow.Name = "pnlMaxKbRow";
            pnlMaxKbRow.RowCount = 1;
            pnlMaxKbRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlMaxKbRow.TabIndex = 0;
            //
            // lblMaxKb
            //
            lblMaxKb.Anchor = AnchorStyles.Left;
            lblMaxKb.AutoSize = true;
            lblMaxKb.Margin = new Padding(0, 0, 10, 0);
            lblMaxKb.Name = "lblMaxKb";
            lblMaxKb.Text = "Max file size (KB, 0 = unlimited):";
            //
            // numMaxKb
            //
            numMaxKb.Margin = new Padding(0);
            numMaxKb.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numMaxKb.Name = "numMaxKb";
            numMaxKb.Size = new Size(102, 22);
            numMaxKb.TabIndex = 0;
            numMaxKb.AccessibleName = "Maximum file size in kilobytes";
            //
            // chkSkipBinary
            //
            chkSkipBinary.AutoSize = true;
            chkSkipBinary.Margin = new Padding(0, 0, 0, 4);
            chkSkipBinary.Name = "chkSkipBinary";
            chkSkipBinary.TabIndex = 1;
            chkSkipBinary.Text = "Skip &binary files (null-byte heuristic)";
            chkSkipBinary.AccessibleName = "Skip binary files";
            chkSkipBinary.UseVisualStyleBackColor = true;
            //
            // chkAutoEncoding
            //
            chkAutoEncoding.AutoSize = true;
            chkAutoEncoding.Margin = new Padding(0, 0, 0, 4);
            chkAutoEncoding.Name = "chkAutoEncoding";
            chkAutoEncoding.TabIndex = 2;
            chkAutoEncoding.Text = "Auto-detect &encoding (BOM + UTF-8 fallback)";
            chkAutoEncoding.AccessibleName = "Auto-detect encoding";
            chkAutoEncoding.UseVisualStyleBackColor = true;
            //
            // chkGitIgnore
            //
            // The caption used to claim this covered .dockerignore as well. It no longer does,
            // and it never should have: merging an idiomatic .dockerignore into the gitignore
            // rules reduced an ordinary repository to zero files with no explanation.
            chkGitIgnore.AutoSize = true;
            chkGitIgnore.Margin = new Padding(0, 0, 0, 4);
            chkGitIgnore.Name = "chkGitIgnore";
            chkGitIgnore.TabIndex = 3;
            chkGitIgnore.Text = "Apply .&gitignore from the folder root";
            chkGitIgnore.AccessibleName = "Apply gitignore";
            chkGitIgnore.UseVisualStyleBackColor = true;
            //
            // chkDockerIgnore
            //
            chkDockerIgnore.AutoSize = true;
            chkDockerIgnore.Margin = new Padding(0, 0, 0, 4);
            chkDockerIgnore.Name = "chkDockerIgnore";
            chkDockerIgnore.TabIndex = 4;
            chkDockerIgnore.Text = "Apply .doc&kerignore as well (can exclude almost everything)";
            chkDockerIgnore.AccessibleName = "Apply dockerignore";
            chkDockerIgnore.UseVisualStyleBackColor = true;
            //
            // chkWatch
            //
            chkWatch.AutoSize = true;
            chkWatch.Margin = new Padding(0, 0, 0, 12);
            chkWatch.Name = "chkWatch";
            chkWatch.TabIndex = 5;
            chkWatch.Text = "&Watch folder for changes and auto-refresh";
            chkWatch.AccessibleName = "Watch folder for changes";
            chkWatch.UseVisualStyleBackColor = true;
            //
            // lblTrustCaption
            //
            lblTrustCaption.AutoSize = true;
            lblTrustCaption.Margin = new Padding(0, 0, 0, 6);
            lblTrustCaption.Name = "lblTrustCaption";
            lblTrustCaption.Text = "Secrets";
            //
            // chkRedactSecrets
            //
            chkRedactSecrets.AutoSize = true;
            chkRedactSecrets.Margin = new Padding(0, 0, 0, 4);
            chkRedactSecrets.Name = "chkRedactSecrets";
            chkRedactSecrets.TabIndex = 6;
            chkRedactSecrets.Text = "&Redact detected credentials before copying or exporting";
            chkRedactSecrets.AccessibleName = "Redact detected credentials";
            chkRedactSecrets.UseVisualStyleBackColor = true;
            //
            // chkWarnOnSecrets
            //
            chkWarnOnSecrets.AutoSize = true;
            chkWarnOnSecrets.Margin = new Padding(0, 0, 0, 12);
            chkWarnOnSecrets.Name = "chkWarnOnSecrets";
            chkWarnOnSecrets.TabIndex = 7;
            chkWarnOnSecrets.Text = "Let me re&view them first";
            chkWarnOnSecrets.AccessibleName = "Review detected credentials before continuing";
            chkWarnOnSecrets.UseVisualStyleBackColor = true;
            //
            // lblTemplatesCaption
            //
            lblTemplatesCaption.AutoSize = true;
            lblTemplatesCaption.Margin = new Padding(0, 0, 0, 6);
            lblTemplatesCaption.Name = "lblTemplatesCaption";
            lblTemplatesCaption.Text = "Prompt templates";
            //
            // btnPromptTemplates
            //
            // The library itself is edited in its own window; a text editor does not belong in a
            // card of checkboxes. This is the route to it from where a user would look first.
            btnPromptTemplates.AutoSize = true;
            btnPromptTemplates.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnPromptTemplates.Cursor = Cursors.Hand;
            btnPromptTemplates.FlatAppearance.BorderSize = 0;
            btnPromptTemplates.FlatStyle = FlatStyle.Flat;
            btnPromptTemplates.Margin = new Padding(0, 0, 0, 12);
            btnPromptTemplates.MinimumSize = new Size(160, 28);
            btnPromptTemplates.Name = "btnPromptTemplates";
            btnPromptTemplates.Padding = new Padding(10, 4, 10, 4);
            btnPromptTemplates.TabIndex = 9;
            btnPromptTemplates.Text = "Edit prompt &templates...";
            btnPromptTemplates.AccessibleName = "Edit prompt templates";
            btnPromptTemplates.AccessibleDescription =
                "Add, edit and delete the templates offered by Copy as prompt.";
            btnPromptTemplates.UseVisualStyleBackColor = false;
            btnPromptTemplates.Click += BtnPromptTemplates_Click;
            //
            // lblBudgetCaption
            //
            lblBudgetCaption.AutoSize = true;
            lblBudgetCaption.Margin = new Padding(0, 0, 0, 6);
            lblBudgetCaption.Name = "lblBudgetCaption";
            lblBudgetCaption.Text = "Token budget";
            //
            // pnlBudgetRow
            //
            pnlBudgetRow.AutoSize = true;
            pnlBudgetRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlBudgetRow.ColumnCount = 4;
            for (int i = 0; i < 4; i++) pnlBudgetRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlBudgetRow.Controls.Add(lblModel, 0, 0);
            pnlBudgetRow.Controls.Add(cmbModel, 1, 0);
            pnlBudgetRow.Controls.Add(lblCustomBudget, 2, 0);
            pnlBudgetRow.Controls.Add(numCustomBudget, 3, 0);
            pnlBudgetRow.Dock = DockStyle.Top;
            pnlBudgetRow.Margin = new Padding(0);
            pnlBudgetRow.Name = "pnlBudgetRow";
            pnlBudgetRow.RowCount = 1;
            pnlBudgetRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlBudgetRow.TabIndex = 10;
            //
            // lblModel
            //
            lblModel.Anchor = AnchorStyles.Left;
            lblModel.AutoSize = true;
            lblModel.Margin = new Padding(0, 0, 10, 0);
            lblModel.Name = "lblModel";
            lblModel.Text = "&Model:";
            //
            // cmbModel
            //
            cmbModel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbModel.Margin = new Padding(0, 0, 16, 0);
            cmbModel.Name = "cmbModel";
            cmbModel.Size = new Size(150, 23);
            cmbModel.TabIndex = 0;
            cmbModel.AccessibleName = "Target model";
            cmbModel.SelectedIndexChanged += CmbModel_SelectedIndexChanged;
            //
            // lblCustomBudget
            //
            lblCustomBudget.Anchor = AnchorStyles.Left;
            lblCustomBudget.AutoSize = true;
            lblCustomBudget.Margin = new Padding(0, 0, 10, 0);
            lblCustomBudget.Name = "lblCustomBudget";
            lblCustomBudget.Text = "Custom tokens:";
            //
            // numCustomBudget
            //
            numCustomBudget.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            numCustomBudget.Margin = new Padding(0);
            numCustomBudget.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numCustomBudget.Name = "numCustomBudget";
            numCustomBudget.Size = new Size(110, 22);
            numCustomBudget.TabIndex = 1;
            numCustomBudget.AccessibleName = "Custom context window in tokens";
            //
            // pnlBottom
            //
            pnlBottom.AutoSize = true;
            pnlBottom.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlBottom.ColumnCount = 1;
            pnlBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlBottom.Controls.Add(pnlButtons, 0, 0);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(13, 8, 13, 10);
            pnlBottom.RowCount = 1;
            pnlBottom.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlBottom.TabIndex = 2;
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
            pnlButtons.TabIndex = 0;
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
            btnOk.AccessibleName = "OK";
            btnOk.UseVisualStyleBackColor = false;
            btnOk.Click += BtnOk_Click;
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
            // OptionsForm
            //
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(520, 470);
            Controls.Add(pnlBody);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(480, 400);
            Name = "OptionsForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Options";

            // Theme roles. Colours and fonts are resolved from ThemeTokens / ThemeFonts at
            // runtime; anything not listed here takes the default for its control type.
            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ThemeRoles.Set(lblHeaderTitle, ThemeRole.HeaderTitle, FontRole.Title);
            ThemeRoles.Set(lblHeaderSubtitle, ThemeRole.HeaderSubtitle, FontRole.BodyItalic);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlCard, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlMaxKbRow, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlBudgetRow, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(lblFiltersCaption, FontRole.MediumBold);
            ThemeRoles.Set(lblTrustCaption, FontRole.MediumBold);
            ThemeRoles.Set(lblBudgetCaption, FontRole.MediumBold);
            ThemeRoles.Set(lblTemplatesCaption, FontRole.MediumBold);
            ThemeRoles.Set(btnPromptTemplates, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(lblMaxKb, FontRole.Body);
            ThemeRoles.Set(numMaxKb, FontRole.Body);
            ThemeRoles.Set(chkSkipBinary, FontRole.Body);
            ThemeRoles.Set(chkAutoEncoding, FontRole.Body);
            ThemeRoles.Set(chkGitIgnore, FontRole.Body);
            ThemeRoles.Set(chkDockerIgnore, FontRole.Body);
            ThemeRoles.Set(chkWatch, FontRole.Body);
            ThemeRoles.Set(chkRedactSecrets, FontRole.Body);
            ThemeRoles.Set(chkWarnOnSecrets, FontRole.Body);
            ThemeRoles.Set(lblModel, FontRole.Body);
            ThemeRoles.Set(cmbModel, FontRole.Body);
            ThemeRoles.Set(lblCustomBudget, FontRole.Body);
            ThemeRoles.Set(numCustomBudget, FontRole.Body);
            ThemeRoles.Set(pnlBottom, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlButtons, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(btnOk, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(btnCancel, ThemeRole.ButtonSecondary, FontRole.BodyBold);
            ThemeRoles.Set(btnHelp, ThemeRole.ButtonSubtle, FontRole.BodyBold);

            HelpTopics.Set(this, HelpTopics.Settings);

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            pnlCard.ResumeLayout(false);
            pnlCard.PerformLayout();
            pnlMaxKbRow.ResumeLayout(false);
            pnlMaxKbRow.PerformLayout();
            pnlBudgetRow.ResumeLayout(false);
            pnlBudgetRow.PerformLayout();
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            pnlButtons.ResumeLayout(false);
            pnlButtons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numMaxKb).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCustomBudget).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
