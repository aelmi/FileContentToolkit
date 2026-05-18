using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit.UI
{
    /// <summary>Small themed text-prompt dialog (replaces ad-hoc inline prompt forms).</summary>
    public static class ThemedPrompt
    {
        public static string? Show(IWin32Window? owner, string title, string prompt, string initial = "")
        {
            using var f = new Form
            {
                Text = title,
                ClientSize = new Size(440, 190),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false
            };
            Theme.ApplyForm(f);

            f.Controls.Add(Theme.BuildHeader(title));

            var body = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.White,
                Padding = new Padding(20, 16, 20, 16)
            };

            var lbl = new Label
            {
                Text = prompt,
                Left = 0,
                Top = 0,
                AutoSize = true,
                ForeColor = Theme.BodyText,
                Font = Theme.BodyFont
            };
            var tb = new TextBox
            {
                Left = 0,
                Top = 28,
                Width = 400,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Text = initial,
                Font = Theme.BodyFont,
                BorderStyle = BorderStyle.FixedSingle
            };
            body.Controls.Add(lbl);
            body.Controls.Add(tb);

            var bottom = Theme.BuildBottomBar();
            var ok = Theme.PrimaryButton("OK");
            ok.Size = new Size(85, 34);
            ok.DialogResult = DialogResult.OK;
            var cancel = Theme.SecondaryButton("Cancel");
            cancel.Size = new Size(85, 34);
            cancel.DialogResult = DialogResult.Cancel;
            bottom.Resize += (s, e) =>
            {
                ok.Left = bottom.ClientSize.Width - 20 - ok.Width;
                ok.Top = (bottom.ClientSize.Height - ok.Height) / 2;
                cancel.Left = ok.Left - cancel.Width - 8;
                cancel.Top = ok.Top;
            };
            bottom.Controls.Add(ok);
            bottom.Controls.Add(cancel);

            f.Controls.Add(bottom);
            f.Controls.Add(body);
            body.BringToFront();

            f.AcceptButton = ok;
            f.CancelButton = cancel;

            return f.ShowDialog(owner) == DialogResult.OK ? tb.Text.Trim() : null;
        }
    }
}
