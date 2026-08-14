using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class MessageDialog
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblHeaderTitle;

        private Panel pnlBody;
        private PictureBox picIcon;
        private TextBox txtMessage;
        private TextBox txtDetails;

        private Panel pnlBottom;
        private Button btnOk;
        private Button btnCopy;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                // The bitmap is built here from a system icon rather than loaded from resources,
                // so nothing else will free it.
                picIcon?.Image?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();
            pnlBody = new Panel();
            picIcon = new PictureBox();
            txtMessage = new TextBox();
            txtDetails = new TextBox();
            pnlBottom = new Panel();
            btnOk = new Button();
            btnCopy = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picIcon).BeginInit();
            pnlBottom.SuspendLayout();
            SuspendLayout();

            //
            // pnlHeader
            //
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(20, 16, 20, 16);
            pnlHeader.Size = new Size(540, 63);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Location = new Point(20, 14);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "Message";
            //
            // pnlBody
            //
            pnlBody.Controls.Add(picIcon);
            pnlBody.Controls.Add(txtMessage);
            pnlBody.Controls.Add(txtDetails);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 63);
            pnlBody.Name = "pnlBody";
            pnlBody.Size = new Size(540, 121);
            pnlBody.TabIndex = 1;
            //
            // picIcon
            //
            picIcon.Location = new Point(20, 20);
            picIcon.Name = "picIcon";
            picIcon.Size = new Size(32, 32);
            picIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            picIcon.TabStop = false;
            //
            // txtMessage
            //
            // A read-only multiline text box rather than a Label: the whole point of this dialog
            // is that the text can be selected with the mouse and copied. TabStop stays off so the
            // caret does not land here ahead of the buttons.
            txtMessage.BackColor = SystemColors.Control;
            txtMessage.BorderStyle = BorderStyle.None;
            txtMessage.Location = new Point(68, 20);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.ReadOnly = true;
            txtMessage.Size = new Size(452, 40);
            txtMessage.TabStop = false;
            txtMessage.AccessibleName = "Message";
            //
            // txtDetails
            //
            txtDetails.BorderStyle = BorderStyle.FixedSingle;
            txtDetails.Location = new Point(68, 72);
            txtDetails.Multiline = true;
            txtDetails.Name = "txtDetails";
            txtDetails.ReadOnly = true;
            txtDetails.ScrollBars = ScrollBars.Vertical;
            txtDetails.Size = new Size(452, 120);
            txtDetails.TabIndex = 0;
            txtDetails.Visible = false;
            txtDetails.WordWrap = false;
            txtDetails.AccessibleName = "Technical details";
            //
            // pnlBottom
            //
            pnlBottom.Controls.Add(btnOk);
            pnlBottom.Controls.Add(btnCopy);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 184);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 14, 20, 14);
            pnlBottom.Size = new Size(540, 68);
            pnlBottom.TabIndex = 2;
            //
            // btnOk
            //
            btnOk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOk.Cursor = Cursors.Hand;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Location = new Point(435, 14);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(85, 38);
            btnOk.TabIndex = 1;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = false;
            //
            // btnCopy
            //
            btnCopy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopy.Cursor = Cursors.Hand;
            btnCopy.FlatAppearance.BorderSize = 0;
            btnCopy.FlatStyle = FlatStyle.Flat;
            btnCopy.Location = new Point(325, 14);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(100, 38);
            btnCopy.TabIndex = 2;
            btnCopy.Text = "Copy";
            btnCopy.UseVisualStyleBackColor = false;
            btnCopy.Click += BtnCopy_Click;
            //
            // MessageDialog
            //
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnOk;
            ClientSize = new Size(540, 252);
            Controls.Add(pnlBody);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MessageDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Message";

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picIcon).EndInit();
            pnlBottom.ResumeLayout(false);

            // Theme roles. Colours and fonts are resolved from ThemeTokens /
            // ThemeFonts at runtime; anything not listed here takes the default
            // for its control type.
            ThemeRoles.Set(btnCopy, ThemeRole.ButtonSecondary, FontRole.BodyBold);
            ThemeRoles.Set(btnOk, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(lblHeaderTitle, FontRole.Title);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlBottom, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            // The message sits on the body panel and must not read as an input well, so it takes
            // the panel's fill rather than the sunken one its type would otherwise imply.
            ThemeRoles.Set(txtMessage, ThemeRole.SurfaceAlt, FontRole.Body);
            ThemeRoles.Set(txtDetails, FontRole.MonoSmall);
            ResumeLayout(false);
        }

        #endregion
    }
}
