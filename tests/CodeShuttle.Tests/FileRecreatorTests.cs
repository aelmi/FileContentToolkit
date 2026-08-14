using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CodeShuttle.Tests
{
    public class FileRecreatorTests
    {
        /// <summary>
        /// WS2 acceptance criterion 4 — the regression test for the P0-3 knock-on. Joining with
        /// Environment.NewLine and trimming the trailing newline made "existing != newContent"
        /// almost always true, so essentially every file was reported Modified even when it was
        /// byte-identical. Users saw a wall of spurious diffs, accepted them, and rewrote their
        /// tree mangled.
        /// </summary>
        [Fact]
        public void BundleMatchingDiskContentYieldsUnchangedForEveryEntry()
        {
            using var temp = new TempDir();

            // Deliberately awkward: LF endings, and one file with no trailing newline.
            // Two different subdirectories, so the shared root stops at the temp folder.
            temp.WriteBytes(@"src\a.cs", Encoding.UTF8.GetBytes("class A\n{\n}\n"));
            temp.WriteBytes(@"docs\b.md", Encoding.UTF8.GetBytes("# B\ntext"));

            var bundle = string.Join("\n", new[]
            {
                Path.Combine(temp.Path, @"src\a.cs") + ":",
                "class A", "{", "}",
                "",
                Path.Combine(temp.Path, @"docs\b.md") + ":",
                "# B", "text"
            });

            var plan = FileRecreator.Plan(bundle, temp.Path);

            Assert.Equal(2, plan.Count);
            Assert.All(plan.Plans, p => Assert.Equal(FilePlanStatus.Unchanged, p.Status));
            Assert.All(plan.Plans, p => Assert.False(p.Include));
        }

        [Fact]
        public void ChangedContentIsReportedModified()
        {
            using var temp = new TempDir();
            // A single-entry bundle has no shared prefix to strip, so the header resolves to the
            // bare file name relative to the folder the user chose.
            temp.WriteBytes("a.cs", Encoding.UTF8.GetBytes("class A\n{\n}\n"));

            var bundle = Path.Combine(temp.Path, "a.cs") + ":\nclass A\n{\n    int x;\n}";
            var plan = FileRecreator.Plan(bundle, temp.Path);

            Assert.Single(plan.Plans);
            Assert.Equal(FilePlanStatus.Modified, plan.Plans[0].Status);
            Assert.True(plan.Plans[0].Include);
        }

        [Fact]
        public void MissingFileIsReportedNew()
        {
            using var temp = new TempDir();
            var bundle = Path.Combine(temp.Path, @"src\brand-new.cs") + ":\ncontent";

            var plan = FileRecreator.Plan(bundle, temp.Path);

            Assert.Single(plan.Plans);
            Assert.Equal(FilePlanStatus.New, plan.Plans[0].Status);
            Assert.True(plan.Plans[0].Include);
        }

        /// <summary>
        /// WS2 acceptance criterion 5, and the backstop for P0-2. UNC headers used to collapse to
        /// bare file names, so src\util.cs, test\util.cs and vendor\util.cs all resolved to one
        /// target and two files were silently destroyed. Duplicate targets now block the apply.
        /// </summary>
        [Fact]
        public void DuplicateTargetPathsBlockThePlanRatherThanOverwriting()
        {
            using var temp = new TempDir();

            // Different shares, identical tail — both would land on one file under the target.
            var bundle = BundleFormat.Write(new[]
            {
                new BundleEntry { Path = @"\\server\shareA\src\util.cs", Content = "content A", HasMetadata = true },
                new BundleEntry { Path = @"\\server\shareB\src\util.cs", Content = "content B", HasMetadata = true }
            });

            var plan = FileRecreator.Plan(bundle, temp.Path);

            Assert.False(plan.CanProceed);
            Assert.NotEmpty(plan.Errors);
            Assert.Contains("same file", plan.Errors[0], StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void UncBundleKeepsDistinctSubdirectoriesDistinct()
        {
            using var temp = new TempDir();

            // The UNC prefix must survive as one indivisible root. Splitting it on separators is
            // what used to collapse all three of these onto a single bare "util.cs".
            var bundle = BundleFormat.Write(new[]
            {
                new BundleEntry { Path = @"\\server\share\src\util.cs", Content = "src version", HasMetadata = true },
                new BundleEntry { Path = @"\\server\share\test\util.cs", Content = "test version", HasMetadata = true },
                new BundleEntry { Path = @"\\server\share\vendor\util.cs", Content = "vendor version", HasMetadata = true }
            });

            var plan = FileRecreator.Plan(bundle, temp.Path);

            Assert.True(plan.CanProceed);
            Assert.Equal(3, plan.Count);
            Assert.Equal(
                new[] { @"src\util.cs", @"test\util.cs", @"vendor\util.cs" }.OrderBy(x => x),
                plan.Plans.Select(p => p.RelativePath).OrderBy(x => x));
        }

        /// <summary>P0-3: writing must preserve the target file's encoding and line endings.</summary>
        [Fact]
        public async Task ApplyPreservesExistingEncodingAndLineEndings()
        {
            using var temp = new TempDir();

            var encoding = new UnicodeEncoding(false, true); // UTF-16LE + BOM
            var originalBytes = encoding.GetPreamble()
                .Concat(encoding.GetBytes("old line one\nold line two"))
                .ToArray();
            temp.WriteBytes("config.reg", originalBytes);

            var bundle = Path.Combine(temp.Path, "config.reg") + ":\nnew line one\nnew line two";
            var plan = FileRecreator.Plan(bundle, temp.Path);
            Assert.Equal(FilePlanStatus.Modified, plan.Plans[0].Status);

            await FileRecreator.ExecuteAsync(plan.Plans, temp.Path,
                backupsRoot: Path.Combine(temp.Path, "_backups"));

            var written = File.ReadAllBytes(temp.File("config.reg"));
            var expected = encoding.GetPreamble()
                .Concat(encoding.GetBytes("new line one\nnew line two"))
                .ToArray();

            Assert.Equal(expected, written);
        }

        /// <summary>
        /// WS2 acceptance criterion 15: the pre-write copies and the manifest must exist before
        /// anything is overwritten, so an apply is recoverable.
        /// </summary>
        [Fact]
        public async Task ApplyCreatesABackupSetWithPreWriteCopiesAndAManifest()
        {
            using var temp = new TempDir();
            temp.WriteBytes("a.cs", Encoding.UTF8.GetBytes("ORIGINAL CONTENT\n"));

            var backupsRoot = Path.Combine(temp.Path, "_backups");
            var bundle = Path.Combine(temp.Path, "a.cs") + ":\nREPLACEMENT CONTENT";

            var plan = FileRecreator.Plan(bundle, temp.Path);
            var report = await FileRecreator.ExecuteAsync(plan.Plans, temp.Path, backupsRoot: backupsRoot);

            Assert.Equal(1, report.Written);
            Assert.NotNull(report.BackupDirectory);

            var backupCopy = Path.Combine(report.BackupDirectory!, "a.cs");
            Assert.True(File.Exists(backupCopy), "pre-write copy is missing");
            Assert.Equal("ORIGINAL CONTENT\n", File.ReadAllText(backupCopy));

            Assert.True(File.Exists(Path.Combine(report.BackupDirectory!, "manifest.json")));
            Assert.Contains("REPLACEMENT CONTENT", File.ReadAllText(temp.File("a.cs")));
        }

        [Fact]
        public async Task ExcludedPlansAreNotWritten()
        {
            using var temp = new TempDir();
            var bundle = Path.Combine(temp.Path, "a.cs") + ":\ncontent";

            var plan = FileRecreator.Plan(bundle, temp.Path);
            plan.Plans[0].Include = false;

            var report = await FileRecreator.ExecuteAsync(plan.Plans, temp.Path,
                backupsRoot: Path.Combine(temp.Path, "_backups"));

            Assert.Equal(0, report.Written);
            Assert.False(File.Exists(temp.File("a.cs")));
        }

        /// <summary>
        /// The ship blocker WS2's acceptance criterion 15 missed by testing only the happy path.
        /// When the backup set could not be created — a denied roaming profile, a full %APPDATA%,
        /// an AV lock, all routine on a corporate SOE — the catch set BackupDirectory to null and
        /// execution fell straight through to the write loop. The caller saw Failed == 0 and
        /// toasted "N file(s) written", so forty source files were overwritten irreversibly and
        /// the user was told it succeeded.
        /// </summary>
        [Fact]
        public async Task AFailedBackupSetRefusesTheApplyRatherThanWritingUnprotected()
        {
            using var temp = new TempDir();
            temp.WriteBytes("a.cs", Encoding.UTF8.GetBytes("ORIGINAL CONTENT\n"));

            // A *file* where the backups root must be a directory: CreateDirectory throws, which
            // is the same shape of failure as a denied or full %APPDATA%.
            var backupsRoot = temp.WriteBytes("_backups", Encoding.UTF8.GetBytes("not a directory"));

            var bundle = Path.Combine(temp.Path, "a.cs") + ":\nREPLACEMENT CONTENT";
            var plan = FileRecreator.Plan(bundle, temp.Path);

            var report = await FileRecreator.ExecuteAsync(plan.Plans, temp.Path, backupsRoot: backupsRoot);

            Assert.True(report.BackupFailed, "the backup failure must be reported, not swallowed");
            Assert.False(string.IsNullOrWhiteSpace(report.BackupError), "the reason must be named");
            Assert.Null(report.BackupDirectory);

            // The whole point: nothing ran.
            Assert.Empty(report.Results);
            Assert.Equal(0, report.Written);
            Assert.Equal("ORIGINAL CONTENT\n", File.ReadAllText(temp.File("a.cs")));
        }

        /// <summary>
        /// The other half of the same guarantee: refusing is the default, not the only option.
        /// The user can consent to an unprotected write, and the report still says so, so the
        /// summary cannot present it as an ordinary clean run.
        /// </summary>
        [Fact]
        public async Task AnUnbackedApplyProceedsOnlyWhenExplicitlyAllowed()
        {
            using var temp = new TempDir();
            temp.WriteBytes("a.cs", Encoding.UTF8.GetBytes("ORIGINAL CONTENT\n"));
            var backupsRoot = temp.WriteBytes("_backups", Encoding.UTF8.GetBytes("not a directory"));

            var bundle = Path.Combine(temp.Path, "a.cs") + ":\nREPLACEMENT CONTENT";
            var plan = FileRecreator.Plan(bundle, temp.Path);

            var report = await FileRecreator.ExecuteAsync(plan.Plans, temp.Path,
                backupsRoot: backupsRoot, allowUnbackedWrite: true);

            Assert.Equal(1, report.Written);
            Assert.True(report.BackupFailed);
            Assert.True(report.WroteWithoutBackup, "a caller must be able to tell this was unprotected");
            Assert.Contains("REPLACEMENT CONTENT", File.ReadAllText(temp.File("a.cs")));
        }

        /// <summary>
        /// P1-A. A per-file backup failure used to be swallowed by a bare catch whose comment
        /// claimed the file was "still reported per-file below" — it was not; ApplyReport.Results
        /// carries write outcomes only. A file locked at backup time whose lock released before
        /// the write was destroyed with no copy, inside a run reported as fully backed up.
        /// </summary>
        [Fact]
        public async Task AFileThatCannotBeBackedUpIsRecordedAndNotOverwritten()
        {
            using var temp = new TempDir();
            temp.WriteBytes("locked.cs", Encoding.UTF8.GetBytes("ORIGINAL CONTENT\n"));
            temp.WriteBytes("fine.cs", Encoding.UTF8.GetBytes("OTHER ORIGINAL\n"));

            var bundle = BundleFormat.Write(new[]
            {
                new BundleEntry { Path = temp.File("locked.cs"), Content = "REPLACEMENT A", HasMetadata = true },
                new BundleEntry { Path = temp.File("fine.cs"), Content = "REPLACEMENT B", HasMetadata = true }
            });

            // Planned before the lock is taken, because planning reads the existing file too.
            var plan = FileRecreator.Plan(bundle, temp.Path);
            Assert.Equal(2, plan.Count);

            ApplyReport report;
            // FileShare.None makes File.Copy fail with a sharing violation — the same failure an
            // AV scanner or an open editor produces, and the one whose lock can release again
            // before the write reaches the file.
            using (new FileStream(temp.File("locked.cs"), FileMode.Open, FileAccess.Read, FileShare.None))
            {
                report = await FileRecreator.ExecuteAsync(plan.Plans, temp.Path,
                    backupsRoot: Path.Combine(temp.Path, "_backups"));
            }

            Assert.False(report.BackupFailed, "the backup set itself was fine");

            var recorded = Assert.Single(report.BackupFailures);
            Assert.Equal(temp.File("locked.cs"), recorded.TargetPath, ignoreCase: true);

            // Skipped, not written and not merely "failed": we declined to destroy a file we
            // could not copy aside first.
            var skipped = Assert.Single(report.Results, r => r.Outcome == ApplyOutcome.Skipped);
            Assert.Equal(temp.File("locked.cs"), skipped.TargetPath, ignoreCase: true);
            Assert.Equal("ORIGINAL CONTENT\n", File.ReadAllText(temp.File("locked.cs")));

            // The file that could be backed up is written as normal.
            Assert.Equal(1, report.Written);
            Assert.Contains("REPLACEMENT B", File.ReadAllText(temp.File("fine.cs")));
        }

        [Fact]
        public void EmptyInputProducesAnEmptyPlanRatherThanThrowing()
        {
            using var temp = new TempDir();

            Assert.Equal(0, FileRecreator.Plan("", temp.Path).Count);
            Assert.Equal(0, FileRecreator.Plan("just some prose with no headers", temp.Path).Count);
        }
    }
}
