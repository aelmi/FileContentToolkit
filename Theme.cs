using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace CodeShuttle.UI
{
    /// <summary>
    /// What is left of the old central palette once theming moved to
    /// <see cref="CodeShuttle.Theming.ThemeManager"/>: the shared application icon, and button
    /// hover states.
    /// </summary>
    /// <remarks>
    /// Everything else that used to live here is gone rather than deprecated. The palette
    /// constants, the two-pass <c>Apply</c>, the dark colour table and the fuzzy RGB-matching
    /// light-to-dark mapper have all been replaced by the token system; the form and
    /// button factories (<c>ApplyForm</c>, <c>BuildHeader</c>, <c>PrimaryButton</c> and friends)
    /// were called by zero forms and have been deleted rather than left behind as a third unused
    /// way to style something.
    /// </remarks>
    public static class Theme
    {
        // App icon, extracted from the running executable so dialogs share the main app icon.
        private static Icon? _cachedIcon;

        public static Icon? AppIcon
        {
            get
            {
                if (_cachedIcon != null) return _cachedIcon;
                try
                {
                    // Environment.ProcessPath, not Assembly.Location: the product ships as a
                    // single-file build, where Location always returns an empty string. Using
                    // it meant every dialog silently lost its icon in the shipped build while
                    // looking correct in a normal debug run (IL3000). ProcessPath is right in
                    // both. There is no Assembly.Location fallback: it would be wrong in exactly
                    // the configuration we ship, and IL3000 rejects it on sight.
                    var entry = Environment.ProcessPath;
                    if (!string.IsNullOrEmpty(entry) && File.Exists(entry))
                        _cachedIcon = Icon.ExtractAssociatedIcon(entry);
                }
                catch
                {
                    // A missing icon is not worth failing a dialog over.
                }
                return _cachedIcon;
            }
        }

        private sealed class HoverHandlers
        {
            public EventHandler? Enter;
            public EventHandler? Leave;
        }

        private static readonly ConditionalWeakTable<Button, HoverHandlers> Hovers = new();

        /// <summary>
        /// Attaches hover colouring to a button, replacing any hover previously attached to it.
        /// </summary>
        /// <remarks>
        /// The theme is re-applied on every palette change, so without detaching first each
        /// toggle would stack another pair of handlers on every button in the application and the
        /// last one attached — carrying a stale colour — would win.
        /// </remarks>
        public static void AttachHover(Button button, Color baseColor, Color hoverColor)
        {
            if (button == null) return;

            var slot = Hovers.GetOrCreateValue(button);
            if (slot.Enter != null) button.MouseEnter -= slot.Enter;
            if (slot.Leave != null) button.MouseLeave -= slot.Leave;

            slot.Enter = (s, e) => button.BackColor = hoverColor;
            slot.Leave = (s, e) => button.BackColor = baseColor;

            button.MouseEnter += slot.Enter;
            button.MouseLeave += slot.Leave;
        }
    }
}
