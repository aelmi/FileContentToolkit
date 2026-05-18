using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit.Dialogs
{
    partial class HelpForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;

        private Panel pnlBody;
        private RichTextBox rtbContent;

        private Panel pnlBottom;
        private Button btnClose;

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
            rtbContent = new RichTextBox();
            pnlBottom = new Panel();
            btnClose = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
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
            pnlHeader.Size = new Size(680, 70);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.White;
            lblHeaderTitle.Location = new Point(20, 12);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "Help";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblHeaderSubtitle.ForeColor = Color.WhiteSmoke;
            lblHeaderSubtitle.Location = new Point(20, 42);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text = "Keyboard shortcuts & feature reference";
            //
            // pnlBody
            //
            pnlBody.BackColor = Color.White;
            pnlBody.Controls.Add(rtbContent);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 70);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(20, 16, 20, 16);
            pnlBody.Size = new Size(680, 460);
            pnlBody.TabIndex = 1;
            //
            // rtbContent
            //
            rtbContent.BackColor = Color.White;
            rtbContent.BorderStyle = BorderStyle.None;
            rtbContent.Dock = DockStyle.Fill;
            rtbContent.Font = new Font("Segoe UI", 9.5F);
            rtbContent.ForeColor = Color.FromArgb(33, 37, 41);
            rtbContent.Location = new Point(20, 16);
            rtbContent.Name = "rtbContent";
            rtbContent.ReadOnly = true;
            rtbContent.Size = new Size(640, 428);
            rtbContent.TabIndex = 0;
            rtbContent.Text = "";
            rtbContent.DetectUrls = true;
            //
            // pnlBottom
            //
            pnlBottom.BackColor = Color.White;
            pnlBottom.Controls.Add(btnClose);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 530);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 12, 20, 12);
            pnlBottom.Size = new Size(680, 60);
            pnlBottom.TabIndex = 2;
            //
            // btnClose
            //
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackColor = Color.FromArgb(51, 122, 183);
            btnClose.Cursor = Cursors.Hand;
            btnClose.DialogResult = DialogResult.OK;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(571, 12);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(90, 36);
            btnClose.TabIndex = 0;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            //
            // HelpForm
            //
            AcceptButton = btnClose;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            CancelButton = btnClose;
            ClientSize = new Size(680, 590);
            Controls.Add(pnlBody);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(560, 460);
            Name = "HelpForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Help";

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
    }
}
