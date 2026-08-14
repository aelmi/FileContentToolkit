using System;
using System.Drawing;
using System.Windows.Forms;

namespace CodeShuttle.UI
{
    public class SplitButton : Button
    {
        public ContextMenuStrip? DropDownMenu { get; set; }

        /// <summary>
        /// Width of the drop-down half, in logical units. Scaled to the current DPI on use: the
        /// previous fixed 22 device pixels was already under the ~24px minimum click target and
        /// shrank further, not larger, on a high-DPI display.
        /// </summary>
        public int DropDownWidth { get; set; } = 24;

        public bool ShowSplit { get; set; } = true;

        /// <summary>Colour of the hairline between the button and its drop-down half.</summary>
        /// <remarks>
        /// Set from <c>ThemeTokens.TextDisabled</c> by the applier. The former hardcoded
        /// (180,180,180) measured 2.07:1 against a white button, well under the 3:1 that WCAG
        /// requires of a non-text interface component.
        /// </remarks>
        public Color SeparatorColor { get; set; } = SystemColors.ControlDark;

        /// <summary>Caret colour when the button is disabled. Was (200,200,200) at 1.67:1.</summary>
        public Color DisabledCaretColor { get; set; } = SystemColors.GrayText;

        private bool _openingFromSplit;

        // The caret font used to be allocated inside OnPaint, so every hover and every resize
        // leaked a GDI handle. It is now cached and rebuilt only when the button's font changes.
        private Font? _caretFont;

        private int ScaledDropDownWidth => LogicalToDeviceUnits(DropDownWidth);

        private Rectangle SplitRect =>
            new Rectangle(Width - ScaledDropDownWidth, 0, ScaledDropDownWidth, Height);

        private Font CaretFont =>
            _caretFont ??= new Font(Font.FontFamily, Math.Max(8, Font.Size - 1), FontStyle.Bold);

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _caretFont?.Dispose();
            _caretFont = null;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _caretFont?.Dispose();
                _caretFont = null;
            }
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            if (!ShowSplit) return;

            var g = pevent.Graphics;
            var rect = ClientRectangle;
            var arrowRect = SplitRect;

            using (var separatorPen = new Pen(SeparatorColor))
                g.DrawLine(separatorPen, arrowRect.Left, rect.Top + 4, arrowRect.Left, rect.Bottom - 4);

            // Dropdown caret — use the button's own ForeColor so it stays readable
            // regardless of the button's BackColor.
            TextRenderer.DrawText(
                g,
                "▼",
                CaretFont,
                arrowRect,
                Enabled ? ForeColor : DisabledCaretColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );
        }

        /// <summary>
        /// Announces as a split button rather than a plain one, so assistive technology tells the
        /// user a menu exists at all.
        /// </summary>
        protected override AccessibleObject CreateAccessibilityInstance() =>
            new SplitButtonAccessibleObject(this);

        private sealed class SplitButtonAccessibleObject : ButtonBaseAccessibleObject
        {
            public SplitButtonAccessibleObject(SplitButton owner) : base(owner) { }
            public override AccessibleRole Role => AccessibleRole.ButtonDropDown;
        }

        /// <summary>Opens the drop-down half. Previously reachable only with a mouse.</summary>
        public void ShowDropDown()
        {
            if (DropDownMenu == null) return;
            DropDownMenu.Show(this, new Point(0, Height));
        }

        /// <summary>
        /// Keyboard access to the drop-down.
        /// </summary>
        /// <remarks>
        /// The menu opened exclusively from <see cref="OnMouseDown"/>, with no handling of the
        /// conventional Alt+Down or F4, so a keyboard-only user could not open it at all. Handled
        /// in <c>ProcessCmdKey</c> rather than <c>OnKeyDown</c> because a Button consumes some of
        /// these before KeyDown sees them.
        /// </remarks>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (Focused && ShowSplit && DropDownMenu != null &&
                (keyData == Keys.F4 || keyData == (Keys.Alt | Keys.Down)))
            {
                ShowDropDown();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (ShowSplit && mevent.Button == MouseButtons.Left && SplitRect.Contains(mevent.Location))
            {
                _openingFromSplit = true; // mark dropdown click
                DropDownMenu?.Show(this, new Point(0, Height));
                return; // prevent normal click
            }
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (_openingFromSplit)
            {
                _openingFromSplit = false;
                return; // swallow click
            }
            base.OnMouseUp(mevent);
        }

        protected override void OnClick(EventArgs e)
        {
            if (_openingFromSplit)
            {
                _openingFromSplit = false;
                return; // don’t raise Click
            }
            base.OnClick(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}
