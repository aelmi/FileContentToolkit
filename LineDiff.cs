using System;
using System.Collections.Generic;

namespace CodeShuttle.Diff
{
    public enum DiffLineKind
    {
        Context,
        Add,
        Remove,

        /// <summary>An informational line emitted instead of a diff that was too large to compute.</summary>
        Summary
    }

    public readonly struct DiffLine
    {
        public DiffLineKind Kind { get; }
        public string Text { get; }
        public DiffLine(DiffLineKind kind, string text) { Kind = kind; Text = text; }
    }

    /// <summary>
    /// Line-by-line diff backed by LCS.
    ///
    /// The LCS matrix is O(n·m) in memory — two 20,000-line files would be a single 1.6 GB
    /// allocation — so identical leading and trailing lines are stripped first, and anything
    /// still over <see cref="MaxMatrixCells"/> falls back to a wholesale-replacement summary.
    /// That trades diff detail for never taking the application down, which is the correct
    /// trade for a viewer.
    /// </summary>
    public static class LineDiff
    {
        /// <summary>Largest LCS matrix we will allocate (~16 MB of int).</summary>
        public const long MaxMatrixCells = 4_000_000;

        public static List<DiffLine> Compute(string oldText, string newText)
            => ComputeFromLines(SplitLines(oldText), SplitLines(newText));

        /// <summary>True when the inputs would fall back to the summary rather than a real diff.</summary>
        public static bool ExceedsBudget(string oldText, string newText)
        {
            var a = SplitLines(oldText);
            var b = SplitLines(newText);
            var (start, endA, endB) = TrimCommon(a, b);
            long n = endA - start + 1;
            long m = endB - start + 1;
            if (n < 0) n = 0;
            if (m < 0) m = 0;
            return n * m > MaxMatrixCells;
        }

        private static string[] SplitLines(string text)
        {
            if (string.IsNullOrEmpty(text)) return Array.Empty<string>();
            return text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        }

        /// <summary>
        /// Returns the index of the first differing line and the last differing index in each
        /// input. When the inputs are identical, endA/endB fall below start.
        /// </summary>
        private static (int Start, int EndA, int EndB) TrimCommon(string[] a, string[] b)
        {
            int start = 0;
            int maxStart = Math.Min(a.Length, b.Length);
            while (start < maxStart && string.Equals(a[start], b[start], StringComparison.Ordinal))
                start++;

            int endA = a.Length - 1;
            int endB = b.Length - 1;
            while (endA >= start && endB >= start && string.Equals(a[endA], b[endB], StringComparison.Ordinal))
            {
                endA--;
                endB--;
            }

            return (start, endA, endB);
        }

        private static List<DiffLine> ComputeFromLines(string[] a, string[] b)
        {
            var result = new List<DiffLine>();
            var (start, endA, endB) = TrimCommon(a, b);

            int coreA = endA - start + 1;
            int coreB = endB - start + 1;
            if (coreA < 0) coreA = 0;
            if (coreB < 0) coreB = 0;

            if ((long)coreA * coreB > MaxMatrixCells)
            {
                result.Add(new DiffLine(DiffLineKind.Summary,
                    $"This file is too large to diff line by line ({a.Length:N0} lines on disk, {b.Length:N0} incoming)."));
                result.Add(new DiffLine(DiffLineKind.Summary,
                    $"It will be replaced wholesale — {coreA:N0} line(s) removed, {coreB:N0} line(s) added."));
                if (start > 0)
                    result.Add(new DiffLine(DiffLineKind.Summary,
                        $"The first {start:N0} line(s) are identical."));
                return result;
            }

            for (int i = 0; i < start; i++)
                result.Add(new DiffLine(DiffLineKind.Context, a[i]));

            AppendLcsDiff(a, b, start, endA, endB, result);

            for (int i = endA + 1; i < a.Length; i++)
                result.Add(new DiffLine(DiffLineKind.Context, a[i]));

            return result;
        }

        private static void AppendLcsDiff(string[] a, string[] b, int start, int endA, int endB, List<DiffLine> result)
        {
            int n = endA - start + 1;
            int m = endB - start + 1;
            if (n <= 0 && m <= 0) return;

            if (n <= 0)
            {
                for (int j = 0; j < m; j++) result.Add(new DiffLine(DiffLineKind.Add, b[start + j]));
                return;
            }
            if (m <= 0)
            {
                for (int i = 0; i < n; i++) result.Add(new DiffLine(DiffLineKind.Remove, a[start + i]));
                return;
            }

            // dp[i, j] = length of the LCS of a[start..start+i-1] and b[start..start+j-1]
            var dp = new int[n + 1, m + 1];
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    dp[i, j] = string.Equals(a[start + i - 1], b[start + j - 1], StringComparison.Ordinal)
                        ? dp[i - 1, j - 1] + 1
                        : Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }

            var stack = new Stack<DiffLine>();
            int ii = n, jj = m;
            while (ii > 0 && jj > 0)
            {
                if (string.Equals(a[start + ii - 1], b[start + jj - 1], StringComparison.Ordinal))
                {
                    stack.Push(new DiffLine(DiffLineKind.Context, a[start + ii - 1]));
                    ii--; jj--;
                }
                else if (dp[ii - 1, jj] >= dp[ii, jj - 1])
                {
                    stack.Push(new DiffLine(DiffLineKind.Remove, a[start + ii - 1]));
                    ii--;
                }
                else
                {
                    stack.Push(new DiffLine(DiffLineKind.Add, b[start + jj - 1]));
                    jj--;
                }
            }
            while (ii > 0) { stack.Push(new DiffLine(DiffLineKind.Remove, a[start + ii - 1])); ii--; }
            while (jj > 0) { stack.Push(new DiffLine(DiffLineKind.Add, b[start + jj - 1])); jj--; }

            while (stack.Count > 0) result.Add(stack.Pop());
        }
    }
}
