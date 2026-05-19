using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit.Dialogs
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
            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(0, 102, 204);
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(lblHeaderSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(20, 12, 20, 10);
            pnlHeader.Size = new Size(560, 70);
            pnlHeader.TabIndex = 0;
            // 
            // lblHeaderTitle
            // 
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.White;
            lblHeaderTitle.Location = new Point(20, 12);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Size = new Size(165, 32);
            lblHeaderTitle.TabIndex = 0;
            lblHeaderTitle.Text = "Find & Replace";
            // 
            // lblHeaderSubtitle
            // 
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblHeaderSubtitle.ForeColor = Color.WhiteSmoke;
            lblHeaderSubtitle.Location = new Point(20, 42);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Size = new Size(205, 25);
            lblHeaderSubtitle.TabIndex = 1;
            lblHeaderSubtitle.Text = "Search the output pane.";
            // 
            // pnlBody
            // 
            pnlBody.BackColor = Color.White;
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
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 70);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(16);
            pnlBody.Size = new Size(560, 174);
            pnlBody.TabIndex = 1;
            // 
            // lblFind
            // 
            lblFind.AutoSize = true;
            lblFind.ForeColor = Color.FromArgb(33, 37, 41);
            lblFind.Location = new Point(16, 24);
            lblFind.Name = "lblFind";
            lblFind.Size = new Size(52, 25);
            lblFind.TabIndex = 0;
            lblFind.Text = "Find:";
            // 
            // cmbFind
            // 
            cmbFind.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbFind.FlatStyle = FlatStyle.Flat;
            cmbFind.Font = new Font("Segoe UI", 9.5F);
            cmbFind.Location = new Point(96, 20);
            cmbFind.Name = "cmbFind";
            cmbFind.Size = new Size(444, 33);
            cmbFind.TabIndex = 0;
            cmbFind.TextChanged += CmbFind_TextChanged;
            // 
            // lblReplace
            // 
            lblReplace.AutoSize = true;
            lblReplace.ForeColor = Color.FromArgb(33, 37, 41);
            lblReplace.Location = new Point(16, 58);
            lblReplace.Name = "lblReplace";
            lblReplace.Size = new Size(81, 25);
            lblReplace.TabIndex = 1;
            lblReplace.Text = "Replace:";
            // 
            // cmbReplace
            // 
            cmbReplace.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbReplace.FlatStyle = FlatStyle.Flat;
            cmbReplace.Font = new Font("Segoe UI", 9.5F);
            cmbReplace.Location = new Point(96, 54);
            cmbReplace.Name = "cmbReplace";
            cmbReplace.Size = new Size(444, 33);
            cmbReplace.TabIndex = 1;
            // 
            // chkCase
            // 
            chkCase.AutoSize = true;
            chkCase.ForeColor = Color.FromArgb(33, 37, 41);
            chkCase.Location = new Point(96, 90);
            chkCase.Name = "chkCase";
            chkCase.Size = new Size(133, 29);
            chkCase.TabIndex = 2;
            chkCase.Text = "Match case";
            chkCase.UseVisualStyleBackColor = true;
            chkCase.CheckedChanged += ChkCase_CheckedChanged;
            // 
            // chkWord
            // 
            chkWord.AutoSize = true;
            chkWord.ForeColor = Color.FromArgb(33, 37, 41);
            chkWord.Location = new Point(210, 90);
            chkWord.Name = "chkWord";
            chkWord.Size = new Size(141, 29);
            chkWord.TabIndex = 3;
            chkWord.Text = "Whole word";
            chkWord.UseVisualStyleBackColor = true;
            chkWord.CheckedChanged += ChkWord_CheckedChanged;
            // 
            // chkRegex
            // 
            chkRegex.AutoSize = true;
            chkRegex.ForeColor = Color.FromArgb(33, 37, 41);
            chkRegex.Location = new Point(330, 90);
            chkRegex.Name = "chkRegex";
            chkRegex.Size = new Size(88, 29);
            chkRegex.TabIndex = 4;
            chkRegex.Text = "Regex";
            chkRegex.UseVisualStyleBackColor = true;
            chkRegex.CheckedChanged += ChkRegex_CheckedChanged;
            // 
            // btnNext
            // 
            btnNext.BackColor = Color.FromArgb(51, 122, 183);
            btnNext.Cursor = Cursors.Hand;
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnNext.ForeColor = Color.White;
            btnNext.Location = new Point(96, 124);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(105, 34);
            btnNext.TabIndex = 5;
            btnNext.Text = "Find Next";
            btnNext.UseVisualStyleBackColor = false;
            btnNext.Click += BtnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.BackColor = Color.FromArgb(108, 117, 125);
            btnPrev.Cursor = Cursors.Hand;
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.FlatStyle = FlatStyle.Flat;
            btnPrev.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnPrev.ForeColor = Color.White;
            btnPrev.Location = new Point(207, 124);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(105, 34);
            btnPrev.TabIndex = 6;
            btnPrev.Text = "Find Prev";
            btnPrev.UseVisualStyleBackColor = false;
            btnPrev.Click += BtnPrev_Click;
            // 
            // btnReplace
            // 
            btnReplace.BackColor = Color.FromArgb(13, 110, 253);
            btnReplace.Cursor = Cursors.Hand;
            btnReplace.FlatAppearance.BorderSize = 0;
            btnReplace.FlatStyle = FlatStyle.Flat;
            btnReplace.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnReplace.ForeColor = Color.White;
            btnReplace.Location = new Point(318, 124);
            btnReplace.Name = "btnReplace";
            btnReplace.Size = new Size(105, 34);
            btnReplace.TabIndex = 7;
            btnReplace.Text = "Replace";
            btnReplace.UseVisualStyleBackColor = false;
            btnReplace.Click += BtnReplace_Click;
            // 
            // btnReplaceAll
            // 
            btnReplaceAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnReplaceAll.BackColor = Color.FromArgb(40, 167, 69);
            btnReplaceAll.Cursor = Cursors.Hand;
            btnReplaceAll.FlatAppearance.BorderSize = 0;
            btnReplaceAll.FlatStyle = FlatStyle.Flat;
            btnReplaceAll.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnReplaceAll.ForeColor = Color.White;
            btnReplaceAll.Location = new Point(429, 124);
            btnReplaceAll.Name = "btnReplaceAll";
            btnReplaceAll.Size = new Size(115, 34);
            btnReplaceAll.TabIndex = 8;
            btnReplaceAll.Text = "Replace All";
            btnReplaceAll.UseVisualStyleBackColor = false;
            btnReplaceAll.Click += BtnReplaceAll_Click;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.AutoEllipsis = true;
            lblStatus.Font = new Font("Segoe UI", 9.5F);
            lblStatus.ForeColor = Color.FromArgb(108, 117, 125);
            lblStatus.Location = new Point(16, 128);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(528, 24);
            lblStatus.TabIndex = 9;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FindReplaceForm
            // 
            AcceptButton = btnNext;
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(560, 244);
            Controls.Add(pnlBody);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(480, 280);
            Name = "FindReplaceForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Find & Replace";
            KeyDown += FindReplaceForm_KeyDown;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}
