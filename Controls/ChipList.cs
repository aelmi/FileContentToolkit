using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Automation;
using CodeShuttle.Theming;

namespace CodeShuttle.Controls
{
    public sealed class ChipEventArgs : EventArgs
    {
        public ChipEventArgs(string value) => Value = value;
        public string Value { get; }
    }

    /// <summary>
    /// A wrapping row of removable pills, plus a trailing "add" pill that opens a menu.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This replaces the file-extension list box and its three stacked buttons — Add, Remove and
    /// Refresh, in three different colours. That arrangement showed five extensions at a time
    /// inside a scrolling box, and removing one meant selecting it in the box and then travelling
    /// to a separate red button. Every extension is now visible at once and removal is a click on
    /// the thing being removed.
    /// </para>
    /// <para>
    /// Owner-drawn as a single control rather than a <see cref="FlowLayoutPanel"/> of child
    /// controls: a panel of twenty buttons costs twenty window handles, flickers as it re-flows,
    /// and gives a screen reader twenty unrelated stops. One control means one handle, one paint,
    /// and one place to declare the keyboard contract.
    /// </para>
    /// <para>
    /// All geometry is derived from the current font, so it scales with the theme's type scale and
    /// with per-monitor DPI without carrying any fixed pixel sizes.
    /// </para>
    /// </remarks>
    public sealed class ChipList : Control
    {
        private const int GapX = 6;
        private const int GapY = 6;
        private const int PadLeft = 10;
        private const int PadRight = 7;
        private const int CloseGap = 6;

        private sealed class Chip
        {
            public string Text = "";
            public Rectangle Bounds;
            public Rectangle Close;
            public bool IsAdd;
        }

        private readonly List<string> _items = new();
        private readonly List<Chip> _chips = new();

        private int _hotIndex = -1;
        private bool _hotClose;
        private int _focusIndex = -1;
        private Size _preferred = Size.Empty;

        /// <summary>Raised when a chip's remove affordance is activated.</summary>
        public event EventHandler<ChipEventArgs>? ChipRemoved;

        /// <summary>Raised when the trailing add pill is activated.</summary>
        public event EventHandler? AddRequested;

