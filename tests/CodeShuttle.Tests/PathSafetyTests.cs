using System;
using System.IO;
using System.Linq;
using Xunit;

namespace CodeShuttle.Tests
{
    public class PathSafetyTests
    {
        private const string Root = @"C:\target";

        [Theory]
        [InlineData(@"src\file.cs")]
        [InlineData(@"src/nested/deep/file.cs")]
        [InlineData(@"file.cs")]
        [InlineData(@"a.b.c\d.e.f")]
        public void ValidNestedPathsAreAccepted(string relative)
        {
            Assert.True(PathSafety.TryResolveContained(Root, relative, out var full, out var reason), reason);
            Assert.StartsWith(Root + Path.DirectorySeparatorChar, full, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(@"..\escape.txt")]
        [InlineData(@"../escape.txt")]
        [InlineData(@"src\..\..\escape.txt")]
        [InlineData(@".\..\..\..\Windows\Temp\evil.bat")]
        public void TraversalIsRejected(string relative)
        {
            Assert.False(PathSafety.TryResolveContained(Root, relative, out _, out var reason));
            Assert.Contains("escape", reason, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(@"C:\Windows\System32\evil.dll")]
        [InlineData(@"\\server\share\evil.dll")]
        [InlineData(@"\absolute\from\root.txt")]
        public void RootedPathsAreRejected(string relative)
        {
            Assert.False(PathSafety.TryResolveContained(Root, relative, out _, out var reason));
            Assert.NotEmpty(reason);
        }

        [Theory]
        [InlineData("CON")]
        [InlineData("nul.txt")]
        [InlineData(@"src\COM1")]
        [InlineData(@"src\LPT9.log")]
        [InlineData("PRN.cs")]
        public void ReservedDeviceNamesAreRejected(string relative)
        {
            Assert.False(PathSafety.TryResolveContained(Root, relative, out _, out var reason));
            Assert.Contains("reserved", reason, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void AlternateDataStreamColonIsRejected()
        {
            Assert.False(PathSafety.TryResolveContained(Root, @"notes.txt:hidden", out _, out var reason));
            Assert.Contains(":", reason, StringComparison.Ordinal);
        }

        [Fact]
        public void TrailingDotOrSpaceIsRejected()
        {
            Assert.False(PathSafety.TryResolveContained(Root, @"evil.bat.", out _, out _));
            Assert.False(PathSafety.TryResolveContained(Root, @"evil.bat ", out _, out _));
        }

        [Fact]
        public void EmptyInputsAreRejected()
        {
            Assert.False(PathSafety.TryResolveContained(Root, "", out _, out _));
            Assert.False(PathSafety.TryResolveContained("", "file.cs", out _, out _));
        }

        [Fact]
        public void SiblingDirectoryWithSharedPrefixIsNotConsideredContained()
        {
            // "C:\target-evil\x" starts with "C:\target" as a STRING but is not inside it.
            Assert.False(PathSafety.TryResolveContained(Root, @"..\target-evil\x.txt", out _, out _));
        }

        /// <summary>
        /// WS2 acceptance criterion 2, end to end: the traversal payload must produce a REJECTED
        /// plan entry — visible in the diff dialog, never silently dropped — and applying the
        /// plan must not create anything outside the chosen folder.
        /// </summary>
        [Fact]
        public async System.Threading.Tasks.Task TraversalHeaderProducesRejectedPlanEntryAndWritesNothingOutside()
        {
            using var temp = new TempDir();
            var targetRoot = Path.Combine(temp.Path, "target");
            var outside = Path.Combine(temp.Path, "outside");
            Directory.CreateDirectory(targetRoot);
            Directory.CreateDirectory(outside);

            // Legacy-format bundle: one innocent file plus the traversal payload.
            var bundle = string.Join("\n", new[]
            {
                @"C:\proj\src\good.cs:",
                "class Good { }",
                "",
                @".\..\..\..\Windows\Temp\evil.bat:",
                "@echo pwned"
            });

            var plan = FileRecreator.Plan(bundle, targetRoot);

            var rejected = plan.Plans.Where(p => p.Status == FilePlanStatus.Rejected).ToList();
            Assert.Single(rejected);
            Assert.Contains("evil.bat", rejected[0].OriginalHeader, StringComparison.OrdinalIgnoreCase);
            Assert.NotEmpty(rejected[0].RejectionReason);
            Assert.False(rejected[0].Include);

            // Applying everything the plan offers must write only inside the target root.
            foreach (var p in plan.Plans) p.Include = true;
            var backups = Path.Combine(temp.Path, "backups");
            await FileRecreator.ExecuteAsync(plan.Plans, targetRoot, backupsRoot: backups);

            Assert.Empty(Directory.GetFileSystemEntries(outside));
            Assert.False(File.Exists(Path.Combine(temp.Path, "evil.bat")));

            // Everything that WAS written stayed underneath the chosen folder.
            var written = Directory.GetFiles(targetRoot, "*", SearchOption.AllDirectories);
            Assert.Contains(written, f => f.EndsWith("good.cs", StringComparison.OrdinalIgnoreCase));
            Assert.All(written, f => Assert.StartsWith(
                targetRoot + Path.DirectorySeparatorChar, Path.GetFullPath(f), StringComparison.OrdinalIgnoreCase));
        }
    }
}
