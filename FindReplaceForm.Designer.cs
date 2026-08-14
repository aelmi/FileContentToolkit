using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class FindReplaceForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;

        private Panel pnlBody;
        private Label lblFind;
        private ComboBox cmbFind;
        private Label lblReplace;
        private ComboBox cmbReplace;
        private CheckBox chkCase;
        private CheckBox chkWord;
        private CheckBox chkRegex;
        private Button btnNext;
        private Button btnPrev;
        private Button btnReplace;
        private Button btnReplaceAll;
        private Label lblStatus;

        /// <summary>
        /// Off-screen zero-size button that exists solely to be <c>CancelButton</c>.
        /// </summary>
        /// <remarks>
        /// This is a modeless tool window, so DialogResult does not close it and Escape was
        /// hand-rolled on KeyDown — where it forgot to set <c>e.Handled</c>, unlike the F3 branch
        /// beside it. Giving the form a real CancelButton routes Escape through the framework and
        /// lets the hand-rolled branch go.
        /// </remarks>
        private Button btnCloseHidden;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();
            lblHeaderSubtitle = new Label();
            pnlBody = new Panel();
            lblFind = new Label();
            cmbFind = new ComboBox();
            lblReplace = new Label();
            cmbReplace = new ComboBox();
            chkCase = new CheckBox();
            chkWord = new CheckBox();
            chkRegex = new CheckBox();
            btnNext = new Button();
            btnPrev = new Button();
            btnReplace = new Button();
            btnReplaceAll = new Button();
            lblStatus = new Label();
            btnCloseHidden = new Button();
            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            SuspendLayout();
            //
            // pnlHeader
            //
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(lblHeaderSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(13, 8, 13, 7);
            pnlHeader.Size = new Size(356, 48);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Location = new Point(13, 8);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Size = new Size(105, 22);
            lblHeaderTitle.TabIndex = 0;
            lblHeaderTitle.Text = "Find & Replace";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Location = new Point(13, 29);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Size = new Size(130, 17);
            lblHeaderSubtitle.TabIndex = 1;
            lblHeaderSubtitle.Text = "Search the output pane.";
            //
            // pnlBody
            //
            pnlBody.Controls.Add(lblFind);
            pnlBody.Controls.Add(cmbFind);
            pnlBody.Controls.Add(lblReplace);
            pnlBody.Controls.Add(cmbReplace);
            pnlBody.Controls.Add(chkCase);
            pnlBody.Controls.Add(chkWord);
            pnlBody.Controls.Add(chkRegex);
            pnlBody.Controls.Add(btnNext);
            pnlBody.Controls.Add(btnPrev);
            pnlBody.Controls.Add(btnReplace);
            pnlBody.Controls.Add(btnReplaceAll);
            pnlBody.Controls.Add(lblStatus);
            pnlBody.Controls.Add(btnCloseHidden);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 48);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(10, 11, 10, 11);
            pnlBody.Size = new Size(356, 146);
            pnlBody.TabIndex = 1;
            //
            // lblFind
            //
            lblFind.AutoSize = true;
            lblFind.Location = new Point(10, 16);
            lblFind.Name = "lblFind";
            lblFind.Size = new Size(33, 17);
            lblFind.TabIndex = 0;
            lblFind.Text = "Find:";
            //
            // cmbFind
            //
            cmbFind.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbFind.FlatStyle = FlatStyle.Flat;
            cmbFind.Location = new Point(61, 14);
            cmbFind.Name = "cmbFind";
            cmbFind.Size = new Size(283, 22);
            cmbFind.TabIndex = 1;
            cmbFind.TextChanged += CmbFind_TextChanged;
            //
            // lblReplace
            //
            lblReplace.AutoSize = true;
            lblReplace.Location = new Point(10, 39);
            lblReplace.Name = "lblReplace";
            lblReplace.Size = new Size(52, 17);
            lblReplace.TabIndex = 2;
            lblReplace.Text = "Replace:";
            //
            // cmbReplace
            //
            cmbReplace.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbReplace.FlatStyle = FlatStyle.Flat;
            cmbReplace.Location = new Point(61, 37);
            cmbReplace.Name = "cmbReplace";
            cmbReplace.Size = new Size(283, 22);
            cmbReplace.TabIndex = 3;
            //
            // chkCase
            //
            chkCase.AutoSize = true;
            chkCase.Location = new Point(61, 61);
            chkCase.Name = "chkCase";
            chkCase.Size = new Size(85, 20);
            chkCase.TabIndex = 4;
            chkCase.Text = "Match case";
            chkCase.UseVisualStyleBackColor = true;
            chkCase.CheckedChanged += ChkCase_CheckedChanged;
            //
            // chkWord
            //
            chkWord.AutoSize = true;
            chkWord.Location = new Point(134, 61);
            chkWord.Name = "chkWord";
            chkWord.Size = new Size(89, 20);
            chkWord.TabIndex = 5;
            chkWord.Text = "Whole word";
            chkWord.UseVisualStyleBackColor = true;
            chkWord.CheckedChanged += ChkWord_CheckedChanged;
            //
            // chkRegex
            //
            chkRegex.AutoSize = true;
            chkRegex.Location = new Point(210, 61);
            chkRegex.Name = "chkRegex";
            chkRegex.Size = new Size(56, 20);
            chkRegex.TabIndex = 6;
            chkRegex.Text = "Regex";
            chkRegex.UseVisualStyleBackColor = true;
            chkRegex.CheckedChanged += ChkRegex_CheckedChanged;
            //
            // btnNext
            //
            btnNext.Cursor = Cursors.Hand;
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Location = new Point(61, 84);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(67, 23);
            btnNext.TabIndex = 7;
            btnNext.Text = "Find &Next";
            btnNext.UseVisualStyleBackColor = false;
            btnNext.Click += BtnNext_Click;
            //
            // btnPrev
            //
            btnPrev.Cursor = Cursors.Hand;
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.FlatStyle = FlatStyle.Flat;
            btnPrev.Location = new Point(132, 84);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(67, 23);
            btnPrev.TabIndex = 8;
            btnPrev.Text = "Find &Prev";
            btnPrev.UseVisualStyleBackColor = false;
            btnPrev.Click += BtnPrev_Click;
            //
            // btnReplace
            //
            btnReplace.Cursor = Cursors.Hand;
            btnReplace.FlatAppearance.BorderSize = 0;
            btnReplace.FlatStyle = FlatStyle.Flat;
            btnReplace.Location = new Point(202, 84);
            btnReplace.Name = "btnReplace";
            btnReplace.Size = new Size(67, 23);
            btnReplace.TabIndex = 9;
            btnReplace.Text = "&Replace";
            btnReplace.UseVisualStyleBackColor = false;
            btnReplace.Click += BtnReplace_Click;
            //
            // btnReplaceAll
            //
            btnReplaceAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnReplaceAll.Cursor = Cursors.Hand;
            btnReplaceAll.FlatAppearance.BorderSize = 0;
            btnReplaceAll.FlatStyle = FlatStyle.Flat;
            btnReplaceAll.Location = new Point(273, 84);
            btnReplaceAll.Name = "btnReplaceAll";
            btnReplaceAll.Size = new Size(73, 23);
            btnReplaceAll.TabIndex = 10;
            btnReplaceAll.Text = "Replace &All";
            btnReplaceAll.UseVisualStyleBackColor = false;
            btnReplaceAll.Click += BtnReplaceAll_Click;
            //
            // lblStatus
            //
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.AutoEllipsis = true;
            // Was at y=87, inside the button row's y=84..107 band and last in z-order, so the
            // form's only feedback channel — and its only regex-error surface — was drawn
            // underneath the buttons and never seen.
            lblStatus.Location = new Point(10, 115);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(336, 18);
            lblStatus.TabIndex = 11;
            lblStatus.AccessibleName = "Search status";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            //
            // btnCloseHidden
            //
            btnCloseHidden.Name = "btnCloseHidden";
            btnCloseHidden.Size = new Size(0, 0);
            btnCloseHidden.Location = new Point(-100, -100);
            btnCloseHidden.TabStop = false;
            btnCloseHidden.Click += (s, e) => Close();
            //
            // FindReplaceForm
            //
            AcceptButton = btnNext;
            AutoScaleDimensions = new SizeF(7F, 15F);
            CancelButton = btnCloseHidden;
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(356, 194);
            Controls.Add(pnlBody);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(305, 218);
            Name = "FindReplaceForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Find & Replace";
            KeyDown += FindReplaceForm_KeyDown;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();

            // Theme roles. Colours and fonts are resolved from ThemeTokens /
            // ThemeFonts at runtime; anything not listed here takes the default
            // for its control type.
            ThemeRoles.Set(btnNext, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(btnPrev, ThemeRole.ButtonSecondary, FontRole.BodyBold);
            ThemeRoles.Set(btnReplace, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(btnReplaceAll, ThemeRole.ButtonSuccess, FontRole.BodyBold);
            ThemeRoles.Set(cmbFind, FontRole.Body);
            ThemeRoles.Set(cmbReplace, FontRole.Body);
            ThemeRoles.Set(lblHeaderSubtitle, FontRole.BodyItalic);
            ThemeRoles.Set(lblHeaderTitle, FontRole.Title);
            ThemeRoles.Set(lblStatus, ThemeRole.TextSecondary, FontRole.Body);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ResumeLayout(false);
        }

        #endregion
    }
}
