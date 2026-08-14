using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Theming;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// Regression cover for the tool-strip hosted-control crash.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>ThemeApplier.ResolveItem</c> used to set <c>BackColor = Color.Transparent</c> on every
    /// <see cref="ToolStripItem"/>, on the reasoning that the renderer paints them and only the
    /// text colour matters. That holds for ordinary items, but <see cref="ToolStripControlHost"/>
    /// forwards <c>BackColor</c> to a real hosted <see cref="Control"/>, and
    /// <see cref="ProgressBar"/> throws <c>ArgumentException: Control does not support transparent
    /// background colors.</c>
    /// </para>
    /// <para>
    /// The status strip's <c>sbProgress</c> is a <see cref="ToolStripProgressBar"/>, so every
    /// theme application against the main window threw during <c>OnLoad</c> — the app crashed on
    /// startup. It survived the whole overhaul because the theme system and the status-strip
    /// progress bar were built by different workstreams and no test applied one to the other.
    /// </para>
    /// </remarks>
    // Builds every form in the product, so it shares the process-wide theme and settings state
    // that the other form-constructing suites use, and must not run alongside them.
    [Collection(AppSettingsCollection.Name)]
    public class ThemeApplierHostedControlTests
    {
        [Fact]
        public void Applying_a_theme_to_a_hosted_progress_bar_does_not_throw()
        {
            StaRunner.Run(() =>
            {
                using var form = new Form();
                using var strip = new StatusStrip();
                var progress = new ToolStripProgressBar();
                var label = new ToolStripStatusLabel("status");

                strip.Items.Add(label);
                strip.Items.Add(progress);
                form.Controls.Add(strip);

                // Threw ArgumentException before the fix, in both palettes.
                ThemeApplier.Apply(form, ThemePalettes.Dark, dark: true);
                ThemeApplier.Apply(form, ThemePalettes.Light, dark: false);
            });
        }

        [Fact]
        public void A_hosted_control_receives_a_real_opaque_fill_not_transparent()
        {
            StaRunner.Run(() =>
            {
                using var form = new Form();
                using var strip = new StatusStrip();
                var progress = new ToolStripProgressBar();
                strip.Items.Add(progress);
                form.Controls.Add(strip);

                ThemeApplier.Apply(form, ThemePalettes.Dark, dark: true);

                // The point of the fix: the hosted control is themed through the normal path, so
                // it ends up with a token colour rather than being skipped or left transparent.
                Assert.NotEqual(Color.Transparent, progress.Control.BackColor);
                Assert.Equal(255, progress.Control.BackColor.A);
            });
        }

        [Fact]
        public void Ordinary_tool_strip_items_are_still_transparent_for_the_renderer()
        {
            StaRunner.Run(() =>
            {
                using var form = new Form();
                using var strip = new StatusStrip();
                var label = new ToolStripStatusLabel("status");
                strip.Items.Add(label);
                form.Controls.Add(strip);

                ThemeApplier.Apply(form, ThemePalettes.Dark, dark: true);

                // The original behaviour must survive for items the renderer actually paints.
                Assert.Equal(Color.Transparent, label.BackColor);
            });
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Every_form_can_be_themed_without_throwing(bool dark)
        {
            StaRunner.Run(() =>
            {
                var tokens = dark ? ThemePalettes.Dark : ThemePalettes.Light;

                // ShellUiTests already builds one of every form to check tab order and scale
                // metrics, but it never applied a theme to them — which is exactly the gap the
                // ToolStripProgressBar crash fell through. Applying both palettes to all 16 forms
                // makes any future "control does not support X" throw a test failure rather than a
                // crash dialog on the user's first launch.
                foreach (var form in ShellUiTests.EveryFormForTheming())
                {
                    using (form)
                    {
                        ThemeApplier.Apply(form, tokens, dark);
                    }
                }
            });
        }

        [Fact]
        public void Drop_down_items_are_still_walked_after_the_host_branch()
        {
            StaRunner.Run(() =>
            {
                using var form = new Form();
                using var menu = new MenuStrip();
                var top = new ToolStripMenuItem("File");
                var child = new ToolStripMenuItem("Open");
                top.DropDownItems.Add(child);
                menu.Items.Add(top);
                form.Controls.Add(menu);

                ThemeApplier.Apply(form, ThemePalettes.Dark, dark: true);

                // The host branch sits directly above the drop-down recursion; this pins that the
                // early return did not swallow it.
                Assert.Equal(ThemePalettes.Dark.TextPrimary, child.ForeColor);
            });
        }
    }
}
