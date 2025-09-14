using System;
using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit.UI
{
    public class SplitButton : Button
    {
        public ContextMenuStrip? DropDownMenu { get; set; }
        public int DropDownWidth { get; set; } = 22;
        public bool ShowSplit { get; set; } = true;

        private bool _openingFromSplit = false;

        private Rectangle SplitRect => new Rectangle(Width - DropDownWidth, 0, DropDownWidth, Height);

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            if (!ShowSplit) return;

            var g = pevent.Graphics;
            var rect = ClientRectangle;
            var arrowRect = SplitRect;

            // Separator line
            using (var pen = new Pen(Color.FromArgb(180, 180, 180)))
            {
                g.DrawLine(pen, arrowRect.Left, rect.Top + 4, arrowRect.Left, rect.Bottom - 4);
            }

            // Dropdown caret
            TextRenderer.DrawText(
                g,
                "▼",
                new Font(Font.FontFamily, Math.Max(8, Font.Size - 1), FontStyle.Bold),
                arrowRect,
                Enabled ? Color.White : Color.FromArgb(200, 200, 200),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (ShowSplit && e.Button == MouseButtons.Left && SplitRect.Contains(e.Location))
            {
                _openingFromSplit = true; // mark dropdown click
                DropDownMenu?.Show(this, new Point(0, Height));
                return; // prevent normal click
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_openingFromSplit)
            {
                _openingFromSplit = false;
                return; // swallow click
            }
            base.OnMouseUp(e);
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
