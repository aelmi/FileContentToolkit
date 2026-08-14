using System;
using System.Linq;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Settings;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    /// <summary>
    /// Wraps a generated pack in a prompt template and copies the result.
    /// </summary>
    /// <remarks>
    /// Two things arrive together here. The pack can now be sent with the user's actual question
    /// embedded — the argument both built-in prompt builders always accepted and never received,
    /// because nothing called them at all. And the two built-ins are the seed of an editable
    /// library rather than the only options.
    /// </remarks>
    public partial class PromptComposerForm : ThemedForm
    {
        private readonly AppSettings _settings;
        private readonly string _rawBundle;
        private bool _loading;

        /// <summary>The composed prompt, available once the dialog returns OK.</summary>
        public string ComposedPrompt { get; private set; } = "";

        public PromptComposerForm(AppSettings settings, string rawBundle)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _rawBundle = rawBundle ?? "";

            InitializeComponent();

            ReloadTemplates(0);
        }

        private void ReloadTemplates(int selectIndex)
        {
            _loading = true;
            try
            {
                var templates = PromptTemplateStore.Load(_settings);

                lstTemplates.Items.Clear();
                foreach (var template in templates) lstTemplates.Items.Add(template);

                if (lstTemplates.Items.Count > 0)
                    lstTemplates.SelectedIndex = Math.Min(Math.Max(0, selectIndex), lstTemplates.Items.Count - 1);
            }
            finally
            {
                _loading = false;
            }

            ShowSelectedBody();
        }

        private PromptTemplate? Selected => lstTemplates.SelectedItem as PromptTemplate;

        private void LstTemplates_SelectedIndexChanged(object? sender, EventArgs e) => ShowSelectedBody();

        private void ShowSelectedBody()
        {
            var template = Selected;
            if (template == null)
            {
                txtBody.Text = "";
                btnDelete.Enabled = false;
                return;
            }

            _loading = true;
            try
            {
                // A built-in with an empty body is rendered by its formatter method. Showing the
                // wording it produces would imply the text is editable when clearing it is what
                // restores the built-in behaviour, so the placeholder says so instead.
                txtBody.Text = template.Body;
                txtBody.PlaceholderText = template.UsesBuiltInRenderer
                    ? "Using the supplied wording. Type here to override it."
                    : "";
            }
            finally
            {
                _loading = false;
            }

            btnDelete.Enabled = true;
        }

        private void TxtBody_TextChanged(object? sender, EventArgs e)
        {
            if (_loading) return;
            var template = Selected;
            if (template == null) return;

            template.Body = txtBody.Text;
            _settings.SaveDebounced();
        }

        private void BtnNew_Click(object? sender, EventArgs e)
        {
            using var prompt = new PromptDialog("New template", "Template name:", "");
            if (prompt.ShowDialog(this) != DialogResult.OK) return;

            var name = prompt.Value.Trim();
            if (name.Length == 0) return;

            if (PromptTemplateStore.Find(_settings, name) != null)
            {
                MessageBox.Show(this, "A template with that name already exists.", "New template",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _settings.PromptTemplates.Add(new PromptTemplate
            {
                Name = name,
                Format = PromptBodyFormat.Markdown,
                Body = "Here are the files:" + Environment.NewLine + Environment.NewLine +
                       PromptTemplate.FilesPlaceholder + Environment.NewLine + Environment.NewLine +
                       PromptTemplate.QuestionPlaceholder + Environment.NewLine,
            });
            _settings.SaveDebounced();

            ReloadTemplates(_settings.PromptTemplates.Count - 1);
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var template = Selected;
            if (template == null) return;

            // Destructive and unrecoverable for a template the user wrote, so it stays modal.
            var answer = MessageBox.Show(this,
                $"Delete the template '{template.Name}'?",
                "Delete template", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (answer != DialogResult.Yes) return;

            int index = lstTemplates.SelectedIndex;
            _settings.PromptTemplates.Remove(template);
            _settings.SaveDebounced();

            ReloadTemplates(Math.Max(0, index - 1));
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            PromptTemplateStore.ResetBuiltIns(_settings);
            _settings.SaveDebounced();
            ReloadTemplates(0);
        }

        private void BtnHelp_Click(object? sender, EventArgs e)
        {
            using var help = new HelpForm(HelpTopics.BuildingThePack);
            help.ShowDialog(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                var template = Selected;
                if (template == null)
                {
                    e.Cancel = true;
                    MessageBox.Show(this, "Choose a template first.", "Copy as prompt",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ComposedPrompt = PromptTemplateStore.Render(template, _rawBundle, txtQuestion.Text);
                _settings.FlushPendingSave();
            }

            base.OnFormClosing(e);
        }
    }
}
