using System;
using System.Collections.Generic;
using CodeShuttle.Filters;

namespace CodeShuttle
{
    /// <summary>
    /// The non-interactive half of the secret gate: find credentials in an assembled pack, and
    /// apply the redactions the user chose.
    /// </summary>
    /// <remarks>
    /// Separated from the dialog so that the decision — what was found, and what the content
    /// becomes — is testable without a message loop, and so that the copy path and the export
    /// path share one implementation. Two implementations would eventually disagree, and the way
    /// they would disagree is one of them not running.
    /// </remarks>
    public static class SecretGuard
    {
        /// <summary>
        /// Scans an assembled pack, attributing each match to the file it came from.
        /// </summary>
        /// <remarks>
        /// The pack is parsed first so that a match can name a file and a line within that file.
        /// Scanning the concatenated text instead would report line numbers relative to the whole
        /// pack, which tells the user nothing about where to go and fix it. If the text does not
        /// parse — the user has been editing the output pane by hand — it is scanned whole rather
        /// than skipped, because failing open on a credential check is the wrong direction to
        /// fail.
        /// </remarks>
        public static List<SecretMatch> Scan(string? bundleText)
        {
            var results = new List<SecretMatch>();
            if (string.IsNullOrEmpty(bundleText)) return results;

            List<BundleEntry>? entries = null;
            try { entries = BundleFormat.Parse(bundleText); }
            catch (FormatException) { /* fall through to the whole-text scan */ }

            if (entries is { Count: > 0 })
            {
                foreach (var entry in entries)
                    results.AddRange(SecretScanner.Scan(entry.Content, entry.Path));
                return results;
            }

            results.AddRange(SecretScanner.Scan(bundleText, "(output)"));
            return results;
        }

        /// <summary>Replaces the chosen matches' values with their redaction markers.</summary>
        public static string Redact(string content, IEnumerable<SecretMatch> matches) =>
            SecretScanner.Redact(content, matches);

        /// <summary>
        /// Whether the gate needs to involve the user at all, given the two settings.
        /// </summary>
        /// <remarks>
        /// With warning off but redaction on, the pack is redacted silently — the user has said
        /// they do not want to be asked, not that they want the credential sent. With both off,
        /// the content passes through untouched, which is a choice the settings dialog makes
        /// explicit rather than a default anyone falls into.
        /// </remarks>
        public static SecretGateAction Decide(int matchCount, bool warnOnSecrets, bool redactSecrets)
        {
            if (matchCount == 0) return SecretGateAction.Pass;
            if (warnOnSecrets) return SecretGateAction.Ask;
            return redactSecrets ? SecretGateAction.RedactSilently : SecretGateAction.Pass;
        }
    }

    /// <summary>What the gate should do with a pack that contains detected credentials.</summary>
    public enum SecretGateAction
    {
        /// <summary>Nothing found, or the user has turned both protections off.</summary>
        Pass,

        /// <summary>Show the review dialog.</summary>
        Ask,

        /// <summary>Redact everything found without interrupting.</summary>
        RedactSilently,
    }
}
