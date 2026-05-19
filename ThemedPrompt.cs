using System.Windows.Forms;
using FileContentToolkit.Dialogs;

namespace FileContentToolkit.UI
{
    /// <summary>
    /// Convenience wrapper around <see cref="PromptDialog"/> so callers don't have to manage the
    /// dialog's lifetime explicitly. Returns the entered text on OK, or null on Cancel.
    /// </summary>
    public static class ThemedPrompt
    {
        public static string? Show(IWin32Window? owner, string title, string prompt, string initial = "")
        {
            using var dlg = new PromptDialog(title, prompt, initial);
            return dlg.ShowDialog(owner) == DialogResult.OK ? dlg.Value.Trim() : null;
        }
    }
}
