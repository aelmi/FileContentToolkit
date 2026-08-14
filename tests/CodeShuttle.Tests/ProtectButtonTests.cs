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

        [Theory]
        // pane content        compress  decompress  encrypt  decrypt
        [InlineData("plain", true, false, true, false)]
        [InlineData("encrypted", false, false, false, true)]
        [InlineData("compressed", false, true, false, false)]
        [InlineData("empty", false, false, false, false)]
        public void The_strip_only_offers_what_the_pane_can_actually_take(
            string kind, bool compress, bool decompress, bool encrypt, bool decrypt)
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
                        "compressed" => CompressedBlob(),
                        _ => string.Empty,
                    };

                    Assert.Equal(compress, FindControl(form, "btnCompress")!.Enabled);
                    Assert.Equal(decompress, FindControl(form, "btnDecompress")!.Enabled);
                    Assert.Equal(encrypt, FindControl(form, "btnCompressEnc")!.Enabled);
                    Assert.Equal(decrypt, FindControl(form, "btnDecompressEnc")!.Enabled);
                }
                finally { form.Close(); }
            });
        }

        /// <summary>
        /// The specific mistake the gating exists to prevent: a second encryption over the first,
        /// producing a blob that needs two passwords in order and records neither.
        /// </summary>
        [Fact]
        public void An_encrypted_pane_cannot_be_encrypted_again()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    var output = (RichTextBox)FindControl(form, "rtbOutput")!;
                    output.ReadOnly = false;
                    output.Text = EncryptedBlob();

                    Assert.False(FindControl(form, "btnCompressEnc")!.Enabled);
                    Assert.False(FindControl(form, "btnCompress")!.Enabled);
                    Assert.True(FindControl(form, "btnDecompressEnc")!.Enabled);
                }
                finally { form.Close(); }
            });
        }

        [Fact]
        public void Edit_is_disabled_while_the_pane_is_empty()
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
                    Assert.False(FindControl(form, "btnEditOutput")!.Enabled);

                    output.Text = "public class A { }";
                    Assert.True(FindControl(form, "btnEditOutput")!.Enabled);
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
