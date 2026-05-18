using System.Drawing;
using System.Windows.Forms;
using FileContentToolkit.UI;

namespace FileContentToolkit.Dialogs
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            if (Theme.AppIcon != null) Icon = Theme.AppIcon;
            Theme.AttachHover(btnClose, btnClose.BackColor);
            PopulateContent();
        }

        private void PopulateContent()
        {
            rtbContent.Clear();

            AppendSection("Keyboard Shortcuts");
            AppendRow("Ctrl + F",         "Open Find & Replace in the output pane");
            AppendRow("Ctrl + H",         "Open Find & Replace in the output pane");
            AppendRow("F3 / Shift + F3",  "(in Find dialog) Next / previous match");
            AppendRow("Esc",              "(in Find dialog) Close the dialog");
            AppendRow("Delete",           "Remove selected file(s) from the list");
            AppendRow("Enter",            "(in extension box) Add the typed extension");
            rtbContent.AppendText("\n");

            AppendSection("File scanning");
            AppendBullet("Type or browse to a folder; the scan runs in the background. Subfolders are included by default.");
            AppendBullet("Use the extensions list on the left to filter what gets picked up. \"Refresh\" re-scans.");
            AppendBullet("\"Tree\" opens a checkbox-tree picker for hand-picking files/folders.");
            AppendBullet("\"Recent ▾\" reopens any of your last 15 folders.");
            AppendBullet("Ignore patterns: comma-separated globs (e.g. *.tmp, bin/). Use the Options dialog to enable .gitignore / .dockerignore as well.");
            rtbContent.AppendText("\n");

            AppendSection("Generating output");
            AppendBullet("Click GENERATE to read every selected file and assemble the concatenated output in the right pane.");
            AppendBullet("Encoding can be auto-detected (BOM + UTF-8 fallback) — see Options.");
            AppendBullet("\"Edit\" toggles the output between read-only and editable.");
            AppendBullet("\"Copy\" sends the output to the clipboard. \"Export\" saves it to a .txt file.");
            AppendBullet("Compress / Decompress GZip+Base64 the text; the *Enc variants additionally encrypt with AES-GCM.");
            rtbContent.AppendText("\n");

            AppendSection("Search");
            AppendBullet("\"Search\" highlights the files whose contents match the term.");
            AppendBullet("Aa = match case, Word = whole word, .* = regex.");
            AppendBullet("\"▾\" on the search row lists your recent search terms.");
            AppendBullet("Match count appears under the row after each search.");
            rtbContent.AppendText("\n");

            AppendSection("Presets");
            AppendBullet("\"Save preset\" stores the current folder, extensions, ignore patterns, and subfolder toggle.");
            AppendBullet("\"Presets ▾\" lists saved presets and includes a \"Manage presets…\" option for renaming/deleting.");
            rtbContent.AppendText("\n");

            AppendSection("Recreate files");
            AppendBullet("After Generate (or after pasting a previously generated block), pick a target folder to reconstruct each file from the output, preserving relative paths.");

            rtbContent.SelectionStart = 0;
            rtbContent.ScrollToCaret();
        }

        private void AppendSection(string title)
        {
            rtbContent.SelectionStart = rtbContent.TextLength;
            rtbContent.SelectionLength = 0;
            rtbContent.SelectionColor = Color.FromArgb(0, 102, 204);
            rtbContent.SelectionFont = new Font(rtbContent.Font.FontFamily, 11F, FontStyle.Bold);
            rtbContent.AppendText(title + "\n");
            rtbContent.SelectionColor = Color.FromArgb(33, 37, 41);
            rtbContent.SelectionFont = rtbContent.Font;
        }

        private void AppendRow(string key, string description)
        {
            rtbContent.SelectionStart = rtbContent.TextLength;
            rtbContent.SelectionLength = 0;
            rtbContent.SelectionFont = new Font(rtbContent.Font.FontFamily, rtbContent.Font.Size, FontStyle.Bold);
            rtbContent.AppendText($"  {key,-22}");
            rtbContent.SelectionFont = rtbContent.Font;
            rtbContent.AppendText("  " + description + "\n");
        }

        private void AppendBullet(string text)
        {
            rtbContent.SelectionStart = rtbContent.TextLength;
            rtbContent.SelectionLength = 0;
            rtbContent.SelectionFont = rtbContent.Font;
            rtbContent.AppendText("  •  " + text + "\n");
        }
    }
}
