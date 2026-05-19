using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit.Dialogs
{
    partial class FolderTreePickerForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;

        private Panel pnlBody;
        private Panel pnlFilterStrip;
        private CheckBox chkExtFilter;
        private TreeView tree;

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
            pnlFilterStrip = new Panel();
            chkExtFilter = new CheckBox();
            tree = new TreeView();
            pnlBottom = new Panel();
            btnOk = new Button();
            btnCancel = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            pnlFilterStrip.SuspendLayout();
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
            pnlHeader.Size = new Size(760, 70);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.White;
            lblHeaderTitle.Location = new Point(20, 12);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "Select files and folders";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblHeaderSubtitle.ForeColor = Color.WhiteSmoke;
            lblHeaderSubtitle.Location = new Point(20, 42);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text = "Tip: checking a folder selects every file inside it.";
            //
            // pnlBody
            //
            pnlBody.BackColor = Color.White;
            pnlBody.Controls.Add(tree);
            pnlBody.Controls.Add(pnlFilterStrip);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 70);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(16, 12, 16, 12);
            pnlBody.Size = new Size(760, 590);
            pnlBody.TabIndex = 1;
            //
            // pnlFilterStrip
            //
            pnlFilterStrip.BackColor = Color.White;
            pnlFilterStrip.Controls.Add(chkExtFilter);
            pnlFilterStrip.Dock = DockStyle.Top;
            pnlFilterStrip.Location = new Point(16, 12);
            pnlFilterStrip.Name = "pnlFilterStrip";
            pnlFilterStrip.Padding = new Padding(0, 6, 0, 6);
            pnlFilterStrip.Size = new Size(728, 36);
            pnlFilterStrip.TabIndex = 0;
            //
            // chkExtFilter
            //
            chkExtFilter.AutoSize = true;
            chkExtFilter.Font = new Font("Segoe UI", 9.5F);
            chkExtFilter.ForeColor = Color.FromArgb(33, 37, 41);
            chkExtFilter.Location = new Point(2, 4);
            chkExtFilter.Name = "chkExtFilter";
            chkExtFilter.TabIndex = 0;
            chkExtFilter.Text = "Filter by configured extensions";
            chkExtFilter.UseVisualStyleBackColor = true;
            chkExtFilter.CheckedChanged += ChkExtFilter_CheckedChanged;
            //
            // tree
            //
            tree.BackColor = Color.White;
            tree.BorderStyle = BorderStyle.FixedSingle;
            tree.CheckBoxes = true;
            tree.Dock = DockStyle.Fill;
            tree.Font = new Font("Segoe UI", 9.5F);
            tree.ForeColor = Color.FromArgb(33, 37, 41);
            tree.HideSelection = false;
            tree.Name = "tree";
            tree.ShowLines = true;
            tree.ShowPlusMinus = true;
            tree.ShowRootLines = true;
            tree.TabIndex = 1;
            tree.BeforeExpand += Tree_BeforeExpand;
            tree.AfterCheck += Tree_AfterCheck;
            //
            // pnlBottom
            //
            pnlBottom.BackColor = Color.White;
            pnlBottom.Controls.Add(btnOk);
            pnlBottom.Controls.Add(btnCancel);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 660);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 12, 20, 12);
            pnlBottom.Size = new Size(760, 60);
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
            btnOk.Location = new Point(650, 12);
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
            btnCancel.Location = new Point(552, 12);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 36);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            //
            // FolderTreePickerForm
            //
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            CancelButton = btnCancel;
            ClientSize = new Size(760, 720);
            Controls.Add(pnlBody);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimumSize = new Size(540, 480);
            Name = "FolderTreePickerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select files and folders";

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlFilterStrip.ResumeLayout(false);
            pnlFilterStrip.PerformLayout();
            pnlBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
    }
}
