using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Theming;
using CodeShuttle.UI;

namespace CodeShuttle.Dialogs
{
    /// <summary>
    /// The help window: a topic list beside rendered Markdown.
    /// </summary>
    /// <remarks>
    /// Deliberately not the WebView2 system the reviews sketched — no topic tree, no inverted
    /// index, no <c>help://</c> deep links, and no new runtime dependency to complicate the
    /// installer. Opening at the topic for the focused pane is the behaviour that was actually
    /// worth having, and it needs none of that machinery.
    /// </remarks>
    public partial class HelpForm : ThemedForm
    {
        /// <summary>
        /// Synthesised rather than embedded: the shortcut list is generated from
        /// <see cref="Shortcuts.All"/>, the same table <c>ProcessCmdKey</c> reads. It used to be
        /// maintained separately here and had already drifted into duplicate Ctrl+F / Ctrl+H rows.
        /// </summary>
        private const string ShortcutsTopicId = "keyboard-shortcuts";

        private sealed record TopicEntry(string Id, string Title)
        {
            public override string ToString() => Title;
        }

        private readonly List<TopicEntry> _topics = new();

        public HelpForm() : this(null) { }

        public HelpForm(string? topicId)
        {
            InitializeComponent();

            _topics.Add(new TopicEntry(ShortcutsTopicId, "Keyboard Shortcuts"));
            foreach (var topic in HelpTopics.All)
                _topics.Add(new TopicEntry(topic.Id, topic.Title));

            lstTopics.Items.AddRange(_topics.ToArray());
            SelectTopic(topicId ?? HelpTopics.GettingStarted);
        }

        /// <summary>Moves to a topic by id, falling back to the first entry.</summary>
        public void SelectTopic(string? topicId)
        {
            int index = _topics.FindIndex(t =>
                string.Equals(t.Id, topicId, StringComparison.OrdinalIgnoreCase));

            lstTopics.SelectedIndex = index >= 0 ? index : 0;
        }

        /// <summary>The topic currently on screen. Used by tests.</summary>
        internal string? CurrentTopicId =>
            lstTopics.SelectedIndex >= 0 && lstTopics.SelectedIndex < _topics.Count
                ? _topics[lstTopics.SelectedIndex].Id
                : null;

        private void LstTopics_SelectedIndexChanged(object? sender, EventArgs e) => Render();

        /// <summary>
        /// The content is coloured rich-text runs, which the control-tree walk cannot reach, so
        /// it is rebuilt from the new palette.
        /// </summary>
        protected override void ApplyTheme()
        {
            base.ApplyTheme();
            if (rtbContent != null && !IsDisposed) Render();
        }

        private void Render()
        {
            if (rtbContent == null || IsDisposed) return;

            var id = CurrentTopicId;
            if (id == null) return;

            rtbContent.Clear();

            if (id == ShortcutsTopicId)
            {
                RenderShortcuts();
            }
            else
            {
                var topic = HelpTopics.Find(id);
                if (topic != null) RenderMarkdown(HelpTopics.Read(topic));
            }

            rtbContent.SelectionStart = 0;
            rtbContent.ScrollToCaret();
        }

        private void RenderShortcuts()
        {
            AppendHeading("Keyboard Shortcuts", FontRole.Heading);

            foreach (var binding in Shortcuts.All)
                AppendRow(binding.Display, binding.Action);

            AppendRow("F3 / Shift+F3", "(in Find dialog) Next / previous match");
            AppendRow("Enter", "(in extension box) Add the typed extension");
        }

        // ------------------------------------------------------------------ Markdown

        /// <summary>
        /// Renders the subset of Markdown the help content actually uses: ATX headings, bullets,
        /// fenced code, tables, and inline bold and code.
        /// </summary>
        /// <remarks>
        /// A hand-rolled renderer rather than a Markdown package, because adding a NuGet
        /// dependency to draw nine pages of documentation is not a trade worth making, and the
        /// content is written against what this understands.
        /// </remarks>
        private void RenderMarkdown(string markdown)
        {
            var lines = markdown.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            bool inFence = false;

            foreach (var raw in lines)
            {
                var line = raw;

                if (line.TrimStart().StartsWith("```", StringComparison.Ordinal))
                {
                    inFence = !inFence;
                    continue;
                }

                if (inFence)
                {
                    AppendPlain(line, FontRole.MonoSmall, ThemeManager.Tokens.TextSecondary);
                    continue;
                }

                if (line.StartsWith("# ", StringComparison.Ordinal))
                {
                    AppendHeading(line.Substring(2).Trim(), FontRole.Title);
                }
                else if (line.StartsWith("## ", StringComparison.Ordinal))
                {
                    AppendBlank();
                    AppendHeading(line.Substring(3).Trim(), FontRole.Heading);
                }
                else if (line.StartsWith("- ", StringComparison.Ordinal))
                {
                    AppendInline("    •  " + line.Substring(2).Trim());
                }
                else if (line.StartsWith('|'))
                {
                    // Tables are rare enough that a real column layout is not worth it; a
                    // monospaced row keeps them legible and aligned.
                    AppendPlain("  " + line.Trim(), FontRole.MonoSmall, ThemeManager.Tokens.TextPrimary);
                }
                else if (line.Length > 0 && IsOrderedItem(line))
                {
                    AppendInline("    " + line.Trim());
                }
                else
                {
                    AppendInline(line);
                }
            }
        }

