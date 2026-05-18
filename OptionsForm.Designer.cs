using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit.Dialogs
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
        private Panel pnlCard;
        private Label lblMaxKb;
        private NumericUpDown numMaxKb;
        private CheckBox chkSkipBinary;
        private CheckBox chkAutoEncoding;
        private CheckBox chkGitIgnore;
        private CheckBox chkWatch;

        // Bottom action bar
        private Panel pnlBottom;
        private Button btnOk;
        private Button btnCancel;

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
            pnlCard = new Panel();
            lblMaxKb = new Label();
            numMaxKb = new NumericUpDown();
            chkSkipBinary = new CheckBox();
            chkAutoEncoding = new CheckBox();
            chkGitIgnore = new CheckBox();
            chkWatch = new CheckBox();
            pnlBottom = new Panel();
            btnOk = new Button();
            btnCancel = new Button();
            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            pnlCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMaxKb).BeginInit();
            pnlBottom.SuspendLayout();
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
            pnlHeader.Size = new Size(580, 70);
            pnlHeader.TabIndex = 0;
            // 
            // lblHeaderTitle
            // 
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.White;
            lblHeaderTitle.Location = new Point(20, 12);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Size = new Size(104, 32);
            lblHeaderTitle.TabIndex = 0;
            lblHeaderTitle.Text = "Options";
            // 
            // lblHeaderSubtitle
            // 
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblHeaderSubtitle.ForeColor = Color.WhiteSmoke;
            lblHeaderSubtitle.Location = new Point(20, 42);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Size = new Size(324, 25);
            lblHeaderSubtitle.TabIndex = 1;
            lblHeaderSubtitle.Text = "Configure how the scanner reads files.";
            // 
            // pnlBody
            // 
            pnlBody.BackColor = Color.White;
            pnlBody.Controls.Add(pnlCard);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 70);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(14, 12, 14, 12);
            pnlBody.Size = new Size(580, 241);
            pnlBody.TabIndex = 1;
            // 
            // pnlCard
            // 
            pnlCard.BackColor = Color.White;
            pnlCard.BorderStyle = BorderStyle.FixedSingle;
            pnlCard.Controls.Add(lblMaxKb);
            pnlCard.Controls.Add(numMaxKb);
            pnlCard.Controls.Add(chkSkipBinary);
            pnlCard.Controls.Add(chkAutoEncoding);
            pnlCard.Controls.Add(chkGitIgnore);
            pnlCard.Controls.Add(chkWatch);
            pnlCard.Dock = DockStyle.Fill;
            pnlCard.Location = new Point(14, 12);
            pnlCard.Name = "pnlCard";
            pnlCard.Size = new Size(552, 217);
            pnlCard.TabIndex = 0;
            // 
            // lblMaxKb
            // 
            lblMaxKb.AutoSize = true;
            lblMaxKb.Font = new Font("Segoe UI", 9.5F);
            lblMaxKb.ForeColor = Color.FromArgb(33, 37, 41);
            lblMaxKb.Location = new Point(24, 26);
            lblMaxKb.Name = "lblMaxKb";
            lblMaxKb.Size = new Size(281, 25);
            lblMaxKb.TabIndex = 0;
            lblMaxKb.Text = "Max file size (KB, 0 = unlimited):";
            // 
            // numMaxKb
            // 
            numMaxKb.Font = new Font("Segoe UI", 9.5F);
            numMaxKb.Location = new Point(313, 22);
            numMaxKb.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numMaxKb.Name = "numMaxKb";
            numMaxKb.Size = new Size(160, 33);
            numMaxKb.TabIndex = 0;
            // 
            // chkSkipBinary
            // 
            chkSkipBinary.AutoSize = true;
            chkSkipBinary.Font = new Font("Segoe UI", 9.5F);
            chkSkipBinary.ForeColor = Color.FromArgb(33, 37, 41);
            chkSkipBinary.Location = new Point(24, 64);
            chkSkipBinary.Name = "chkSkipBinary";
            chkSkipBinary.Size = new Size(340, 29);
            chkSkipBinary.TabIndex = 1;
            chkSkipBinary.Text = "Skip binary files (null-byte heuristic)";
            chkSkipBinary.UseVisualStyleBackColor = true;
            // 
            // chkAutoEncoding
            // 
            chkAutoEncoding.AutoSize = true;
            chkAutoEncoding.Font = new Font("Segoe UI", 9.5F);
            chkAutoEncoding.ForeColor = Color.FromArgb(33, 37, 41);
            chkAutoEncoding.Location = new Point(24, 102);
            chkAutoEncoding.Name = "chkAutoEncoding";
            chkAutoEncoding.Size = new Size(424, 29);
            chkAutoEncoding.TabIndex = 2;
            chkAutoEncoding.Text = "Auto-detect encoding (BOM + UTF-8 fallback)";
            chkAutoEncoding.UseVisualStyleBackColor = true;
            // 
            // chkGitIgnore
            // 
            chkGitIgnore.AutoSize = true;
            chkGitIgnore.Font = new Font("Segoe UI", 9.5F);
            chkGitIgnore.ForeColor = Color.FromArgb(33, 37, 41);
            chkGitIgnore.Location = new Point(24, 140);
            chkGitIgnore.Name = "chkGitIgnore";
            chkGitIgnore.Size = new Size(445, 29);
            chkGitIgnore.TabIndex = 3;
            chkGitIgnore.Text = "Apply .gitignore / .dockerignore from folder root";
            chkGitIgnore.UseVisualStyleBackColor = true;
            // 
            // chkWatch
            // 
            chkWatch.AutoSize = true;
            chkWatch.Font = new Font("Segoe UI", 9.5F);
            chkWatch.ForeColor = Color.FromArgb(33, 37, 41);
            chkWatch.Location = new Point(24, 178);
            chkWatch.Name = "chkWatch";
            chkWatch.Size = new Size(397, 29);
            chkWatch.TabIndex = 4;
            chkWatch.Text = "Watch folder for changes and auto-refresh";
            chkWatch.UseVisualStyleBackColor = true;
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.White;
            pnlBottom.Controls.Add(btnOk);
            pnlBottom.Controls.Add(btnCancel);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 311);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 12, 20, 12);
            pnlBottom.Size = new Size(580, 60);
            pnlBottom.TabIndex = 2;
            // 
            // btnOk
            // 
            btnOk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOk.BackColor = Color.FromArgb(51, 122, 183);
            btnOk.Cursor = Cursors.Hand;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnOk.ForeColor = Color.White;
            btnOk.Location = new Point(471, 12);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(90, 36);
            btnOk.TabIndex = 0;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = false;
            btnOk.Click += BtnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.BackColor = Color.FromArgb(108, 117, 125);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(373, 12);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 36);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // OptionsForm
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            CancelButton = btnCancel;
            ClientSize = new Size(580, 371);
            Controls.Add(pnlBody);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OptionsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Options";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlCard.ResumeLayout(false);
            pnlCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numMaxKb).EndInit();
            pnlBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
    }
}
