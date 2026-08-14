using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeShuttle.Filters;
using CodeShuttle.Help;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    /// <summary>
    /// Review of credentials found in a pack, shown before it can be copied or exported.
    /// </summary>
    /// <remarks>
    /// The detection is <see cref="SecretScanner"/>'s, delivered and unit-tested previously; this
    /// is the surface that makes it matter. It sits on the copy and export paths rather than on
    /// Generate, because generating is private to the machine and copying is the point at which
    /// the content is about to be handed to a third party.
    ///
    /// Values are shown masked. A dialog whose whole purpose is to stop credentials being pasted
    /// somewhere should not begin by printing them on screen, and the preview — four characters
    /// and a length — is enough to recognise a key you already know.
    /// </remarks>
    public partial class SecretWarningForm : ThemedForm
    {
        private readonly List<SecretMatch> _matches;

        public SecretWarningForm(IEnumerable<SecretMatch> matches, bool redactByDefault)
        {
            ArgumentNullException.ThrowIfNull(matches);

            InitializeComponent();

            _matches = matches.ToList();
            Populate(redactByDefault);
        }

        /// <summary>The matches the user chose to redact.</summary>
        public IReadOnlyList<SecretMatch> Redacted =>
            lstMatches.CheckedIndices.Cast<int>().Select(i => _matches[i]).ToList();

        /// <summary>The matches the user chose to keep as-is.</summary>
        public IReadOnlyList<SecretMatch> Kept =>
            Enumerable.Range(0, _matches.Count)
                      .Where(i => !lstMatches.Items[i].Checked)
                      .Select(i => _matches[i])
                      .ToList();

        private void Populate(bool redactByDefault)
        {
            lblHeaderTitle.Text = _matches.Count == 1
                ? "1 possible credential in this pack"
                : $"{_matches.Count} possible credentials in this pack";

            lstMatches.BeginUpdate();
            try
            {
                foreach (var match in _matches)
                {
                    var item = new ListViewItem(ShortPath(match.Path)) { Checked = redactByDefault };
                    item.SubItems.Add(match.Line.ToString(System.Globalization.CultureInfo.CurrentCulture));
                    item.SubItems.Add(Describe(match.Kind));
                    item.SubItems.Add(match.Preview);

                    // The full path belongs in the tooltip, not the column, but the value never
                    // goes anywhere near either.
                    item.ToolTipText = match.Path;
                    lstMatches.Items.Add(item);
                }
            }
            finally
            {
                lstMatches.EndUpdate();
            }

            lstMatches.ShowItemToolTips = true;
        }

        private static string ShortPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "(pack)";
            return path.Length <= 60 ? path : string.Concat("…", path.AsSpan(path.Length - 59));
        }

        internal static string Describe(SecretKind kind) => kind switch
        {
            SecretKind.AwsAccessKeyId => "AWS access key ID",
            SecretKind.PrivateKey => "Private key block",
            SecretKind.ApiKeyAssignment => "API key or secret assignment",
            SecretKind.ConnectionStringPassword => "Connection-string password",
            SecretKind.JsonWebToken => "JSON web token",
            SecretKind.HighEntropyEnvValue => "High-entropy .env value",
            _ => kind.ToString(),
        };

        private void BtnRedactAll_Click(object? sender, EventArgs e) => SetAll(true);

        private void BtnKeepAll_Click(object? sender, EventArgs e) => SetAll(false);

        private void SetAll(bool checkedState)
        {
            lstMatches.BeginUpdate();
            try
            {
                foreach (ListViewItem item in lstMatches.Items) item.Checked = checkedState;
            }
            finally
            {
                lstMatches.EndUpdate();
            }
        }

        private void BtnHelp_Click(object? sender, EventArgs e)
        {
            using var help = new HelpForm(HelpTopics.BuildingThePack);
            help.ShowDialog(this);
        }
    }
}
