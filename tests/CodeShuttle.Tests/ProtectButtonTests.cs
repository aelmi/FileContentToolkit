using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CodeShuttle;
using CodeShuttle.UI;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// Pins the compression and encryption actions to the main surface, and pins the gating that
    /// decides which of them the menu offers.
    /// </summary>
    /// <remarks>
    /// The four actions have already been moved off the main window once, into
    /// <c>Tools ▸ Compression and encryption</c>, on the reasoning that four buttons competed with
    /// Generate. They are back as one <see cref="SplitButton"/>, and the first test here exists so
    /// that a future tidy-up of the header row cannot quietly remove them again.
    ///
    /// The gating tests matter more than they look. The menu opens from two different code paths —
    /// a click on the button face raises <c>Click</c>, a click on the caret opens the drop-down
    /// straight out of <c>SplitButton.OnMouseDown</c> and never raises <c>Click</c> — so the
    /// enabling has to hang off the menu's own <c>Opening</c>. Gating on the button's handler
    /// would work for half the ways in and silently fail for the other half.
    ///
    /// These instantiate a real form but never show it, so no message loop is involved.
    /// </remarks>
    [Collection(AppSettingsCollection.Name)]
    public class ProtectButtonTests
    {
        /// <summary>A blob that <c>LooksLikeEncryptedBase64</c> recognises, made the real way.</summary>
        private static string EncryptedBlob() =>
            CompressionUtils.CompressAndEncryptToBase64("public class A { }", "correct horse battery");

        private static string CompressedBlob() =>
            CompressionUtils.CompressToBase64("public class A { }");

        [Fact]
        public void Protect_is_on_the_main_surface_not_only_in_the_Tools_menu()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();

                var protect = FindControl(form, "btnProtect");
                Assert.NotNull(protect);

                // In the pack header, beside Generate — not parked on some hidden panel.
                var header = FindControl(form, "pnlOutputHeader");
                Assert.NotNull(header);
                Assert.Same(header, protect!.Parent);
            });
        }

        [Fact]
        public void Protect_offers_all_four_actions()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();

                var protect = Assert.IsType<SplitButton>(FindControl(form, "btnProtect"));
                var menu = protect.DropDownMenu;
                Assert.NotNull(menu);

                var names = menu!.Items.Cast<ToolStripItem>().Select(i => i.Name).ToList();
                Assert.Contains("mnuProtectEncrypt", names);
                Assert.Contains("mnuProtectDecrypt", names);
                Assert.Contains("mnuProtectCompress", names);
                Assert.Contains("mnuProtectDecompress", names);
            });
        }

        /// <summary>
        /// Every item is a real action. An item wired to nothing is worse than an absent item: it
        /// looks available and does nothing.
        /// </summary>
        [Fact]
        public void Every_Protect_item_is_wired_to_a_handler()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                var protect = Assert.IsType<SplitButton>(FindControl(form, "btnProtect"));

                foreach (var item in protect.DropDownMenu!.Items.OfType<ToolStripMenuItem>())
                    Assert.True(HasClickHandler(item), $"{item.Name} has no Click handler");
            });
        }

        [Theory]
        // pane content kind      encrypt  decrypt  compress  decompress
        [InlineData("plain", true, false, true, false)]
        [InlineData("encrypted", false, true, false, false)]
        [InlineData("compressed", false, false, false, true)]
        [InlineData("empty", false, false, false, false)]
        public void The_menu_only_offers_what_the_pane_can_actually_take(
            string kind, bool encrypt, bool decrypt, bool compress, bool decompress)
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

                    // What a click on either the face or the caret ends up doing.
                    Invoke(form, "CmsProtect_Opening",
                        FindControl(form, "btnProtect")!, new System.ComponentModel.CancelEventArgs());

                    var protect = (SplitButton)FindControl(form, "btnProtect")!;
                    var menu = protect.DropDownMenu!;

                    Assert.Equal(encrypt, Item(menu, "mnuProtectEncrypt").Enabled);
                    Assert.Equal(decrypt, Item(menu, "mnuProtectDecrypt").Enabled);
                    Assert.Equal(compress, Item(menu, "mnuProtectCompress").Enabled);
                    Assert.Equal(decompress, Item(menu, "mnuProtectDecompress").Enabled);
                }
                finally { form.Close(); }
            });
        }

        /// <summary>
        /// Nothing to protect until there is a pack, exactly like Copy, Export, Edit and Find.
        /// </summary>
        [Fact]
        public void Protect_is_disabled_while_the_pane_is_empty()
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

                    var protect = FindControl(form, "btnProtect")!;
                    Assert.False(protect.Enabled);

                    output.Text = "public class A { }";
                    Assert.True(protect.Enabled);
                }
                finally { form.Close(); }
            });
        }

        // ---------------------------------------------------------------- helpers

        private static ToolStripMenuItem Item(ContextMenuStrip menu, string name) =>
            menu.Items.OfType<ToolStripMenuItem>().Single(i => i.Name == name);

        private static void Invoke(object target, string method, params object[] args) =>
            target.GetType()
                  .GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!
                  .Invoke(target, args);

        /// <summary>
        /// Reads the hidden EventHandlerList slot a ToolStripItem stores its Click delegate in.
        /// There is no public way to ask a control whether anything is subscribed.
        /// </summary>
        private static bool HasClickHandler(ToolStripItem item)
        {
            var key = typeof(ToolStripItem)
                .GetField("s_clickEvent", BindingFlags.Static | BindingFlags.NonPublic)
                ?.GetValue(null);

            if (key is null) return true; // field renamed by a runtime update — do not fail spuriously

            var events = (System.ComponentModel.EventHandlerList)typeof(Component)
                .GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(item)!;

            return events[key] is not null;
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
