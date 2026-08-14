using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeShuttle.Theming;
using CodeShuttle.UI;

namespace CodeShuttle.Dialogs
{
    public partial class FindReplaceForm : ThemedForm
    {
        private readonly RichTextBox _target;
        private readonly Action<string>? _onSearchUsed;
        private readonly System.Windows.Forms.Timer _matchDebounce = new() { Interval = 250 };

        /// <summary>
        /// A user-supplied pattern is untrusted: "(a+)+$" backtracks catastrophically and used to
        /// hang the UI permanently, because the regex had no timeout at all.
        /// </summary>
        internal static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(2);

        public FindReplaceForm(RichTextBox target,
                               IEnumerable<string>? recentSearches = null,
                               bool initialRegex = false,
                               bool initialCase = false,
                               bool initialWord = false,
                               Action<string>? onSearchUsed = null)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _onSearchUsed = onSearchUsed;

            InitializeComponent();


            if (recentSearches != null)
                foreach (var r in recentSearches) cmbFind.Items.Add(r);

            chkRegex.Checked = initialRegex;
            chkCase.Checked = initialCase;
            chkWord.Checked = initialWord;

            _matchDebounce.Tick += (s, e) =>
            {
                _matchDebounce.Stop();
                UpdateMatchCount();
            };
            Disposed += (s, e) => _matchDebounce.Dispose();
        }

        public void SetInitialQuery(string text)
        {
            if (!string.IsNullOrEmpty(text)) cmbFind.Text = text;
            UpdateMatchCount();
            cmbFind.Focus();
            cmbFind.SelectAll();
        }

        // -------------------- designer-wired handlers --------------------

        private void CmbFind_TextChanged(object? sender, EventArgs e) => DebounceMatchCount();
        private void ChkCase_CheckedChanged(object? sender, EventArgs e) => DebounceMatchCount();
        private void ChkWord_CheckedChanged(object? sender, EventArgs e) => DebounceMatchCount();
        private void ChkRegex_CheckedChanged(object? sender, EventArgs e) => DebounceMatchCount();

        /// <summary>
        /// The live match count used to run over the whole output on every keystroke, so a
        /// half-typed regex was evaluated repeatedly against megabytes of text.
        /// </summary>
        private void DebounceMatchCount()
        {
            _matchDebounce.Stop();
            _matchDebounce.Start();
        }

        private void BtnNext_Click(object? sender, EventArgs e) => FindOne(forward: true);
        private void BtnPrev_Click(object? sender, EventArgs e) => FindOne(forward: false);
        private void BtnReplace_Click(object? sender, EventArgs e) => ReplaceCurrent();
        private void BtnReplaceAll_Click(object? sender, EventArgs e) => ReplaceAll();

        private void FindReplaceForm_KeyDown(object? sender, KeyEventArgs e)
        {
            // The Escape branch that used to live here never set e.Handled, unlike the F3 branch
            // immediately below it. Escape is now the form's CancelButton, handled by the
            // framework.
            if (e.KeyCode == Keys.F3)
            {
                FindOne(forward: !e.Shift);
                e.Handled = true;
            }
        }

        // -------------------- search logic --------------------

        private void UpdateMatchCount()
        {
            var (count, _) = ComputeMatches();
            lblStatus.Text = string.IsNullOrEmpty(cmbFind.Text)
                ? ""
                : $"{count} match{(count == 1 ? "" : "es")}";
        }

        private (int Count, List<(int Index, int Length)> Matches) ComputeMatches()
        {
            var hits = new List<(int, int)>();
            var pattern = cmbFind.Text;
            if (string.IsNullOrEmpty(pattern)) return (0, hits);
            var text = _target.Text ?? string.Empty;
            try
            {
                var regex = BuildRegex(pattern, chkRegex.Checked, chkCase.Checked, chkWord.Checked);
                foreach (Match m in regex.Matches(text))
                    hits.Add((m.Index, m.Length));
            }
            catch (ArgumentException) { }
            catch (RegexMatchTimeoutException) { }
            return (hits.Count, hits);
        }

