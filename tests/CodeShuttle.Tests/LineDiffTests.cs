using System;
using System.Diagnostics;
using System.Linq;
using CodeShuttle.Diff;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// The most recent commit before this work was "Fix InvalidOperationException when opening
    /// Recreate Files diff viewer", so this area has a demonstrated history of breaking.
    /// </summary>
    public class LineDiffTests
    {
        [Fact]
        public void IdenticalInputsProduceOnlyContext()
        {
            var diff = LineDiff.Compute("a\nb\nc", "a\nb\nc");

            Assert.All(diff, d => Assert.Equal(DiffLineKind.Context, d.Kind));
            Assert.Equal(3, diff.Count);
        }

        [Fact]
        public void PureInsertIsAllAdds()
        {
            var diff = LineDiff.Compute("a\nc", "a\nb\nc");

            Assert.Equal("b", Assert.Single(diff, d => d.Kind == DiffLineKind.Add).Text);
            Assert.DoesNotContain(diff, d => d.Kind == DiffLineKind.Remove);
        }

        [Fact]
        public void PureDeleteIsAllRemoves()
        {
            var diff = LineDiff.Compute("a\nb\nc", "a\nc");

            Assert.Equal("b", Assert.Single(diff, d => d.Kind == DiffLineKind.Remove).Text);
            Assert.DoesNotContain(diff, d => d.Kind == DiffLineKind.Add);
        }

        [Fact]
        public void EmptyToNonEmptyIsAllAdds()
        {
            var diff = LineDiff.Compute("", "a\nb");

            Assert.Equal(2, diff.Count(d => d.Kind == DiffLineKind.Add));
            Assert.DoesNotContain(diff, d => d.Kind == DiffLineKind.Remove);
        }

        [Fact]
        public void NonEmptyToEmptyIsAllRemoves()
        {
            var diff = LineDiff.Compute("a\nb", "");

            Assert.Equal(2, diff.Count(d => d.Kind == DiffLineKind.Remove));
            Assert.DoesNotContain(diff, d => d.Kind == DiffLineKind.Add);
        }

        [Fact]
        public void BothEmptyProducesNoDiff()
        {
            Assert.Empty(LineDiff.Compute("", ""));
        }

        [Fact]
        public void TrailingNewlineDifferenceIsVisible()
        {
            var diff = LineDiff.Compute("a\nb", "a\nb\n");

            // The trailing newline creates an extra (empty) final line.
            Assert.Contains(diff, d => d.Kind == DiffLineKind.Add);
        }

        [Fact]
        public void CrlfAndLfInputsAreComparedEquivalently()
        {
            var diff = LineDiff.Compute("a\r\nb\r\nc", "a\nb\nc");

            Assert.All(diff, d => Assert.Equal(DiffLineKind.Context, d.Kind));
        }

        /// <summary>
        /// WS2 acceptance criterion 9, and the P1-6 crash fix. Two 20,000-line files would be a
        /// 400M-cell matrix — a single 1.6 GB allocation thrown from a synchronous UI event with
        /// no try/catch. The guard must take the summary path, fast.
        /// </summary>
        [Fact]
        public void TwentyThousandLineInputsTakeTheSummaryPathQuicklyWithoutAllocatingTheMatrix()
        {
            var oldText = string.Join("\n", Enumerable.Range(0, 20000).Select(i => $"old line {i}"));
            var newText = string.Join("\n", Enumerable.Range(0, 20000).Select(i => $"new line {i}"));

            Assert.True(LineDiff.ExceedsBudget(oldText, newText));

            var sw = Stopwatch.StartNew();
            var diff = LineDiff.Compute(oldText, newText);
            sw.Stop();

            Assert.True(sw.Elapsed < TimeSpan.FromSeconds(2), $"took {sw.Elapsed}");
            Assert.NotEmpty(diff);
            Assert.All(diff, d => Assert.Equal(DiffLineKind.Summary, d.Kind));
            Assert.Contains(diff, d => d.Text.Contains("replaced wholesale", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Prefix/suffix stripping means two large but nearly-identical files still get a real
        /// line-by-line diff rather than the summary.
        /// </summary>
        [Fact]
        public void LargeButNearlyIdenticalFilesStillGetARealDiff()
        {
            var lines = Enumerable.Range(0, 20000).Select(i => $"line {i}").ToArray();
            var oldText = string.Join("\n", lines);
            lines[10000] = "line 10000 CHANGED";
            var newText = string.Join("\n", lines);

            Assert.False(LineDiff.ExceedsBudget(oldText, newText));

            var diff = LineDiff.Compute(oldText, newText);

            Assert.DoesNotContain(diff, d => d.Kind == DiffLineKind.Summary);
            Assert.Single(diff, d => d.Kind == DiffLineKind.Add);
            Assert.Single(diff, d => d.Kind == DiffLineKind.Remove);
        }
    }
}
