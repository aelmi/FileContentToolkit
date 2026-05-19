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

        // Dark palette — chosen to match VS Code / Windows 11 dark
        private static readonly Color DarkFormBg     = Color.FromArgb(0x1E, 0x1E, 0x1E); // form background
        private static readonly Color DarkSurface    = Color.FromArgb(0x25, 0x25, 0x26); // textbox / listbox / output bg
        private static readonly Color DarkSurfaceAlt = Color.FromArgb(0x2D, 0x2D, 0x30); // very light gray remap
        private static readonly Color DarkPanel      = Color.FromArgb(0x37, 0x37, 0x3A); // light gray panels (status bar, move btns)
        private static readonly Color DarkHeader     = Color.FromArgb(0x2A, 0x2D, 0x33); // header strip (was vivid blue)
        private static readonly Color DarkBanner     = Color.FromArgb(0x3C, 0x37, 0x23); // cream/yellow banner remap

        private static readonly Color DarkTextStrong = Color.FromArgb(0xE8, 0xE6, 0xE3); // primary text
        private static readonly Color DarkTextMid    = Color.FromArgb(0xD0, 0xD4, 0xD8); // secondary text
        private static readonly Color DarkTextSubtle = Color.FromArgb(0xA0, 0xA8, 0xB0); // subtle text
        private static readonly Color DarkAccentText = Color.FromArgb(0xA9, 0xC8, 0xEB); // group-box title (was vivid blue)

        private sealed class ThemeSnapshot
        {
            public Color Back; public Color Fore;
            public ThemeSnapshot(Color b, Color f) { Back = b; Fore = f; }
        }

        // Weak side-tables so toggling between modes is round-trip safe and Control.Tag
        // (used by some controls like TreeNodes) isn't clobbered.
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Control, ThemeSnapshot> _snapshots = new();
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<ToolStripItem, ThemeSnapshot> _itemSnapshots = new();

        public static void Apply(Form form, bool dark)
        {
            if (form == null) return;
            IsDark = dark;

            // PASS 1: capture every snapshot BEFORE changing anything, otherwise children
            // that inherit BackColor will snapshot the already-modified parent's color.
            CaptureSnapshots(form);

            // Swap the app-wide ToolStrip renderer so menu popups (which ignore BackColor
            // and paint their own gradients) render with dark surfaces.
            ToolStripManager.Renderer = dark
                ? new ToolStripProfessionalRenderer(new DarkColorTable()) { RoundedEdges = false }
                : new ToolStripProfessionalRenderer();

            // PASS 2: apply the actual colors.
            ApplyToControl(form, dark);
            form.Invalidate(true);
        }

        private sealed class DarkColorTable : ProfessionalColorTable
        {
            // Surfaces
            public override Color ToolStripDropDownBackground => Color.FromArgb(0x2D, 0x2D, 0x30);
            public override Color MenuStripGradientBegin => Color.FromArgb(0x2A, 0x2D, 0x33);
            public override Color MenuStripGradientEnd   => Color.FromArgb(0x2A, 0x2D, 0x33);
            public override Color ImageMarginGradientBegin  => Color.FromArgb(0x2D, 0x2D, 0x30);
            public override Color ImageMarginGradientMiddle => Color.FromArgb(0x2D, 0x2D, 0x30);
            public override Color ImageMarginGradientEnd    => Color.FromArgb(0x2D, 0x2D, 0x30);

            // Status / tool strip back
            public override Color ToolStripGradientBegin  => Color.FromArgb(0x37, 0x37, 0x3A);
            public override Color ToolStripGradientMiddle => Color.FromArgb(0x37, 0x37, 0x3A);
            public override Color ToolStripGradientEnd    => Color.FromArgb(0x37, 0x37, 0x3A);
            public override Color ToolStripContentPanelGradientBegin => Color.FromArgb(0x37, 0x37, 0x3A);
            public override Color ToolStripContentPanelGradientEnd   => Color.FromArgb(0x37, 0x37, 0x3A);
            public override Color ToolStripPanelGradientBegin => Color.FromArgb(0x37, 0x37, 0x3A);
            public override Color ToolStripPanelGradientEnd   => Color.FromArgb(0x37, 0x37, 0x3A);
            public override Color StatusStripGradientBegin => Color.FromArgb(0x37, 0x37, 0x3A);
            public override Color StatusStripGradientEnd   => Color.FromArgb(0x37, 0x37, 0x3A);

            // Hover / selected
            public override Color MenuItemSelected               => Color.FromArgb(0x3D, 0x42, 0x50);
            public override Color MenuItemSelectedGradientBegin  => Color.FromArgb(0x3D, 0x42, 0x50);
            public override Color MenuItemSelectedGradientEnd    => Color.FromArgb(0x3D, 0x42, 0x50);
            public override Color MenuItemPressedGradientBegin   => Color.FromArgb(0x4A, 0x50, 0x5E);
            public override Color MenuItemPressedGradientMiddle  => Color.FromArgb(0x4A, 0x50, 0x5E);
            public override Color MenuItemPressedGradientEnd     => Color.FromArgb(0x4A, 0x50, 0x5E);
            public override Color ButtonSelectedGradientBegin    => Color.FromArgb(0x3D, 0x42, 0x50);
            public override Color ButtonSelectedGradientMiddle   => Color.FromArgb(0x3D, 0x42, 0x50);
            public override Color ButtonSelectedGradientEnd      => Color.FromArgb(0x3D, 0x42, 0x50);
            public override Color ButtonPressedGradientBegin     => Color.FromArgb(0x4A, 0x50, 0x5E);
            public override Color ButtonPressedGradientMiddle    => Color.FromArgb(0x4A, 0x50, 0x5E);
            public override Color ButtonPressedGradientEnd       => Color.FromArgb(0x4A, 0x50, 0x5E);

            // Borders / separators
            public override Color MenuBorder           => Color.FromArgb(0x4A, 0x4A, 0x4F);
            public override Color MenuItemBorder       => Color.FromArgb(0x3D, 0x42, 0x50);
            public override Color ButtonSelectedBorder => Color.FromArgb(0x3D, 0x42, 0x50);
            public override Color SeparatorDark        => Color.FromArgb(0x4A, 0x4A, 0x4F);
            public override Color SeparatorLight       => Color.FromArgb(0x4A, 0x4A, 0x4F);

            // Checked / disabled
            public override Color CheckBackground         => Color.FromArgb(0x3D, 0x42, 0x50);
            public override Color CheckPressedBackground  => Color.FromArgb(0x4A, 0x50, 0x5E);
            public override Color CheckSelectedBackground => Color.FromArgb(0x3D, 0x42, 0x50);
            public override Color GripDark  => Color.FromArgb(0x55, 0x55, 0x5A);
            public override Color GripLight => Color.FromArgb(0x40, 0x40, 0x45);
        }

        private static void CaptureSnapshots(Control c)
        {
            if (!_snapshots.TryGetValue(c, out _))
                _snapshots.Add(c, new ThemeSnapshot(c.BackColor, c.ForeColor));

            if (c is ToolStrip ts)
                foreach (ToolStripItem item in ts.Items)
                    CaptureItemSnapshots(item);

            foreach (Control child in c.Controls)
                CaptureSnapshots(child);
        }

        private static void CaptureItemSnapshots(ToolStripItem item)
        {
            if (!_itemSnapshots.TryGetValue(item, out _))
                _itemSnapshots.Add(item, new ThemeSnapshot(item.BackColor, item.ForeColor));

            if (item is ToolStripDropDownItem dd)
            {
                // The dropdown popup is itself a Control (ToolStripDropDown).
                CaptureSnapshots(dd.DropDown);
                foreach (ToolStripItem child in dd.DropDownItems)
                    CaptureItemSnapshots(child);
            }
        }

        private static void ApplyToControl(Control c, bool dark)
        {
            if (!_snapshots.TryGetValue(c, out var snap)) return; // shouldn't happen — Capture ran first

            if (dark)
            {
                c.BackColor = ToDarkBg(snap.Back);
                c.ForeColor = ToDarkFg(snap.Fore);
            }
            else
            {
                c.BackColor = snap.Back;
                c.ForeColor = snap.Fore;
            }

            if (c is ToolStrip ts)
                foreach (ToolStripItem item in ts.Items)
                    ApplyToolStripItem(item, dark);

            foreach (Control child in c.Controls)
                ApplyToControl(child, dark);
        }

        private static void ApplyToolStripItem(ToolStripItem item, bool dark)
        {
            if (!_itemSnapshots.TryGetValue(item, out var snap)) return;

            if (dark)
            {
                item.BackColor = ToDarkBg(snap.Back);
                item.ForeColor = ToDarkFg(snap.Fore);
            }
            else
            {
                item.BackColor = snap.Back;
                item.ForeColor = snap.Fore;
            }

            if (item is ToolStripDropDownItem dd)
            {
                ApplyToControl(dd.DropDown, dark);
                foreach (ToolStripItem child in dd.DropDownItems)
                    ApplyToolStripItem(child, dark);
            }
        }

        // -------------------- color mapping --------------------

        private static bool Near(Color a, int r, int g, int b, int tol = 6) =>
            Math.Abs(a.R - r) <= tol &&
            Math.Abs(a.G - g) <= tol &&
            Math.Abs(a.B - b) <= tol;

        // Map a light-mode background to its dark-mode equivalent.
        // Vivid accent colors (button primaries) are kept as-is.
        private static Color ToDarkBg(Color c)
        {
            // Specific known palette colors first
            if (Near(c, 0, 102, 204)) return DarkHeader;          // page header strip
            if (Near(c, 255, 255, 224)) return DarkBanner;        // recreate-info banner (pale yellow)
            if (Near(c, 245, 247, 250)) return DarkFormBg;        // form bg / pnl bg
            if (Near(c, 248, 249, 250)) return DarkSurfaceAlt;    // output toolbar buttons
            if (Near(c, 233, 236, 239)) return DarkPanel;         // status bar / move buttons

            // Generic "white-ish" surfaces (textboxes, listboxes, output area, group boxes)
            if (c.R >= 250 && c.G >= 250 && c.B >= 250) return DarkSurface;
            // Very light grays (almost white)
            if (c.R >= 235 && c.G >= 235 && c.B >= 235 && c.R + c.G + c.B >= 720) return DarkSurfaceAlt;

            // Otherwise leave vivid accent colors alone
            return c;
        }

        // Map a light-mode foreground to its dark-mode equivalent.
        private static Color ToDarkFg(Color c)
        {
            // White text on accent buttons should stay white
            if (c.R >= 240 && c.G >= 240 && c.B >= 240) return c;

            // Specific palette text colors
            if (Near(c, 0, 102, 204)) return DarkAccentText;      // group-box titles (vivid blue)
            if (Near(c, 33, 37, 41)) return DarkTextStrong;       // body text
            if (Near(c, 73, 80, 87)) return DarkTextMid;          // toolbar button text
            if (Near(c, 108, 117, 125)) return DarkTextSubtle;    // subtle text

            // Generic dark text → light text
            int sum = c.R + c.G + c.B;
            if (sum < 240) return DarkTextStrong;
            if (sum < 400) return DarkTextMid;

            return c;
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
