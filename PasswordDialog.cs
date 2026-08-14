using System;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    public partial class PasswordDialog : ThemedForm
    {
        /// <summary>
        /// Below this, a password is not worth the false sense of security. The previous
        /// four-character floor was both too weak and never applied on the real encryption path.
        /// </summary>
        public const int MinimumPasswordLength = 8;

        private bool _requireConfirmation;

        public string Password => txtPassword.Text;

        /// <summary>
        /// When true the user must type the password twice and the two must match exactly before
        /// the dialog will close with OK. Encryption is one-way with no recovery: a single typo
        /// in a password used to produce a permanently undecryptable blob.
        /// </summary>
        public bool RequireConfirmation
        {
            get => _requireConfirmation;
            set
            {
                _requireConfirmation = value;
                // The rows are AutoSize, and a TableLayoutPanel ignores invisible children when
                // measuring, so hiding these collapses the rows and the dialog shrinks to fit
                // rather than leaving a hole where the confirm field would have been.
                lblConfirm.Visible = value;
                txtConfirm.Visible = value;
            }
        }

        public string Prompt
        {
            get => lblPrompt.Text;
            set => lblPrompt.Text = value;
        }

        public string HeaderText
        {
            get => lblHeader.Text;
            // The padlock emoji is gone: screen readers announce it literally, and it depended on
            // Segoe UI Emoji being present.
            set => lblHeader.Text = value;
        }

        public PasswordDialog() : this("Enter Password", "Enter password:")
        {
        }

        public PasswordDialog(string title) : this(title, "Enter password:")
        {
        }

        public PasswordDialog(string title, string prompt) : this(title, prompt, requireConfirmation: false)
        {
        }

        public PasswordDialog(string title, string prompt, bool requireConfirmation)
        {
            InitializeComponent();

            this.Text = title;
            this.HeaderText = title;
            this.Prompt = prompt;
            this.RequireConfirmation = requireConfirmation;
        }

        /// <summary>
        /// The dialog enforces its own rules, so no caller can reach OK with an invalid or
        /// unconfirmed password regardless of which entry point it used.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK && !ValidatePassword(out var error))
            {
                e.Cancel = true;
                MessageBox.Show(this, error, "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (RequireConfirmation && txtPassword.Text == txtConfirm.Text)
                {
                    txtPassword.Clear();
                    txtConfirm.Clear();
                    txtPassword.Focus();
                }
                else if (RequireConfirmation)
                {
                    txtConfirm.Clear();
                    txtConfirm.Focus();
                }
                else
                {
                    txtPassword.Focus();
                }
                DialogResult = DialogResult.None;
                return;
            }
            base.OnFormClosing(e);
        }

        private void PasswordDialog_Shown(object sender, EventArgs e)
        {
            txtPassword.Focus();
            txtPassword.Select();
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            txtConfirm.UseSystemPasswordChar = !chkShowPassword.Checked;
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

        /// <summary>
        /// Shows the password dialog and returns the entered password, or null if cancelled.
        /// </summary>
        public static string? ShowDialog(IWin32Window owner, string title, string prompt = "Enter password:")
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

            if (string.IsNullOrEmpty(Password))
            {
                errorMessage = "Password cannot be empty.";
                return false;
            }

            if (Password.Length < MinimumPasswordLength)
            {
                errorMessage = $"Password must be at least {MinimumPasswordLength} characters long.";
                return false;
            }

            if (RequireConfirmation && !string.Equals(Password, txtConfirm.Text, StringComparison.Ordinal))
            {
                errorMessage = "The two passwords do not match. Encrypted content cannot be recovered " +
                               "without the exact password, so both entries must be identical.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Shows the dialog with password validation. Returns null if the user cancelled.
        /// Pass <paramref name="requireConfirmation"/> for any path that ENCRYPTS.
        /// </summary>
        public static string? ShowDialogWithValidation(
            IWin32Window owner,
            string title,
            string prompt = "Enter password:",
            bool requireConfirmation = false)
        {
            using var dialog = new PasswordDialog(title, prompt, requireConfirmation);
            // The dialog itself refuses to close with OK unless the password is valid, so a
            // single ShowDialog is enough — no retry loop, no chance of slipping past validation.
            return dialog.ShowDialog(owner) == DialogResult.OK ? dialog.Password : null;
        }
    }
}