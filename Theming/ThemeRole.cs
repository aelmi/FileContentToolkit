using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace CodeShuttle.Theming
{
    /// <summary>
    /// What a control <em>is</em>, so the applier can decide what colour it should be. The
    /// predecessor inferred this backwards, by fuzzy-matching the literal RGB a control already
    /// had; anything outside its eight hardcoded cases silently stayed light.
    /// </summary>
    public enum ThemeRole
    {
        /// <summary>Colour is chosen from the control's type. Correct for the large majority.</summary>
        Default = 0,

        /// <summary>Window/panel background.</summary>
        Surface,

        /// <summary>Raised bar: toolbars, button strips, section headers.</summary>
        SurfaceAlt,

        /// <summary>Recessed content area.</summary>
        SurfaceSunken,

        /// <summary>Accent-filled page header strip.</summary>
        Header,

        /// <summary>Title text on a <see cref="Header"/>.</summary>
        HeaderTitle,

        /// <summary>Subtitle text on a <see cref="Header"/>.</summary>
        HeaderSubtitle,

        /// <summary>Pale information/caution strip.</summary>
        Banner,

        /// <summary>Text on a <see cref="Banner"/>.</summary>
        BannerText,

        /// <summary>Body text.</summary>
        TextPrimary,

        /// <summary>Supporting text.</summary>
        TextSecondary,

        /// <summary>Inert text.</summary>
        TextDisabled,

        /// <summary>Accent-coloured section heading or link.</summary>
        Heading,

        /// <summary>Hairline or separator drawn as a thin Panel.</summary>
        Separator,

        ButtonAccent,
        ButtonSuccess,
        ButtonDanger,
        ButtonSecondary,

        /// <summary>Low-emphasis button that sits on a bar rather than standing alone.</summary>
        ButtonSubtle,
    }

    /// <summary>
    /// Side-table mapping controls and tool-strip items to their roles.
    /// </summary>
    /// <remarks>
    /// A <see cref="ConditionalWeakTable{TKey,TValue}"/> rather than <c>Control.Tag</c>, for the
    /// same reason the previous implementation used one: <c>Tag</c> is already spoken for
    /// elsewhere in this codebase and overwriting it breaks tree nodes. Entries die with the
    /// control, so a dialog opened and closed a thousand times leaks nothing.
    /// </remarks>
    public static class ThemeRoles
    {
        private sealed class Assignment
        {
            public ThemeRole Role = ThemeRole.Default;
            public ThemeRole Text = ThemeRole.Default;
            public FontRole Font = FontRole.Inherit;
        }

        private static readonly ConditionalWeakTable<Control, Assignment> ControlRoles = new();
        private static readonly ConditionalWeakTable<ToolStripItem, Assignment> ItemRoles = new();

        public static void Set(Control control, ThemeRole role)
        {
            if (control == null) return;
            ControlRoles.GetOrCreateValue(control).Role = role;
        }

        public static void Set(Control control, FontRole font)
        {
            if (control == null) return;
            ControlRoles.GetOrCreateValue(control).Font = font;
        }

        public static void Set(Control control, ThemeRole role, FontRole font)
        {
            if (control == null) return;
            var a = ControlRoles.GetOrCreateValue(control);
            a.Role = role;
            a.Font = font;
        }

        /// <summary>
        /// Overrides only the text colour, leaving the fill to <see cref="Set(Control, ThemeRole)"/>.
        /// Needed by controls that paint their own caption in a different colour from their body —
        /// a group box being the obvious case.
        /// </summary>
        public static void SetText(Control control, ThemeRole role)
        {
            if (control == null) return;
            ControlRoles.GetOrCreateValue(control).Text = role;
        }

        public static ThemeRole TextRoleOf(Control control) =>
            ControlRoles.TryGetValue(control, out var a) ? a.Text : ThemeRole.Default;

        public static void Set(ToolStripItem item, ThemeRole role)
        {
            if (item == null) return;
            ItemRoles.GetOrCreateValue(item).Role = role;
        }

        public static ThemeRole RoleOf(Control control) =>
            ControlRoles.TryGetValue(control, out var a) ? a.Role : ThemeRole.Default;

        public static FontRole FontOf(Control control) =>
            ControlRoles.TryGetValue(control, out var a) ? a.Font : FontRole.Inherit;

        public static ThemeRole RoleOf(ToolStripItem item) =>
            ItemRoles.TryGetValue(item, out var a) ? a.Role : ThemeRole.Default;
    }
}
