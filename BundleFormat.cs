using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeShuttle
{
    /// <summary>How a text file terminates its lines.</summary>
    public enum EolStyle
    {
        Crlf,
        Lf,
        Cr,

        /// <summary>More than one style in the same file; the exact sequence lives in <see cref="BundleEntry.EolMap"/>.</summary>
        Mixed
    }

    /// <summary>
    /// One file inside a bundle, plus everything needed to write it back byte-for-byte:
    /// its encoding (including whether a BOM was present), its line-ending style, and whether
    /// it ended with a newline.
    /// </summary>
    public sealed class BundleEntry
    {
        /// <summary>Path exactly as it appeared in the bundle header.</summary>
        public string Path { get; init; } = "";

        /// <summary>Content normalised to LF, with no trailing newline (see <see cref="EndsWithNewline"/>).</summary>
        public string Content { get; init; } = "";

        public string EncodingToken { get; init; } = BundleFormat.Utf8NoBom;

        public EolStyle Eol { get; init; } = EolStyle.Crlf;

        /// <summary>One char per line terminator ('C' = CRLF, 'L' = LF, 'R' = CR). Only set when <see cref="Eol"/> is Mixed.</summary>
        public string? EolMap { get; init; }

        public bool EndsWithNewline { get; init; }

        /// <summary>
        /// False for entries recovered from the legacy "path:" format, which carried no encoding
        /// or line-ending information. Callers should then preserve whatever the target file
        /// already uses rather than imposing a default.
        /// </summary>
        public bool HasMetadata { get; init; }
    }

    /// <summary>
    /// The single implementation of the bundle format — read and write, current and legacy.
    ///
    /// The v1 framed format states a line count up front and terminates each entry with a
    /// sentinel that is *verified* rather than searched for. Content therefore cannot forge a
    /// header: the parser never scans inside an entry's declared line span. The legacy reader
    /// (bare "C:\path\file.cs:" headers) is retained so bundles produced by earlier builds, and
    /// anything an AI echoes back in that shape, still parse.
    /// </summary>
    public static class BundleFormat
    {
        public const string Utf8NoBom = "utf-8";
        public const string Utf8Bom = "utf-8-bom";

        private const string BundleHeader = ">>>> CodeShuttle bundle v1";
        private const string FilePrefix = ">>>> file: ";

        /// <summary>
        /// The per-file marker, exposed so the UI can name it when a pack contains none rather
        /// than describing it vaguely and letting the two wordings drift apart.
        /// </summary>
        public const string FileHeaderPrefix = FilePrefix;
        private const string MetaPrefix = ">>>> meta: ";
        private const string EndSentinel = "<<<< end file";

        // -------------------- writing --------------------

        /// <summary>Serialises entries into the v1 framed format.</summary>
        public static string Write(IEnumerable<BundleEntry> entries)
        {
            var sb = new StringBuilder();
            sb.Append(BundleHeader).Append('\n');
            foreach (var e in entries)
            {
                var lines = SplitContent(e.Content);
                sb.Append(FilePrefix).Append(e.Path).Append('\n');
                sb.Append(MetaPrefix)
                  .Append("lines=").Append(lines.Length.ToString(CultureInfo.InvariantCulture))
                  .Append("; encoding=").Append(e.EncodingToken)
                  .Append("; eol=").Append(EolToToken(e.Eol))
                  .Append("; eofNewline=").Append(e.EndsWithNewline ? "true" : "false");
                if (e.Eol == EolStyle.Mixed && !string.IsNullOrEmpty(e.EolMap))
                    sb.Append("; eolMap=").Append(e.EolMap);
                sb.Append('\n');

                foreach (var line in lines)
                    sb.Append(line).Append('\n');

                sb.Append(EndSentinel).Append('\n');
            }
            return sb.ToString();
        }

        // -------------------- reading --------------------

        public static bool IsFramed(string? text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return text.TrimStart('﻿', ' ', '\t', '\r', '\n')
                       .StartsWith(BundleHeader, StringComparison.Ordinal);
        }

        /// <summary>
        /// Parses either format. Framed input is detected by its banner; anything else falls
        /// back to the legacy "path:" reader.
        /// </summary>
        public static List<BundleEntry> Parse(string? text)
            => IsFramed(text) ? ParseFramed(text!) : ParseLegacy(text);

        private static List<BundleEntry> ParseFramed(string text)
        {
            var result = new List<BundleEntry>();
            var lines = SplitAnyNewline(text);
            int i = 0;

            while (i < lines.Length && !lines[i].StartsWith(FilePrefix, StringComparison.Ordinal)) i++;

            while (i < lines.Length)
            {
                if (!lines[i].StartsWith(FilePrefix, StringComparison.Ordinal)) { i++; continue; }

                var path = lines[i].Substring(FilePrefix.Length).Trim();
                i++;
                if (i >= lines.Length || !lines[i].StartsWith(MetaPrefix, StringComparison.Ordinal))
                    throw new FormatException($"Bundle entry '{path}' is missing its meta line.");

                var meta = ParseMeta(lines[i].Substring(MetaPrefix.Length));
                i++;

                int count = 0;
                if (meta.TryGetValue("lines", out var lineCountText))
                    int.TryParse(lineCountText, NumberStyles.Integer, CultureInfo.InvariantCulture, out count);

                if (count < 0 || i + count > lines.Length)
                    throw new FormatException($"Bundle entry '{path}' declares {count} lines but the bundle ends first.");

                var content = count == 0 ? string.Empty : string.Join("\n", lines, i, count);
                i += count;

                if (i >= lines.Length || !lines[i].StartsWith(EndSentinel, StringComparison.Ordinal))
                    throw new FormatException($"Bundle entry '{path}' is not terminated by '{EndSentinel}'.");
                i++;

                result.Add(new BundleEntry
                {
                    Path = path,
                    Content = content,
                    EncodingToken = meta.TryGetValue("encoding", out var enc) && enc.Length > 0 ? enc : Utf8NoBom,
                    Eol = TokenToEol(meta.TryGetValue("eol", out var eol) ? eol : "crlf"),
                    EolMap = meta.TryGetValue("eolMap", out var map) ? map : null,
                    EndsWithNewline = !meta.TryGetValue("eofNewline", out var nl) ||
                                      string.Equals(nl, "true", StringComparison.OrdinalIgnoreCase),
                    HasMetadata = true
                });
            }

            return result;
        }

        /// <summary>
        /// The pre-v1 reader: a header is a line ending in ':' that either has a drive letter in
        /// position 1 or starts with ".\". Content is everything until the next header, with the
        /// blank-line separators the old writer emitted between files trimmed off.
        /// </summary>
        private static List<BundleEntry> ParseLegacy(string? text)
        {
            var result = new List<BundleEntry>();
            if (string.IsNullOrEmpty(text)) return result;

            var lines = SplitAnyNewline(text);
            string? current = null;
            var buf = new List<string>();

            void Flush()
            {
                if (current == null) return;
                int end = buf.Count;
                while (end > 0 && buf[end - 1].Length == 0) end--;
                result.Add(new BundleEntry
                {
                    Path = current,
                    Content = string.Join("\n", buf.Take(end)),
                    EndsWithNewline = true,
                    HasMetadata = false
                });
                buf.Clear();
            }

            foreach (var line in lines)
            {
                if (IsLegacyHeader(line))
                {
                    Flush();
                    current = line.TrimEnd(':');
                }
                else if (current != null)
                {
                    buf.Add(line);
                }
            }
            Flush();
            return result;
        }

        private static bool IsLegacyHeader(string line)
            => line.EndsWith(':')
               && line.Length > 2
               && (line[1] == ':' || line.StartsWith(".\\", StringComparison.Ordinal));

        private static Dictionary<string, string> ParseMeta(string meta)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var part in meta.Split(';'))
            {
                var kv = part.Split('=', 2);
                if (kv.Length != 2) continue;
                map[kv[0].Trim()] = kv[1].Trim();
            }
            return map;
        }

        // -------------------- file analysis and reconstruction --------------------

        /// <summary>Reads a file and captures everything needed to reproduce it byte-for-byte.</summary>
        public static BundleEntry FromFile(string path, string bundlePath)
            => FromBytes(File.ReadAllBytes(path), bundlePath);

        public static BundleEntry FromBytes(byte[] bytes, string bundlePath)
        {
            var token = DetectEncodingToken(bytes);
            var encoding = ResolveEncoding(token);
            var preamble = encoding.GetPreamble();
            int offset = 0;
            if (preamble.Length > 0 && bytes.Length >= preamble.Length &&
                bytes.Take(preamble.Length).SequenceEqual(preamble))
            {
                offset = preamble.Length;
            }

            var decoded = encoding.GetString(bytes, offset, bytes.Length - offset);
            var (content, eol, eolMap, endsWithNewline) = AnalyzeText(decoded);

            return new BundleEntry
            {
                Path = bundlePath,
                Content = content,
                EncodingToken = token,
                Eol = eol,
                EolMap = eolMap,
                EndsWithNewline = endsWithNewline,
                HasMetadata = true
            };
        }

        /// <summary>Splits decoded text into LF-normalised content plus its line-ending shape.</summary>
        public static (string Content, EolStyle Eol, string? EolMap, bool EndsWithNewline) AnalyzeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return (string.Empty, EolStyle.Lf, null, false);

            var sb = new StringBuilder(text.Length);
            var map = new StringBuilder();
            int i = 0;
            while (i < text.Length)
            {
                char c = text[i];
                if (c == '\r')
                {
                    if (i + 1 < text.Length && text[i + 1] == '\n') { map.Append('C'); i += 2; }
                    else { map.Append('R'); i++; }
                    sb.Append('\n');
                }
                else if (c == '\n')
                {
                    map.Append('L');
                    sb.Append('\n');
                    i++;
                }
                else
                {
                    sb.Append(c);
                    i++;
                }
            }

            var terminators = map.ToString();
            bool endsWithNewline = sb.Length > 0 && sb[sb.Length - 1] == '\n';
            if (endsWithNewline) sb.Length -= 1;

            EolStyle style;
            string? eolMap = null;
            if (terminators.Length == 0)
            {
                style = EolStyle.Lf;
            }
            else if (terminators.All(t => t == 'C')) style = EolStyle.Crlf;
            else if (terminators.All(t => t == 'L')) style = EolStyle.Lf;
            else if (terminators.All(t => t == 'R')) style = EolStyle.Cr;
            else { style = EolStyle.Mixed; eolMap = terminators; }

            return (sb.ToString(), style, eolMap, endsWithNewline);
        }

        /// <summary>Rebuilds the on-disk bytes for an entry, honouring encoding, BOM, EOL and trailing newline.</summary>
        public static byte[] Render(BundleEntry entry)
            => Render(entry.Content, entry.EncodingToken, entry.Eol, entry.EolMap, entry.EndsWithNewline);

        public static byte[] Render(string lfContent, string encodingToken, EolStyle eol, string? eolMap, bool endsWithNewline)
        {
            var encoding = ResolveEncoding(encodingToken);
            var text = ApplyEol(lfContent ?? string.Empty, eol, eolMap, endsWithNewline);
            var preamble = encoding.GetPreamble();
            var body = encoding.GetBytes(text);
            if (preamble.Length == 0) return body;

            var all = new byte[preamble.Length + body.Length];
            Buffer.BlockCopy(preamble, 0, all, 0, preamble.Length);
            Buffer.BlockCopy(body, 0, all, preamble.Length, body.Length);
            return all;
        }

        private static string ApplyEol(string lfContent, EolStyle eol, string? eolMap, bool endsWithNewline)
        {
            var withTerminator = endsWithNewline ? lfContent + "\n" : lfContent;
            if (withTerminator.Length == 0) return string.Empty;

            if (eol == EolStyle.Mixed && !string.IsNullOrEmpty(eolMap))
            {
                var sb = new StringBuilder(withTerminator.Length + eolMap.Length);
                int t = 0;
                foreach (var c in withTerminator)
                {
                    if (c != '\n') { sb.Append(c); continue; }
                    char kind = t < eolMap.Length ? eolMap[t] : 'L';
                    t++;
                    sb.Append(kind switch { 'C' => "\r\n", 'R' => "\r", _ => "\n" });
                }
                return sb.ToString();
            }

            return eol switch
            {
                EolStyle.Crlf => withTerminator.Replace("\n", "\r\n"),
                EolStyle.Cr => withTerminator.Replace("\n", "\r"),
                _ => withTerminator
            };
        }

        // -------------------- encodings --------------------

        public static string DetectEncodingToken(byte[] bytes)
        {
            if (bytes.Length >= 4 && bytes[0] == 0xFF && bytes[1] == 0xFE && bytes[2] == 0x00 && bytes[3] == 0x00)
                return "utf-32le-bom";
            if (bytes.Length >= 4 && bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0xFE && bytes[3] == 0xFF)
                return "utf-32be-bom";
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                return Utf8Bom;
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                return "utf-16le-bom";
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
                return "utf-16be-bom";

            return LooksLikeUtf8(bytes) ? Utf8NoBom : "iso-8859-1";
        }

        public static Encoding ResolveEncoding(string? token) => (token ?? Utf8NoBom).ToLowerInvariant() switch
        {
            Utf8Bom => new UTF8Encoding(true),
            "utf-16le" => new UnicodeEncoding(false, false),
            "utf-16le-bom" => new UnicodeEncoding(false, true),
            "utf-16be" => new UnicodeEncoding(true, false),
            "utf-16be-bom" => new UnicodeEncoding(true, true),
            "utf-32le" => new UTF32Encoding(false, false),
            "utf-32le-bom" => new UTF32Encoding(false, true),
            "utf-32be" => new UTF32Encoding(true, false),
            "utf-32be-bom" => new UTF32Encoding(true, true),
            "ascii" => Encoding.ASCII,
            "iso-8859-1" or "latin-1" => Encoding.Latin1,
            _ => new UTF8Encoding(false)
        };

        /// <summary>Maps a live <see cref="Encoding"/> back onto a bundle token.</summary>
        public static string TokenFor(Encoding encoding)
        {
            if (encoding == null) return Utf8NoBom;
            bool bom = encoding.GetPreamble().Length > 0;
            return encoding switch
            {
                UTF32Encoding u32 => (u32.GetPreamble().Length > 0, IsBigEndian(u32)) switch
                {
                    (true, true) => "utf-32be-bom",
                    (true, false) => "utf-32le-bom",
                    (false, true) => "utf-32be",
                    _ => "utf-32le"
                },
                UnicodeEncoding u16 => (bom, IsBigEndian(u16)) switch
                {
                    (true, true) => "utf-16be-bom",
                    (true, false) => "utf-16le-bom",
                    (false, true) => "utf-16be",
                    _ => "utf-16le"
                },
                UTF8Encoding => bom ? Utf8Bom : Utf8NoBom,
                _ when encoding.CodePage == Encoding.ASCII.CodePage => "ascii",
                _ when encoding.CodePage == 28591 => "iso-8859-1",
                _ => Utf8NoBom
            };
        }

        private static bool IsBigEndian(Encoding encoding)
        {
            // The preamble distinguishes endianness for BOM'd encodings; for BOM-less ones we
            // probe by encoding a known character.
            var probe = encoding.GetBytes("A");
            return probe.Length >= 2 && probe[0] == 0x00;
        }

        private static bool LooksLikeUtf8(byte[] buf)
        {
            int n = Math.Min(buf.Length, 65536);
            int i = 0;
            while (i < n)
            {
                byte b = buf[i];
                if (b <= 0x7F) { i++; continue; }
                int extra;
                if ((b & 0xE0) == 0xC0) extra = 1;
                else if ((b & 0xF0) == 0xE0) extra = 2;
                else if ((b & 0xF8) == 0xF0) extra = 3;
                else return false;
                if (i + extra >= n) return true; // truncated sequence at the sample boundary
                for (int j = 1; j <= extra; j++)
                    if ((buf[i + j] & 0xC0) != 0x80) return false;
                i += extra + 1;
            }
            return true;
        }

        // -------------------- helpers --------------------

        private static string[] SplitAnyNewline(string text)
            => text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        private static string[] SplitContent(string content)
            => content.Length == 0 ? Array.Empty<string>() : content.Split('\n');

        private static string EolToToken(EolStyle eol) => eol switch
        {
            EolStyle.Crlf => "crlf",
            EolStyle.Cr => "cr",
            EolStyle.Mixed => "mixed",
            _ => "lf"
        };

        private static EolStyle TokenToEol(string? token) => (token ?? "").ToLowerInvariant() switch
        {
            "crlf" => EolStyle.Crlf,
            "cr" => EolStyle.Cr,
            "mixed" => EolStyle.Mixed,
            _ => EolStyle.Lf
        };

        /// <summary>Content comparison that ignores line-ending style and trailing blank lines.</summary>
        public static bool ContentEquals(string? a, string? b)
            => string.Equals(Normalise(a), Normalise(b), StringComparison.Ordinal);

        public static string Normalise(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace("\r\n", "\n").Replace('\r', '\n').TrimEnd('\n');
        }
    }
}
