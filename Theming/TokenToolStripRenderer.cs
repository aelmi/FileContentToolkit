using System.Drawing;
using System.Windows.Forms;

namespace CodeShuttle.Theming
{
    /// <summary>
    /// Token-driven port of the former <c>Theme.DarkColorTable</c>.
    /// </summary>
    /// <remarks>
    /// The original was forty-eight lines of hard-won correctness and is reproduced here override
    /// for override — menus and tool strips ignore <c>BackColor</c> entirely and paint their own
    /// gradients, so without this table every popup in the product reverted to the system light
    /// gradient no matter what the rest of the theming did. The only change is the source of each
    /// colour: a token lookup instead of a hardcoded <c>Color.FromArgb</c>, which is what makes the
    /// table work for the light palette as well rather than for dark alone.
    /// </remarks>
    internal sealed class TokenColorTable : ProfessionalColorTable
    {
        private readonly ThemeTokens _t;

        public TokenColorTable(ThemeTokens tokens)
        {
            _t = tokens;
            UseSystemColors = false;
        }

        /// <summary>The pressed state, one step beyond <see cref="ThemeTokens.Selection"/>.</summary>
        private Color Pressed => Blend(_t.Selection, _t.Accent, 0.28f);

        private static Color Blend(Color a, Color b, float amount)
        {
            int Mix(int x, int y) => (int)(x + ((y - x) * amount));
            return Color.FromArgb(Mix(a.R, b.R), Mix(a.G, b.G), Mix(a.B, b.B));
        }

        // Surfaces
        public override Color ToolStripDropDownBackground => _t.SurfaceAlt;
        public override Color MenuStripGradientBegin => _t.SurfaceAlt;
        public override Color MenuStripGradientEnd => _t.SurfaceAlt;
        public override Color ImageMarginGradientBegin => _t.SurfaceAlt;
        public override Color ImageMarginGradientMiddle => _t.SurfaceAlt;
        public override Color ImageMarginGradientEnd => _t.SurfaceAlt;

        // Status / tool strip back
        public override Color ToolStripGradientBegin => _t.SurfaceAlt;
        public override Color ToolStripGradientMiddle => _t.SurfaceAlt;
        public override Color ToolStripGradientEnd => _t.SurfaceAlt;
        public override Color ToolStripContentPanelGradientBegin => _t.SurfaceAlt;
        public override Color ToolStripContentPanelGradientEnd => _t.SurfaceAlt;
        public override Color ToolStripPanelGradientBegin => _t.SurfaceAlt;
        public override Color ToolStripPanelGradientEnd => _t.SurfaceAlt;
        public override Color StatusStripGradientBegin => _t.SurfaceAlt;
        public override Color StatusStripGradientEnd => _t.SurfaceAlt;

        // Hover / selected
        public override Color MenuItemSelected => _t.Selection;
        public override Color MenuItemSelectedGradientBegin => _t.Selection;
        public override Color MenuItemSelectedGradientEnd => _t.Selection;
        public override Color MenuItemPressedGradientBegin => Pressed;
        public override Color MenuItemPressedGradientMiddle => Pressed;
        public override Color MenuItemPressedGradientEnd => Pressed;
        public override Color ButtonSelectedGradientBegin => _t.Selection;
        public override Color ButtonSelectedGradientMiddle => _t.Selection;
        public override Color ButtonSelectedGradientEnd => _t.Selection;
        public override Color ButtonPressedGradientBegin => Pressed;
        public override Color ButtonPressedGradientMiddle => Pressed;
        public override Color ButtonPressedGradientEnd => Pressed;

        // Borders / separators
        public override Color MenuBorder => _t.Border;
        public override Color MenuItemBorder => _t.Selection;
        public override Color ButtonSelectedBorder => _t.Selection;
        public override Color SeparatorDark => _t.Border;
        public override Color SeparatorLight => _t.Border;

        // Checked / disabled
        public override Color CheckBackground => _t.Selection;
        public override Color CheckPressedBackground => Pressed;
        public override Color CheckSelectedBackground => _t.Selection;
        public override Color GripDark => _t.Border;
        public override Color GripLight => _t.SurfaceAlt;
    }

    /// <summary>
    /// The renderer installed on <see cref="ToolStripManager"/> for the active palette.
    /// </summary>
    public sealed class TokenToolStripRenderer : ToolStripProfessionalRenderer
    {
        public TokenToolStripRenderer(ThemeTokens tokens) : base(new TokenColorTable(tokens))
        {
            Tokens = tokens;
            RoundedEdges = false;
        }

        public ThemeTokens Tokens { get; }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            // The professional renderer picks its own disabled/selected text colours from system
            // colours, which are wrong for a dark palette.
            if (!e.Item.Enabled) e.TextColor = Tokens.TextDisabled;
            else if (e.Item.Selected || e.Item.Pressed) e.TextColor = Tokens.TextPrimary;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = e.Item?.Enabled == false ? Tokens.TextDisabled : Tokens.TextPrimary;
            base.OnRenderArrow(e);
        }
    }
}
