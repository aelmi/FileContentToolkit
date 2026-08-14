using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.UI;

namespace CodeShuttle.Theming
{
    /// <summary>
    /// Walks a control tree and paints it from <see cref="ThemeTokens"/>, keyed on each control's
    /// <see cref="ThemeRole"/> and type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The predecessor derived dark colours from whatever light literal a control happened to
    /// carry, using fuzzy RGB matching against eight hardcoded cases; anything that missed all
    /// eight fell through unchanged and stayed light. That is why a third palette was impossible.
    /// Here the direction is reversed — a role determines a token, a token determines a colour —
    /// so a control can never be "missed", and swapping palettes is a data change.
    /// </para>
    /// <para>
    /// The walk keeps the two-pass shape of the original for the same reason it had one: a child
    /// that inherits its parent's background must be decided against the parent's <em>resolved</em>
    /// colour, not against whatever the parent happens to be showing mid-walk. Pass one resolves,
    /// pass two assigns. The snapshot side-tables the original needed are gone, because nothing is
    /// reverse-engineered any more and there is consequently nothing to restore.
    /// </para>
    /// </remarks>
    public static class ThemeApplier
    {
        private sealed class Resolved
        {
            public Control Control = null!;
            public Color? Back;
            public Color? Fore;
            public FontRole Font = FontRole.Inherit;
        }

        public static void Apply(Control root, ThemeTokens tokens, bool dark)
        {
            if (root == null) return;

            // PASS 1 — resolve.
            var plan = new List<Resolved>();
            Resolve(root, tokens, tokens.Surface, plan);

            // PASS 2 — assign.
            foreach (var r in plan)
            {
                if (r.Back.HasValue) r.Control.BackColor = r.Back.Value;
                if (r.Fore.HasValue) r.Control.ForeColor = r.Fore.Value;
                if (r.Font != FontRole.Inherit) r.Control.Font = ThemeFonts.Get(r.Font);

                StyleSpecialCases(r.Control, tokens, dark);
                NativeTheming.ApplyControlTheme(r.Control, dark);
            }

            root.Invalidate(true);
        }

        private static void Resolve(Control c, ThemeTokens t, Color inheritedBack, List<Resolved> plan)
        {
            var role = ThemeRoles.RoleOf(c);
            var entry = new Resolved { Control = c, Font = ThemeRoles.FontOf(c) };

            Color back = inheritedBack;

            switch (role)
            {
                case ThemeRole.Surface: back = t.Surface; entry.Back = back; entry.Fore = t.TextPrimary; break;
                case ThemeRole.SurfaceAlt: back = t.SurfaceAlt; entry.Back = back; entry.Fore = t.TextPrimary; break;
                case ThemeRole.SurfaceSunken: back = t.SurfaceSunken; entry.Back = back; entry.Fore = t.TextPrimary; break;
                case ThemeRole.Header: back = t.Accent; entry.Back = back; entry.Fore = t.AccentText; break;
                case ThemeRole.HeaderTitle: entry.Back = Color.Transparent; entry.Fore = t.AccentText; break;
                case ThemeRole.HeaderSubtitle: entry.Back = Color.Transparent; entry.Fore = Blend(t.AccentText, t.Accent, 0.18f); break;
                case ThemeRole.Banner: back = t.Warning; entry.Back = back; entry.Fore = t.WarningText; break;
                case ThemeRole.BannerText: entry.Back = Color.Transparent; entry.Fore = t.WarningText; break;
                case ThemeRole.TextPrimary: entry.Back = Color.Transparent; entry.Fore = t.TextPrimary; break;
                case ThemeRole.TextSecondary: entry.Back = Color.Transparent; entry.Fore = t.TextSecondary; break;
                case ThemeRole.TextDisabled: entry.Back = Color.Transparent; entry.Fore = t.TextDisabled; break;
                case ThemeRole.Heading: entry.Back = Color.Transparent; entry.Fore = t.AccentOnSurface; break;
                case ThemeRole.Separator: back = t.Border; entry.Back = back; entry.Fore = t.Border; break;

                case ThemeRole.ButtonAccent: StyleButton(c, entry, t.Accent, t.AccentHover, t.AccentText); break;
                case ThemeRole.ButtonSuccess: StyleButton(c, entry, t.Success, t.SuccessHover, t.AccentText); break;
                case ThemeRole.ButtonDanger: StyleButton(c, entry, t.Danger, t.DangerHover, t.AccentText); break;
                case ThemeRole.ButtonSecondary: StyleButton(c, entry, t.Neutral, t.NeutralHover, t.AccentText); break;
                case ThemeRole.ButtonSubtle: StyleButton(c, entry, t.SurfaceAlt, t.Selection, t.TextPrimary); break;

                default:
                    ResolveByType(c, t, inheritedBack, entry, ref back);
                    break;
            }

            // An explicit text role wins over whatever the fill implied.
            switch (ThemeRoles.TextRoleOf(c))
            {
                case ThemeRole.Default: break;
                case ThemeRole.Heading: entry.Fore = t.AccentOnSurface; break;
                case ThemeRole.TextSecondary: entry.Fore = t.TextSecondary; break;
                case ThemeRole.TextDisabled: entry.Fore = t.TextDisabled; break;
                case ThemeRole.BannerText: entry.Fore = t.WarningText; break;
                case ThemeRole.HeaderTitle: entry.Fore = t.AccentText; break;
                default: entry.Fore = t.TextPrimary; break;
            }

            plan.Add(entry);

            foreach (Control child in c.Controls)
                Resolve(child, t, back, plan);

            if (c is ToolStrip ts)
                foreach (ToolStripItem item in ts.Items)
                    ResolveItem(item, t, plan);
        }

