using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeShuttle
{
    /// <summary>How a pack sits against the selected model's context window.</summary>
    public enum BudgetLevel
    {
        /// <summary>Comfortably inside the window.</summary>
        Ok,

        /// <summary>Past the warning threshold but still nominally fitting.</summary>
        Near,

        /// <summary>Larger than the window; the paste will be truncated or refused.</summary>
        Over,
    }

    /// <summary>A target model and the context window to measure a pack against.</summary>
    public sealed class TokenModel
    {
        public string Id { get; init; } = "";

        /// <summary>What the model dropdown shows.</summary>
        public string Display { get; init; } = "";

        /// <summary>Context window in tokens. Zero means "unbounded", used by the custom entry only.</summary>
        public int ContextTokens { get; init; }

        public override string ToString() => Display;
    }

    /// <summary>
    /// The token-budget model: which context window the user is aiming at, and whether the
    /// assembled pack fits it.
    /// </summary>
    /// <remarks>
    /// The token count itself is <see cref="TokenEstimator"/>'s characters-per-token
    /// approximation, deliberately not a per-model BPE tokenizer — shipping four real
    /// tokenizers would mean four native dependencies for a number whose only job is to answer
    /// "roughly, will this fit". Every surface that shows the number is required to label it as
    /// an estimate; <see cref="EstimateCaveat"/> is that label, in one place.
    /// </remarks>
    public static class TokenBudget
    {
        /// <summary>The wording every UI surface uses so the number is never mistaken for exact.</summary>
        public const string EstimateCaveat =
            "Estimated from character count, not a model tokenizer — treat it as approximate.";

        /// <summary>Fraction of the window at which the gauge turns amber.</summary>
        public const double NearThreshold = 0.80;

        public const string CustomModelId = "custom";

        public static readonly TokenModel Claude =
            new() { Id = "claude", Display = "Claude (200k)", ContextTokens = 200_000 };

        public static readonly TokenModel Gpt =
            new() { Id = "gpt", Display = "GPT (128k)", ContextTokens = 128_000 };

        public static readonly TokenModel Gemini =
            new() { Id = "gemini", Display = "Gemini (1M)", ContextTokens = 1_000_000 };

        public static readonly TokenModel Custom =
            new() { Id = CustomModelId, Display = "Custom…", ContextTokens = 0 };

        public static IReadOnlyList<TokenModel> All { get; } = new[] { Claude, Gpt, Gemini, Custom };

        /// <summary>Resolves a persisted model id, falling back to Claude for an unknown value.</summary>
        public static TokenModel Resolve(string? id) =>
            All.FirstOrDefault(m => string.Equals(m.Id, id, StringComparison.OrdinalIgnoreCase)) ?? Claude;

        /// <summary>
        /// The window to measure against: the model's own, or the user's custom figure when the
        /// custom entry is selected.
        /// </summary>
        public static int WindowFor(TokenModel model, int customTokens) =>
            model.ContextTokens > 0 ? model.ContextTokens : Math.Max(0, customTokens);

        public static BudgetLevel Classify(int tokens, int windowTokens)
        {
            // A window of zero means the user has not told us what to measure against, so there
            // is nothing to be over.
            if (windowTokens <= 0) return BudgetLevel.Ok;
            if (tokens > windowTokens) return BudgetLevel.Over;
            return tokens >= windowTokens * NearThreshold ? BudgetLevel.Near : BudgetLevel.Ok;
        }

        /// <summary>Percentage of the window used, clamped to 0..100 for a progress control.</summary>
        public static int PercentOf(int tokens, int windowTokens)
        {
            if (windowTokens <= 0) return 0;
            var pct = (int)Math.Round(tokens * 100.0 / windowTokens);
            return Math.Min(100, Math.Max(0, pct));
        }

        public static string Describe(int tokens, int windowTokens)
        {
            if (windowTokens <= 0) return $"~{tokens:N0} tokens";
            return $"~{tokens:N0} / {windowTokens:N0} tokens ({PercentOf(tokens, windowTokens)}%)";
        }

        /// <summary>One file's share of the pack, for the breakdown view.</summary>
        public sealed record FileTokens(string Path, int Tokens);

        /// <summary>
        /// Ranks the pack's entries by estimated size, largest first. Used both for the
        /// breakdown list and for the "remove these to fit" suggestion.
        /// </summary>
        public static List<FileTokens> Breakdown(string? bundleText)
        {
            var result = new List<FileTokens>();
            if (string.IsNullOrEmpty(bundleText)) return result;

            List<BundleEntry> entries;
            try { entries = BundleFormat.Parse(bundleText); }
            catch (FormatException) { return result; }

            foreach (var entry in entries)
                result.Add(new FileTokens(entry.Path, TokenEstimator.Estimate(entry.Content)));

            result.Sort((a, b) => b.Tokens.CompareTo(a.Tokens));
            return result;
        }

        /// <summary>
        /// The fewest largest files that would need to come out to get under the window.
        /// Returns an empty list when the pack already fits, so callers can treat "nothing to
        /// suggest" and "already fine" identically.
        /// </summary>
        public static List<FileTokens> SuggestTrim(IReadOnlyList<FileTokens> breakdown, int totalTokens, int windowTokens)
        {
            var suggestion = new List<FileTokens>();
            if (windowTokens <= 0 || totalTokens <= windowTokens) return suggestion;

            int remaining = totalTokens;
            foreach (var file in breakdown)
            {
                suggestion.Add(file);
                remaining -= file.Tokens;
                if (remaining <= windowTokens) break;
            }
            return suggestion;
        }
    }
}
