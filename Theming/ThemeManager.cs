using System;
using System.Windows.Forms;

namespace CodeShuttle.Theming
{
    /// <summary>
    /// The single source of truth for the active palette, and the only thing that raises
    /// <see cref="ThemeChanged"/>.
    /// </summary>
    /// <remarks>
    /// Theming used to be opt-in and exactly two call sites took the option, so ten of the eleven
    /// forms were hardcoded light: switching to dark mode and opening any dialog produced a
    /// full-brightness white flash. Every form now derives from <see cref="ThemedForm"/> and
    /// subscribes here, so being themed is the default and staying light is impossible.
    /// </remarks>
    public static class ThemeManager
    {
        private static ThemeMode _mode = ThemeMode.Light;

        /// <summary>Raised after <see cref="Tokens"/> has changed. Forms repaint themselves from it.</summary>
        public static event EventHandler? ThemeChanged;

        public static ThemeMode Mode
        {
            get => _mode;
            set
            {
                if (_mode == value) return;
                _mode = value;
                Refresh();
            }
        }

        public static ThemeTokens Tokens { get; private set; } = ThemePalettes.For(ThemeMode.Light);

        /// <summary>True when the active palette is a dark one — the native theming calls need it.</summary>
        public static bool IsDark => ThemeManager.Mode == ThemeMode.Dark;

        /// <summary>
        /// Sets the mode without raising the change event, for use before any form exists.
        /// </summary>
        public static void Initialize(ThemeMode mode)
        {
            _mode = mode;
            Tokens = ThemePalettes.For(mode);
            InstallRenderer();
        }

        private static void Refresh()
        {
            Tokens = ThemePalettes.For(_mode);
            InstallRenderer();
            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }

        private static void InstallRenderer()
        {
            // Menu and status strips paint their own gradients and ignore BackColor, so the
            // renderer has to be swapped app-wide rather than per form.
            ToolStripManager.Renderer = new TokenToolStripRenderer(Tokens);
        }

        /// <summary>Paints a single form (and everything in it) with the active palette.</summary>
        public static void ApplyTo(Form form)
        {
            if (form == null) return;
            ThemeApplier.Apply(form, Tokens, IsDark);
            NativeTheming.ApplyTitleBar(form, IsDark);
        }
    }
}
