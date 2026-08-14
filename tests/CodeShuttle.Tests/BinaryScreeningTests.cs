using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeShuttle.Filters;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// Files that cannot be read as text must never reach the pack.
    /// </summary>
    /// <remarks>
    /// The folder scan always classified its candidates. The two explicit routes — dropping files
    /// on the list, and the Add files dialog — bypassed that and added whatever they were handed,
    /// so dropping an .ico produced a bundle entry whose body was a .NET decoder error. Because a
    /// bundle is designed to be pasted to an AI and applied back to disk, that error string sat
    /// exactly where the file's source belongs, one round trip away from being written into the
    /// user's real file.
    /// </remarks>
    [Collection(AppSettingsCollection.Name)]
    public class BinaryScreeningTests : IDisposable
    {
        private readonly string _dir;

        public BinaryScreeningTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "cs-binary-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
        }

        public void Dispose()
        {
            try { Directory.Delete(_dir, recursive: true); } catch { /* best effort */ }
            GC.SuppressFinalize(this);
        }

        private string WriteBinary(string name)
        {
            var path = Path.Combine(_dir, name);
            // An .ico begins 00 00 01 00 — null bytes in the first sample, which is exactly what
            // the detector looks for.
            File.WriteAllBytes(path, new byte[] { 0x00, 0x00, 0x01, 0x00, 0x10, 0x10, 0x00, 0x00, 0xED, 0x42 });
            return path;
        }

        private string WriteText(string name, string content)
        {
            var path = Path.Combine(_dir, name);
            File.WriteAllText(path, content);
            return path;
        }

        [Fact]
        public void An_icon_is_classified_as_binary()
        {
            Assert.Equal(FileReadability.Binary, BinaryFileDetector.Classify(WriteBinary("app.ico")));
        }

        [Fact]
        public void A_source_file_is_classified_as_text()
        {
            Assert.Equal(FileReadability.Text,
                BinaryFileDetector.Classify(WriteText("Program.cs", "class P { }")));
        }

        /// <summary>
        /// The reported defect: an icon dropped on the file list was added, and then appeared in
        /// the pack with a decoder error as its content.
        /// </summary>
        [Fact]
        public void Dropping_a_binary_file_does_not_add_it()
        {
            var icon = WriteBinary("app.ico");
            var source = WriteText("Program.cs", "class P { }");

            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    Add(form, new[] { icon, source });

                    var files = SelectedFiles(form);
                    Assert.Contains(source, files);
                    Assert.DoesNotContain(icon, files);
                }
                finally { form.Hide(); }
            });
        }

        /// <summary>
        /// Tests the policy, not the window. When every chosen file is refused the UI shows a
        /// modal explanation, and driving that from a test blocks the STA thread — which once led
        /// to the dialog being deleted so the test would pass, leaving a dropped icon looking like
        /// a drop that silently failed.
        /// </summary>
        [Fact]
        public void Screening_refuses_every_binary()
        {
            var icon = WriteBinary("app.ico");
            var png = WriteBinary("logo.png");

            var (accepted, refused) = MainForm.ScreenChosenFiles(new[] { icon, png });

            Assert.Empty(accepted);
            Assert.Equal(2, refused.Count);
            Assert.All(refused, f => Assert.Equal(SkipReason.Binary, f.Reason));
        }

        /// <summary>
        /// The message has to name the file and say why, because it is the only thing standing
        /// between the user and the conclusion that the application is broken.
        /// </summary>
        [Fact]
        public void Refusal_message_names_the_file_and_the_reason()
        {
            var icon = WriteBinary("app.ico");
            var (_, refused) = MainForm.ScreenChosenFiles(new[] { icon });

            var message = MainForm.DescribeRefusals(refused);

            Assert.Contains("app.ico", message, StringComparison.Ordinal);
            Assert.Contains("binary file", message, StringComparison.Ordinal);
            Assert.Contains("plain text", message, StringComparison.Ordinal);
        }

        /// <summary>
        /// Naming a file explicitly outranks the extension filters — but not readability.
        /// </summary>
        [Fact]
        public void An_explicitly_chosen_file_keeps_its_extension_privilege()
        {
            var odd = WriteText("notes.unconfigured", "plain text");

            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    Add(form, new[] { odd });
                    Assert.Contains(odd, SelectedFiles(form));
                }
                finally { form.Hide(); }
            });
        }

        /// <summary>
        /// The defence that matters: a binary already sitting in the file list — restored from a
        /// saved session, loaded from a preset, or put there by an older build that had no
        /// screening — must still not reach the pack. Add-time screening cannot cover any of
        /// those, which is why generate screens again.
        /// </summary>
        [Fact]
        public void A_binary_already_in_the_list_never_reaches_the_pack()
        {
            var icon = WriteBinary("app.ico");
            var source = WriteText("Program.cs", "class P { }");

            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                form.Show();
                try
                {
                    // Straight into the service, bypassing every UI path — exactly what restoring
                    // a persisted list does.
                    Service(form).AddFiles(new[] { icon, source });

                    var pack = Generate(form);

                    Assert.Contains("Program.cs", pack, StringComparison.Ordinal);
                    Assert.DoesNotContain("app.ico", pack, StringComparison.Ordinal);
                    Assert.DoesNotContain("Error reading", pack, StringComparison.Ordinal);
                }
                finally { form.Hide(); }
            });
        }

        /// <summary>Runs the real generate and returns the pack text.</summary>
        private static string Generate(Form form)
        {
            var button = (Button)FindControl(form, "btnGenerate");
            button.PerformClick();

            var output = (RichTextBox)FindControl(form, "rtbOutput");
            var deadline = DateTime.UtcNow.AddSeconds(30);
            while (DateTime.UtcNow < deadline && output.TextLength == 0)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(25);
            }
            // Let the styling and statistics tail finish before reading.
            for (int i = 0; i < 20; i++) { Application.DoEvents(); System.Threading.Thread.Sleep(25); }
            return output.Text;
        }

        private static Control FindControl(Control root, string name)
        {
            foreach (Control child in root.Controls)
            {
                if (child.Name == name) return child;
                var deeper = FindControl2(child, name);
                if (deeper != null) return deeper;
            }
            throw new Xunit.Sdk.XunitException($"control '{name}' not found");
        }

        private static Control? FindControl2(Control root, string name)
        {
            foreach (Control child in root.Controls)
            {
                if (child.Name == name) return child;
                var deeper = FindControl2(child, name);
                if (deeper != null) return deeper;
            }
            return null;
        }

        private static FileContentService Service(Form form)
        {
            var field = typeof(MainForm).GetField("fileService",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            return (FileContentService)field.GetValue(form)!;
        }

        // ---------------------------------------------------------------- helpers

        private static void Add(Form form, string[] paths)
        {
            var method = typeof(MainForm).GetMethod("AddChosenFiles",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.True(method != null, "AddChosenFiles not found on MainForm");
            method!.Invoke(form, new object[] { paths });
        }

        private static System.Collections.Generic.List<string> SelectedFiles(Form form)
        {
            var field = typeof(MainForm).GetField("fileService",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.True(field != null, "fileService not found on MainForm");
            var service = (FileContentService)field!.GetValue(form)!;
            return service.SelectedFiles.ToList();
        }
    }
}
