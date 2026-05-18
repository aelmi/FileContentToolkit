using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FileContentToolkit.UI;

namespace FileContentToolkit.Dialogs
{
    public class FindReplaceForm : Form
    {
        private readonly RichTextBox _target;
        private readonly Action<string>? _onSearchUsed;

        private readonly ComboBox _cmbFind;
        private readonly ComboBox _cmbReplace;
        private readonly CheckBox _chkCase;
        private readonly CheckBox _chkWord;
        private readonly CheckBox _chkRegex;
        private readonly Button _btnNext;
        private readonly Button _btnPrev;
        private readonly Button _btnReplace;
        private readonly Button _btnReplaceAll;
        private readonly Label _lblStatus;

        public FindReplaceForm(RichTextBox target,
                               IEnumerable<string>? recentSearches = null,
                               bool initialRegex = false,
                               bool initialCase = false,
                               bool initialWord = false,
                               Action<string>? onSearchUsed = null)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _onSearchUsed = onSearchUsed;

            Text = "Find & Replace";
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(560, 290);
            MinimumSize = new Size(480, 280);
            KeyPreview = true;
            Theme.ApplyForm(this);

            Controls.Add(Theme.BuildHeader("Find & Replace", "Search the output pane."));

            var body = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.White,
                Padding = new Padding(16, 16, 16, 16)
            };

            var lblFind = new Label { Text = "Find:", Left = 0, Top = 8, Width = 70, ForeColor = Theme.BodyText, AutoSize = true };
            _cmbFind = new ComboBox
            {
                Left = 80,
                Top = 4,
                Width = 440,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = Theme.BodyFont,
                FlatStyle = FlatStyle.Flat
            };

            var lblReplace = new Label { Text = "Replace:", Left = 0, Top = 42, Width = 70, ForeColor = Theme.BodyText, AutoSize = true };
            _cmbReplace = new ComboBox
            {
                Left = 80,
                Top = 38,
                Width = 440,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = Theme.BodyFont,
                FlatStyle = FlatStyle.Flat
            };

            if (recentSearches != null)
                foreach (var r in recentSearches) _cmbFind.Items.Add(r);

            _chkCase = new CheckBox { Text = "Match case", Left = 80, Top = 74, Width = 110, Checked = initialCase, ForeColor = Theme.BodyText, Font = Theme.BodyFont };
            _chkWord = new CheckBox { Text = "Whole word", Left = 200, Top = 74, Width = 110, Checked = initialWord, ForeColor = Theme.BodyText, Font = Theme.BodyFont };
            _chkRegex = new CheckBox { Text = "Regex", Left = 320, Top = 74, Width = 90, Checked = initialRegex, ForeColor = Theme.BodyText, Font = Theme.BodyFont };

            _btnNext = Theme.PrimaryButton("Find Next");
            _btnNext.Size = new Size(105, 34); _btnNext.Left = 80; _btnNext.Top = 108;

            _btnPrev = Theme.SecondaryButton("Find Prev");
            _btnPrev.Size = new Size(105, 34); _btnPrev.Left = _btnNext.Right + 6; _btnPrev.Top = 108;

            _btnReplace = Theme.ActionButton("Replace");
            _btnReplace.Size = new Size(105, 34); _btnReplace.Left = _btnPrev.Right + 6; _btnReplace.Top = 108;

            _btnReplaceAll = Theme.SuccessButton("Replace All");
            _btnReplaceAll.Size = new Size(115, 34);
            _btnReplaceAll.Left = _btnReplace.Right + 6; _btnReplaceAll.Top = 108;
            _btnReplaceAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            _lblStatus = new Label
            {
                Left = 0,
                Top = 156,
                Width = 540,
                Height = 24,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Theme.SubtleText,
                Font = Theme.BodyFont
            };

            body.Controls.AddRange(new Control[] {
                lblFind, _cmbFind, lblReplace, _cmbReplace,
                _chkCase, _chkWord, _chkRegex,
                _btnNext, _btnPrev, _btnReplace, _btnReplaceAll, _lblStatus
            });

            Controls.Add(body);
            body.BringToFront();

            AcceptButton = _btnNext;
            _btnNext.Click += (s, e) => FindOne(forward: true);
            _btnPrev.Click += (s, e) => FindOne(forward: false);
            _btnReplace.Click += (s, e) => ReplaceCurrent();
            _btnReplaceAll.Click += (s, e) => ReplaceAll();

