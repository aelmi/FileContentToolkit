using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace FileContentToolkit
{
    /// <summary>
    /// Converts the toolkit's native "path:\ncontent\n\n\n\n" output into other
    /// commonly-requested formats. All methods are pure; they take the raw output
    /// text and return the converted string.
    /// </summary>
    public static class OutputFormatter
    {
        public static string ToMarkdown(string raw)
        {
            var sb = new StringBuilder();
            foreach (var (path, content) in Parse(raw))
            {
                var lang = GuessLanguage(path);
                sb.Append("### ").AppendLine(path);
                sb.AppendLine();
                sb.Append("```").AppendLine(lang);
                sb.AppendLine(content.TrimEnd());
                sb.AppendLine("```");
                sb.AppendLine();
            }
            return sb.ToString().TrimEnd() + Environment.NewLine;
        }

        public static string ToXmlClaude(string raw)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<documents>");
            foreach (var (path, content) in Parse(raw))
            {
                sb.Append("  <document path=\"").Append(EscapeXml(path)).AppendLine("\">");
                sb.AppendLine(EscapeXml(content.TrimEnd()));
                sb.AppendLine("  </document>");
            }
            sb.AppendLine("</documents>");
            return sb.ToString();
        }

        public static string ToJsonArray(string raw)
        {
            var list = new List<object>();
            foreach (var (path, content) in Parse(raw))
                list.Add(new { path, content });
            var opts = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(list, opts);
        }

        public static string ForClaudePrompt(string raw, string? userQuestion = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("I'm going to share several source files. Please read them carefully.");
            sb.AppendLine();
            sb.Append(ToXmlClaude(raw));
            sb.AppendLine();
            if (!string.IsNullOrWhiteSpace(userQuestion))
                sb.AppendLine(userQuestion);
            else
                sb.AppendLine("What would you like me to do with these files?");
            return sb.ToString();
        }

        public static string ForChatGptPrompt(string raw, string? userQuestion = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Here are the source files I'd like you to consider:");
            sb.AppendLine();
            sb.Append(ToMarkdown(raw));
            sb.AppendLine();
            if (!string.IsNullOrWhiteSpace(userQuestion))
                sb.AppendLine(userQuestion);
            else
                sb.AppendLine("What would you like me to do with these files?");
            return sb.ToString();
        }

        // -------------------- parsing --------------------

        private static IEnumerable<(string Path, string Content)> Parse(string? raw)
        {
            if (string.IsNullOrEmpty(raw)) yield break;
            var lines = raw.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            string? current = null;
            var buf = new StringBuilder();
            foreach (var line in lines)
            {
                if (IsHeader(line))
                {
                    if (current != null)
                        yield return (current, buf.ToString());
                    current = line.TrimEnd(':');
                    buf.Clear();
                }
                else if (current != null)
                {
                    if (buf.Length > 0) buf.Append('\n');
                    buf.Append(line);
                }
            }
            if (current != null)
                yield return (current, buf.ToString());
        }

        private static bool IsHeader(string line)
            => line.EndsWith(":") && line.Length > 2 && (line[1] == ':' || line.StartsWith(".\\"));

        private static string EscapeXml(string s) =>
            s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

        private static string GuessLanguage(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".cs" => "cs",
                ".csproj" or ".xml" or ".config" or ".csproj.user" => "xml",
                ".sln" => "",
                ".razor" or ".cshtml" => "razor",
                ".c" or ".h" => "c",
                ".cpp" or ".cc" or ".cxx" or ".hpp" => "cpp",
                ".html" or ".htm" => "html",
                ".css" => "css",
                ".scss" => "scss",
                ".js" or ".mjs" or ".cjs" => "javascript",
                ".jsx" => "jsx",
                ".ts" => "typescript",
                ".tsx" => "tsx",
                ".json" => "json",
                ".md" => "markdown",
                ".py" or ".pyx" or ".pyi" => "python",
                ".java" => "java",
                ".kt" or ".kts" => "kotlin",
                ".go" => "go",
                ".rs" => "rust",
                ".rb" => "ruby",
                ".php" => "php",
                ".swift" => "swift",
                ".sh" or ".bash" or ".zsh" => "bash",
                ".ps1" => "powershell",
                ".bat" or ".cmd" => "batch",
                ".yaml" or ".yml" => "yaml",
                ".toml" => "toml",
                ".ini" => "ini",
                ".sql" => "sql",
                ".gradle" => "groovy",
                _ => ""
            };
        }
    }
}
