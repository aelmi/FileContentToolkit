using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit.Dialogs
{
    partial class DiffViewerForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;

        private SplitContainer split;
        private CheckedListBox lstFilePlans;
        private RichTextBox rtbDiff;

        private Panel pnlBottom;
        private Button btnWrite;
        private Button btnCancel;
        private Label lblWriteHint;

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
            split = new SplitContainer();
            lstFilePlans = new CheckedListBox();
            rtbDiff = new RichTextBox();
            pnlBottom = new Panel();
            btnWrite = new Button();
            btnCancel = new Button();
            lblWriteHint = new Label();

            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)split).BeginInit();
            split.Panel1.SuspendLayout();
            split.Panel2.SuspendLayout();
            split.SuspendLayout();
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
            pnlHeader.Size = new Size(1100, 70);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.White;
            lblHeaderTitle.Location = new Point(20, 12);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "Review changes before writing";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblHeaderSubtitle.ForeColor = Color.WhiteSmoke;
            lblHeaderSubtitle.Location = new Point(20, 42);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text = "Pick a file to see its diff. Uncheck files you do not want to write.";
            //
            // split
            //
            split.BackColor = Color.White;
            split.Dock = DockStyle.Fill;
            split.FixedPanel = FixedPanel.Panel1;
            split.Location = new Point(0, 70);
            split.Name = "split";
            split.Panel1.Controls.Add(lstFilePlans);
            split.Panel1.Padding = new Padding(8);
            split.Panel2.Controls.Add(rtbDiff);
            split.Panel2.Padding = new Padding(0);
            split.Size = new Size(1100, 590);
            split.SplitterDistance = 380;
            split.TabIndex = 1;
            //
            // lstFilePlans
            //
            lstFilePlans.BorderStyle = BorderStyle.FixedSingle;
            lstFilePlans.CheckOnClick = true;
            lstFilePlans.Dock = DockStyle.Fill;
            lstFilePlans.Font = new Font("Segoe UI", 9F);
            lstFilePlans.FormattingEnabled = true;
            lstFilePlans.Name = "lstFilePlans";
            lstFilePlans.SelectedIndexChanged += LstFilePlans_SelectedIndexChanged;
            lstFilePlans.ItemCheck += LstFilePlans_ItemCheck;
            //
            // rtbDiff
            //
            rtbDiff.BackColor = Color.White;
            rtbDiff.BorderStyle = BorderStyle.None;
            rtbDiff.Dock = DockStyle.Fill;
            rtbDiff.Font = new Font("Cascadia Mono", 9F);
            rtbDiff.ForeColor = Color.FromArgb(33, 37, 41);
            rtbDiff.Name = "rtbDiff";
            rtbDiff.ReadOnly = true;
            rtbDiff.WordWrap = false;
            //
            // pnlBottom
            //
            pnlBottom.BackColor = Color.White;
            pnlBottom.Controls.Add(btnCancel);
            pnlBottom.Controls.Add(btnWrite);
            pnlBottom.Controls.Add(lblWriteHint);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 660);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 12, 20, 12);
            pnlBottom.Size = new Size(1100, 60);
            pnlBottom.TabIndex = 2;
            //
            // lblWriteHint
            //
            lblWriteHint.AutoSize = true;
            lblWriteHint.Font = new Font("Segoe UI", 9F);
            lblWriteHint.ForeColor = Color.FromArgb(108, 117, 125);
            lblWriteHint.Location = new Point(20, 20);
            lblWriteHint.Name = "lblWriteHint";
            lblWriteHint.Text = "0 files selected for write.";
            //
            // btnWrite
            //
            btnWrite.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnWrite.BackColor = Color.FromArgb(40, 167, 69);
            btnWrite.Cursor = Cursors.Hand;
            btnWrite.DialogResult = DialogResult.OK;
            btnWrite.FlatAppearance.BorderSize = 0;
            btnWrite.FlatStyle = FlatStyle.Flat;
            btnWrite.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnWrite.ForeColor = Color.White;
            btnWrite.Location = new Point(871, 12);
            btnWrite.Name = "btnWrite";
            btnWrite.Size = new Size(150, 36);
            btnWrite.TabIndex = 0;
            btnWrite.Text = "Write Selected";
            btnWrite.UseVisualStyleBackColor = false;
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
            btnCancel.Location = new Point(771, 12);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(95, 36);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            //
            // DiffViewerForm
            //
            AcceptButton = btnWrite;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            CancelButton = btnCancel;
            ClientSize = new Size(1100, 720);
            Controls.Add(split);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimumSize = new Size(720, 480);
            Name = "DiffViewerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Review changes";

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            split.Panel1.ResumeLayout(false);
            split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)split).EndInit();
            split.ResumeLayout(false);
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}
