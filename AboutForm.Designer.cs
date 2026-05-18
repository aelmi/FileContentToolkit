using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit.Dialogs
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;

        private Panel pnlBody;
        private PictureBox picIcon;
        private Label lblAppName;
        private Label lblVersion;
        private Label lblDescription;
        private Label lblCopyright;
        private LinkLabel lnkProject;

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
            picIcon = new PictureBox();
            lblAppName = new Label();
            lblVersion = new Label();
            lblDescription = new Label();
            lblCopyright = new Label();
            lnkProject = new LinkLabel();
            pnlBottom = new Panel();
            btnClose = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picIcon).BeginInit();
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
            pnlHeader.Size = new Size(520, 70);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.White;
            lblHeaderTitle.Location = new Point(20, 12);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "About";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblHeaderSubtitle.ForeColor = Color.WhiteSmoke;
            lblHeaderSubtitle.Location = new Point(20, 42);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text = "File Content Toolkit";
            //
            // pnlBody
            //
            pnlBody.BackColor = Color.White;
            pnlBody.Controls.Add(picIcon);
            pnlBody.Controls.Add(lblAppName);
            pnlBody.Controls.Add(lblVersion);
            pnlBody.Controls.Add(lblDescription);
            pnlBody.Controls.Add(lblCopyright);
            pnlBody.Controls.Add(lnkProject);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 70);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(24, 22, 24, 22);
            pnlBody.Size = new Size(520, 280);
            pnlBody.TabIndex = 1;
            //
            // picIcon
            //
            picIcon.Location = new Point(28, 26);
            picIcon.Name = "picIcon";
            picIcon.Size = new Size(72, 72);
            picIcon.SizeMode = PictureBoxSizeMode.Zoom;
            picIcon.TabIndex = 0;
            picIcon.TabStop = false;
            //
            // lblAppName
            //
            lblAppName.AutoSize = true;
            lblAppName.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblAppName.ForeColor = Color.FromArgb(33, 37, 41);
            lblAppName.Location = new Point(120, 24);
            lblAppName.Name = "lblAppName";
            lblAppName.Text = "File Content Toolkit";
            //
            // lblVersion
            //
            lblVersion.AutoSize = true;
            lblVersion.Font = new Font("Segoe UI", 9.5F);
            lblVersion.ForeColor = Color.FromArgb(108, 117, 125);
            lblVersion.Location = new Point(122, 56);
            lblVersion.Name = "lblVersion";
            lblVersion.Text = "Version 0.0.0.0";
            //
            // lblDescription
            //
            lblDescription.Location = new Point(120, 86);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(370, 96);
            lblDescription.Font = new Font("Segoe UI", 9.5F);
            lblDescription.ForeColor = Color.FromArgb(33, 37, 41);
            lblDescription.Text =
                "Collect, scan and bundle the contents of source files into a single output. " +
                "Includes folder watching, .gitignore support, encoding auto-detect, regex search, " +
                "presets, find/replace, and one-click recreation.";
            //
            // lblCopyright
            //
            lblCopyright.AutoSize = true;
            lblCopyright.Font = new Font("Segoe UI", 9F);
            lblCopyright.ForeColor = Color.FromArgb(108, 117, 125);
            lblCopyright.Location = new Point(120, 192);
            lblCopyright.Name = "lblCopyright";
            lblCopyright.Text = "© 2026";
            //
            // lnkProject
            //
            lnkProject.AutoSize = true;
            lnkProject.Font = new Font("Segoe UI", 9F);
            lnkProject.LinkColor = Color.FromArgb(13, 110, 253);
            lnkProject.Location = new Point(120, 216);
            lnkProject.Name = "lnkProject";
            lnkProject.TabIndex = 5;
            lnkProject.TabStop = true;
            lnkProject.Text = "Show settings folder";
            lnkProject.LinkClicked += LnkProject_LinkClicked;
            //
            // pnlBottom
            //
            pnlBottom.BackColor = Color.White;
            pnlBottom.Controls.Add(btnClose);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 350);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 12, 20, 12);
            pnlBottom.Size = new Size(520, 60);
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
            btnClose.Location = new Point(411, 12);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(90, 36);
            btnClose.TabIndex = 0;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            //
            // AboutForm
            //
            AcceptButton = btnClose;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            CancelButton = btnClose;
            ClientSize = new Size(520, 410);
            Controls.Add(pnlBody);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "About";

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picIcon).EndInit();
            pnlBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
    }
}
