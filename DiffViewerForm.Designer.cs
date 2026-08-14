using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
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
            if (disposing)
            {
                components?.Dispose();
                DisposeStyledFonts();
            }
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
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(lblHeaderSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(20, 14, 20, 11);
            pnlHeader.Size = new Size(1100, 79);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Location = new Point(20, 14);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "Review changes before writing";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Location = new Point(20, 48);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text = "Pick a file to see its diff. Uncheck files you do not want to write.";
            //
            // split
            //
            split.Dock = DockStyle.Fill;
            split.FixedPanel = FixedPanel.Panel1;
            split.Location = new Point(0, 79);
            split.Name = "split";
            split.Panel1.Controls.Add(lstFilePlans);
            split.Panel1.Padding = new Padding(8, 9, 8, 9);
            split.Panel2.Controls.Add(rtbDiff);
            split.Panel2.Padding = new Padding(0, 0, 0, 0);
            split.Size = new Size(1100, 669);
            split.SplitterDistance = 380;
            split.TabIndex = 1;
            //
            // lstFilePlans
            //
            lstFilePlans.BorderStyle = BorderStyle.FixedSingle;
            lstFilePlans.CheckOnClick = true;
            lstFilePlans.Dock = DockStyle.Fill;
            lstFilePlans.FormattingEnabled = true;
            lstFilePlans.Name = "lstFilePlans";
            lstFilePlans.TabIndex = 0;
            lstFilePlans.AccessibleName = "Files to change";
            lstFilePlans.AccessibleDescription = "Tick a file to include it when writing.";
            lstFilePlans.SelectedIndexChanged += LstFilePlans_SelectedIndexChanged;
            lstFilePlans.ItemCheck += LstFilePlans_ItemCheck;
            //
            // rtbDiff
            //
            rtbDiff.BorderStyle = BorderStyle.None;
            rtbDiff.Dock = DockStyle.Fill;
            rtbDiff.Name = "rtbDiff";
            rtbDiff.ReadOnly = true;
            // The renderer prefixes "+ " and "- " so colour is not the only signal, but with
            // wrapping off, scrolling right pushed those prefixes off screen and left colour
            // alone carrying the meaning.
            rtbDiff.WordWrap = true;
            rtbDiff.AccessibleName = "Differences";
            rtbDiff.AccessibleDescription = "Line-by-line changes for the selected file. Added lines start with a plus, removed lines with a minus.";
            //
            // pnlBottom
            //
            pnlBottom.Controls.Add(btnCancel);
            pnlBottom.Controls.Add(btnWrite);
            pnlBottom.Controls.Add(lblWriteHint);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 748);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 14, 20, 14);
            pnlBottom.Size = new Size(1100, 68);
            pnlBottom.TabIndex = 2;
            //
            // lblWriteHint
            //
            lblWriteHint.AutoSize = true;
            lblWriteHint.Location = new Point(20, 23);
            lblWriteHint.Name = "lblWriteHint";
            lblWriteHint.Text = "0 files selected for write.";
            //
            // btnWrite
            //
            btnWrite.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnWrite.Cursor = Cursors.Hand;
            btnWrite.DialogResult = DialogResult.OK;
            btnWrite.FlatAppearance.BorderSize = 0;
            btnWrite.FlatStyle = FlatStyle.Flat;
            btnWrite.Location = new Point(871, 14);
            btnWrite.Name = "btnWrite";
            btnWrite.Size = new Size(150, 40);
            btnWrite.TabIndex = 0;
            btnWrite.Text = "&Write Selected";
            btnWrite.AccessibleDescription = "Writes the ticked files to disk, overwriting the existing contents.";
            btnWrite.UseVisualStyleBackColor = false;
            //
            // btnCancel
            //
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(771, 14);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(95, 40);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            //
            // DiffViewerForm
            //
            // Deliberately no default button. It used to be btnWrite, so Enter in the file list
            // wrote to disk — the destructive action bound to the most casual keystroke there is.
            // Escape already dismisses via CancelButton, so pointing AcceptButton at Cancel would
            // only add a second way to do the same thing; leaving it unset means writing requires
            // deliberately choosing the Write button, by mouse, Tab-and-Space, or Alt+W.
            AcceptButton = null;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(1100, 816);
            Controls.Add(split);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimumSize = new Size(720, 544);
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

            // Theme roles. Colours and fonts are resolved from ThemeTokens /
            // ThemeFonts at runtime; anything not listed here takes the default
            // for its control type.
            ThemeRoles.Set(btnCancel, ThemeRole.ButtonSecondary, FontRole.BodyBold);
            ThemeRoles.Set(btnWrite, ThemeRole.ButtonSuccess, FontRole.BodyBold);
            ThemeRoles.Set(lblHeaderSubtitle, FontRole.BodyItalic);
            ThemeRoles.Set(lblHeaderTitle, FontRole.Title);
            ThemeRoles.Set(lblWriteHint, ThemeRole.TextSecondary, FontRole.Small);
            ThemeRoles.Set(lstFilePlans, FontRole.Small);
            ThemeRoles.Set(pnlBottom, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ThemeRoles.Set(rtbDiff, FontRole.MonoSmall);
            ThemeRoles.Set(split, ThemeRole.SurfaceAlt);
            ResumeLayout(false);
        }

        #endregion
    }
}
