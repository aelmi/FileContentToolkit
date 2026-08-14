using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeShuttle.Controls;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    public enum MessageKind
    {
        Info,
        Warning,
        Error,
    }

    /// <summary>
    /// The product's modal message surface: a themed dialog that stays until dismissed and whose
    /// text can be selected and copied.
    /// </summary>
    /// <remarks>
    /// Failures used to be reported through <see cref="Toast"/>, which dismisses itself after
    /// 3.2 seconds and paints its text rather than hosting it. A user who looked away missed the
    /// message entirely, and a user who read it could not copy it into a bug report or a search.
    /// Toast is right for "saved" and "cancelled"; it was never right for "this did not work".
    /// </remarks>
    public partial class MessageDialog : ThemedForm
    {
        private const int ContentWidth = 452;
        private const int MinBodyHeight = 40;
        private const int MaxMessageHeight = 260;
        private const int DetailsHeight = 160;
        private const int BodyPadding = 20;

        private readonly MessageKind _kind;
        private readonly string _title;
        private readonly string _message;
        private readonly string _details;

        public MessageDialog(MessageKind kind, string title, string message, string? details = null)
        {
            _kind = kind;
            _title = string.IsNullOrWhiteSpace(title) ? "CodeShuttle" : title;
            _message = message ?? string.Empty;
            _details = details ?? string.Empty;

            InitializeComponent();

            Text = _title;
            lblHeaderTitle.Text = _title;
            // A multiline TextBox breaks on CRLF only. Callers write "\n\n" for a paragraph break
            // and a bare "\n" reaches the control as an unrenderable box, so both fields are
            // normalised rather than trusting every call site to use Environment.NewLine.
            txtMessage.Text = ToCrLf(_message);
            txtMessage.AccessibleName = _message;

            bool hasDetails = _details.Length > 0;
            txtDetails.Visible = hasDetails;
            txtDetails.Text = hasDetails ? ToCrLf(_details) : string.Empty;
            btnCopy.Text = hasDetails ? "Copy details" : "Copy";

            picIcon.Image = LoadIcon(kind);

            // Screen readers announce a dialog by its role; naming the role explicitly is what
            // distinguishes "error" from "notice" for a user who cannot see the glyph.
            AccessibleRole = AccessibleRole.Dialog;
            AccessibleDescription = _message;
        }

        /// <summary>
        /// The whole message as plain text — what both the Copy button and Ctrl+C put on the
        /// clipboard, and what a user pastes into a bug report.
        /// </summary>
        public string ToPlainText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("CodeShuttle — " + _kind.ToString().ToUpperInvariant() + ": " + _title);
            sb.AppendLine();
            sb.AppendLine(_message);
            if (_details.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Details:");
                sb.AppendLine(_details);
            }
            return sb.ToString();
        }

        /// <summary>Normalises any line-ending convention to the CRLF a multiline TextBox needs.</summary>
        private static string ToCrLf(string text) =>
            text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");

        private static Bitmap? LoadIcon(MessageKind kind)
        {
            var source = kind switch
            {
                MessageKind.Error => SystemIcons.Error,
                MessageKind.Warning => SystemIcons.Warning,
                _ => SystemIcons.Information,
            };

            // The 32x32 variant is requested explicitly: SystemIcons hands back whatever size the
            // shell cached, which on a scaled display is often 16x16 and stretches to mush.
            try
            {
                using var sized = new Icon(source, 32, 32);
                return sized.ToBitmap();
            }
            catch (ArgumentException)
            {
                return source.ToBitmap();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            // Fonts are assigned by the theme during base.OnLoad, so measuring before this point
            // would size the window to the designer's font rather than the one it will render in.
            base.OnLoad(e);
            LayoutForContent();

            // A TextBox selects its whole contents when it first takes focus, so the details pane
            // opened as a wall of highlight and Ctrl+C copied the stack trace instead of the
            // message. Dismissal is the common action, so the OK button holds focus.
            txtDetails.Select(0, 0);
            txtMessage.Select(0, 0);
            ActiveControl = btnOk;
        }

        protected override void ApplyTheme()
        {
            base.ApplyTheme();
            // A theme switch can change the body font, and a taller font in the same box clips the
            // last line. Re-measuring keeps the window honest.
            if (IsHandleCreated) LayoutForContent();
        }

        private void LayoutForContent()
        {
            int contentWidth = txtMessage.Width > 0 ? txtMessage.Width : ContentWidth;

            var measured = TextRenderer.MeasureText(
                _message, txtMessage.Font, new Size(contentWidth, 0), TextFormatFlags.WordBreak);

            int messageHeight = Math.Clamp(measured.Height + 4, MinBodyHeight, MaxMessageHeight);
            txtMessage.Height = messageHeight;
            txtMessage.ScrollBars = measured.Height + 4 > MaxMessageHeight ? ScrollBars.Vertical : ScrollBars.None;

            // The icon column and the text column are independent; the body is as tall as the
            // taller of the two.
            int bodyContent = Math.Max(messageHeight, picIcon.Height);

            if (txtDetails.Visible)
            {
                txtDetails.Top = txtMessage.Top + messageHeight + 12;
                txtDetails.Height = DetailsHeight;
                bodyContent = txtDetails.Top + txtDetails.Height - txtMessage.Top;
            }

            int bodyHeight = BodyPadding + bodyContent + BodyPadding;
            ClientSize = new Size(ClientSize.Width, pnlHeader.Height + bodyHeight + pnlBottom.Height);
        }

        /// <summary>
        /// Ctrl+C copies the whole message. A selection inside one of the text boxes wins, so a
        /// user who deliberately highlighted part of the details still gets just that part.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                if (ActiveControl is TextBoxBase { SelectionLength: > 0 })
                    return base.ProcessCmdKey(ref msg, keyData);

                CopyToClipboard();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void BtnCopy_Click(object? sender, EventArgs e) => CopyToClipboard();

        private void CopyToClipboard()
        {
            try
            {
                Clipboard.SetText(ToPlainText());
                Toast.Show(this, "Message copied to clipboard.");
            }
            catch (ExternalException)
            {
                // Another process holds the clipboard. Saying nothing would look like the button
                // does not work, and a nested message dialog here would be absurd.
                Toast.Show(this, "Could not copy: the clipboard is in use by another program.",
                    Toast.ToastKind.Info);
            }
        }
    }
}
