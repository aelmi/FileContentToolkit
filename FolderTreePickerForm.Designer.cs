using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
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
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(lblHeaderSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(20, 14, 20, 11);
            pnlHeader.Size = new Size(760, 79);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Location = new Point(20, 14);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "Select files and folders";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Location = new Point(20, 48);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text = "Tip: checking a folder selects every file inside it.";
            //
            // pnlBody
            //
            pnlBody.Controls.Add(tree);
            pnlBody.Controls.Add(pnlFilterStrip);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 79);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(16, 14, 16, 14);
            pnlBody.Size = new Size(760, 669);
            pnlBody.TabIndex = 1;
            //
            // pnlFilterStrip
            //
            pnlFilterStrip.Controls.Add(chkExtFilter);
            pnlFilterStrip.Dock = DockStyle.Top;
            pnlFilterStrip.Location = new Point(16, 14);
            pnlFilterStrip.Name = "pnlFilterStrip";
            pnlFilterStrip.Padding = new Padding(0, 7, 0, 7);
            pnlFilterStrip.Size = new Size(728, 40);
            pnlFilterStrip.TabIndex = 0;
            //
            // chkExtFilter
            //
            chkExtFilter.AutoSize = true;
            chkExtFilter.Location = new Point(2, 5);
            chkExtFilter.Name = "chkExtFilter";
            chkExtFilter.TabIndex = 0;
            chkExtFilter.Text = "Filter by configured extensions";
            chkExtFilter.UseVisualStyleBackColor = true;
            chkExtFilter.CheckedChanged += ChkExtFilter_CheckedChanged;
            //
            // tree
            //
            tree.BorderStyle = BorderStyle.FixedSingle;
            tree.CheckBoxes = true;
            tree.Dock = DockStyle.Fill;
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
            pnlBottom.Controls.Add(btnOk);
            pnlBottom.Controls.Add(btnCancel);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 748);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 14, 20, 14);
            pnlBottom.Size = new Size(760, 68);
            pnlBottom.TabIndex = 2;
            //
            // btnOk
            //
            btnOk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOk.Cursor = Cursors.Hand;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Location = new Point(650, 14);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(90, 40);
            btnOk.TabIndex = 0;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = false;
            btnOk.Click += BtnOk_Click;
            //
            // btnCancel
            //
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(552, 14);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 40);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            //
            // FolderTreePickerForm
            //
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(760, 816);
            Controls.Add(pnlBody);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimumSize = new Size(540, 544);
            Name = "FolderTreePickerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select files and folders";

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlFilterStrip.ResumeLayout(false);
            pnlFilterStrip.PerformLayout();
            pnlBottom.ResumeLayout(false);

            // Theme roles. Colours and fonts are resolved from ThemeTokens /
            // ThemeFonts at runtime; anything not listed here takes the default
            // for its control type.
            ThemeRoles.Set(btnCancel, ThemeRole.ButtonSecondary, FontRole.BodyBold);
            ThemeRoles.Set(btnOk, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(chkExtFilter, FontRole.Body);
            ThemeRoles.Set(lblHeaderSubtitle, FontRole.BodyItalic);
            ThemeRoles.Set(lblHeaderTitle, FontRole.Title);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlBottom, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlFilterStrip, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ThemeRoles.Set(tree, FontRole.Body);
            ResumeLayout(false);
        }

        #endregion
    }
}
