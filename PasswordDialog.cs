using System;
using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit.Dialogs
{
    public partial class PasswordDialog : Form
    {
        private Color btnOKDefaultColor = Color.FromArgb(0, 123, 255);
        private Color btnOKHoverColor = Color.FromArgb(0, 86, 179);
        private Color btnCancelDefaultColor = Color.FromArgb(108, 117, 125);
        private Color btnCancelHoverColor = Color.FromArgb(90, 98, 104);

        public string Password => txtPassword.Text;

        public string Prompt
        {
            get => lblPrompt.Text;
            set => lblPrompt.Text = value;
        }

        public string HeaderText
        {
            get => lblHeader.Text;
            set => lblHeader.Text = "🔐 " + value;
        }

        public PasswordDialog() : this("Enter Password", "Enter password:")
        {
        }

        public PasswordDialog(string title) : this(title, "Enter password:")
        {
        }

        public PasswordDialog(string title, string prompt)
        {
            InitializeComponent();

            this.Text = title;
            this.HeaderText = title;
            this.Prompt = prompt;
        }

        private void PasswordDialog_Shown(object sender, EventArgs e)
        {
            txtPassword.Focus();
            txtPassword.Select();
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void BtnOK_MouseEnter(object sender, EventArgs e)
        {
            btnOK.BackColor = btnOKHoverColor;
        }

        private void BtnOK_MouseLeave(object sender, EventArgs e)
        {
            btnOK.BackColor = btnOKDefaultColor;
        }

        private void BtnCancel_MouseEnter(object sender, EventArgs e)
        {
            btnCancel.BackColor = btnCancelHoverColor;
        }

        private void BtnCancel_MouseLeave(object sender, EventArgs e)
        {
            btnCancel.BackColor = btnCancelDefaultColor;
        }

        /// <summary>
        /// Shows the password dialog and returns the entered password, or null if cancelled.
        /// </summary>
        public static string ShowDialog(IWin32Window owner, string title, string prompt = "Enter password:")
        {
            using (var dialog = new PasswordDialog(title, prompt))
            {
                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    return dialog.Password;
                }
                return null;
            }
        }

        /// <summary>
        /// Validates that the password meets minimum requirements.
        /// </summary>
        public bool ValidatePassword(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Password))
            {
                errorMessage = "Password cannot be empty.";
                return false;
            }

            if (Password.Length < 4)
            {
                errorMessage = "Password must be at least 4 characters long.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Shows the dialog with password validation.
        /// </summary>
        public static string ShowDialogWithValidation(IWin32Window owner, string title, string prompt = "Enter password:")
        {
            using (var dialog = new PasswordDialog(title, prompt))
            {
                while (true)
                {
                    if (dialog.ShowDialog(owner) == DialogResult.OK)
                    {
                        if (dialog.ValidatePassword(out string errorMessage))
                        {
                            return dialog.Password;
                        }
                        else
                        {
                            MessageBox.Show(errorMessage, "Invalid Password",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dialog.txtPassword.Clear();
                            dialog.txtPassword.Focus();
                            continue;
                        }
                    }
                    else
                    {
                        return null; // User cancelled
                    }
                }
            }
        }
    }
}