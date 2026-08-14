using System.Windows.Forms;
using CodeShuttle.Theming;
using CodeShuttle.UI;

namespace CodeShuttle.Dialogs
{
    public partial class PromptDialog : ThemedForm
    {
        public string Value
        {
            get => txtInput.Text;
            set => txtInput.Text = value ?? string.Empty;
        }

        public PromptDialog() : this("Prompt", "Value:", "")
        {
        }

        public PromptDialog(string title, string prompt, string initial)
        {
            InitializeComponent();


            Text = title;
            lblHeaderTitle.Text = title;
            lblPrompt.Text = prompt;
            // The caller supplies the prompt, so the accessible name has to follow it rather than
            // stay at the designer's placeholder.
            txtInput.AccessibleName = prompt?.TrimEnd(':', ' ') ?? "Value";
            txtInput.Text = initial ?? string.Empty;
            txtInput.SelectAll();
        }
    }
}
