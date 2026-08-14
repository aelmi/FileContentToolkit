using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Automation;
using CodeShuttle.Theming;

namespace CodeShuttle.Controls
{
    /// <summary>
    /// A transient, non-modal confirmation that appears in the bottom-right of its host form and
    /// dismisses itself.
    /// </summary>
    /// <remarks>
    /// The product used a modal <see cref="MessageBox"/> for routine successes — "Output exported
    /// successfully", "Preset saved" — each of which stops the user to acknowledge something that
    /// already worked. Errors and destructive confirmations deliberately keep their message box;
    /// this replaces only the cases where the answer is always OK.
    ///
    /// Implemented as a child control rather than a borderless form on purpose: a form would take
    /// activation away from whatever the user was typing into, and would have to be tracked and
    /// disposed independently of the window that raised it.
    /// </remarks>
    public sealed class Toast : Control
    {
        private const int LifetimeMs = 3200;
        private const int MarginPx = 16;
        private const int PaddingX = 16;
        private const int PaddingY = 11;
        private const int CornerRadius = 6;

        private readonly System.Windows.Forms.Timer _timer;
        private readonly ToastKind _kind;

        public enum ToastKind
        {
            Info,
            Success,
        }

        private Toast(string message, ToastKind kind)
        {
            _kind = kind;
            Text = message;
            TabStop = false;
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            // Announced to a screen reader as a status message rather than an interactive control.
            AccessibleRole = AccessibleRole.Alert;
            AccessibleName = message;

            _timer = new System.Windows.Forms.Timer { Interval = LifetimeMs };
            _timer.Tick += (_, _) => Close();
        }

        /// <summary>
        /// Shows <paramref name="message"/> over <paramref name="host"/>. Safe to call from a
        /// background thread and safe to call when the form is closing.
        /// </summary>
        public static void Show(Form host, string message, ToastKind kind = ToastKind.Success)
        {
            if (host == null || string.IsNullOrWhiteSpace(message)) return;
            if (host.IsDisposed || !host.IsHandleCreated) return;

            if (host.InvokeRequired)
            {
                try { host.BeginInvoke(new Action(() => Show(host, message, kind))); }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                return;
            }

            // Only one at a time: stacking them turns a confirmation into a wall.
            foreach (Control existing in host.Controls)
            {
                if (existing is Toast old) { old.Close(); break; }
            }

            var toast = new Toast(message, kind);
            host.Controls.Add(toast);
            toast.BringToFront();
            toast.ApplyTokens();
            toast.LayoutInHost(host);
            toast.Visible = true;
            toast._timer.Start();

            AnnounceToScreenReader(toast, message);
        }

        /// <summary>
        /// Pushes the text through UI Automation. Without this the toast is drawn and vanishes
        /// with nothing announced, which for a screen-reader user is silence where a sighted user
        /// gets confirmation.
        /// </summary>
        private static void AnnounceToScreenReader(Control control, string message)
        {
            try
            {
                control.AccessibilityObject?.RaiseAutomationNotification(
                    AutomationNotificationKind.ActionCompleted,
                    AutomationNotificationProcessing.MostRecent,
                    message);
            }
            catch (NotSupportedException)
            {
                // Older Windows builds do not implement the notification event.
            }
        }

        private void ApplyTokens()
        {
            var t = ThemeManager.Tokens;
            BackColor = _kind == ToastKind.Success ? t.Success : t.Neutral;
            ForeColor = t.AccentText;
            Font = ThemeFonts.Get(FontRole.Body);
        }

        private void LayoutInHost(Form host)
        {
            var size = TextRenderer.MeasureText(Text, Font, new Size(420, 0), TextFormatFlags.WordBreak);
            Size = new Size(size.Width + PaddingX * 2, size.Height + PaddingY * 2);

            Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Location = new Point(
                Math.Max(MarginPx, host.ClientSize.Width - Width - MarginPx),
                Math.Max(MarginPx, host.ClientSize.Height - Height - MarginPx * 2));
        }

        private void Close()
        {
            _timer.Stop();
            Parent?.Controls.Remove(this);
            Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var path = RoundedRect(r, CornerRadius))
            using (var fill = new SolidBrush(BackColor))
            {
                g.FillPath(fill, path);
            }

            TextRenderer.DrawText(
                g, Text, Font,
                new Rectangle(PaddingX, PaddingY, Width - PaddingX * 2, Height - PaddingY * 2),
                ForeColor,
                TextFormatFlags.WordBreak | TextFormatFlags.Left);
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _timer.Dispose();
            base.Dispose(disposing);
        }
    }
}
