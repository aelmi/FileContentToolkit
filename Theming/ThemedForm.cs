using System;
using System.Windows.Forms;
using CodeShuttle.UI;

namespace CodeShuttle.Theming
{
    /// <summary>
    /// Base class for every window in the product. Inheriting it is the whole retrofit: a form
    /// picks up the palette, the shared application icon and a dark title bar without writing a
    /// line of theming code.
    /// </summary>
    public class ThemedForm : Form
    {
        private bool _subscribed;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Icon == null && Theme.AppIcon != null) Icon = Theme.AppIcon;

            if (!_subscribed)
            {
                ThemeManager.ThemeChanged += OnThemeChanged;
                _subscribed = true;
            }

            ApplyTheme();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // The caption is drawn by DWM before the first paint, so it has to be set as soon as
            // the handle exists rather than waiting for Load.
            NativeTheming.ApplyTitleBar(this, ThemeManager.IsDark);
        }

        /// <summary>
        /// Unsubscribes. A dialog opened repeatedly would otherwise be pinned alive by the static
        /// event for the lifetime of the process — one leaked window per open.
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_subscribed)
            {
                ThemeManager.ThemeChanged -= OnThemeChanged;
                _subscribed = false;
            }
            base.OnFormClosed(e);
        }

        private void OnThemeChanged(object? sender, EventArgs e)
        {
            if (IsDisposed || Disposing) return;
            ApplyTheme();
        }

        /// <summary>
        /// Repaints from the active palette. Overridden by forms that also render coloured content
        /// of their own — rich-text runs, in particular, which no control-tree walk can reach.
        /// </summary>
        protected virtual void ApplyTheme()
        {
            ThemeManager.ApplyTo(this);

            // Focus visuals. Every button is flat with a zero-width border, so without this the
            // only focus indication is Windows' dotted rectangle, which is invisible against the
            // accent fills. Attaching is idempotent, and the ring reads its colour from the
            // active palette at paint time, so re-running this on a theme change is harmless.
            CodeShuttle.Controls.FocusRing.AttachAll(this);
        }
    }
}