        private static void ResolveByType(Control c, ThemeTokens t, Color inheritedBack, Resolved entry, ref Color back)
        {
            switch (c)
            {
                case Form:
                    back = t.Surface;
                    entry.Back = back;
                    entry.Fore = t.TextPrimary;
                    break;

                // Recessed content. These are the controls a user reads from or types into.
                case TextBoxBase:
                case ListBox:
                case ListView:
                case TreeView:
                case DataGridView:
                case ComboBox:
                case NumericUpDown:
                    back = t.SurfaceSunken;
                    entry.Back = back;
                    entry.Fore = t.TextPrimary;
                    break;

                case MenuStrip:
                case StatusStrip:
                case ToolStrip:
                    back = t.SurfaceAlt;
                    entry.Back = back;
                    entry.Fore = t.TextPrimary;
                    break;

                case Button:
                    StyleButton(c, entry, t.SurfaceAlt, t.Selection, t.TextPrimary);
                    break;

                // LinkLabel first: it derives from Label, so the general case would swallow it.
                case LinkLabel:
                case Label:
                case CheckBox:
                case RadioButton:
                    // Transparent so a label sitting on an accent header keeps the header's fill.
                    entry.Back = Color.Transparent;
                    entry.Fore = ContrastingText(t, inheritedBack);
                    if (c is LinkLabel link)
                    {
                        link.LinkColor = t.AccentOnSurface;
                        // AccentHover is a button FILL, sized for white text on top of it, and
                        // reading it as a foreground on a surface was a category error — it scored
                        // 4.23:1 on the dark surface even before the hover fills were corrected.
                        // The active state is instead the on-surface accent pushed towards the body
                        // text, which is distinguishable in both palettes and moves away from the
                        // background in both rather than towards it.
                        link.ActiveLinkColor = Blend(t.AccentOnSurface, t.TextPrimary, 0.45f);
                        link.VisitedLinkColor = t.AccentOnSurface;
                    }
                    break;

                case ProgressBar:
                    entry.Back = t.SurfaceSunken;
                    entry.Fore = t.Accent;
                    break;

                default:
                    // Panels, table layouts, split containers, group boxes: inherit, so a
                    // container never punches a differently-coloured hole in its parent.
                    entry.Back = inheritedBack;
                    entry.Fore = ContrastingText(t, inheritedBack);
                    break;
            }
        }

        private static void StyleButton(Control c, Resolved entry, Color fill, Color hover, Color text)
        {
            entry.Back = fill;
            entry.Fore = text;

            if (c is Button b)
            {
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.FlatAppearance.BorderColor = fill;
                b.FlatAppearance.MouseOverBackColor = hover;
                b.FlatAppearance.MouseDownBackColor = hover;
                b.UseVisualStyleBackColor = false;
                Theme.AttachHover(b, fill, hover);
            }
        }

