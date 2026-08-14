using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class PromptDialog
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblHeaderTitle;

        private Panel pnlBody;
        private Label lblPrompt;
        private TextBox txtInput;

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
            pnlBody = new Panel();
            lblPrompt = new Label();
            txtInput = new TextBox();
            pnlBottom = new Panel();
            btnOk = new Button();
            btnCancel = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
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
            pnlHeader.Size = new Size(440, 63);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Location = new Point(20, 14);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "Prompt";
            //
            // pnlBody
            //
            // The label must precede its input in the Controls collection or UI Automation cannot
            // infer the association. This is the product's generic text-input primitive, so while
            // the order was wrong every text entry in the application presented an unnamed edit
            // box to a screen reader.
            pnlBody.Controls.Add(lblPrompt);
            pnlBody.Controls.Add(txtInput);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 63);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(20, 18, 20, 18);
            pnlBody.Size = new Size(440, 125);
            pnlBody.TabIndex = 1;
            //
            // lblPrompt
            //
            lblPrompt.AutoSize = true;
            lblPrompt.Location = new Point(20, 18);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Text = "Value:";
            //
            // txtInput
            //
            txtInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtInput.BorderStyle = BorderStyle.FixedSingle;
            txtInput.Location = new Point(20, 50);
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(400, 26);
            txtInput.TabIndex = 0;
            txtInput.AccessibleName = "Value";
            //
            // pnlBottom
            //
            pnlBottom.Controls.Add(btnOk);
            pnlBottom.Controls.Add(btnCancel);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 188);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 14, 20, 14);
            pnlBottom.Size = new Size(440, 68);
            pnlBottom.TabIndex = 2;
            //
            // btnOk
            //
            btnOk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOk.Cursor = Cursors.Hand;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Location = new Point(335, 14);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(85, 38);
            btnOk.TabIndex = 0;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = false;
            //
            // btnCancel
            //
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(241, 14);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(85, 38);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            //
            // PromptDialog
            //
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(440, 256);
            Controls.Add(pnlBody);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PromptDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Prompt";

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            pnlBottom.ResumeLayout(false);

            // Theme roles. Colours and fonts are resolved from ThemeTokens /
            // ThemeFonts at runtime; anything not listed here takes the default
            // for its control type.
            ThemeRoles.Set(btnCancel, ThemeRole.ButtonSecondary, FontRole.BodyBold);
            ThemeRoles.Set(btnOk, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(lblHeaderTitle, FontRole.Title);
            ThemeRoles.Set(lblPrompt, FontRole.Body);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlBottom, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ThemeRoles.Set(txtInput, FontRole.Body);
            ResumeLayout(false);
        }

        #endregion
    }
}
