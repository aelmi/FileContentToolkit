using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CodeShuttle.UI
{
    /// <summary>One keyboard binding: the chord, what it does, and how to describe it.</summary>
    public sealed record ShortcutBinding(Keys Keys, string Action, string Description)
    {
        /// <summary>Human-readable chord, e.g. "Ctrl+Shift+O".</summary>
        public string Display => Shortcuts.Format(Keys);
    }

    /// <summary>
    /// The application's keyboard map, in one place.
    /// </summary>
    /// <remarks>
    /// The product previously had exactly one shortcut (F1). Ctrl+F and Ctrl+H existed only as a
    /// lambda on the output box's KeyDown, so they worked only while that box had focus, and the
    /// help window described them from a second, independently-maintained list that had already
    /// drifted into duplicate entries. Declaring them once here means <c>ProcessCmdKey</c> and the
    /// help window cannot disagree.
    /// </remarks>
    public static class Shortcuts
    {
        public static readonly ShortcutBinding BrowseFolder =
            new(Keys.Control | Keys.O, "Browse for folder", "Choose the folder to scan.");

        public static readonly ShortcutBinding AddFolder =
            new(Keys.Control | Keys.Shift | Keys.O, "Add folder", "Scan another folder and append its files.");

        public static readonly ShortcutBinding Refresh =
            new(Keys.F5, "Refresh file list", "Rescan the current folder.");

        public static readonly ShortcutBinding RefreshAlt =
            new(Keys.Control | Keys.R, "Refresh file list", "Rescan the current folder.");

        public static readonly ShortcutBinding Generate =
            new(Keys.Control | Keys.G, "Generate output", "Build the output pack from the selected files.");

        public static readonly ShortcutBinding GenerateAlt =
            new(Keys.F9, "Generate output", "Build the output pack from the selected files.");

        public static readonly ShortcutBinding CopyOutput =
            new(Keys.Control | Keys.C, "Copy output", "Copy the whole output pane. Inside a text box, copies the selection instead.");

        public static readonly ShortcutBinding CopyOutputAs =
            new(Keys.Control | Keys.Shift | Keys.C, "Copy output as…", "Open the format menu: Markdown, XML or JSON.");

        public static readonly ShortcutBinding ExportOutput =
            new(Keys.Control | Keys.E, "Export output", "Write the output pane to a file.");

        public static readonly ShortcutBinding Find =
            new(Keys.Control | Keys.F, "Find in output", "Open Find and Replace, from anywhere in the window.");

        public static readonly ShortcutBinding Replace =
            new(Keys.Control | Keys.H, "Replace in output", "Open Find and Replace, from anywhere in the window.");

        public static readonly ShortcutBinding Options =
            new(Keys.Control | Keys.Oemcomma, "Options", "Filters, encoding and folder watching.");

        public static readonly ShortcutBinding Presets =
            new(Keys.Control | Keys.P, "Presets", "Open the saved preset list.");

        public static readonly ShortcutBinding CancelOperation =
            new(Keys.Escape, "Cancel", "Stop the running scan, generate or apply.");

        public static readonly ShortcutBinding PasteResponse =
            new(Keys.Control | Keys.Shift | Keys.V, "Paste AI response",
                "Bring an AI's reply back in, diff it against a folder and apply it. No Generate needed first.");

        /// <summary>
        /// Replaces the access key that used to live on the search field's caption. The caption is
        /// gone — the toggles moved inside the field and the section header names the area — and a
        /// real chord is better than what it replaced: it reaches the box from anywhere in the
        /// window rather than only when the caption is on screen and unobstructed.
        /// </summary>
        public static readonly ShortcutBinding SearchInFiles =
            new(Keys.Control | Keys.Shift | Keys.F, "Search in files",
                "Jump to the search box and look inside the selected files.");

        public static readonly ShortcutBinding Help =
            new(Keys.F1, "Help for this pane", "Open help at the topic for whatever currently has focus.");

        public static readonly ShortcutBinding HelpContents =
            new(Keys.Shift | Keys.F1, "Help contents", "Open help at the beginning.");

        public static readonly ShortcutBinding DeleteSelected =
            new(Keys.Delete, "Remove selected", "Remove the selected files or extensions from the list.");

        /// <summary>Every binding, in the order the help window should present them.</summary>
        public static IReadOnlyList<ShortcutBinding> All { get; } = new[]
        {
            BrowseFolder, AddFolder, Refresh, RefreshAlt,
            Generate, GenerateAlt,
            CopyOutput, CopyOutputAs, ExportOutput,
            PasteResponse,
            Find, Replace, SearchInFiles,
            Options, Presets,
            DeleteSelected, CancelOperation, Help, HelpContents,
        };

        /// <summary>
        /// Formats a chord the way Windows writes them. <see cref="KeysConverter"/> is not used:
        /// it renders the comma key as "Oemcomma" and the function keys inconsistently.
        /// </summary>
        public static string Format(Keys keys)
        {
            var sb = new StringBuilder();
            if ((keys & Keys.Control) == Keys.Control) sb.Append("Ctrl+");
            if ((keys & Keys.Alt) == Keys.Alt) sb.Append("Alt+");
            if ((keys & Keys.Shift) == Keys.Shift) sb.Append("Shift+");

            var code = keys & Keys.KeyCode;
            sb.Append(code switch
            {
                Keys.Oemcomma => ",",
                Keys.OemPeriod => ".",
                Keys.Escape => "Esc",
                Keys.Delete => "Del",
                _ => code.ToString(),
            });
            return sb.ToString();
        }
    }
}
