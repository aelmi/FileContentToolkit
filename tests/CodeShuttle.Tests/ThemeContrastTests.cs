using System;
using System.Collections.Generic;
using System.Drawing;
using CodeShuttle.Theming;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// Pins the measured WCAG 2.1 contrast failures so they cannot come back.
    /// </summary>
    /// <remarks>
    /// Every one of these pairs was measured as failing before the token system: white on the
    /// success green at 3.13:1, secondary text on the form background at 3.98:1, the diff green on
    /// white at about 3:1, the split-button separator at 2.07:1 and its disabled caret at 1.67:1.
    /// The green failed in dark mode too, because the old mapper passed vivid accent colours
    /// through unchanged — which is why every assertion here runs against both palettes.
    /// </remarks>
    public class ThemeContrastTests
    {
        public static IEnumerable<object[]> Palettes() => new[]
        {
            new object[] { "Light", ThemePalettes.Light },
            new object[] { "Dark", ThemePalettes.Dark },
        };

        private const double AaText = 4.5;      // WCAG 2.1 AA, normal-size text
        private const double AaNonText = 3.0;   // WCAG 2.1 AA, user-interface components

        [Theory]
        [MemberData(nameof(Palettes))]
        public void TextOnAccentFillsMeetsAa(string name, ThemeTokens t)
        {
            // The specific regression: white on Success used to score 3.13:1 in both palettes.
            AssertAtLeast(AaText, t.AccentText, t.Success, $"{name}: AccentText on Success");
            AssertAtLeast(AaText, t.AccentText, t.Accent, $"{name}: AccentText on Accent");
            AssertAtLeast(AaText, t.AccentText, t.Danger, $"{name}: AccentText on Danger");
            AssertAtLeast(AaText, t.AccentText, t.Neutral, $"{name}: AccentText on Neutral");

            // The hover fills, which this theory used to omit. Theme.AttachHover swaps BackColor
            // and leaves ForeColor at AccentText, so a hover state is a text-on-fill pair like any
            // other — and it was failing precisely while the pointer was over the button, where
            // nobody looks for a contrast bug. Measured before the fix: white on the dark
            // AccentHover 3.94:1, on the dark SuccessHover 3.86:1.
            AssertAtLeast(AaText, t.AccentText, t.AccentHover, $"{name}: AccentText on AccentHover");
            AssertAtLeast(AaText, t.AccentText, t.SuccessHover, $"{name}: AccentText on SuccessHover");
            AssertAtLeast(AaText, t.AccentText, t.DangerHover, $"{name}: AccentText on DangerHover");
            AssertAtLeast(AaText, t.AccentText, t.NeutralHover, $"{name}: AccentText on NeutralHover");
        }

        [Theory]
        [MemberData(nameof(Palettes))]
        public void BodyAndSecondaryTextMeetAaOnEverySurface(string name, ThemeTokens t)
        {
            foreach (var (surfaceName, surface) in new[]
                     {
                         ("Surface", t.Surface),
                         ("SurfaceAlt", t.SurfaceAlt),
                         ("SurfaceSunken", t.SurfaceSunken),
                     })
            {
                AssertAtLeast(AaText, t.TextPrimary, surface, $"{name}: TextPrimary on {surfaceName}");
                // The other measured failure: (108,117,125) on the form background.
                AssertAtLeast(AaText, t.TextSecondary, surface, $"{name}: TextSecondary on {surfaceName}");
            }
        }

        [Theory]
        [MemberData(nameof(Palettes))]
        public void DiffColoursMeetAaAgainstTheDiffBackground(string name, ThemeTokens t)
        {
            AssertAtLeast(AaText, t.DiffAdd, t.SurfaceSunken, $"{name}: DiffAdd on SurfaceSunken");
            AssertAtLeast(AaText, t.DiffRemove, t.SurfaceSunken, $"{name}: DiffRemove on SurfaceSunken");
            AssertAtLeast(AaText, t.DiffContext, t.SurfaceSunken, $"{name}: DiffContext on SurfaceSunken");
            AssertAtLeast(AaText, t.DiffAdd, t.DiffAddBg, $"{name}: DiffAdd on DiffAddBg");
            AssertAtLeast(AaText, t.DiffRemove, t.DiffRemoveBg, $"{name}: DiffRemove on DiffRemoveBg");
        }

        [Theory]
        [MemberData(nameof(Palettes))]
        public void BannerTextMeetsAaOnTheBanner(string name, ThemeTokens t)
        {
            AssertAtLeast(AaText, t.WarningText, t.Warning, $"{name}: WarningText on Warning");
        }

        [Theory]
        [MemberData(nameof(Palettes))]
        public void InertChromeMeetsTheNonTextThreshold(string name, ThemeTokens t)
        {
            // TextDisabled is what the split button now draws its separator and its disabled caret
            // with; those were 2.07:1 and 1.67:1 as hardcoded greys.
            AssertAtLeast(AaNonText, t.TextDisabled, t.Surface, $"{name}: TextDisabled on Surface");
            AssertAtLeast(AaNonText, t.TextDisabled, t.SurfaceAlt, $"{name}: TextDisabled on SurfaceAlt");
            AssertAtLeast(AaNonText, t.BorderFocus, t.Surface, $"{name}: BorderFocus on Surface");
        }

        [Theory]
        [MemberData(nameof(Palettes))]
        public void AccentTextOnSurfaceMeetsAa(string name, ThemeTokens t)
        {
            AssertAtLeast(AaText, t.AccentOnSurface, t.Surface, $"{name}: AccentOnSurface on Surface");
            AssertAtLeast(AaText, t.AccentOnSurface, t.SurfaceAlt, $"{name}: AccentOnSurface on SurfaceAlt");
        }

        [Fact]
        public void ThePreviousPaletteWouldHaveFailed()
        {
            // Guards the test itself: if ContrastRatio were wrong, everything above would pass
            // vacuously. These are the exact colours and the exact ratios from the audit.
            Assert.InRange(ContrastRatio(Color.White, Color.FromArgb(40, 167, 69)), 3.0, 3.3);
            Assert.InRange(ContrastRatio(Color.FromArgb(108, 117, 125), Color.FromArgb(245, 247, 250)), 3.9, 4.45);
            Assert.InRange(ContrastRatio(Color.FromArgb(180, 180, 180), Color.White), 2.0, 2.2);
            Assert.InRange(ContrastRatio(Color.FromArgb(200, 200, 200), Color.White), 1.6, 1.8);
        }

        private static void AssertAtLeast(double minimum, Color foreground, Color background, string what)
        {
            double actual = ContrastRatio(foreground, background);
            Assert.True(actual >= minimum,
                $"{what}: contrast {actual:0.00}:1 is below the required {minimum:0.0}:1 " +
                $"(fg #{foreground.R:X2}{foreground.G:X2}{foreground.B:X2}, " +
                $"bg #{background.R:X2}{background.G:X2}{background.B:X2}).");
        }

        /// <summary>WCAG 2.1 relative-luminance contrast ratio.</summary>
        private static double ContrastRatio(Color a, Color b)
        {
            double la = RelativeLuminance(a);
            double lb = RelativeLuminance(b);
            double lighter = Math.Max(la, lb);
            double darker = Math.Min(la, lb);
            return (lighter + 0.05) / (darker + 0.05);
        }

        private static double RelativeLuminance(Color c)
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