        public ChipList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.ResizeRedraw | ControlStyles.UserPaint
                     | ControlStyles.SupportsTransparentBackColor, true);
            TabStop = true;
            AutoSize = true;
            Dock = DockStyle.Top;
            AccessibleRole = AccessibleRole.List;
            AccessibleName = "File extensions";
            AccessibleDescription =
                "Only files with these extensions are collected. "
                + "Use the arrow keys to move between extensions and Delete to remove one.";
        }

        /// <summary>Text of the trailing pill. Empty hides it.</summary>
        public string AddText { get; set; } = "+ add";

        /// <summary>Menu opened by the add pill. Optional — <see cref="AddRequested"/> fires regardless.</summary>
        public ContextMenuStrip? AddMenu { get; set; }

        public IReadOnlyList<string> Items => _items;

        /// <summary>Replaces the contents. Cheap enough to call on every model change.</summary>
        public void SetItems(IEnumerable<string>? items)
        {
            var incoming = items?.Where(i => !string.IsNullOrWhiteSpace(i)).ToList() ?? new List<string>();
            if (_items.SequenceEqual(incoming, StringComparer.Ordinal)) return;

            _items.Clear();
            _items.AddRange(incoming);
            if (_focusIndex >= _items.Count) _focusIndex = _items.Count - 1;

            Rebuild();
        }

        private void Rebuild()
        {
            _preferred = Size.Empty;
            _chips.Clear();
            PerformLayout();
            Invalidate();
        }

        // ---------------------------------------------------------------- layout

        private int ChipHeight => Font.Height + 8;

        private void BuildChips(int availableWidth)
        {
            _chips.Clear();
            if (availableWidth <= 0) return;

            int h = ChipHeight;
            int closeW = Font.Height;               // square hit target for the ✕
            int x = Padding.Left, y = Padding.Top;
            int lineRight = availableWidth - Padding.Right;

            foreach (var text in _items)
            {
                int textW = TextRenderer.MeasureText(text, Font, Size.Empty, TextFormatFlags.NoPadding).Width;
                int w = PadLeft + textW + CloseGap + closeW + PadRight;

                if (x > Padding.Left && x + w > lineRight)
                {
                    x = Padding.Left;
                    y += h + GapY;
                }

                var bounds = new Rectangle(x, y, w, h);
                _chips.Add(new Chip
                {
                    Text = text,
                    Bounds = bounds,
                    Close = new Rectangle(bounds.Right - PadRight - closeW, bounds.Y + (h - closeW) / 2, closeW, closeW),
                });
                x += w + GapX;
            }

            if (!string.IsNullOrEmpty(AddText))
            {
                int textW = TextRenderer.MeasureText(AddText, Font, Size.Empty, TextFormatFlags.NoPadding).Width;
                int w = PadLeft + textW + PadLeft;
                if (x > Padding.Left && x + w > lineRight)
                {
                    x = Padding.Left;
                    y += h + GapY;
                }
                _chips.Add(new Chip { Text = AddText, Bounds = new Rectangle(x, y, w, h), IsAdd = true });
            }

            int bottom = _chips.Count > 0 ? _chips[^1].Bounds.Bottom : Padding.Top;
            _preferred = new Size(availableWidth, bottom + Padding.Bottom);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            // Docked to the top, so the width is dictated and only the height is ours to choose.
            int width = Width > 0 ? Width : proposedSize.Width;
            if (width <= 0) return new Size(0, ChipHeight + Padding.Vertical);

            BuildChips(width);
            return _preferred;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            BuildChips(Width);
            base.OnLayout(levent);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Rebuild();
        }

        // ---------------------------------------------------------------- painting

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_chips.Count == 0) BuildChips(Width);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var t = ThemeManager.Tokens;
            bool showFocus = Focused && _focusIndex >= 0;

            for (int i = 0; i < _chips.Count; i++)
            {
                var chip = _chips[i];
                bool hot = i == _hotIndex;
                bool focused = showFocus && i == _focusIndex;

                using var path = Rounded(chip.Bounds, chip.Bounds.Height / 2);

                if (chip.IsAdd)
                {
                    using var pen = new Pen(hot ? t.AccentOnSurface : t.Border) { DashStyle = DashStyle.Dash };
                    g.DrawPath(pen, path);
                    TextRenderer.DrawText(g, chip.Text, Font, chip.Bounds,
                        hot ? t.AccentOnSurface : t.TextSecondary,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                }
                else
                {
                    using (var fill = new SolidBrush(hot ? t.Selection : t.Selection))
                        g.FillPath(fill, path);

                    var textRect = new Rectangle(
                        chip.Bounds.X + PadLeft, chip.Bounds.Y,
                        chip.Close.Left - CloseGap - (chip.Bounds.X + PadLeft), chip.Bounds.Height);
                    TextRenderer.DrawText(g, chip.Text, Font, textRect, t.AccentOnSurface,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding
                        | TextFormatFlags.EndEllipsis);

                    DrawClose(g, chip.Close, hot && _hotClose ? t.Danger : t.AccentOnSurface, hot && _hotClose);
                }

                if (focused)
                {
                    using var ring = new Pen(t.BorderFocus, 1.6f);
                    var r = chip.Bounds; r.Inflate(2, 2);
                    using var ringPath = Rounded(r, r.Height / 2);
                    g.DrawPath(ring, ringPath);
                }
            }
        }

        private static void DrawClose(Graphics g, Rectangle box, Color colour, bool emphasised)
        {
            if (emphasised)
            {
                using var bg = new SolidBrush(Color.FromArgb(28, colour));
                g.FillEllipse(bg, box);
            }

            // Inset so the glyph reads as a mark rather than filling its hit target.
            int inset = Math.Max(3, box.Width / 3);
            var r = Rectangle.Inflate(box, -inset, -inset);
            using var pen = new Pen(Color.FromArgb(emphasised ? 255 : 170, colour), 1.5f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
            };
            g.DrawLine(pen, r.Left, r.Top, r.Right, r.Bottom);
            g.DrawLine(pen, r.Right, r.Top, r.Left, r.Bottom);
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

        // ---------------------------------------------------------------- mouse

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int index = IndexAt(e.Location);
            bool onClose = index >= 0 && !_chips[index].IsAdd && _chips[index].Close.Contains(e.Location);

            if (index == _hotIndex && onClose == _hotClose) return;

            _hotIndex = index;
            _hotClose = onClose;
            Cursor = index >= 0 ? Cursors.Hand : Cursors.Default;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hotIndex < 0 && !_hotClose) return;
            _hotIndex = -1;
            _hotClose = false;
            Cursor = Cursors.Default;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();

            int index = IndexAt(e.Location);
            if (index < 0) return;

            _focusIndex = index;
            Invalidate();

            if (e.Button != MouseButtons.Left) return;

            var chip = _chips[index];
            if (chip.IsAdd) { OpenAddMenu(chip.Bounds); return; }

            // The whole pill is not a remove target: clicking the label must not delete the thing
            // the user was only pointing at. Only the ✕ removes.
            if (chip.Close.Contains(e.Location)) Remove(chip.Text);
        }

        private int IndexAt(Point p)
        {
            for (int i = 0; i < _chips.Count; i++)
                if (_chips[i].Bounds.Contains(p)) return i;
            return -1;
        }

        // ---------------------------------------------------------------- keyboard

        protected override bool IsInputKey(Keys keyData) => keyData switch
        {
            Keys.Left or Keys.Right or Keys.Up or Keys.Down or Keys.Home or Keys.End => true,
            _ => base.IsInputKey(keyData),
        };

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (_focusIndex < 0 && _chips.Count > 0) _focusIndex = 0;
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (_chips.Count == 0) return;

            switch (e.KeyCode)
            {
                case Keys.Left or Keys.Up:
                    MoveFocus(-1); e.Handled = true; break;

                case Keys.Right or Keys.Down:
                    MoveFocus(1); e.Handled = true; break;

                case Keys.Home:
                    _focusIndex = 0; Announce(); Invalidate(); e.Handled = true; break;

                case Keys.End:
                    _focusIndex = _chips.Count - 1; Announce(); Invalidate(); e.Handled = true; break;

                case Keys.Delete or Keys.Back:
                    if (Current is { IsAdd: false } chip) { Remove(chip.Text); e.Handled = true; }
                    break;

                case Keys.Enter or Keys.Space:
                    if (Current is { } c)
                    {
                        if (c.IsAdd) OpenAddMenu(c.Bounds);
                        else Remove(c.Text);
                        e.Handled = true;
                    }
                    break;
            }
        }

        private Chip? Current =>
            _focusIndex >= 0 && _focusIndex < _chips.Count ? _chips[_focusIndex] : null;

        private void MoveFocus(int delta)
        {
            if (_focusIndex < 0) _focusIndex = 0;
            else _focusIndex = Math.Clamp(_focusIndex + delta, 0, _chips.Count - 1);
            Announce();
            Invalidate();
        }

        private void Announce()
        {
            if (Current is not { } chip) return;
            try
            {
                AccessibilityObject?.RaiseAutomationNotification(
                    AutomationNotificationKind.Other,
                    AutomationNotificationProcessing.MostRecent,
                    chip.IsAdd ? "Add extension" : $"{chip.Text}, {_focusIndex + 1} of {_items.Count}");
            }
            catch (NotSupportedException)
            {
                // Older Windows builds do not implement the notification event.
            }
        }

        private void Remove(string value)
        {
            ChipRemoved?.Invoke(this, new ChipEventArgs(value));
        }

        private void OpenAddMenu(Rectangle anchor)
        {
            AddRequested?.Invoke(this, EventArgs.Empty);
            AddMenu?.Show(this, new Point(anchor.Left, anchor.Bottom + 2));
        }
    }
}
