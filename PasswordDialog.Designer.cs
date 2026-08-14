using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class PasswordDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Rebuilt rather than repaired.
        /// </summary>
        /// <remarks>
        /// The previous layout declared <c>AutoScaleDimensions (10F, 25F)</c> but never set
        /// <c>this.Font</c>, so it inherited Segoe UI 9pt — measured at (7, 15) — and WinForms
        /// rescaled it by 0.70 horizontally and 0.60 vertically. Non-uniformly. Control sizes had
        /// been recorded at the large metric while locations and <c>ClientSize</c> stayed at the
        /// small one, which is why <c>chkShowPassword</c>, nominally at y=90 inside a parent 90
        /// tall, ended up outside its parent entirely. Those coordinates were internally
        /// inconsistent and no amount of nudging would have survived the scaling change, so the
        /// layout is expressed here as a flow of docked rows with no absolute coordinates left to
        /// go stale, and the dialog sizes itself to its content via <c>AutoSize</c>.
        ///
        /// The confirm-password row added by the previous workstream is carried through; it is
        /// collapsed rather than merely hidden when confirmation is not required, so the dialog
        /// shrinks to fit instead of leaving a gap.
        /// </remarks>
        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblHeader = new Label();
            pnlContent = new TableLayoutPanel();
            lblPrompt = new Label();
            txtPassword = new TextBox();
            lblConfirm = new Label();
            txtConfirm = new TextBox();
            chkShowPassword = new CheckBox();
            pnlButtons = new FlowLayoutPanel();
            btnCancel = new Button();
            btnOK = new Button();

            pnlHeader.SuspendLayout();
            pnlContent.SuspendLayout();
            pnlButtons.SuspendLayout();
            SuspendLayout();
            //
            // pnlHeader
            //
            pnlHeader.AutoSize = true;
            pnlHeader.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlHeader.Controls.Add(lblHeader);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(16, 12, 16, 12);
            pnlHeader.TabIndex = 0;
            //
            // lblHeader
            //
            lblHeader.AutoSize = true;
            lblHeader.Dock = DockStyle.Fill;
            lblHeader.Name = "lblHeader";
            lblHeader.Text = "Enter Password";
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            //
            // pnlContent
            //
            // One column, one row per field. Every row is AutoSize, so the dialog grows with the
            // font rather than clipping when the user raises the Windows text size.
            pnlContent.AutoSize = true;
            pnlContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlContent.ColumnCount = 1;
            pnlContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlContent.Dock = DockStyle.Top;
            pnlContent.Name = "pnlContent";
            pnlContent.Padding = new Padding(16, 14, 16, 10);
            pnlContent.RowCount = 5;
            for (int i = 0; i < 5; i++) pnlContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlContent.TabIndex = 1;

            // The label must precede its input in the Controls collection or UI Automation cannot
            // infer the association, which for a password field means an unnamed edit box.
            pnlContent.Controls.Add(lblPrompt, 0, 0);
            pnlContent.Controls.Add(txtPassword, 0, 1);
            pnlContent.Controls.Add(lblConfirm, 0, 2);
            pnlContent.Controls.Add(txtConfirm, 0, 3);
            pnlContent.Controls.Add(chkShowPassword, 0, 4);
            //
            // lblPrompt
            //
            lblPrompt.AutoSize = true;
            lblPrompt.Margin = new Padding(0, 0, 0, 4);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Text = "Enter password:";
            lblPrompt.TabIndex = 0;
            //
            // txtPassword
            //
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.Dock = DockStyle.Fill;
            txtPassword.Margin = new Padding(0, 0, 0, 12);
            txtPassword.Name = "txtPassword";
            txtPassword.TabIndex = 1;
            txtPassword.AccessibleName = "Password";
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.KeyDown += TxtPassword_KeyDown;
            //
            // lblConfirm
            //
            lblConfirm.AutoSize = true;
            lblConfirm.Margin = new Padding(0, 0, 0, 4);
            lblConfirm.Name = "lblConfirm";
            lblConfirm.Text = "Confirm password:";
            lblConfirm.TabIndex = 2;
            //
            // txtConfirm
            //
            txtConfirm.BorderStyle = BorderStyle.FixedSingle;
            txtConfirm.Dock = DockStyle.Fill;
            txtConfirm.Margin = new Padding(0, 0, 0, 12);
            txtConfirm.Name = "txtConfirm";
            txtConfirm.TabIndex = 3;
            txtConfirm.AccessibleName = "Confirm password";
            txtConfirm.UseSystemPasswordChar = true;
            txtConfirm.KeyDown += TxtPassword_KeyDown;
            //
            // chkShowPassword
            //
            chkShowPassword.AutoSize = true;
            chkShowPassword.Cursor = Cursors.Hand;
            chkShowPassword.Margin = new Padding(0, 0, 0, 0);
            chkShowPassword.Name = "chkShowPassword";
            chkShowPassword.TabIndex = 4;
            chkShowPassword.Text = "Show password";
            chkShowPassword.UseVisualStyleBackColor = true;
            chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;
            //
            // pnlButtons
            //
            pnlButtons.AutoSize = true;
            pnlButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlButtons.Controls.Add(btnOK);
            pnlButtons.Controls.Add(btnCancel);
            pnlButtons.Dock = DockStyle.Top;
            pnlButtons.FlowDirection = FlowDirection.RightToLeft;
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Padding = new Padding(16, 8, 16, 14);
            pnlButtons.TabIndex = 2;
            pnlButtons.WrapContents = false;
            //
            // btnOK
            //
            btnOK.AutoSize = true;
            btnOK.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnOK.Cursor = Cursors.Hand;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Margin = new Padding(8, 0, 0, 0);
            btnOK.MinimumSize = new Size(88, 30);
            btnOK.Name = "btnOK";
            btnOK.Padding = new Padding(10, 4, 10, 4);
            btnOK.TabIndex = 0;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = false;
            //
            // btnCancel
            //
            btnCancel.AutoSize = true;
            btnCancel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Margin = new Padding(8, 0, 0, 0);
            btnCancel.MinimumSize = new Size(88, 30);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(10, 4, 10, 4);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            //
            // PasswordDialog
            //
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            CancelButton = btnCancel;
            ClientSize = new Size(400, 260);
            Controls.Add(pnlButtons);
            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(400, 0);
            Name = "PasswordDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Password Required";
            Shown += PasswordDialog_Shown;

            // Theme roles.
            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ThemeRoles.Set(lblHeader, ThemeRole.HeaderTitle, FontRole.Title);
            ThemeRoles.Set(pnlContent, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(lblPrompt, FontRole.Medium);
            ThemeRoles.Set(txtPassword, FontRole.Medium);
            ThemeRoles.Set(lblConfirm, FontRole.Medium);
            ThemeRoles.Set(txtConfirm, FontRole.Medium);
            ThemeRoles.Set(chkShowPassword, ThemeRole.TextSecondary, FontRole.Small);
            ThemeRoles.Set(pnlButtons, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(btnOK, ThemeRole.ButtonAccent, FontRole.MediumBold);
            ThemeRoles.Set(btnCancel, ThemeRole.ButtonSecondary, FontRole.Medium);

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlContent.ResumeLayout(false);
            pnlContent.PerformLayout();
            pnlButtons.ResumeLayout(false);
            pnlButtons.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlHeader;
        private Label lblHeader;
        private TableLayoutPanel pnlContent;
        private CheckBox chkShowPassword;
        private TextBox txtPassword;
        private Label lblPrompt;
        private Label lblConfirm;
        private TextBox txtConfirm;
        private FlowLayoutPanel pnlButtons;
        private Button btnCancel;
        private Button btnOK;
    }
}
