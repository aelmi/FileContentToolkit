using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    /// <summary>
    /// One rule per row, with a live count of what each removes and a box to test a single path.
    /// </summary>
    /// <remarks>
    /// Replaces a single comma-separated text box whose only documentation was a tooltip. It
    /// matters more than it looks: the matching semantics changed underneath these patterns —
    /// substring matching became globs, and a <c>dir/</c> rule went from never matching anything
    /// to working — so a user with existing patterns needs to be shown what their rules now do
    /// rather than find out by noticing a file is missing.
    /// </remarks>
    public partial class ExclusionRuleEditorForm : ThemedForm
    {
        private readonly List<string> _rules;
        private readonly string _folder;
        private readonly List<string> _candidates;

        /// <summary>The edited rule list. Read after the dialog returns OK.</summary>
        public IReadOnlyList<string> Rules => _rules;

        /// <param name="rules">The current rules.</param>
        /// <param name="folder">The scan root the candidates are relative to.</param>
        /// <param name="candidates">
        /// The files the counts are measured against. Supplied by the caller rather than
        /// re-enumerated here so that opening this dialog never triggers a disk scan.
        /// </param>
        public ExclusionRuleEditorForm(IEnumerable<string> rules, string folder, IEnumerable<string> candidates)
        {
            InitializeComponent();

            _rules = (rules ?? Enumerable.Empty<string>())
                .Select(r => r?.Trim() ?? "")
                .Where(r => r.Length > 0)
                .ToList();

            _folder = folder ?? "";
            _candidates = (candidates ?? Enumerable.Empty<string>()).ToList();

            RefreshRules();
        }

        private void RefreshRules()
        {
            lstRules.BeginUpdate();
            try
            {
                lstRules.Items.Clear();
                foreach (var rule in _rules)
                {
                    var item = new ListViewItem(rule);
                    item.SubItems.Add(DescribeExclusions(rule));
                    lstRules.Items.Add(item);
                }
            }
            finally
            {
                lstRules.EndUpdate();
            }

            UpdateSummary();
            UpdateTestResult();
        }

        /// <summary>How many of the candidates a single rule removes, on its own.</summary>
        private string DescribeExclusions(string rule)
        {
            if (_candidates.Count == 0) return "no scan to measure against";

            int n = CountExcluded(new[] { rule });
            if (n == 0) return "excludes nothing";
            return n == 1 ? "excludes 1 file" : $"excludes {n:N0} files";
        }

        private int CountExcluded(IReadOnlyList<string> rules)
        {
            int count = 0;
            foreach (var path in _candidates)
            {
                if (FileContentService.IsIgnoredByUserPatterns(path, _folder, rules)) count++;
            }
            return count;
        }

        private void UpdateSummary()
        {
            if (_candidates.Count == 0)
            {
                lblSummary.Text = "No files scanned yet, so there is nothing to measure against.";
                return;
            }

            int excluded = _rules.Count == 0 ? 0 : CountExcluded(_rules);
            int kept = _candidates.Count - excluded;

            lblSummary.Text =
                $"{_rules.Count} rule{(_rules.Count == 1 ? "" : "s")} — " +
                $"{excluded:N0} of {_candidates.Count:N0} files excluded, {kept:N0} kept.";
        }

        private void UpdateTestResult()
        {
            var probe = txtTest.Text.Trim();
            if (probe.Length == 0)
            {
                lblTestResult.Text = "";
                return;
            }

            if (_rules.Count == 0)
            {
                lblTestResult.Text = "No rules, so nothing is excluded.";
                return;
            }

            // Reported per rule rather than as a single yes/no: knowing that a path is excluded
            // is much less useful than knowing which rule did it.
            var hits = _rules
                .Where(r => FileContentService.IsIgnoredByUserPatterns(Combine(probe), _folder, new[] { r }))
                .ToList();

            lblTestResult.Text = hits.Count == 0
                ? "Not excluded by any rule."
                : "Excluded by: " + string.Join(", ", hits);
        }

        /// <summary>
        /// Makes a typed relative path absolute against the scan root, so that the test box takes
        /// the same shape of input the rules are written against.
        /// </summary>
        private string Combine(string probe)
        {
            if (string.IsNullOrEmpty(_folder)) return probe;
            try { return System.IO.Path.Combine(_folder, probe); }
            catch (ArgumentException) { return probe; }
        }

        private void LstRules_SelectedIndexChanged(object? sender, EventArgs e)
        {
            btnRemoveRule.Enabled = lstRules.SelectedIndices.Count > 0;
            if (lstRules.SelectedIndices.Count > 0)
                txtNewRule.Text = _rules[lstRules.SelectedIndices[0]];
        }

        private void BtnAddRule_Click(object? sender, EventArgs e) => AddTypedRule();

        private void TxtNewRule_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            // Otherwise Enter reaches the form's AcceptButton and closes the dialog, which is a
            // surprising way to lose the rule you were halfway through typing.
            e.Handled = true;
            e.SuppressKeyPress = true;
            AddTypedRule();
        }

        private void AddTypedRule()
        {
            var rule = txtNewRule.Text.Trim();
            if (rule.Length == 0) return;

            if (_rules.Contains(rule, StringComparer.Ordinal))
            {
                lblTestResult.Text = "That rule is already in the list.";
                return;
            }

            _rules.Add(rule);
            txtNewRule.Clear();
            RefreshRules();
        }

        private void BtnRemoveRule_Click(object? sender, EventArgs e)
        {
            if (lstRules.SelectedIndices.Count == 0) return;

            _rules.RemoveAt(lstRules.SelectedIndices[0]);
            RefreshRules();
        }

        private void TxtTest_TextChanged(object? sender, EventArgs e) => UpdateTestResult();

        private void BtnHelp_Click(object? sender, EventArgs e)
        {
            using var help = new HelpForm(HelpTopics.SelectingFiles);
            help.ShowDialog(this);
        }
    }
}
