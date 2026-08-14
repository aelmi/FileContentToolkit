using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeShuttle.Filters
{
    // Minimal gitignore / dockerignore implementation. Covers comments, blank lines,
    // negation (!), trailing-slash directory-only patterns, anchoring, and the *, **, ?
    // wildcards. Not 100% spec-compliant — nested ignore files and negation-under-an-excluded
    // parent are deliberately out of scope — but correct for the common cases.
    public class GitIgnoreParser
    {
        private static readonly TimeSpan MatchTimeout = TimeSpan.FromMilliseconds(200);

        private readonly List<Rule> _rules = new();

        /// <summary>
        /// Git is case-sensitive by default. Matching case-insensitively meant a stray "*.MD"
        /// silently excluded every .md file in the repository.
        /// </summary>
        public bool IgnoreCase { get; }

        public GitIgnoreParser(bool ignoreCase = false)
        {
            IgnoreCase = ignoreCase;
        }

        public bool HasRules => _rules.Count > 0;

        public int RuleCount => _rules.Count;

        /// <summary>
        /// Loads .gitignore only. .dockerignore is deliberately NOT merged in here: idiomatic
        /// dockerignore files start with "*" and re-include with "!", which under a single merged
        /// rule list excluded every file in an otherwise normal repository.
        /// </summary>
        public static GitIgnoreParser FromFolder(string folder, bool ignoreCase = false)
            => FromFile(folder, ".gitignore", ignoreCase);

        public static GitIgnoreParser FromDockerIgnore(string folder, bool ignoreCase = false)
            => FromFile(folder, ".dockerignore", ignoreCase);

        private static GitIgnoreParser FromFile(string folder, string fileName, bool ignoreCase)
        {
            var parser = new GitIgnoreParser(ignoreCase);
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) return parser;
            var path = Path.Combine(folder, fileName);
            if (!File.Exists(path)) return parser;
            try { parser.AddRules(File.ReadAllLines(path)); } catch { /* unreadable: no rules */ }
            return parser;
        }

        // -------------------- caching --------------------
        // Re-parsing and recompiling every rule on each scan meant a full reparse per keystroke
        // under the typing debounce, and a continuous reparse storm while watching a folder
        // during a build. The cache is keyed on the ignore file's identity and timestamp, so an
        // edit to .gitignore still takes effect immediately.

        private static readonly ConcurrentDictionary<string, (long Ticks, long Length, GitIgnoreParser Parser)> Cache = new();

        public static GitIgnoreParser FromFolderCached(string folder, bool ignoreCase = false)
            => FromFileCached(folder, ".gitignore", ignoreCase, static (f, ic) => FromFile(f, ".gitignore", ic));

        public static GitIgnoreParser FromDockerIgnoreCached(string folder, bool ignoreCase = false)
            => FromFileCached(folder, ".dockerignore", ignoreCase, static (f, ic) => FromFile(f, ".dockerignore", ic));

        private static GitIgnoreParser FromFileCached(
            string folder, string fileName, bool ignoreCase, Func<string, bool, GitIgnoreParser> factory)
        {
            if (string.IsNullOrEmpty(folder)) return new GitIgnoreParser(ignoreCase);

            var path = Path.Combine(folder, fileName);
            long ticks = 0, length = 0;
            try
            {
                var info = new FileInfo(path);
                if (info.Exists) { ticks = info.LastWriteTimeUtc.Ticks; length = info.Length; }
            }
            catch { /* fall through to a fresh parse */ }

            var key = (ignoreCase ? "i:" : "s:") + path.ToLowerInvariant();
            if (Cache.TryGetValue(key, out var cached) && cached.Ticks == ticks && cached.Length == length)
                return cached.Parser;

            var parser = factory(folder, ignoreCase);
            Cache[key] = (ticks, length, parser);
            return parser;
        }

        /// <summary>Drops every cached parser. Exposed for tests and for a forced rescan.</summary>
        public static void ClearCache() => Cache.Clear();

        // -------------------- rules --------------------

        public void AddRules(IEnumerable<string> lines)
        {
            foreach (var raw in lines)
            {
                if (raw == null) continue;
                var line = raw.TrimEnd();
                if (line.Length == 0) continue;
                if (line.StartsWith('#')) continue;

                bool negate = false;
                if (line.StartsWith('!'))
                {
                    negate = true;
                    line = line.Substring(1);
                }

                bool dirOnly = line.EndsWith('/');
                if (dirOnly) line = line.TrimEnd('/');

                // Git anchors a pattern to the root if it contains a '/' anywhere other than at
                // the very end. Treating only a LEADING slash as anchoring made "doc/frotz"
                // wrongly match "a/b/doc/frotz".
                bool anchored = line.Contains('/', StringComparison.Ordinal);
                if (line.StartsWith('/')) line = line.TrimStart('/');

                if (line.Length == 0) continue;

                var options = RegexOptions.CultureInvariant;
                if (IgnoreCase) options |= RegexOptions.IgnoreCase;

                // RegexOptions.Compiled was ~200 JIT compilations per scan for no measurable win
                // on patterns this small; it is deliberately absent.
                _rules.Add(new Rule(new Regex(GlobToRegex(line, anchored), options, MatchTimeout), negate, dirOnly));
            }
        }

        // Returns true if relativePath (forward slashes) is ignored.
        public bool IsIgnored(string relativePath, bool isDirectory = false)
        {
            if (_rules.Count == 0) return false;
            var path = relativePath.Replace('\\', '/').TrimStart('/');
            bool ignored = false;
            try
            {
                foreach (var rule in _rules)
                {
                    if (rule.DirOnly && !isDirectory && !PathLooksLikeDirMatch(path, rule.Regex))
                        continue;
                    if (rule.Regex.IsMatch(path))
                        ignored = !rule.Negate;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            return ignored;
        }

        private static bool PathLooksLikeDirMatch(string path, Regex regex)
        {
            // A dir-only rule matches a file if any ancestor directory matches.
            var parts = path.Split('/');
            var sb = new StringBuilder();
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (i > 0) sb.Append('/');
                sb.Append(parts[i]);
                if (regex.IsMatch(sb.ToString())) return true;
            }
            return false;
        }

        private static string GlobToRegex(string glob, bool anchored)
        {
            var sb = new StringBuilder();
            sb.Append(anchored ? "^" : "(^|.*/)");
            for (int i = 0; i < glob.Length; i++)
            {
                char c = glob[i];
                switch (c)
                {
                    case '*':
                        if (i + 1 < glob.Length && glob[i + 1] == '*')
                        {
                            // ** = any number of directories
                            sb.Append(".*");
                            i++;
                            if (i + 1 < glob.Length && glob[i + 1] == '/') i++; // consume trailing /
                        }
                        else
                        {
                            sb.Append("[^/]*");
                        }
                        break;
                    case '?':
                        sb.Append("[^/]");
                        break;
                    case '.':
                    case '+':
                    case '(':
                    case ')':
                    case '|':
                    case '^':
                    case '$':
                    case '{':
                    case '}':
                    case '[':
                    case ']':
                    case '\\':
                        sb.Append('\\').Append(c);
                        break;
                    case '/':
                        sb.Append('/');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            sb.Append("($|/)");
            return sb.ToString();
        }

        private sealed class Rule
        {
            public Regex Regex { get; }
            public bool Negate { get; }
            public bool DirOnly { get; }
            public Rule(Regex r, bool n, bool d) { Regex = r; Negate = n; DirOnly = d; }
        }
    }
}
