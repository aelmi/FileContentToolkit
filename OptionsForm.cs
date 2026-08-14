using System;
using System.Linq;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Settings;
using CodeShuttle.Theming;
using CodeShuttle.UI;

namespace CodeShuttle.Dialogs
{
    public partial class OptionsForm : ThemedForm
    {
        private readonly AppSettings _settings;

        public OptionsForm(AppSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            InitializeComponent();

            foreach (var model in TokenBudget.All) cmbModel.Items.Add(model);

            LoadFromSettings();
        }

        private void LoadFromSettings()
        {
            numMaxKb.Value = Math.Min(int.MaxValue, Math.Max(0, _settings.MaxFileSizeBytes / 1024));
            chkSkipBinary.Checked = _settings.SkipBinaryFiles;
            chkAutoEncoding.Checked = _settings.AutoDetectEncoding;
            chkGitIgnore.Checked = _settings.UseGitIgnoreFiles;
            chkDockerIgnore.Checked = _settings.UseDockerIgnoreFiles;
            chkWatch.Checked = _settings.WatchFolderForChanges;

            chkRedactSecrets.Checked = _settings.RedactSecrets;
            chkWarnOnSecrets.Checked = _settings.WarnOnSecrets;

            var model = TokenBudget.Resolve(_settings.TokenModelId);
            cmbModel.SelectedItem = TokenBudget.All.First(m => m.Id == model.Id);

            numCustomBudget.Value = Math.Min(numCustomBudget.Maximum, Math.Max(0, _settings.CustomTokenBudget));
            UpdateCustomBudgetEnabled();
        }

        private void CmbModel_SelectedIndexChanged(object? sender, EventArgs e) => UpdateCustomBudgetEnabled();

        /// <summary>
        /// The custom figure only means anything when the custom entry is selected. Leaving it
        /// live against a named model would imply it overrides that model's window.
        /// </summary>
        private void UpdateCustomBudgetEnabled()
        {
            bool custom = (cmbModel.SelectedItem as TokenModel)?.Id == TokenBudget.CustomModelId;
            lblCustomBudget.Enabled = custom;
            numCustomBudget.Enabled = custom;
        }

        /// <summary>
        /// Opens the template library. The composer doubles as the editor, so there is one place
        /// templates are managed rather than a second, near-identical editor reachable only here.
        /// </summary>
        private void BtnPromptTemplates_Click(object? sender, EventArgs e)
        {
            using var dlg = new PromptComposerForm(_settings, "");
            dlg.ShowDialog(this);
        }

        private void BtnHelp_Click(object? sender, EventArgs e)
        {
            using var help = new HelpForm(HelpTopics.Settings);
            help.ShowDialog(this);
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            _settings.MaxFileSizeBytes = (long)numMaxKb.Value * 1024L;
            _settings.SkipBinaryFiles = chkSkipBinary.Checked;
            _settings.AutoDetectEncoding = chkAutoEncoding.Checked;
            _settings.UseGitIgnoreFiles = chkGitIgnore.Checked;
            _settings.UseDockerIgnoreFiles = chkDockerIgnore.Checked;
            _settings.WatchFolderForChanges = chkWatch.Checked;

            _settings.RedactSecrets = chkRedactSecrets.Checked;
            _settings.WarnOnSecrets = chkWarnOnSecrets.Checked;

            if (cmbModel.SelectedItem is TokenModel model) _settings.TokenModelId = model.Id;
            _settings.CustomTokenBudget = (int)numCustomBudget.Value;
        }
    }
}
