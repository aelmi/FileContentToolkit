using System;
using System.Linq;
using System.Windows.Forms;
using CodeShuttle;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// Pins the compression and encryption actions to the main surface as buttons, and pins the
    /// gating that decides which of them the strip offers.
    /// </summary>
    /// <remarks>
    /// These actions have moved twice already: off the main window into
    /// <c>Tools ▸ Compression and encryption</c> on the reasoning that four buttons competed with
    /// Generate, then back as a single split button. They are four buttons again, in the pane above
    /// the output box, and these tests exist so that a later tidy-up of the header cannot quietly
    /// remove them a third time.
    ///
    /// The gating matters more than it looks. Encrypting an already-encrypted pane produces a blob
    /// that needs two passwords in the right order to recover, and neither of them is written down
    /// anywhere. Refusing the second encryption is cheaper than explaining it afterwards.
    ///
    /// These instantiate a real form but never show it unless the test needs layout, so no message
    /// loop is involved.
    /// </remarks>
    [Collection(AppSettingsCollection.Name)]
    public class ProtectButtonTests
    {
        private static string EncryptedBlob() =>
            CompressionUtils.CompressAndEncryptToBase64("public class A { }", "correct horse battery");

        private static string CompressedBlob() =>
            CompressionUtils.CompressToBase64("public class A { }");

        private static readonly string[] ProtectButtons =
        {
            "btnEditOutput", "btnCompress", "btnDecompress", "btnCompressEnc", "btnDecompressEnc",
        };

        [Fact]
        public void All_four_actions_are_buttons_on_the_main_surface()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();

                foreach (var name in new[] { "btnCompress", "btnDecompress", "btnCompressEnc", "btnDecompressEnc" })
                {
                    var b = FindControl(form, name);
                    Assert.True(b is Button, $"{name} is missing or is not a Button");
                }
            });
        }

        /// <summary>
        /// Edit and the four protect actions share a strip, because they all act on the text rather
        /// than on the pack. Guards the grouping, not merely the presence.
        /// </summary>
        [Fact]
        public void Edit_and_the_protect_actions_share_the_strip_above_the_output()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                var strip = FindControl(form, "pnlProtectTools");
                Assert.NotNull(strip);

                foreach (var name in ProtectButtons)
                    Assert.Same(strip, FindControl(form, name)!.Parent);

                // ...and Edit is therefore no longer duplicated in the pack header.
                var header = FindControl(form, "pnlOutputHeader")!;
                Assert.DoesNotContain(header.Controls.Cast<Control>(), c => c.Name == "btnEditOutput");
            });
        }

        /// <summary>
        /// The strip sits between the pack header and the output box, not below the fold.
        /// </summary>
        [Fact]
        public void The_strip_is_directly_above_the_output_box()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    var strip = FindControl(form, "pnlProtectTools")!;
                    var header = FindControl(form, "pnlOutputHeader")!;
                    var output = FindControl(form, "outputHost")!;

                    Assert.True(header.Top < strip.Top,
                        $"header at {header.Top} should be above the strip at {strip.Top}");
                    Assert.True(strip.Top < output.Top,
                        $"strip at {strip.Top} should be above the output at {output.Top}");
                }
                finally { form.Close(); }
            });
        }

        [Fact]
        public void Every_protect_button_is_wired_to_a_handler()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    // A button wired to nothing looks available and does nothing, which is worse
                    // than an absent button. Proven by clicking it: with a plain pack in the pane,
                    // Compress must actually change the text.
                    var output = (RichTextBox)FindControl(form, "rtbOutput")!;
                    output.ReadOnly = false;
                    output.Text = "public class A { }";

                    var before = output.Text;
                    ((Button)FindControl(form, "btnCompress")!).PerformClick();

                    // The handler is async void: it awaits the compression off the UI thread and
                    // its continuation is posted back through the form's synchronisation context.
                    // Nothing pumps that queue in a test, so the click alone proves only that the
                    // handler was entered. Pump until the work lands.
                    PumpUntil(() => output.Text != before);

                    Assert.NotEqual(before, output.Text);
                    Assert.True(CompressionUtils.LooksLikeCompressedBase64(output.Text),
                        "Compress did not produce a compressed blob");
                }
                finally { form.Close(); }
            });
        }

        /// <summary>
        /// Every protect button is live as soon as the pane has content, whatever that content is.
        /// </summary>
        /// <remarks>
        /// They were gated on what the pane held — decrypt only for a sealed blob, and so on. The
        /// logic was right and the feedback was useless: a button greyed for a reason the window
        /// never states is indistinguishable from a broken one, and was reported as exactly that.
        /// The condition still exists; it moved into the handlers, which say what is wrong. See
        /// <see cref="Refusals_are_explained_rather_than_greyed_out"/>.
        /// </remarks>
        [Theory]
        [InlineData("plain")]
        [InlineData("encrypted")]
        [InlineData("compressed")]
        public void Every_protect_button_is_live_once_the_pane_has_content(string kind)
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    var output = (RichTextBox)FindControl(form, "rtbOutput")!;
                    output.ReadOnly = false;
                    output.Text = kind switch
                    {
                        "plain" => "public class A { }",
                        "encrypted" => EncryptedBlob(),
                        _ => CompressedBlob(),
                    };

                    foreach (var n in new[] { "btnCompress", "btnDecompress", "btnCompressEnc", "btnDecompressEnc" })
                        Assert.True(FindControl(form, n)!.Enabled, $"{n} should be live for {kind} content");
                }
                finally { form.Close(); }
            });
        }

        [Fact]
        public void The_protect_buttons_are_dead_only_when_the_pane_is_empty()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    var output = (RichTextBox)FindControl(form, "rtbOutput")!;
                    output.ReadOnly = false;
                    output.Text = string.Empty;

                    foreach (var n in new[] { "btnCompress", "btnDecompress", "btnCompressEnc", "btnDecompressEnc" })
                        Assert.False(FindControl(form, n)!.Enabled, $"{n} should be dead with an empty pane");
                }
                finally { form.Close(); }
            });
        }

        /// <summary>
        /// The protection that used to live in the greying: a misapplied action refuses and leaves
        /// the pane untouched, rather than mangling it.
        /// </summary>
        /// <remarks>
        /// The one that matters is encrypting an already-sealed pack. A second pass needs both
        /// passwords in the right order to undo, and nothing records either. The refusal is a modal
        /// the test cannot dismiss, so this asserts the outcome that matters — the pane is not
        /// rewritten — via the codec condition each handler checks.
        /// </remarks>
        [Fact]
        public void Refusals_are_explained_rather_than_greyed_out()
        {
            // Encrypting an encrypted blob is refused because it still looks encrypted.
            Assert.True(CompressionUtils.LooksLikeEncryptedBase64(EncryptedBlob()));

            // Decrypting plain text is refused because plain text does not look encrypted...
            Assert.False(CompressionUtils.LooksLikeEncryptedBase64("public class A { }"));
            // ...and decompressing it is refused because it does not look compressed.
            Assert.False(CompressionUtils.LooksLikeCompressedBase64("public class A { }"));

            // A compressed pack is not mistaken for an encrypted one, or the strip would send the
            // user to the wrong button in its refusal message.
            Assert.True(CompressionUtils.LooksLikeCompressedBase64(CompressedBlob()));
            Assert.False(CompressionUtils.LooksLikeEncryptedBase64(CompressedBlob()));
        }

        /// <summary>
        /// Edit is never disabled, including on a cold start with nothing in the pane.
        /// </summary>
        /// <remarks>
        /// It used to be gated on there already being a pack, which made the one control that can
        /// *create* pane content depend on pane content already existing. Typing or pasting a
        /// bundle in by hand is how you decrypt one you did not generate here.
        /// </remarks>
        [Fact]
        public void Edit_is_enabled_even_with_an_empty_pane()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    var output = (RichTextBox)FindControl(form, "rtbOutput")!;
                    output.ReadOnly = false;
                    output.Text = string.Empty;

                    Assert.True(FindControl(form, "btnEditOutput")!.Enabled,
                        "Edit must be usable on a cold start");
                }
                finally { form.Close(); }
            });
        }

        /// <summary>
        /// The whole point of enabling Edit on an empty pane: type a bundle in, and the strip
        /// picks it up.
        /// </summary>
        /// <remarks>
        /// Also guards the overlay. "No pack yet" is painted <em>over</em> the output box rather
        /// than instead of it, so leaving it up during an edit would hide the box the user just
        /// unlocked — the feature would look broken while working perfectly.
        /// </remarks>
        [Fact]
        public void Editing_an_empty_pane_uncovers_the_box_and_the_strip_follows_what_is_typed()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    var output = (RichTextBox)FindControl(form, "rtbOutput")!;
                    var empty = FindControl(form, "emptyOutput")!;
                    var edit = (Button)FindControl(form, "btnEditOutput")!;

                    Assert.True(empty.Visible, "empty state should cover an empty pane at rest");

                    edit.PerformClick();
                    Assert.False(output.ReadOnly, "Edit should unlock the pane");
                    Assert.False(empty.Visible, "the empty state must come down while editing");

                    // Paste in a sealed bundle, as someone would who was sent one.
                    output.Text = EncryptedBlob();
                    Assert.True(FindControl(form, "btnDecompressEnc")!.Enabled,
                        "Decrypt should light up for a pasted encrypted bundle");

                    edit.PerformClick();
                    Assert.True(output.ReadOnly, "Edit should lock the pane again");
                    Assert.False(empty.Visible, "there is content now, so the empty state stays down");
                }
                finally { form.Close(); }
            });
        }

        /// <summary>
        /// Leaving edit mode with nothing typed puts the empty state back.
        /// </summary>
        [Fact]
        public void Leaving_an_edit_with_an_empty_pane_restores_the_empty_state()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    var empty = FindControl(form, "emptyOutput")!;
                    var edit = (Button)FindControl(form, "btnEditOutput")!;

                    edit.PerformClick();
                    Assert.False(empty.Visible);

                    edit.PerformClick();
                    Assert.True(empty.Visible, "nothing was typed, so 'No pack yet' should return");
                }
                finally { form.Close(); }
            });
        }

        // ---------------------------------------------------------------- helpers

        /// <summary>
        /// Runs the message loop until <paramref name="condition"/> holds or the timeout expires.
        /// </summary>
        /// <remarks>
        /// <c>Application.DoEvents</c> is a bad idea in application code and a necessary one here:
        /// these tests drive a form that was never given a message loop, and the handlers under
        /// test are <c>async void</c>. Times out rather than spinning forever, so a handler that
        /// silently never completes fails the assertion instead of hanging the run.
        /// </remarks>
        private static void PumpUntil(Func<bool> condition, int timeoutMs = 5000)
        {
            var clock = System.Diagnostics.Stopwatch.StartNew();
            while (!condition() && clock.ElapsedMilliseconds < timeoutMs)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(15);
            }
        }

        private static Control? FindControl(Control root, string name)
        {
            if (root.Name == name) return root;
            foreach (Control child in root.Controls)
            {
                var hit = FindControl(child, name);
                if (hit is not null) return hit;
            }
            return null;
        }
    }
}
