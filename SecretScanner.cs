using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeShuttle.Filters
{
    /// <summary>What kind of credential a <see cref="SecretMatch"/> represents.</summary>
    public enum SecretKind
    {
        AwsAccessKeyId,
        PrivateKey,
        ApiKeyAssignment,
        ConnectionStringPassword,
        JsonWebToken,
        HighEntropyEnvValue
    }

    /// <summary>A single suspected credential found in scanned content.</summary>
    public sealed class SecretMatch
    {
        public string Path { get; init; } = "";

        /// <summary>1-based line number within the scanned content.</summary>
        public int Line { get; init; }

        public SecretKind Kind { get; init; }

        /// <summary>The matched value. Held in memory only — never write this to a log or to disk.</summary>
        public string Value { get; init; } = "";

        /// <summary>What the value is replaced with when redaction is applied.</summary>
        public string Replacement => $"[REDACTED: {Kind}]";

        /// <summary>A safe-to-display fragment: first four characters and a length, nothing more.</summary>
        public string Preview =>
            Value.Length <= 4 ? new string('•', Value.Length)
                              : string.Concat(Value.AsSpan(0, 4), new string('•', Math.Min(12, Value.Length - 4)));

        public override string ToString() => $"{Kind} at {Path}:{Line}";
    }

    /// <summary>
    /// Finds credentials in content that is about to be handed to a third-party AI service.
    /// Deliberately biased toward recall: a false positive costs the user one click, a false
    /// negative leaks a production key into a chat transcript.
    ///
    /// WS2 delivers detection only. The warning dialog and the redaction toggle are WS5.
    /// </summary>
    public static class SecretScanner
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(250);

        private static readonly Regex AwsAccessKey =
            new(@"\b(?:AKIA|ASIA|ABIA|ACCA)[0-9A-Z]{16}\b", RegexOptions.CultureInvariant, Timeout);

        private static readonly Regex PemPrivateKey =
            new(@"-----BEGIN (?:RSA |DSA |EC |OPENSSH |PGP |ENCRYPTED )?PRIVATE KEY-----",
                RegexOptions.CultureInvariant, Timeout);

        private static readonly Regex JwtToken =
            new(@"\beyJ[A-Za-z0-9_-]{8,}\.[A-Za-z0-9_-]{8,}\.[A-Za-z0-9_-]{8,}\b",
                RegexOptions.CultureInvariant, Timeout);

        // key = "value" / key: value / KEY=value, for names that read like a credential.
        private static readonly Regex ApiKeyAssignment =
            new(@"(?i)\b(?<name>[A-Za-z0-9_.-]*(?:api[_-]?key|secret|token|passwd|password|access[_-]?key|private[_-]?key|client[_-]?secret)[A-Za-z0-9_.-]*)\s*[:=]\s*(?<q>[""']?)(?<value>[^\s""',;]{8,})\k<q>",
                RegexOptions.CultureInvariant, Timeout);

        private static readonly Regex ConnectionStringPassword =
            new(@"(?i)\b(?:password|pwd)\s*=\s*(?<value>[^;""'\s]{4,})\s*;",
                RegexOptions.CultureInvariant, Timeout);

        // Bare KEY=value, the .env shape. Only reported when the value looks random.
        private static readonly Regex EnvAssignment =
            new(@"^\s*(?:export\s+)?(?<name>[A-Z][A-Z0-9_]{2,})\s*=\s*(?<q>[""']?)(?<value>[^\s""']{16,})\k<q>\s*$",
                RegexOptions.CultureInvariant, Timeout);

        /// <summary>Values at or above this Shannon entropy (bits/char) are treated as random.</summary>
        public const double EntropyThreshold = 3.6;

        /// <summary>Scans content and returns every suspected credential, ordered by line.</summary>
        public static List<SecretMatch> Scan(string? content, string path)
        {
            var results = new List<SecretMatch>();
            if (string.IsNullOrEmpty(content)) return results;

            var lines = content.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.Length == 0) continue;
                int lineNumber = i + 1;

                try
                {
                    foreach (Match m in AwsAccessKey.Matches(line))
                        Add(results, path, lineNumber, SecretKind.AwsAccessKeyId, m.Value);

                    if (PemPrivateKey.IsMatch(line))
                        Add(results, path, lineNumber, SecretKind.PrivateKey, line.Trim());

                    foreach (Match m in JwtToken.Matches(line))
                        Add(results, path, lineNumber, SecretKind.JsonWebToken, m.Value);

                    foreach (Match m in ConnectionStringPassword.Matches(line))
                        Add(results, path, lineNumber, SecretKind.ConnectionStringPassword, m.Groups["value"].Value);

                    foreach (Match m in ApiKeyAssignment.Matches(line))
                    {
                        var value = m.Groups["value"].Value;
                        if (LooksLikePlaceholder(value)) continue;
                        Add(results, path, lineNumber, SecretKind.ApiKeyAssignment, value);
                    }

                    var env = EnvAssignment.Match(line);
                    if (env.Success)
                    {
                        var value = env.Groups["value"].Value;
                        if (!LooksLikePlaceholder(value) && ShannonEntropy(value) >= EntropyThreshold)
                            Add(results, path, lineNumber, SecretKind.HighEntropyEnvValue, value);
                    }
                }
                catch (RegexMatchTimeoutException)
                {
                    // A pathological line is not worth stalling the scan for; move on.
                }
            }

            return results;
        }

        private static void Add(List<SecretMatch> results, string path, int line, SecretKind kind, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            if (results.Any(r => r.Line == line && r.Kind == kind && r.Value == value)) return;
            results.Add(new SecretMatch { Path = path, Line = line, Kind = kind, Value = value });
        }

        private static readonly string[] Placeholders =
        {
            "changeme", "your_", "yourkey", "example", "placeholder", "xxxxx", "todo",
            "<your", "insert", "dummy", "sample", "redacted", "null", "none", "false", "true"
        };

        private static bool LooksLikePlaceholder(string value)
        {
            var lower = value.ToLowerInvariant();
            if (Placeholders.Any(p => lower.Contains(p, StringComparison.Ordinal))) return true;
            // ${VAR}, $(VAR), %VAR%, {{VAR}} — indirection, not a literal secret.
            return lower.StartsWith("${", StringComparison.Ordinal)
                || lower.StartsWith("$(", StringComparison.Ordinal)
                || lower.StartsWith("{{", StringComparison.Ordinal)
                || (lower.StartsWith('%') && lower.EndsWith('%'));
        }

        /// <summary>Shannon entropy in bits per character.</summary>
        public static double ShannonEntropy(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            var counts = new Dictionary<char, int>();
            foreach (var c in value)
                counts[c] = counts.TryGetValue(c, out var n) ? n + 1 : 1;

            double entropy = 0;
            foreach (var count in counts.Values)
            {
                double p = (double)count / value.Length;
                entropy -= p * Math.Log2(p);
            }
            return entropy;
        }

        /// <summary>Applies every supplied match to the content, replacing values with their redaction marker.</summary>
        public static string Redact(string content, IEnumerable<SecretMatch> matches)
        {
            if (string.IsNullOrEmpty(content)) return content ?? string.Empty;
            var result = content;
            foreach (var m in matches.OrderByDescending(m => m.Value.Length))
            {
                if (string.IsNullOrEmpty(m.Value)) continue;
                result = result.Replace(m.Value, m.Replacement, StringComparison.Ordinal);
            }
            return result;
        }
    }
}
