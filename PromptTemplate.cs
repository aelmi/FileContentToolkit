using System;
using System.Collections.Generic;

namespace CodeShuttle
{
    /// <summary>Which representation of the pack a template substitutes for <c>{files}</c>.</summary>
    public enum PromptBodyFormat
    {
        /// <summary>The raw framed bundle, exactly as the output pane holds it.</summary>
        Plain,

        /// <summary>Fenced Markdown code blocks, one per file.</summary>
        Markdown,

        /// <summary>The <c>&lt;documents&gt;</c> form.</summary>
        Xml,
    }

    /// <summary>Identifies the two seeded templates and the formatter method behind each.</summary>
    public enum PromptBuiltIn
    {
        /// <summary>Not a built-in; <see cref="PromptTemplate.Body"/> is authoritative.</summary>
        None = 0,

        /// <summary>Rendered by <c>OutputFormatter.ForClaudePrompt</c>.</summary>
        Claude,

        /// <summary>Rendered by <c>OutputFormatter.ForChatGptPrompt</c>.</summary>
        ChatGpt,
    }

    /// <summary>
    /// A reusable prompt wrapper around a generated pack.
    /// </summary>
    /// <remarks>
    /// <c>OutputFormatter.ForClaudePrompt</c> and <c>ForChatGptPrompt</c> were complete, correct
    /// and called from nowhere, and neither could ever receive the <c>userQuestion</c> argument
    /// they accept. They are now the renderers behind the two seeded templates, so wiring the
    /// dead code and shipping a template library are the same change rather than two parallel
    /// implementations of one prompt.
    ///
    /// A built-in whose <see cref="Body"/> the user has edited stops using the formatter method
    /// and renders generically — editing a built-in makes it a normal template, which is the
    /// only behaviour that does not silently discard the edit.
    /// </remarks>
    public sealed class PromptTemplate
    {
        public const string FilesPlaceholder = "{files}";
        public const string QuestionPlaceholder = "{question}";

        /// <summary>What a caller-supplied question defaults to when the field is left blank.</summary>
        public const string DefaultQuestion = "What would you like me to do with these files?";

        public string Name { get; set; } = "";

        /// <summary>
        /// The template text. May contain <c>{files}</c> and <c>{question}</c>. Empty on an
        /// unedited built-in, which defers to <see cref="BuiltIn"/> instead.
        /// </summary>
        public string Body { get; set; } = "";

        public PromptBodyFormat Format { get; set; } = PromptBodyFormat.Plain;

        /// <summary>Which formatter method renders this template while <see cref="Body"/> is empty.</summary>
        public PromptBuiltIn BuiltIn { get; set; } = PromptBuiltIn.None;

        /// <summary>True while this template still renders through its formatter method.</summary>
        public bool UsesBuiltInRenderer =>
            BuiltIn != PromptBuiltIn.None && string.IsNullOrWhiteSpace(Body);

        /// <summary>Normalises a possibly-blank question to the wording the built-ins also use.</summary>
        public static string NormaliseQuestion(string? question) =>
            string.IsNullOrWhiteSpace(question) ? DefaultQuestion : question.Trim();

        /// <summary>
        /// Substitutes the pack and the user's question into <see cref="Body"/>.
        /// </summary>
        /// <remarks>
        /// A template with no <c>{files}</c> placeholder still gets the pack — appended — rather
        /// than silently producing a prompt with no code in it, which is the one outcome that
        /// wastes a whole round trip.
        /// </remarks>
        public string Render(string formattedFiles, string? question)
        {
            var body = Body ?? "";
            bool hasFiles = body.Contains(FilesPlaceholder, StringComparison.Ordinal);

            var text = body
                .Replace(FilesPlaceholder, formattedFiles ?? "", StringComparison.Ordinal)
                .Replace(QuestionPlaceholder, NormaliseQuestion(question), StringComparison.Ordinal);

            if (!hasFiles)
                text = text.TrimEnd() + Environment.NewLine + Environment.NewLine + (formattedFiles ?? "");

            return text;
        }

        public PromptTemplate Clone() => new()
        {
            Name = Name,
            Body = Body,
            Format = Format,
            BuiltIn = BuiltIn,
        };

        public override string ToString() => Name;
    }

    /// <summary>The two seeded templates, kept out of <see cref="PromptTemplate"/> itself.</summary>
    public static class PromptTemplates
    {
        public const string ClaudeName = "Claude — analyse these files";
        public const string ChatGptName = "ChatGPT — analyse these files";

        public static List<PromptTemplate> BuiltIns() => new()
        {
            new PromptTemplate
            {
                Name = ClaudeName,
                Format = PromptBodyFormat.Xml,
                BuiltIn = PromptBuiltIn.Claude,
                Body = "",
            },
            new PromptTemplate
            {
                Name = ChatGptName,
                Format = PromptBodyFormat.Markdown,
                BuiltIn = PromptBuiltIn.ChatGpt,
                Body = "",
            },
        };
    }
}
