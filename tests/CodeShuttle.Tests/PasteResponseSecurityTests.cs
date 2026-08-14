using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeShuttle.Dialogs;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// The paste surface is a new inbound path for untrusted text, so it gets its own containment
    /// tests rather than relying on the ones behind <c>FileRecreator.Plan</c>.
    /// </summary>
    /// <remarks>
    /// The distinction matters. <c>PathSafetyTests</c> proves the containment logic is correct;
    /// these prove <em>this dialog reaches it</em>. A second, unvalidated parser here would leave
    /// every one of those tests passing while reintroducing an arbitrary file write — a bundle
    /// header escaping the target folder into the Windows Startup directory is arbitrary code
    /// execution. Everything below drives <c>PasteResponseForm.BuildPlan</c>, which is the exact
    /// method the Review button calls.
    /// </remarks>
    public class PasteResponseSecurityTests
    {
        /// <summary>Builds a legacy-format bundle — the bare <c>path:</c> header of older packs.</summary>
        private static string Bundle(string header, string content) =>
            header + ":\n" + content + "\n\n\n\n";

        /// <summary>
        /// Builds a framed bundle, the format Generate emits today.
        /// </summary>
        /// <remarks>
        /// The two formats are not interchangeable for these tests. The legacy header rule only
        /// recognises a line as a header when its second character is a colon or it begins with
        /// <c>.\</c>, so relative escapes such as <c>..\x.cs</c> or <c>a\..\..\x.cs</c> are not
        /// headers to it at all and parse to nothing. The framed format accepts an arbitrary
        /// path, which is exactly why containment has to be enforced downstream rather than
        /// relying on the parser to be picky.
        /// </remarks>
        private static string Framed(params (string Path, string Content)[] entries) =>
            BundleFormat.Write(entries.Select(e => new BundleEntry
            {
                Path = e.Path,
                Content = e.Content,
                EndsWithNewline = true,
                HasMetadata = true,
            }));

        [Fact]
        public void Traversal_payload_pasted_into_the_dialog_is_rejected_and_writes_nothing()
        {
            using var dir = new TempDir();
            var target = Path.Combine(dir.Path, "target");
            Directory.CreateDirectory(target);

            // The payload from the original finding: escape the chosen folder and land in the
            // per-user Startup directory, where Windows will run it at next logon.
            const string payload =
                @".\..\..\..\Users\me\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\evil.bat";

            StaRunner.Run(() =>
            {
                using var dlg = new PasteResponseForm();
                dlg.ResponseText = Bundle(payload, "@echo off\r\ncalc.exe");
                dlg.TargetFolder = target;

                var plan = dlg.BuildPlan(out var problem);

                Assert.True(plan != null, "the dialog should produce a plan, with the entry rejected: " + problem);

                Assert.All(plan!.Plans, p =>
                    Assert.Equal(FilePlanStatus.Rejected, p.Status));

                Assert.All(plan.Plans, p =>
                    Assert.False(string.IsNullOrWhiteSpace(p.RejectionReason),
                        "a rejected entry must say why; a silent drop is indistinguishable from a bug"));
            });

            // Nothing outside the chosen folder, and nothing inside it either: planning writes.
            Assert.Empty(Directory.GetFiles(dir.Path, "*.bat", SearchOption.AllDirectories));
        }

        /// <summary>
        /// The invariant that actually matters: whatever is pasted, no entry may resolve to a
        /// path outside the chosen folder. An entry is allowed to be accepted or rejected — a
        /// lone absolute path is legitimately rebased under the target, which is how applying a
        /// pack to a different checkout is meant to work — but it may never point outside.
        /// </summary>
        /// <remarks>
        /// Asserting "everything is rejected" would have been the wrong test: it fails on the
        /// rebasing case, and it would also pass against a dialog that rejected every input,
        /// including valid ones.
        /// </remarks>
        [Theory]
        [InlineData(@"..\escape.cs")]
        [InlineData(@"C:\Windows\Temp\rooted.cs")]
        [InlineData(@"sub\..\..\escape.cs")]
        [InlineData(@"good\..\..\..\evil.cs")]
        [InlineData(@"..\..\..\Windows\System32\drivers\etc\hosts")]
        [InlineData(@".\..\..\Startup\evil.bat")]
        public void No_pasted_entry_can_ever_resolve_outside_the_chosen_folder(string header)
        {
            using var dir = new TempDir();
            var target = Path.Combine(dir.Path, "target");
            Directory.CreateDirectory(target);
            var targetFull = Path.GetFullPath(target) + Path.DirectorySeparatorChar;

            StaRunner.Run(() =>
            {
                using var dlg = new PasteResponseForm();
                dlg.ResponseText = Framed((header, "payload"));
                dlg.TargetFolder = target;

                var plan = dlg.BuildPlan(out var problem);
                Assert.True(plan != null, problem);
                Assert.NotEmpty(plan!.Plans);

                foreach (var entry in plan.Plans)
                {
                    if (entry.Status == FilePlanStatus.Rejected)
                    {
                        Assert.False(string.IsNullOrWhiteSpace(entry.RejectionReason),
                            "a rejected entry must say why");
                        continue;
                    }

                    Assert.StartsWith(targetFull, Path.GetFullPath(entry.TargetPath),
                        System.StringComparison.OrdinalIgnoreCase);
                }
            });
        }

        /// <summary>
        /// The containment check is on the resolved path, so a legitimate nested file must still
        /// come through. A test that only proves things are rejected passes just as well against
        /// a dialog that rejects everything.
        /// </summary>
        [Fact]
        public void A_legitimate_nested_path_is_still_accepted()
        {
            using var dir = new TempDir();
            var target = Path.Combine(dir.Path, "target");
            Directory.CreateDirectory(target);

            StaRunner.Run(() =>
            {
                using var dlg = new PasteResponseForm();
                dlg.ResponseText = Bundle(@".\src\deep\ok.cs", "class Ok { }");
                dlg.TargetFolder = target;

                var plan = dlg.BuildPlan(out var problem);

                Assert.True(plan != null, problem);
                Assert.NotEmpty(plan!.Plans);
                Assert.DoesNotContain(plan.Plans, p => p.Status == FilePlanStatus.Rejected);
                Assert.True(plan.CanProceed);
            });
        }

        /// <summary>
        /// Two entries resolving to one file is the UNC-flattening class of bug. The refusal
        /// lives behind <c>CanProceed</c>, and the dialog must be checking it rather than looking
        /// only at the entry count.
        /// </summary>
        [Fact]
        public void Duplicate_targets_pasted_into_the_dialog_leave_the_plan_unable_to_proceed()
        {
            using var dir = new TempDir();
            var target = Path.Combine(dir.Path, "target");
            Directory.CreateDirectory(target);

            StaRunner.Run(() =>
            {
                using var dlg = new PasteResponseForm();
                dlg.ResponseText = Framed(
                    (@"\\server\share\src\util.cs", "one"),
                    (@"\\server\share\test\util.cs", "two"));
                dlg.TargetFolder = target;

                var plan = dlg.BuildPlan(out _);

                if (plan != null && plan.Plans.Select(p => p.TargetPath)
                                       .Distinct(System.StringComparer.OrdinalIgnoreCase).Count() < plan.Plans.Count)
                {
                    Assert.False(plan.CanProceed,
                        "two entries resolved to one file and the plan still claimed it could proceed");
                    Assert.NotEmpty(plan.Errors);
                }
            });
        }

        /// <summary>Text that is not a pack is reported, not silently treated as an empty plan.</summary>
        [Fact]
        public void Prose_that_is_not_a_pack_is_reported_rather_than_applied()
        {
            using var dir = new TempDir();

            StaRunner.Run(() =>
            {
                using var dlg = new PasteResponseForm();
                dlg.ResponseText = "Sure! Here is what I would change in your code:\n\n- rename the variable";
                dlg.TargetFolder = dir.Path;

                var plan = dlg.BuildPlan(out var problem);

                Assert.Null(plan);
                Assert.False(string.IsNullOrWhiteSpace(problem));
            });
        }

        [Fact]
        public void A_target_folder_that_does_not_exist_is_reported()
        {
            StaRunner.Run(() =>
            {
                using var dlg = new PasteResponseForm();
                dlg.ResponseText = Bundle(@".\a.cs", "x");
                dlg.TargetFolder = @"Z:\definitely\not\here";

                var plan = dlg.BuildPlan(out var problem);

                Assert.Null(plan);
                Assert.Contains("does not exist", problem);
            });
        }

        /// <summary>
        /// The Review button must stay disabled until there is both text and a folder, so the
        /// affirmative action is never available in a state that cannot succeed.
        /// </summary>
        [Fact]
        public void Review_is_unavailable_until_both_a_response_and_a_folder_are_present()
        {
            using var dir = new TempDir();

            StaRunner.Run(() =>
            {
                using var dlg = new PasteResponseForm();
                var review = FindButton(dlg, "btnReview");

                Assert.False(review.Enabled);

                dlg.ResponseText = Bundle(@".\a.cs", "x");
                Assert.False(review.Enabled);

                dlg.TargetFolder = dir.Path;
                Assert.True(review.Enabled);
            });
        }

        private static System.Windows.Forms.Button FindButton(System.Windows.Forms.Control root, string name)
        {
            var found = FindButtonOrNull(root, name);
            Assert.True(found != null, $"button '{name}' not found");
            return found!;
        }

        private static System.Windows.Forms.Button? FindButtonOrNull(System.Windows.Forms.Control root, string name)
        {
            foreach (System.Windows.Forms.Control child in root.Controls)
            {
                if (child.Name == name && child is System.Windows.Forms.Button b) return b;
                var deeper = FindButtonOrNull(child, name);
                if (deeper != null) return deeper;
            }
            return null;
        }
    }
}
