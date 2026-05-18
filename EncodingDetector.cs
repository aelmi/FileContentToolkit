using System;
using System.IO;
using System.Text;

namespace FileContentToolkit.Filters
{
    public static class EncodingDetector
    {
        // BOM-first detection; if no BOM, validate as UTF-8; else fall back to the caller's choice.
        public static Encoding Detect(string path, Encoding fallback)
        {
            try
            {
                using var s = File.OpenRead(path);
                Span<byte> bom = stackalloc byte[4];
                int read = s.Read(bom);

                if (read >= 3 && bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                    return new UTF8Encoding(true);
                if (read >= 4 && bom[0] == 0xFF && bom[1] == 0xFE && bom[2] == 0x00 && bom[3] == 0x00)
                    return new UTF32Encoding(false, true);
                if (read >= 4 && bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == 0xFE && bom[3] == 0xFF)
                    return new UTF32Encoding(true, true);
                if (read >= 2 && bom[0] == 0xFF && bom[1] == 0xFE)
                    return Encoding.Unicode;
                if (read >= 2 && bom[0] == 0xFE && bom[1] == 0xFF)
                    return Encoding.BigEndianUnicode;

                // No BOM. Try a UTF-8 validation on a sample; if it passes, use UTF-8 without BOM.
                s.Position = 0;
                if (LooksLikeUtf8(s)) return new UTF8Encoding(false);
                return fallback;
            }
            catch
            {
                return fallback;
            }
        }

        private static bool LooksLikeUtf8(Stream s, int sampleSize = 4096)
        {
            var buf = new byte[Math.Min(sampleSize, (int)Math.Min(s.Length - s.Position, sampleSize))];
            int n = s.Read(buf, 0, buf.Length);
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
                if (i + extra >= n) return true; // partial multi-byte at end of sample: don't penalise
                for (int j = 1; j <= extra; j++)
                    if ((buf[i + j] & 0xC0) != 0x80) return false;
                i += extra + 1;
            }
            return true;
        }
    }
}
