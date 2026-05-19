using System.Windows.Forms;
using FileContentToolkit.UI;

namespace FileContentToolkit.Dialogs
{
    public partial class PromptDialog : Form
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

            if (Theme.AppIcon != null) Icon = Theme.AppIcon;
            Theme.AttachHover(btnOk, btnOk.BackColor);
            Theme.AttachHover(btnCancel, btnCancel.BackColor);

            Text = title;
            lblHeaderTitle.Text = title;
            lblPrompt.Text = prompt;
            txtInput.Text = initial ?? string.Empty;
            txtInput.SelectAll();
        }
    }
}
