using System.Drawing;

namespace CodeShuttle.Theming
{
    /// <summary>
    /// The shipped palettes. A third palette is a matter of adding one more static field here and
    /// one more enum member to <see cref="ThemeMode"/> — deliberately, but high contrast and
    /// system-following are both out of scope for this pass.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The neutral ramp is biased cool — a slate with a blue cast rather than a pure grey — so it
    /// reads as chosen rather than inherited, and so the accent sits on it as a near-analogous
    /// rather than a clash.
    /// </para>
    /// <para>
    /// The family is three blues, each with exactly one job, which is what stops three blues reading
    /// as a mess. <b>Bright cobalt</b> is the accent fill: one filled button on screen at a time, and
    /// nothing else. <b>Azure</b> is the focus ring, and — deepened far enough to carry as text — the
    /// on-surface accent for links and chips. <b>Light sky blue</b> is the wash: selection and state
    /// tints in light mode. In dark mode sky and cobalt swap roles, because a cobalt dark enough to
    /// carry white text is too dark to read <em>against</em> a dark ground; the fill stays cobalt and
    /// the on-surface accent becomes sky.
    /// </para>
    /// <para>
    /// Colour that is not the accent is semantic — <see cref="ThemeTokens.Danger"/> for destructive,
    /// <see cref="ThemeTokens.Warning"/> for caution — and nothing else is coloured at all. Warning
    /// is the one token that is not blue, and deliberately so: a caution banner in the accent hue is
    /// a caution banner nobody reads as a caution.
    /// </para>
    /// <para>
    /// Every pair asserted in ThemeContrastTests clears 4.5:1 for text and 3:1 for non-text chrome
    /// in <em>both</em> palettes, with headroom; the tightest pair is DiffAdd on DiffAddBg at
    /// 4.79:1. Hover fills are darker than their base rather than lighter, because a hover swaps
    /// BackColor only — <see cref="ThemeTokens.AccentText"/> stays white — so a hover fill has to
    /// clear 4.5:1 against white in its own right, and the base fills have no headroom to brighten
    /// into.
    /// </para>
    /// </remarks>
    public static class ThemePalettes
    {
        public static readonly ThemeTokens Light = new()
        {
            // White is the content ground; the chrome (rail, bars, section headers) is the grey.
            // This is the reverse of the previous arrangement, where a grey form held white wells,
            // and it is what lets the output pane read as the centre of the window. The greys are
            // sky-tinted rather than neutral, so the chrome belongs to the accent family without
            // ever competing with it.
            Surface = Color.FromArgb(0xFF, 0xFF, 0xFF),
            SurfaceAlt = Color.FromArgb(0xED, 0xF2, 0xFA),
            SurfaceSunken = Color.FromArgb(0xF6, 0xF9, 0xFE),

            TextPrimary = Color.FromArgb(0x13, 0x1A, 0x24),
            TextSecondary = Color.FromArgb(0x54, 0x60, 0x7A),
            // Also carries the split-button separator and disabled caret, so it has to clear the
            // 3:1 non-text threshold against Surface, not merely look grey.
            TextDisabled = Color.FromArgb(0x74, 0x80, 0x9A),

            // Bright cobalt. 6.39:1 on white, so it carries the label of the one filled button.
            Accent = Color.FromArgb(0x0B, 0x57, 0xD0),
            AccentText = Color.White,
            AccentHover = Color.FromArgb(0x08, 0x40, 0x9C),
            // Azure, deepened to 7.28:1 so it can be read as text. Plain azure (#007FFF) is 3.83:1
            // and would have been a link nobody with less than perfect sight could read.
            AccentOnSurface = Color.FromArgb(0x0A, 0x4F, 0xBF),

            Border = Color.FromArgb(0xD3, 0xDD, 0xEC),
            // True azure. A focus ring is chrome, not text, so 3.83:1 clears its 3:1 bar — and the
            // brighter hue is what makes the ring visible as a ring rather than as a border.
            BorderFocus = Color.FromArgb(0x00, 0x7F, 0xFF),

            Danger = Color.FromArgb(0xB4, 0x23, 0x18),
            DangerHover = Color.FromArgb(0x8F, 0x1D, 0x13),

            // No button in the product is green any more — Save preset and Add files are now
            // neutral. Success survives for the diff view and for any future affirmative state.
            Success = Color.FromArgb(0x1B, 0x7A, 0x43),
            SuccessHover = Color.FromArgb(0x15, 0x61, 0x34),

            Neutral = Color.FromArgb(0x44, 0x50, 0x6A),
            NeutralHover = Color.FromArgb(0x35, 0x3F, 0x55),

            Warning = Color.FromArgb(0xFD, 0xF3, 0xE7),
            WarningText = Color.FromArgb(0x8A, 0x4B, 0x0A),

            // Light sky blue at wash strength, so a selected row and an accent chip are visibly the
            // same idea rather than two unrelated blues.
            Selection = Color.FromArgb(0xE1, 0xED, 0xFD),

            DiffAdd = Color.FromArgb(0x1B, 0x7A, 0x43),
            DiffAddBg = Color.FromArgb(0xE8, 0xF5, 0xEE),
            DiffRemove = Color.FromArgb(0xB4, 0x23, 0x18),
            DiffRemoveBg = Color.FromArgb(0xFB, 0xEC, 0xEA),
            DiffContext = Color.FromArgb(0x44, 0x50, 0x6A),
        };

