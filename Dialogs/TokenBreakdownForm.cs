using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    /// <summary>
    /// Where the pack's tokens are going, and what to remove if it does not fit.
    /// </summary>
    /// <remarks>
    /// "The pack is too big" is only actionable if the user can see which files caused it. The
    /// suggestion names the fewest largest files that would bring the pack under the window,
    /// rather than telling the user to "remove some files".
    /// </remarks>
    public partial class TokenBreakdownForm : ThemedForm
    {
        public TokenBreakdownForm(string bundleText, TokenModel model, int windowTokens)
        {
            InitializeComponent();

            var breakdown = TokenBudget.Breakdown(bundleText);
            int total = TokenEstimator.Estimate(bundleText);

            lblHeaderSubtitle.Text = windowTokens > 0
                ? $"{model.Display} — {TokenBudget.Describe(total, windowTokens)}"
                : $"{model.Display} — ~{total:N0} tokens";

            lblCaveat.Text = TokenBudget.EstimateCaveat;

            lstFiles.BeginUpdate();
            try
            {
                foreach (var file in breakdown)
                {
                    var item = new ListViewItem(file.Path);
                    item.SubItems.Add(file.Tokens.ToString("N0", CultureInfo.CurrentCulture));
                    item.SubItems.Add(total > 0
                        ? (file.Tokens * 100.0 / total).ToString("0.0", CultureInfo.CurrentCulture) + "%"
                        : "—");
                    item.ToolTipText = file.Path;
                    lstFiles.Items.Add(item);
                }
            }
            finally
            {
                lstFiles.EndUpdate();
            }

            lstFiles.ShowItemToolTips = true;

            var trim = TokenBudget.SuggestTrim(breakdown, total, windowTokens);
            if (trim.Count > 0)
            {
                var names = string.Join(", ", trim.Take(5).Select(f => System.IO.Path.GetFileName(f.Path)));
                if (trim.Count > 5) names += $", and {trim.Count - 5} more";

                lblSuggestion.Text =
                    $"Over budget by ~{total - windowTokens:N0} tokens. Removing the largest " +
                    $"{trim.Count} file{(trim.Count == 1 ? "" : "s")} would fit: {names}.";
                pnlSuggestion.Visible = true;
            }
        }

        private void BtnHelp_Click(object? sender, EventArgs e)
        {
            using var help = new HelpForm(HelpTopics.BuildingThePack);
            help.ShowDialog(this);
        }
    }
}