        private static void ResolveItem(ToolStripItem item, ThemeTokens t, List<Resolved> plan)
        {
            // Tool-strip items are painted by the renderer, not by BackColor; the only thing worth
            // setting here is the text colour, which the renderer reads from the item.
            var role = ThemeRoles.RoleOf(item);
            item.ForeColor = role switch
            {
                ThemeRole.TextSecondary => t.TextSecondary,
                ThemeRole.TextDisabled => t.TextDisabled,
                ThemeRole.Heading => t.AccentOnSurface,
                _ => t.TextPrimary,
            };
            // ToolStripControlHost (ToolStripProgressBar, ToolStripComboBox, ToolStripTextBox…)
            // is not painted by the renderer at all: it forwards BackColor to a real hosted
            // Control, and several of those — ProgressBar among them — throw
            // "Control does not support transparent background colors." Theme the hosted control
            // through the normal path so it gets a real token fill instead of a transparent one.
            if (item is ToolStripControlHost host)
            {
                if (host.Control is not null)
                    Resolve(host.Control, t, t.SurfaceAlt, plan);
            }
            else
            {
                item.BackColor = Color.Transparent;
            }

            if (item is ToolStripDropDownItem dd)
            {
                Resolve(dd.DropDown, t, t.SurfaceAlt, plan);
                foreach (ToolStripItem child in dd.DropDownItems)
                    ResolveItem(child, t, plan);
            }
        }

        private static void StyleSpecialCases(Control c, ThemeTokens t, bool dark)
        {
            switch (c)
            {
                case DataGridView grid:
                    grid.BackgroundColor = t.SurfaceSunken;
                    grid.GridColor = t.Border;
                    grid.EnableHeadersVisualStyles = false;
                    grid.DefaultCellStyle.BackColor = t.SurfaceSunken;
                    grid.DefaultCellStyle.ForeColor = t.TextPrimary;
                    grid.DefaultCellStyle.SelectionBackColor = t.Selection;
                    grid.DefaultCellStyle.SelectionForeColor = t.TextPrimary;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = t.SurfaceAlt;
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = t.TextPrimary;
                    grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = t.SurfaceAlt;
                    grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = t.TextPrimary;
                    grid.RowHeadersDefaultCellStyle.BackColor = t.SurfaceAlt;
                    grid.RowHeadersDefaultCellStyle.ForeColor = t.TextPrimary;
                    break;

                case TreeView tree:
                    tree.LineColor = t.Border;
                    break;

                case SplitContainer split:
                    split.BackColor = t.Border;
                    break;

                case SplitButton sb:
                    sb.SeparatorColor = t.TextDisabled;
                    sb.DisabledCaretColor = t.TextDisabled;
                    break;
            }

            _ = dark;
        }

        /// <summary>Body text that will be legible on the given background.</summary>
        private static Color ContrastingText(ThemeTokens t, Color background) =>
            Luminance(background) > 0.5 ? DarkestText(t) : LightestText(t);

        private static Color DarkestText(ThemeTokens t) =>
            Luminance(t.TextPrimary) <= Luminance(t.AccentText) ? t.TextPrimary : t.AccentText;

        private static Color LightestText(ThemeTokens t) =>
            Luminance(t.TextPrimary) >= Luminance(t.AccentText) ? t.TextPrimary : t.AccentText;

        /// <summary>Mixes <paramref name="amount"/> of <paramref name="b"/> into <paramref name="a"/>.</summary>
        private static Color Blend(Color a, Color b, float amount)
        {
            int Mix(int x, int y) => (int)(x + ((y - x) * amount));
            return Color.FromArgb(Mix(a.R, b.R), Mix(a.G, b.G), Mix(a.B, b.B));
        }

        private static double Luminance(Color c)
        {
            static double Channel(int v)
            {
                double s = v / 255.0;
                return s <= 0.03928 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
            }
            return (0.2126 * Channel(c.R)) + (0.7152 * Channel(c.G)) + (0.0722 * Channel(c.B));
        }
    }
}