            _cmbFind.TextChanged += (s, e) => UpdateMatchCount();
            _chkCase.CheckedChanged += (s, e) => UpdateMatchCount();
            _chkWord.CheckedChanged += (s, e) => UpdateMatchCount();
            _chkRegex.CheckedChanged += (s, e) => UpdateMatchCount();

            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) Close();
                else if (e.KeyCode == Keys.F3) { FindOne(forward: !e.Shift); e.Handled = true; }
            };
        }

        public void SetInitialQuery(string text)
        {
            if (!string.IsNullOrEmpty(text)) _cmbFind.Text = text;
            UpdateMatchCount();
            _cmbFind.Focus();
            _cmbFind.SelectAll();
        }

        private void UpdateMatchCount()
        {
            var (count, _) = ComputeMatches();
            if (string.IsNullOrEmpty(_cmbFind.Text))
                _lblStatus.Text = "";
            else
                _lblStatus.Text = $"{count} match{(count == 1 ? "" : "es")}";
        }

        private (int Count, List<(int Index, int Length)> Matches) ComputeMatches()
        {
            var hits = new List<(int, int)>();
            var pattern = _cmbFind.Text;
            if (string.IsNullOrEmpty(pattern)) return (0, hits);
            var text = _target.Text ?? string.Empty;
            try
            {
                var regex = BuildRegex(pattern, _chkRegex.Checked, _chkCase.Checked, _chkWord.Checked);
                foreach (Match m in regex.Matches(text))
                    hits.Add((m.Index, m.Length));
            }
            catch (ArgumentException) { }
            return (hits.Count, hits);
        }

        private static Regex BuildRegex(string pattern, bool isRegex, bool matchCase, bool wholeWord)
        {
            var options = RegexOptions.Multiline;
            if (!matchCase) options |= RegexOptions.IgnoreCase;
            var rx = isRegex ? pattern : Regex.Escape(pattern);
            if (wholeWord) rx = $@"\b(?:{rx})\b";
            return new Regex(rx, options);
        }

        private void FindOne(bool forward)
        {
            var pattern = _cmbFind.Text;
            if (string.IsNullOrEmpty(pattern)) return;

            var (count, matches) = ComputeMatches();
            if (count == 0) { _lblStatus.Text = "No matches"; return; }

            int caret = _target.SelectionStart + (forward ? Math.Max(_target.SelectionLength, 0) : 0);
            int target;
            if (forward)
            {
                target = matches.FindIndex(m => m.Index >= caret);
                if (target < 0) target = 0;
            }
            else
            {
                target = matches.FindLastIndex(m => m.Index < _target.SelectionStart);
                if (target < 0) target = matches.Count - 1;
            }

            var (idx, len) = matches[target];
            _target.Select(idx, len);
            _target.ScrollToCaret();
            _target.Focus();
            _lblStatus.Text = $"Match {target + 1} of {count}";

            _onSearchUsed?.Invoke(pattern);
            if (!_cmbFind.Items.Contains(pattern)) _cmbFind.Items.Insert(0, pattern);
        }

        private void ReplaceCurrent()
        {
            if (_target.ReadOnly)
            {
                _lblStatus.Text = "Output is read-only — toggle Edit first.";
                return;
            }
            var pattern = _cmbFind.Text;
            if (string.IsNullOrEmpty(pattern)) return;

            var sel = _target.SelectedText;
            if (!string.IsNullOrEmpty(sel))
            {
                try
                {
                    var regex = BuildRegex(pattern, _chkRegex.Checked, _chkCase.Checked, _chkWord.Checked);
                    if (regex.IsMatch(sel))
                    {
                        var replacement = _cmbReplace.Text ?? "";
                        var replaced = _chkRegex.Checked ? regex.Replace(sel, replacement) : replacement;
                        _target.SelectedText = replaced;
                    }
                }
                catch (ArgumentException ex)
                {
                    _lblStatus.Text = "Regex error: " + ex.Message;
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
                _lblStatus.Text = "Output is read-only — toggle Edit first.";
                return;
            }
            var pattern = _cmbFind.Text;
            if (string.IsNullOrEmpty(pattern)) return;
            try
            {
                var regex = BuildRegex(pattern, _chkRegex.Checked, _chkCase.Checked, _chkWord.Checked);
                var replacement = _cmbReplace.Text ?? "";
                var text = _target.Text ?? "";
                var newText = _chkRegex.Checked
                    ? regex.Replace(text, replacement)
                    : regex.Replace(text, _ => replacement);
                int count = regex.Matches(text).Count;
                _target.Text = newText;
                _lblStatus.Text = $"Replaced {count} occurrence{(count == 1 ? "" : "s")}";
                _onSearchUsed?.Invoke(pattern);
            }
            catch (ArgumentException ex)
            {
                _lblStatus.Text = "Regex error: " + ex.Message;
            }
        }
    }
}