        internal static Regex BuildRegex(string pattern, bool isRegex, bool matchCase, bool wholeWord)
        {
            var options = RegexOptions.Multiline;
            if (!matchCase) options |= RegexOptions.IgnoreCase;
            var rx = isRegex ? pattern : Regex.Escape(pattern);
            if (wholeWord) rx = $@"\b(?:{rx})\b";

            if (!isRegex)
            {
                // An escaped literal cannot backtrack, so the non-backtracking engine is safe
                // here and removes the failure mode entirely.
                try { return new Regex(rx, options | RegexOptions.NonBacktracking, MatchTimeout); }
                catch (NotSupportedException) { /* fall through */ }
            }

            return new Regex(rx, options, MatchTimeout);
        }

        private void FindOne(bool forward)
        {
            var pattern = cmbFind.Text;
            if (string.IsNullOrEmpty(pattern)) return;

            var (count, matches) = ComputeMatches();
            if (count == 0) { lblStatus.Text = "No matches"; return; }

            int caret = _target.SelectionStart + (forward ? Math.Max(_target.SelectionLength, 0) : 0);
            int target;
            if (forward)
            {
                target = matches.FindIndex(m => m.Index >= caret);
                if (target < 0) target = 0; // wrap
            }
            else
            {
                target = matches.FindLastIndex(m => m.Index < _target.SelectionStart);
                if (target < 0) target = matches.Count - 1; // wrap
            }

            var (idx, len) = matches[target];
            _target.Select(idx, len);
            _target.ScrollToCaret();
            _target.Focus();
            lblStatus.Text = $"Match {target + 1} of {count}";

            _onSearchUsed?.Invoke(pattern);
            if (!cmbFind.Items.Contains(pattern)) cmbFind.Items.Insert(0, pattern);
        }

        private void ReplaceCurrent()
        {
            if (_target.ReadOnly)
            {
                lblStatus.Text = "Output is read-only — toggle Edit first.";
                return;
            }
            var pattern = cmbFind.Text;
            if (string.IsNullOrEmpty(pattern)) return;

            var sel = _target.SelectedText;
            if (!string.IsNullOrEmpty(sel))
            {
                try
                {
                    var regex = BuildRegex(pattern, chkRegex.Checked, chkCase.Checked, chkWord.Checked);
                    if (regex.IsMatch(sel))
                    {
                        var replacement = cmbReplace.Text ?? "";
                        var replaced = chkRegex.Checked ? regex.Replace(sel, replacement) : replacement;
                        _target.SelectedText = replaced;
                    }
                }
                catch (ArgumentException ex)
                {
                    lblStatus.Text = "Regex error: " + ex.Message;
                    return;
                }
                catch (RegexMatchTimeoutException)
                {
                    lblStatus.Text = "That pattern took too long to evaluate and was abandoned.";
                    return;
                }
            }
            FindOne(forward: true);
            _onSearchUsed?.Invoke(pattern);
        }

        private void ReplaceAll()
        {
            if (_target.ReadOnly)
            {
                lblStatus.Text = "Output is read-only — toggle Edit first.";
                return;
            }
            var pattern = cmbFind.Text;
            if (string.IsNullOrEmpty(pattern)) return;
            try
            {
                var regex = BuildRegex(pattern, chkRegex.Checked, chkCase.Checked, chkWord.Checked);
                var replacement = cmbReplace.Text ?? "";
                var text = _target.Text ?? "";
                var newText = chkRegex.Checked
                    ? regex.Replace(text, replacement)
                    : regex.Replace(text, _ => replacement);
                int count = regex.Count(text);
                _target.Text = newText;
                lblStatus.Text = $"Replaced {count} occurrence{(count == 1 ? "" : "s")}";
                _onSearchUsed?.Invoke(pattern);
            }
            catch (ArgumentException ex)
            {
                lblStatus.Text = "Regex error: " + ex.Message;
            }
            catch (RegexMatchTimeoutException)
            {
                lblStatus.Text = "That pattern took too long to evaluate and was abandoned.";
            }
        }
    }
}
