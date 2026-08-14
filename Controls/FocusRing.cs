using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Controls
{
    /// <summary>
    /// Draws a visible focus ring on flat buttons and check boxes.
    /// </summary>
    /// <remarks>
    /// Every button in the product is <see cref="FlatStyle.Flat"/> with
    /// <c>FlatAppearance.BorderSize = 0</c>, which leaves only Windows' 1px dotted focus rectangle
    /// — effectively invisible, and completely invisible on the saturated accent and success
    /// fills. A keyboard-only user could not tell where focus was.
    ///
    /// Implemented by subscribing to Paint rather than by subclassing, because subclassing would
    /// mean changing the declared type of every control in eleven Designer files. The colour is
    /// read from <see cref="ThemeManager"/> at paint time, so the ring follows a theme change
    /// without needing to be reattached.
    /// </remarks>
    public static class FocusRing
    {
        private const int Thickness = 2;
        private const int Inset = 1;

        /// <summary>Controls already wired, so a repeated theme apply cannot stack handlers.</summary>
        private static readonly ConditionalWeakTable<Control, object> Attached = new();

        /// <summary>Wires every flat button and check box beneath <paramref name="root"/>.</summary>
        public static void AttachAll(Control root)
        {
            if (root == null) return;

            if (root is ButtonBase button && button.FlatStyle == FlatStyle.Flat)
                Attach(button);

            foreach (Control child in root.Controls) AttachAll(child);
        }

        private static void Attach(ButtonBase button)
        {
            if (Attached.TryGetValue(button, out _)) return;
            Attached.Add(button, new object());

            button.Paint += (s, e) =>
            {
                if (s is not Control c || !c.Focused) return;

                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var r = new Rectangle(
                    Inset,
                    Inset,
                    c.Width - (Inset * 2) - 1,
                    c.Height - (Inset * 2) - 1);
                if (r.Width <= 0 || r.Height <= 0) return;

                using var pen = new Pen(ThemeManager.Tokens.BorderFocus, Thickness);
                g.DrawRectangle(pen, r);
            };

            // A flat button does not repaint on focus change by itself, so the ring would appear
            // only after some other event happened to invalidate it.
            button.GotFocus += (s, e) => ((Control)s!).Invalidate();
            button.LostFocus += (s, e) => ((Control)s!).Invalidate();
        }
    }
}
