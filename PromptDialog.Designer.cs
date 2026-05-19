using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit.Dialogs
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
            pnlHeader.BackColor = Color.FromArgb(0, 102, 204);
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(20, 14, 20, 14);
            pnlHeader.Size = new Size(440, 56);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.White;
            lblHeaderTitle.Location = new Point(20, 12);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "Prompt";
            //
            // pnlBody
            //
            pnlBody.BackColor = Color.White;
            pnlBody.Controls.Add(txtInput);
            pnlBody.Controls.Add(lblPrompt);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 56);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(20, 16, 20, 16);
            pnlBody.Size = new Size(440, 110);
            pnlBody.TabIndex = 1;
            //
            // lblPrompt
            //
            lblPrompt.AutoSize = true;
            lblPrompt.Font = new Font("Segoe UI", 9.5F);
            lblPrompt.ForeColor = Color.FromArgb(33, 37, 41);
            lblPrompt.Location = new Point(20, 16);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Text = "Value:";
            //
            // txtInput
            //
            txtInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtInput.BackColor = Color.White;
            txtInput.BorderStyle = BorderStyle.FixedSingle;
            txtInput.Font = new Font("Segoe UI", 9.5F);
            txtInput.Location = new Point(20, 44);
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(400, 23);
            txtInput.TabIndex = 0;
            //
            // pnlBottom
            //
            pnlBottom.BackColor = Color.White;
            pnlBottom.Controls.Add(btnOk);
            pnlBottom.Controls.Add(btnCancel);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 166);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 12, 20, 12);
            pnlBottom.Size = new Size(440, 60);
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
            btnOk.Location = new Point(335, 12);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(85, 34);
            btnOk.TabIndex = 0;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = false;
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
            btnCancel.Location = new Point(241, 12);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(85, 34);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            //
            // PromptDialog
            //
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            CancelButton = btnCancel;
            ClientSize = new Size(440, 226);
            Controls.Add(pnlBody);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
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
            ResumeLayout(false);
        }

        #endregion
    }
}