        private static bool IsOrderedItem(string line)
        {
            var t = line.TrimStart();
            return t.Length > 2 && char.IsDigit(t[0]) && t[1] == '.' && t[2] == ' ';
        }

        private void AppendHeading(string text, FontRole role)
        {
            Seek();
            rtbContent.SelectionColor = ThemeManager.Tokens.AccentOnSurface;
            rtbContent.SelectionFont = ThemeFonts.Get(role);
            rtbContent.AppendText(text + "\n");
            ResetRun();
        }

        private void AppendPlain(string text, FontRole role, Color colour)
        {
            Seek();
            rtbContent.SelectionColor = colour;
            rtbContent.SelectionFont = ThemeFonts.Get(role);
            rtbContent.AppendText(text + "\n");
            ResetRun();
        }

        private void AppendBlank()
        {
            Seek();
            rtbContent.AppendText("\n");
        }

        /// <summary>Emits a line, switching runs for <c>**bold**</c> and <c>`code`</c>.</summary>
        private void AppendInline(string text)
        {
            Seek();

            int i = 0;
            while (i < text.Length)
            {
                int bold = text.IndexOf("**", i, StringComparison.Ordinal);
                int code = text.IndexOf('`', i);

                int next = -1;
                bool isBold = false;
                if (bold >= 0 && (code < 0 || bold < code)) { next = bold; isBold = true; }
                else if (code >= 0) { next = code; }

                if (next < 0)
                {
                    Emit(text.Substring(i), FontRole.Body, ThemeManager.Tokens.TextPrimary);
                    break;
                }

                if (next > i) Emit(text.Substring(i, next - i), FontRole.Body, ThemeManager.Tokens.TextPrimary);

                var marker = isBold ? "**" : "`";
                int close = text.IndexOf(marker, next + marker.Length, StringComparison.Ordinal);
                if (close < 0)
                {
                    Emit(text.Substring(next), FontRole.Body, ThemeManager.Tokens.TextPrimary);
                    break;
                }

                int start = next + marker.Length;
                var inner = text.Substring(start, close - start);
                Emit(inner,
                     isBold ? FontRole.BodyBold : FontRole.MonoSmall,
                     isBold ? ThemeManager.Tokens.TextPrimary : ThemeManager.Tokens.AccentOnSurface);

                i = close + marker.Length;
            }

            rtbContent.AppendText("\n");
            ResetRun();
        }

        private void Emit(string text, FontRole role, Color colour)
        {
            if (text.Length == 0) return;
            rtbContent.SelectionStart = rtbContent.TextLength;
            rtbContent.SelectionLength = 0;
            rtbContent.SelectionColor = colour;
            rtbContent.SelectionFont = ThemeFonts.Get(role);
            rtbContent.AppendText(text);
        }

        private void Seek()
        {
            rtbContent.SelectionStart = rtbContent.TextLength;
            rtbContent.SelectionLength = 0;
        }

        private void ResetRun()
        {
            rtbContent.SelectionColor = ThemeManager.Tokens.TextPrimary;
            rtbContent.SelectionFont = ThemeFonts.Get(FontRole.Body);
        }

        /// <summary>
        /// Two columns via a real tab stop. This used to pad the key to a width of 22 with spaces
        /// and render it in proportional Segoe UI, so the description column came out ragged —
        /// space padding only lines up in a monospaced font.
        /// </summary>
        private void AppendRow(string key, string description)
        {
            Seek();
            rtbContent.SelectionTabs = new[] { 150 };
            rtbContent.SelectionFont = ThemeFonts.Get(FontRole.BodyBold);
            rtbContent.SelectionColor = ThemeManager.Tokens.TextPrimary;
            rtbContent.AppendText("  " + key + "\t");
            rtbContent.SelectionFont = ThemeFonts.Get(FontRole.Body);
            rtbContent.AppendText(description + "\n");
            ResetRun();
        }
    }
}
