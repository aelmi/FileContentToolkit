using System;
using System.Collections.Generic;
using System.Drawing;

namespace CodeShuttle.Theming
{
    /// <summary>Semantic font roles, the typographic counterpart to <see cref="ThemeRole"/>.</summary>
    public enum FontRole
    {
        /// <summary>Leave the control's font alone.</summary>
        Inherit = 0,
        Small,
        SmallBold,
        Body,
        BodyBold,
        BodyItalic,
        Medium,
        MediumBold,
        Heading,
        Title,
        TitleLarge,
        Mono,
        MonoSmall,
    }

    /// <summary>
    /// Resolves <see cref="FontRole"/> to real fonts, scaled from the user's message-box font.
    /// </summary>
    /// <remarks>
    /// The Designer files previously contained forty-three hardcoded <c>new Font(...)</c> calls in
    /// nine sizes, which is precisely why the application ignored the Windows "Make text bigger"
    /// accessibility setting: nothing in the product ever consulted a system metric. Deriving every
    /// role from <see cref="SystemFonts.MessageBoxFont"/> makes the whole application respond to it.
    ///
    /// The instances are process-lifetime and bounded (one per role), so they are intentionally
    /// never disposed — unlike the per-line fonts that used to exhaust the GDI handle limit.
    /// </remarks>
    public static class ThemeFonts
    {
        private static readonly object Gate = new();
        private static readonly Dictionary<FontRole, Font> Cache = new();
        private static Font? _base;

        /// <summary>
        /// The user's UI font. 9pt Segoe UI on a default install, larger when "Make text bigger"
        /// has been raised.
        /// </summary>
        public static Font Base
        {
            get
            {
                lock (Gate)
                {
                    return _base ??= (Font)(SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont).Clone();
                }
            }
        }

        /// <summary>Drops every cached font. Called when the system font preference changes.</summary>
        public static void Invalidate()
        {
            lock (Gate)
            {
                // Deliberately not disposed: a control somewhere may still hold the reference, and
                // this happens at most a handful of times in a session.
                Cache.Clear();
                _base = null;
            }
        }

        public static Font Get(FontRole role)
        {
            if (role == FontRole.Inherit) return Base;

            lock (Gate)
            {
                if (Cache.TryGetValue(role, out var cached)) return cached;

                float b = Base.Size;
                var f = role switch
                {
                    FontRole.Small => new Font(Base.FontFamily, b, FontStyle.Regular),
                    FontRole.SmallBold => new Font(Base.FontFamily, b, FontStyle.Bold),
                    FontRole.Body => new Font(Base.FontFamily, b * (9.5f / 9f), FontStyle.Regular),
                    FontRole.BodyBold => new Font(Base.FontFamily, b * (9.5f / 9f), FontStyle.Bold),
                    FontRole.BodyItalic => new Font(Base.FontFamily, b * (9.5f / 9f), FontStyle.Italic),
                    FontRole.Medium => new Font(Base.FontFamily, b * (10f / 9f), FontStyle.Regular),
                    FontRole.MediumBold => new Font(Base.FontFamily, b * (10f / 9f), FontStyle.Bold),
                    FontRole.Heading => new Font(Base.FontFamily, b * (11f / 9f), FontStyle.Bold),
                    FontRole.Title => new Font(Base.FontFamily, b * (12f / 9f), FontStyle.Bold),
                    FontRole.TitleLarge => new Font(Base.FontFamily, b * (14f / 9f), FontStyle.Bold),
                    FontRole.Mono => MonoFont(b * (10f / 9f), FontStyle.Regular),
                    FontRole.MonoSmall => MonoFont(b, FontStyle.Regular),
                    _ => (Font)Base.Clone(),
                };

                Cache[role] = f;
                return f;
            }
        }

        private static Font MonoFont(float size, FontStyle style)
        {
            // Cascadia Mono ships with Windows 11 and recent Visual Studio; Consolas is present on
            // every supported Windows. GDI+ silently substitutes Microsoft Sans Serif for a missing
            // family rather than throwing, so the family is verified before it is used.
            foreach (var name in new[] { "Cascadia Mono", "Consolas", "Courier New" })
            {
                try
                {
                    var f = new Font(name, size, style);
                    if (string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase)) return f;
                    f.Dispose();
                }
                catch
                {
                    // Try the next candidate.
                }
            }
            return new Font(FontFamily.GenericMonospace, size, style);
        }
    }
}
