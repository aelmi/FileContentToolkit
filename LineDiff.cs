using System;
using System.Collections.Generic;

namespace FileContentToolkit.Diff
{
    public enum DiffLineKind { Context, Add, Remove }

    public readonly struct DiffLine
    {
        public DiffLineKind Kind { get; }
        public string Text { get; }
        public DiffLine(DiffLineKind kind, string text) { Kind = kind; Text = text; }
    }

    /// <summary>
    /// Plain line-by-line diff backed by LCS (longest common subsequence).
    /// Good enough for config/code files up to a few thousand lines; complexity is O(n*m).
    /// </summary>
    public static class LineDiff
    {
        public static List<DiffLine> Compute(string oldText, string newText)
        {
            var oldLines = SplitLines(oldText);
            var newLines = SplitLines(newText);
            return ComputeFromLines(oldLines, newLines);
        }

        private static string[] SplitLines(string text)
        {
            if (string.IsNullOrEmpty(text)) return Array.Empty<string>();
            return text.Replace("\r\n", "\n").Split('\n');
        }

        private static List<DiffLine> ComputeFromLines(string[] a, string[] b)
        {
            int n = a.Length, m = b.Length;
            // dp[i, j] = length of LCS of a[0..i-1] and b[0..j-1]
            var dp = new int[n + 1, m + 1];
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    dp[i, j] = a[i - 1] == b[j - 1]
                        ? dp[i - 1, j - 1] + 1
                        : Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }

            // Walk back to emit the diff in order.
            var result = new List<DiffLine>();
            int ii = n, jj = m;
            var stack = new Stack<DiffLine>();
            while (ii > 0 && jj > 0)
            {
                if (a[ii - 1] == b[jj - 1])
                {
                    stack.Push(new DiffLine(DiffLineKind.Context, a[ii - 1]));
                    ii--; jj--;
                }
                else if (dp[ii - 1, jj] >= dp[ii, jj - 1])
                {
                    stack.Push(new DiffLine(DiffLineKind.Remove, a[ii - 1]));
                    ii--;
                }
                else
                {
                    stack.Push(new DiffLine(DiffLineKind.Add, b[jj - 1]));
                    jj--;
                }
            }
            while (ii > 0)
            {
                stack.Push(new DiffLine(DiffLineKind.Remove, a[ii - 1]));
                ii--;
            }
            while (jj > 0)
            {
                stack.Push(new DiffLine(DiffLineKind.Add, b[jj - 1]));
                jj--;
            }
            while (stack.Count > 0) result.Add(stack.Pop());
            return result;
        }
    }
}