        public static readonly ThemeTokens Dark = new()
        {
            Surface = Color.FromArgb(0x15, 0x1A, 0x22),
            SurfaceAlt = Color.FromArgb(0x1D, 0x24, 0x2E),
            SurfaceSunken = Color.FromArgb(0x10, 0x15, 0x1C),

            TextPrimary = Color.FromArgb(0xE4, 0xEA, 0xF3),
            TextSecondary = Color.FromArgb(0x98, 0xA6, 0xBD),
            TextDisabled = Color.FromArgb(0x6A, 0x77, 0x89),

            // Still cobalt, lifted slightly: this is a fill carrying white text, so it is bounded by
            // contrast against white, not by visibility against the dark ground.
            Accent = Color.FromArgb(0x1A, 0x5F, 0xCC),
            AccentText = Color.White,
            AccentHover = Color.FromArgb(0x15, 0x4C, 0xA3),
            // Light sky blue. Cobalt scores 2.7:1 on this ground and would be all but invisible, so
            // the on-surface accent crosses to the light end of the family instead.
            AccentOnSurface = Color.FromArgb(0x6F, 0xBE, 0xF6),

            Border = Color.FromArgb(0x2C, 0x35, 0x42),
            BorderFocus = Color.FromArgb(0x5B, 0xB4, 0xF5),

            Danger = Color.FromArgb(0xA8, 0x1F, 0x14),
            DangerHover = Color.FromArgb(0x8C, 0x19, 0x11),

            Success = Color.FromArgb(0x19, 0x70, 0x3E),
            SuccessHover = Color.FromArgb(0x13, 0x59, 0x33),

            Neutral = Color.FromArgb(0x3E, 0x49, 0x59),
            NeutralHover = Color.FromArgb(0x4D, 0x59, 0x6C),

            Warning = Color.FromArgb(0x2E, 0x24, 0x17),
            WarningText = Color.FromArgb(0xE8, 0xBE, 0x7E),

            Selection = Color.FromArgb(0x12, 0x26, 0x3D),

            DiffAdd = Color.FromArgb(0x5F, 0xC9, 0x8A),
            DiffAddBg = Color.FromArgb(0x11, 0x29, 0x1B),
            DiffRemove = Color.FromArgb(0xF0, 0x82, 0x7A),
            DiffRemoveBg = Color.FromArgb(0x2E, 0x16, 0x16),
            DiffContext = Color.FromArgb(0xB7, 0xC3, 0xD4),
        };

        public static ThemeTokens For(ThemeMode mode) => mode switch
        {
            ThemeMode.Dark => Dark,
            // System-following is deferred: the enum member exists so the setting can be stored
            // and migrated, but until the watcher ships it resolves to Light rather than
            // pretending to track anything.
            _ => Light,
        };
    }
}
