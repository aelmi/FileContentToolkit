using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace FileContentToolkit.UI
{
    /// <summary>
    /// Central palette + factories so every dialog inherits the same look as MainForm / ExtensionCountsForm.
    /// </summary>
    public static class Theme
    {
        // Palette
        public static readonly Color Header = Color.FromArgb(0, 102, 204);   // Page-header blue
        public static readonly Color Primary = Color.FromArgb(51, 122, 183); // Primary button blue
        public static readonly Color Action = Color.FromArgb(13, 110, 253);  // Primary action / accent
        public static readonly Color Success = Color.FromArgb(40, 167, 69);  // Green
        public static readonly Color Danger = Color.FromArgb(220, 53, 69);   // Red
        public static readonly Color Secondary = Color.FromArgb(108, 117, 125); // Gray
        public static readonly Color FormBg = Color.FromArgb(245, 247, 250);
        public static readonly Color BodyText = Color.FromArgb(33, 37, 41);
        public static readonly Color SubtleText = Color.FromArgb(108, 117, 125);
        public static readonly Color White = Color.White;
        public static readonly Color HeaderSubtitle = Color.WhiteSmoke;

        // Fonts
        public static readonly Font TitleFont = new Font("Segoe UI", 12F, FontStyle.Bold);
        public static readonly Font BodyFont = new Font("Segoe UI", 9.5F);
        public static readonly Font ButtonFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        public static readonly Font SubtitleFont = new Font("Segoe UI", 9.5F, FontStyle.Italic);

        // App icon, extracted from the running executable so dialogs share the main app icon
        private static Icon? _cachedIcon;
        public static Icon? AppIcon
        {
            get
            {
                if (_cachedIcon != null) return _cachedIcon;
                try
                {
                    var entry = Assembly.GetEntryAssembly()?.Location;
                    if (!string.IsNullOrEmpty(entry) && File.Exists(entry))
                        _cachedIcon = Icon.ExtractAssociatedIcon(entry);
                }
                catch { }
                return _cachedIcon;
            }
        }

        /// <summary>Apply common form defaults (bg, font, icon).</summary>
        public static void ApplyForm(Form f)
        {
            f.BackColor = FormBg;
            f.Font = BodyFont;
            if (AppIcon != null) f.Icon = AppIcon;
        }

        /// <summary>Build a header panel (blue strip with white title and optional subtitle).</summary>
        public static Panel BuildHeader(string title, string? subtitle = null, int height = 70)
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Top,
                Height = height,
                BackColor = Header,
                Padding = new Padding(20, 12, 20, 10)
            };

            var lblTitle = new Label
            {
                AutoSize = true,
                Font = TitleFont,
                ForeColor = White,
                Location = new Point(20, 12),
                Text = title
            };
            pnl.Controls.Add(lblTitle);

            if (!string.IsNullOrEmpty(subtitle))
            {
                var lblSub = new Label
                {
                    AutoSize = true,
                    Font = SubtitleFont,
                    ForeColor = HeaderSubtitle,
                    Location = new Point(20, 42),
                    Text = subtitle,
                    Name = "lblHeaderSubtitle"
                };
                pnl.Controls.Add(lblSub);
            }

            return pnl;
        }

        // -------------------- Button factories --------------------

        public static Button PrimaryButton(string text) => MakeButton(text, Primary);
        public static Button ActionButton(string text)  => MakeButton(text, Action);
        public static Button SuccessButton(string text) => MakeButton(text, Success);
        public static Button DangerButton(string text)  => MakeButton(text, Danger);
        public static Button SecondaryButton(string text) => MakeButton(text, Secondary);

        public static Button MakeButton(string text, Color back)
        {
            var b = new Button
            {
                Text = text,
                BackColor = back,
                FlatStyle = FlatStyle.Flat,
                ForeColor = White,
                Font = ButtonFont,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            b.FlatAppearance.BorderSize = 0;
            AttachHover(b, back);
            return b;
        }

        public static void AttachHover(Button button, Color baseColor)
        {
            button.MouseEnter += (s, e) => button.BackColor = ControlPaint.Light(baseColor, 0.2f);
            button.MouseLeave += (s, e) => button.BackColor = baseColor;
        }

        // -------------------- Dark mode --------------------

        public static bool IsDark { get; private set; }

        // Map of "neutral" light colors to their dark equivalents. Accent colors
        // (header blue, success green, danger red, etc.) are intentionally absent
        // so they remain vivid in dark mode.
        private static readonly Dictionary<Color, Color> DarkRemap = new()
        {
            [Color.White] = Color.FromArgb(0x25, 0x25, 0x26),
            [Color.FromArgb(245, 247, 250)] = Color.FromArgb(0x1E, 0x1E, 0x1E),
            [Color.FromArgb(248, 249, 250)] = Color.FromArgb(0x32, 0x32, 0x35),
            [Color.FromArgb(233, 236, 239)] = Color.FromArgb(0x37, 0x37, 0x3A),
            [Color.FromArgb(33, 37, 41)]    = Color.FromArgb(0xE8, 0xE6, 0xE3),
            [Color.FromArgb(73, 80, 87)]    = Color.FromArgb(0xD0, 0xD0, 0xD0),
            [Color.FromArgb(108, 117, 125)] = Color.FromArgb(0xB0, 0xB5, 0xBB),
            [Color.FromArgb(220, 220, 220)] = Color.FromArgb(0x45, 0x45, 0x45),
            [Color.Black] = Color.FromArgb(0xE8, 0xE6, 0xE3),
        };

        private sealed class ThemeSnapshot
        {
            public Color Back; public Color Fore;
            public ThemeSnapshot(Color b, Color f) { Back = b; Fore = f; }
        }

        // Use a weak side-table keyed by control rather than abusing Control.Tag,
        // because some controls (TreeNode, etc.) already use Tag for their own data.
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Control, ThemeSnapshot> _snapshots = new();

        public static void Apply(Form form, bool dark)
        {
            if (form == null) return;
            IsDark = dark;
            ApplyToControl(form, dark);
        }

        private static void ApplyToControl(Control c, bool dark)
        {
            if (!_snapshots.TryGetValue(c, out var snap))
            {
                snap = new ThemeSnapshot(c.BackColor, c.ForeColor);
                _snapshots.Add(c, snap);
            }

            if (dark)
            {
                c.BackColor = DarkRemap.TryGetValue(snap.Back, out var b) ? b : snap.Back;
                c.ForeColor = DarkRemap.TryGetValue(snap.Fore, out var f) ? f : snap.Fore;
            }
            else
            {
                c.BackColor = snap.Back;
                c.ForeColor = snap.Fore;
            }

            // MenuStrip / StatusStrip / ContextMenuStrip items are not in Controls — walk them.
            if (c is ToolStrip ts)
            {
                foreach (ToolStripItem item in ts.Items)
                    ApplyToolStripItem(item, dark);
            }

            foreach (Control child in c.Controls)
                ApplyToControl(child, dark);
        }

        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<ToolStripItem, ThemeSnapshot> _itemSnapshots = new();

        private static void ApplyToolStripItem(ToolStripItem item, bool dark)
        {
            if (!_itemSnapshots.TryGetValue(item, out var snap))
            {
                snap = new ThemeSnapshot(item.BackColor, item.ForeColor);
                _itemSnapshots.Add(item, snap);
            }

            if (dark)
            {
                item.BackColor = DarkRemap.TryGetValue(snap.Back, out var b) ? b : snap.Back;
                item.ForeColor = DarkRemap.TryGetValue(snap.Fore, out var f) ? f : snap.Fore;
            }
            else
            {
                item.BackColor = snap.Back;
                item.ForeColor = snap.Fore;
            }

            if (item is ToolStripDropDownItem dd)
                foreach (ToolStripItem child in dd.DropDownItems)
                    ApplyToolStripItem(child, dark);
        }

        /// <summary>Build a bottom "action bar" panel with white background and right-aligned buttons.</summary>
        public static Panel BuildBottomBar(int height = 64)
        {
            return new Panel
            {
                Dock = DockStyle.Bottom,
                Height = height,
                BackColor = White,
                Padding = new Padding(20, 12, 20, 12)
            };
        }
    }
}
