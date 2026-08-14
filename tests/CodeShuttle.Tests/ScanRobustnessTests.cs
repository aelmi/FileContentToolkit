using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CodeShuttle.Diagnostics;
using Xunit;

namespace CodeShuttle.Tests
{
    public class ScanRobustnessTests
    {
        private static FileContentService ServiceFor(TempDir temp, params string[] extensions)
        {
            var service = new FileContentService { SkipBinaryFiles = true, UseGitIgnoreFiles = false };
            service.SetFolderPath(temp.Path);
            service.SetExtensions(extensions);
            return service;
        }

        /// <summary>
        /// P1-5: the pattern "bin" used to exclude Robinson.cs and Binder.cs because matching was
        /// a raw substring test.
        /// </summary>
        [Fact]
        public void UserIgnorePatternsAreGlobsNotSubstrings()
        {
            Assert.False(FileContentService.IsIgnoredByUserPatterns(
                @"C:\proj\src\Robinson.cs", @"C:\proj", new[] { "bin" }));
            Assert.False(FileContentService.IsIgnoredByUserPatterns(
                @"C:\proj\src\Binder.cs", @"C:\proj", new[] { "bin" }));
            Assert.True(FileContentService.IsIgnoredByUserPatterns(
                @"C:\proj\bin\app.dll", @"C:\proj", new[] { "bin" }));
        }

        /// <summary>
        /// P1-5: the documented "dir/" form never matched anything, because GetRelativePath
        /// returns backslashes on Windows and the check compared against "bin/".
        /// </summary>
        [Fact]
        public void DirectorySlashPatternActuallyMatchesOnWindows()
        {
            Assert.True(FileContentService.IsIgnoredByUserPatterns(
                @"C:\proj\bin\Debug\app.dll", @"C:\proj", new[] { "bin/" }));
        }

        [Fact]
        public void ExtensionPatternsStillWork()
        {
            Assert.True(FileContentService.IsIgnoredByUserPatterns(
                @"C:\proj\a.tmp", @"C:\proj", new[] { "*.tmp" }));
            Assert.False(FileContentService.IsIgnoredByUserPatterns(
                @"C:\proj\a.cs", @"C:\proj", new[] { "*.tmp" }));
        }

        [Fact]
        public async Task ScanReturnsResultsRatherThanMutatingInPlace()
        {
            using var temp = new TempDir();
            temp.WriteText("a.cs", "class A { }");
            temp.WriteText("b.cs", "class B { }");

            var service = ServiceFor(temp, ".cs");
            var result = await service.ScanAsync(null, CancellationToken.None);

            // The scan does not touch SelectedFiles until the caller adopts the result, so a
            // superseded scan cannot clobber a newer one's list.
            Assert.Empty(service.SelectedFiles);
            Assert.Equal(2, result.Files.Count);

            service.ApplyScanResult(result);
            Assert.Equal(2, service.SelectedFiles.Count);
        }

        /// <summary>P1-17: sizes come from the enumeration, not from re-stat'ing on the UI thread.</summary>
        [Fact]
        public async Task ScanCapturesTotalSizeWithoutReStatting()
        {
            using var temp = new TempDir();
            temp.WriteBytes("a.cs", new byte[100]);
            temp.WriteBytes("b.cs", new byte[50]);

            // All-zero bytes would be classified binary, so keep real text content.
            temp.WriteText("a.cs", new string('x', 100));
            temp.WriteText("b.cs", new string('y', 50));

            var service = ServiceFor(temp, ".cs");
            var result = await service.ScanAsync(null, CancellationToken.None);
            service.ApplyScanResult(result);

            Assert.Equal(150, service.TotalSelectedBytes);
        }

        /// <summary>P1-19: excluded files are recorded with a reason instead of vanishing.</summary>
        [Fact]
        public async Task SkippedFilesAreReportedWithAReason()
        {
            using var temp = new TempDir();
            temp.WriteText("keep.cs", "class Keep { }");
            temp.WriteBytes("binary.cs", new byte[] { 0x41, 0x00, 0x42, 0x00 });
            temp.WriteText("huge.cs", new string('x', 5000));

            var service = ServiceFor(temp, ".cs");
            service.MaxFileSizeBytes = 1000;

            var result = await service.ScanAsync(null, CancellationToken.None);

            Assert.Contains("keep.cs", result.Files.Single(), StringComparison.OrdinalIgnoreCase);
            Assert.Contains(result.Skipped, s => s.Reason == SkipReason.Binary);
            Assert.Contains(result.Skipped, s => s.Reason == SkipReason.TooLarge);
        }

        [Fact]
        public async Task GitIgnoredFilesAreRecordedAsIgnoredByRule()
        {
            using var temp = new TempDir();
            temp.WriteText(".gitignore", "generated.cs\n");
            temp.WriteText("keep.cs", "class Keep { }");
            temp.WriteText("generated.cs", "class Generated { }");

            CodeShuttle.Filters.GitIgnoreParser.ClearCache();
            var service = ServiceFor(temp, ".cs");
            service.UseGitIgnoreFiles = true;

            var result = await service.ScanAsync(null, CancellationToken.None);

            Assert.Single(result.Files);
            Assert.Contains(result.Skipped, s => s.Reason == SkipReason.IgnoredByRule);
        }

