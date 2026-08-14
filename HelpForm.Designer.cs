using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class HelpForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;

        private Panel pnlBody;
        private ListBox lstTopics;
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
            lstTopics = new ListBox();
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
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(lblHeaderSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(20, 14, 20, 11);
            pnlHeader.Size = new Size(680, 79);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Location = new Point(20, 14);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "Help";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Location = new Point(20, 48);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text = "Keyboard shortcuts & feature reference";
            //
            // pnlBody
            //
            // rtbContent fills what lstTopics leaves. Index 0 docks last, so the reading pane is
            // added first and the topic list resolves against the left edge.
            pnlBody.Controls.Add(rtbContent);
            pnlBody.Controls.Add(lstTopics);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 79);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(20, 18, 20, 18);
            pnlBody.Size = new Size(680, 522);
            pnlBody.TabIndex = 1;
            //
            // rtbContent
            //
            rtbContent.BorderStyle = BorderStyle.None;
            rtbContent.Dock = DockStyle.Fill;
            rtbContent.Location = new Point(20, 18);
            rtbContent.Name = "rtbContent";
            rtbContent.AccessibleName = "Help contents";
            rtbContent.ReadOnly = true;
            rtbContent.Size = new Size(640, 485);
            rtbContent.TabIndex = 0;
            rtbContent.Text = "";
            rtbContent.DetectUrls = true;
            //
            // lstTopics
            //
            // The contextual half of F1: the window opens on the topic for whatever pane had
            // focus, and this is how the user gets to the rest without a topic tree, a search
            // index or a browser control.
            lstTopics.BorderStyle = BorderStyle.None;
            lstTopics.Dock = DockStyle.Left;
            lstTopics.IntegralHeight = false;
            lstTopics.Location = new Point(20, 18);
            lstTopics.Name = "lstTopics";
            lstTopics.Size = new Size(172, 485);
            lstTopics.TabIndex = 1;
            lstTopics.AccessibleName = "Help topics";
            lstTopics.AccessibleDescription = "Choose a help section to read.";
            lstTopics.SelectedIndexChanged += LstTopics_SelectedIndexChanged;
            //
            // pnlBottom
            //
            pnlBottom.Controls.Add(btnClose);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 601);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 14, 20, 14);
            pnlBottom.Size = new Size(680, 68);
            pnlBottom.TabIndex = 2;
            //
            // btnClose
            //
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Cursor = Cursors.Hand;
            // Escape returned DialogResult.OK, because CancelButton pointed at a button whose
            // DialogResult was OK. Dismissing a dialog is not consenting to it; both keys
            // now report Cancel, which is what a dismiss-only dialog should say.
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(571, 14);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(90, 40);
            btnClose.TabIndex = 0;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            //
            // HelpForm
            //
            AcceptButton = btnClose;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new Size(680, 669);
            Controls.Add(pnlBody);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(560, 521);
            Name = "HelpForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Help";

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBottom.ResumeLayout(false);

            // Theme roles. Colours and fonts are resolved from ThemeTokens /
            // ThemeFonts at runtime; anything not listed here takes the default
            // for its control type.
            ThemeRoles.Set(btnClose, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(lblHeaderSubtitle, FontRole.BodyItalic);
            ThemeRoles.Set(lblHeaderTitle, FontRole.Title);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlBottom, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ThemeRoles.Set(rtbContent, FontRole.Body);
            ThemeRoles.Set(lstTopics, ThemeRole.SurfaceAlt, FontRole.Body);
            ResumeLayout(false);
        }

        #endregion
    }
}
