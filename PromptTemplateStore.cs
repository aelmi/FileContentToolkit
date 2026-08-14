using System;
using System.Collections.Generic;
using System.Linq;
using CodeShuttle.Settings;

namespace CodeShuttle
{
    /// <summary>
    /// Reads and writes the user's prompt-template library, and renders a template against a
    /// generated pack.
    /// </summary>
    /// <remarks>
    /// The library lives in <see cref="AppSettings.PromptTemplates"/> so it exports and imports
    /// with everything else. An empty collection is re-seeded rather than left empty: settings
    /// files written by every build before this one have no templates at all, and an empty
    /// "Copy as prompt" menu would read as a broken feature rather than an upgrade.
    /// </remarks>
    public static class PromptTemplateStore
    {
        public static IReadOnlyList<PromptTemplate> Load(AppSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);

            if (settings.PromptTemplates.Count == 0)
                settings.PromptTemplates.AddRange(CodeShuttle.PromptTemplates.BuiltIns());

            return settings.PromptTemplates;
        }

        /// <summary>Restores the two built-ins, leaving anything the user added alone.</summary>
        public static void ResetBuiltIns(AppSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);

            settings.PromptTemplates.RemoveAll(t => t.BuiltIn != PromptBuiltIn.None);
            settings.PromptTemplates.InsertRange(0, CodeShuttle.PromptTemplates.BuiltIns());
        }

        /// <summary>
        /// Renders <paramref name="template"/> against the raw output pane text.
        /// </summary>
        /// <remarks>
        /// An unedited built-in goes through the matching <see cref="OutputFormatter"/> method —
        /// the two that were written, correct and called from nowhere. This is the call site
        /// they never had, and the first time either receives the <c>userQuestion</c> argument
        /// its signature has always accepted.
        /// </remarks>
        public static string Render(PromptTemplate template, string rawBundle, string? question)
        {
            ArgumentNullException.ThrowIfNull(template);

            var q = PromptTemplate.NormaliseQuestion(question);

            if (template.UsesBuiltInRenderer)
            {
                return template.BuiltIn switch
                {
                    PromptBuiltIn.Claude => OutputFormatter.ForClaudePrompt(rawBundle, q),
                    PromptBuiltIn.ChatGpt => OutputFormatter.ForChatGptPrompt(rawBundle, q),
                    _ => rawBundle ?? "",
                };
            }

            var files = template.Format switch
            {
                PromptBodyFormat.Markdown => OutputFormatter.ToMarkdown(rawBundle),
                PromptBodyFormat.Xml => OutputFormatter.ToXmlClaude(rawBundle),
                _ => rawBundle ?? "",
            };

            return template.Render(files, q);
        }

        /// <summary>Finds a template by name, case-insensitively.</summary>
        public static PromptTemplate? Find(AppSettings settings, string name) =>
            Load(settings).FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}