        /// <summary>P2-8: three spellings of one path used to count as three separate files.</summary>
        [Fact]
        public void AddFilesDedupesOnTheCanonicalPath()
        {
            using var temp = new TempDir();
            var path = temp.WriteText("a.cs", "class A { }");

            var service = new FileContentService();
            service.AddFiles(new[] { path, path.Replace('\\', '/'), path.ToUpperInvariant() });

            Assert.Single(service.SelectedFiles);
        }

        [Fact]
        public void AddFilesIgnoresDirectories()
        {
            using var temp = new TempDir();
            Directory.CreateDirectory(temp.File("subdir"));

            var service = new FileContentService();
            service.AddFiles(new[] { temp.File("subdir") });

            Assert.Empty(service.SelectedFiles);
        }

        [Fact]
        public async Task InaccessibleSubtreesDoNotAbortTheWholeEnumeration()
        {
            using var temp = new TempDir();
            temp.WriteText(@"sub\a.cs", "class A { }");
            temp.WriteText("b.cs", "class B { }");

            var service = ServiceFor(temp, ".cs");
            var result = await service.ScanAsync(null, CancellationToken.None);

            Assert.Equal(2, result.Files.Count);
        }

        /// <summary>
        /// The regex DoS guard: "(a+)+$" against a non-matching string used to hang the UI
        /// permanently because the pattern carried no timeout.
        /// </summary>
        [Fact]
        public void SearchRegexesCarryAMatchTimeout()
        {
            var literal = MainForm.BuildSearchRegex("plain text", isRegex: false, matchCase: false, wholeWord: false);
            var pattern = MainForm.BuildSearchRegex("(a+)+$", isRegex: true, matchCase: true, wholeWord: false);

            Assert.NotEqual(Regex.InfiniteMatchTimeout, literal.MatchTimeout);
            Assert.NotEqual(Regex.InfiniteMatchTimeout, pattern.MatchTimeout);
            Assert.Equal(MainForm.RegexMatchTimeout, pattern.MatchTimeout);
        }

        [Fact]
        public void FindReplaceRegexesCarryAMatchTimeoutToo()
        {
            var rx = CodeShuttle.Dialogs.FindReplaceForm.BuildRegex("(a+)+$", true, true, false);
            Assert.Equal(CodeShuttle.Dialogs.FindReplaceForm.MatchTimeout, rx.MatchTimeout);
        }

        /// <summary>P2-1: the release URL is remote data handed to a shell execute.</summary>
        [Theory]
        [InlineData("https://github.com/owner/repo/releases/tag/v1.0.0", true)]
        [InlineData("https://api.github.com/repos/owner/repo", true)]
        [InlineData("http://github.com/owner/repo", false)]
        [InlineData("file:///C:/Windows/System32/calc.exe", false)]
        [InlineData(@"\\attacker\share\evil.exe", false)]
        [InlineData("https://github.com.evil.test/owner/repo", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void OnlyHttpsGithubReleaseUrlsAreShellExecuted(string? url, bool expected)
        {
            Assert.Equal(expected, MainForm.IsTrustedReleaseUrl(url));
        }

        /// <summary>
        /// The crash log is plaintext a user may email to support, so it must not carry the
        /// structure of their private source tree.
        /// </summary>
        [Fact]
        public void CrashLogRedactsPathsBelowTheScanRoot()
        {
            var previous = CrashLogger.ScanRoot;
            try
            {
                CrashLogger.ScanRoot = @"C:\clients\acme\secret-project";
                var redacted = CrashLogger.Redact(
                    @"Could not open C:\clients\acme\secret-project\src\Billing.cs");

                Assert.DoesNotContain("secret-project", redacted, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("<scan-root>", redacted, StringComparison.Ordinal);
            }
            finally { CrashLogger.ScanRoot = previous; }
        }

        [Fact]
        public void LineCountingIsCorrectAtTheEdges()
        {
            Assert.Equal(0, MainForm.CountLines(""));
            Assert.Equal(1, MainForm.CountLines("a"));
            Assert.Equal(1, MainForm.CountLines("a\n"));
            Assert.Equal(2, MainForm.CountLines("a\nb"));
            Assert.Equal(2, MainForm.CountLines("a\nb\n"));
        }

        /// <summary>
        /// The estimate exists to answer "will this fit". Over-estimating is safe; the old 4.0
        /// ratio under-estimated code, which is the failure the user actually feels.
        /// </summary>
        [Fact]
        public void TokenEstimateIsPessimisticForCode()
        {
            Assert.True(TokenEstimator.CharsPerToken <= 3.5);
            Assert.Equal(0, TokenEstimator.Estimate(""));
            Assert.True(TokenEstimator.Estimate(new string('x', 3300)) >= 1000);
        }
    }
}
