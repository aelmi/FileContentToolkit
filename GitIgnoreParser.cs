using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FileContentToolkit.Filters
{
    // Minimal gitignore / dockerignore implementation. Covers comments, blank lines,
    // negation (!), trailing-slash directory-only patterns, leading-slash anchoring,
    // and the *, **, ? wildcards. Not 100% spec-compliant but enough for the common cases.
    public class GitIgnoreParser
    {
        private readonly List<Rule> _rules = new();

        public bool HasRules => _rules.Count > 0;

        public static GitIgnoreParser FromFolder(string folder)
        {
            var parser = new GitIgnoreParser();
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) return parser;
            foreach (var name in new[] { ".gitignore", ".dockerignore" })
            {
                var p = Path.Combine(folder, name);
                if (File.Exists(p))
                {
                    try { parser.AddRules(File.ReadAllLines(p)); } catch { /* ignore */ }
                }
            }
            return parser;
        }

        public void AddRules(IEnumerable<string> lines)
        {
            foreach (var raw in lines)
            {
                if (raw == null) continue;
                var line = raw.TrimEnd();
                if (line.Length == 0) continue;
                if (line.StartsWith("#")) continue;

                bool negate = false;
                if (line.StartsWith("!"))
                {
                    negate = true;
                    line = line.Substring(1);
                }

                bool dirOnly = line.EndsWith("/");
                if (dirOnly) line = line.TrimEnd('/');

                bool anchored = line.StartsWith("/");
                if (anchored) line = line.TrimStart('/');

                if (line.Length == 0) continue;

                var pattern = GlobToRegex(line, anchored);
                _rules.Add(new Rule(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled), negate, dirOnly));
            }
        }

        // Returns true if relativePath (forward slashes) is ignored.
        public bool IsIgnored(string relativePath, bool isDirectory = false)
        {
            if (_rules.Count == 0) return false;
            var path = relativePath.Replace('\\', '/').TrimStart('/');
            bool ignored = false;
            foreach (var rule in _rules)
            {
                if (rule.DirOnly && !isDirectory && !PathLooksLikeDirMatch(path, rule.Regex))
                {
                    // dir-only rule: only matches if some directory segment matches
                    continue;
                }
                if (rule.Regex.IsMatch(path))
                    ignored = !rule.Negate;
            }
            return ignored;
        }

        private static bool PathLooksLikeDirMatch(string path, Regex regex)
        {
            // Allow dir-only rule to match if any ancestor path matches.
            var parts = path.Split('/');
            var sb = new StringBuilder();
            for (int i = 0; i < parts.Length - 1; i++) // up to second-to-last (ancestor dirs)
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
