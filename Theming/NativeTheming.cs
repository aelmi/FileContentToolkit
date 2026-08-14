using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CodeShuttle.Theming
{
    /// <summary>
    /// The handful of native calls needed for controls that are not really WinForms controls at
    /// all — tree views, list views, text boxes, progress bars and the window caption are drawn by
    /// comctl32 and DWM, and ignore <c>BackColor</c> completely.
    /// </summary>
    /// <remarks>
    /// Every entry point here is best-effort: the theming calls are cosmetic, they are not present
    /// on every Windows build, and a failure must never be visible to the user. All of them are
    /// therefore wrapped and swallow their exceptions.
    /// </remarks>
    internal static class NativeTheming
    {
        /// <summary>DWMWA_USE_IMMERSIVE_DARK_MODE. The reason the title bar stayed light in dark mode.</summary>
        private const int DwmwaUseImmersiveDarkMode = 20;

        /// <summary>The value the attribute had on Windows 10 builds before 19041.</summary>
        private const int DwmwaUseImmersiveDarkModeLegacy = 19;

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int SetWindowTheme(IntPtr hwnd, string? appName, string? idList);

        /// <summary>
        /// Darkens or restores the window caption. Called on every handle creation and on every
        /// theme change; a window whose handle does not exist yet is skipped and picked up later.
        /// </summary>
        public static void ApplyTitleBar(IWin32Window window, bool dark)
        {
            if (window == null) return;
            try
            {
                var hwnd = window.Handle;
                if (hwnd == IntPtr.Zero) return;

                int value = dark ? 1 : 0;
                if (DwmSetWindowAttribute(hwnd, DwmwaUseImmersiveDarkMode, ref value, sizeof(int)) != 0)
                    _ = DwmSetWindowAttribute(hwnd, DwmwaUseImmersiveDarkModeLegacy, ref value, sizeof(int));
            }
            catch
            {
                // Cosmetic only.
            }
        }

        /// <summary>
        /// Switches a native common control to its dark visual style. The scrollbars and the
        /// tree-view expander glyphs come from here, and nothing else can reach them.
        /// </summary>
        public static void ApplyControlTheme(Control control, bool dark)
        {
            if (control == null || !control.IsHandleCreated) return;

            string? appName = control switch
            {
                TreeView => dark ? "DarkMode_Explorer" : "Explorer",
                ListView => dark ? "DarkMode_ItemsView" : "Explorer",
                // Stripped in both themes, not only in dark. comctl32 ignores ForeColor while a
                // visual style is active, so a themed progress bar renders in the stock Windows
                // green no matter what the palette says — the one piece of live data in the window,
                // in the one colour the product does not use. Stripping the style is what lets
                // PBM_SETBARCOLOR through and makes the bar the accent.
                ProgressBar => "",
                // DarkMode_CFD, not DarkMode_Explorer: Explorer darkens a combo's edit portion and
                // leaves the drop-down list white, which is worse than not darkening it at all.
                ComboBox => dark ? "DarkMode_CFD" : "Explorer",
                TextBoxBase or ListBox or DataGridView => dark ? "DarkMode_Explorer" : "Explorer",
                _ => null,
            };
            if (appName == null) return;

            try
            {
                _ = SetWindowTheme(control.Handle, appName, appName.Length == 0 ? "" : null);
            }
            catch
            {
                // Cosmetic only.
            }
        }
    }
}
