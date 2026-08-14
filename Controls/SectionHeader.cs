using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Controls
{
    /// <summary>
    /// A rail section's heading: a small tracked-out label, an optional count pill, and an
    /// optional trailing action.
    /// </summary>
    /// <remarks>
    /// The rail previously used <see cref="GroupBox"/> frames, whose titles named controls
    /// ("File Extensions", "Selected Files") rather than steps, and whose etched borders drew more
    /// attention than the content inside them. A tracked-out uppercase label at small size
    /// organises the column without competing with it, and the count pill puts the number where
    /// the user is already looking instead of only in the status bar.
    /// </remarks>
    public sealed class SectionHeader : Control
    {
        private const int PillPadX = 7;
        private const int ActionGap = 12;
        private const float Tracking = 1.4f;

        private string _title = "";
        private string _count = "";
        private string _actionText = "";
        private Rectangle _actionBounds;
        private bool _actionHot;

        /// <summary>Raised when the trailing action is clicked or activated from the keyboard.</summary>
        public event EventHandler? ActionClicked;

        public SectionHeader()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.ResizeRedraw | ControlStyles.UserPaint
                     | ControlStyles.SupportsTransparentBackColor, true);
            Dock = DockStyle.Top;
            AutoSize = true;
            TabStop = false;
            AccessibleRole = AccessibleRole.Grouping;
        }

        public string Title
        {
            get => _title;
            set { _title = value ?? ""; AccessibleName = _title; Invalidate(); }
        }

        /// <summary>Count shown as a pill beside the title. Empty hides it.</summary>
        public string Count
        {
            get => _count;
            set { _count = value ?? ""; Invalidate(); }
        }

        /// <summary>Trailing action label, e.g. "Presets ▾". Empty hides it.</summary>
        public string ActionText
        {
            get => _actionText;
            set { _actionText = value ?? ""; Invalidate(); }
        }

        public override Size GetPreferredSize(Size proposedSize) =>
            new(proposedSize.Width, Font.Height + Padding.Vertical + 6);

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            PerformLayout();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var t = ThemeManager.Tokens;
            int midY = Padding.Top + (Height - Padding.Vertical) / 2;

            int x = Padding.Left;
            x += DrawTracked(g, _title.ToUpperInvariant(), x, midY, t.TextDisabled);

            if (_count.Length > 0)
            {
                x += 8;
                var size = TextRenderer.MeasureText(_count, Font, Size.Empty, TextFormatFlags.NoPadding);
                var pill = new Rectangle(x, midY - (Font.Height + 2) / 2, size.Width + PillPadX * 2, Font.Height + 2);
                using (var path = Rounded(pill, pill.Height / 2))
                using (var fill = new SolidBrush(t.Selection))
                    g.FillPath(fill, path);
                TextRenderer.DrawText(g, _count, Font, pill, t.AccentOnSurface,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                x = pill.Right;
            }

            if (_actionText.Length > 0)
            {
                var size = TextRenderer.MeasureText(_actionText, Font, Size.Empty, TextFormatFlags.NoPadding);
                _actionBounds = new Rectangle(
                    Width - Padding.Right - size.Width - 4, midY - Font.Height / 2 - 2,
                    size.Width + 8, Font.Height + 4);

                // Never let the action overlap the title: at narrow widths the title wins, because
                // it is the thing that identifies the section.
                if (_actionBounds.Left > x + ActionGap)
                {
                    TextRenderer.DrawText(g, _actionText, Font, _actionBounds,
                        _actionHot ? t.TextPrimary : t.AccentOnSurface,
                        TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                }
                else
                {
                    _actionBounds = Rectangle.Empty;
                }
            }
            else
            {
                _actionBounds = Rectangle.Empty;
            }
        }

        /// <summary>
        /// Draws text with extra spacing between glyphs and returns the width consumed. GDI+ has no
        /// letter-spacing, and at this size the tracking is what makes an uppercase label read as a
        /// label rather than as shouting.
        /// </summary>
        private int DrawTracked(Graphics g, string text, int x, int midY, Color colour)
        {
            int cursor = x;
            int top = midY - Font.Height / 2;
            foreach (char ch in text)
            {
                var s = ch.ToString();
                TextRenderer.DrawText(g, s, Font, new Point(cursor, top), colour, TextFormatFlags.NoPadding);
                cursor += TextRenderer.MeasureText(s, Font, Size.Empty, TextFormatFlags.NoPadding).Width
                          + (int)Math.Round(Tracking);
            }
            return cursor - x;
        }

        private static GraphicsPath Rounded(Rectangle r, int radius)
        {
            int d = Math.Max(2, radius * 2);
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool hot = !_actionBounds.IsEmpty && _actionBounds.Contains(e.Location);
            if (hot == _actionHot) return;
            _actionHot = hot;
            Cursor = hot ? Cursors.Hand : Cursors.Default;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!_actionHot) return;
            _actionHot = false;
            Cursor = Cursors.Default;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left && !_actionBounds.IsEmpty && _actionBounds.Contains(e.Location))
                ActionClicked?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>The rectangle a drop-down opened by the action should align to.</summary>
        public Rectangle ActionBounds => _actionBounds;
    }
}
