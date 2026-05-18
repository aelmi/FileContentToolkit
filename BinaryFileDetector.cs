using System;
using System.IO;

namespace FileContentToolkit.Filters
{
    public static class BinaryFileDetector
    {
        // Heuristic: read the first N bytes; if any null byte appears, treat as binary.
        // Cheap, conservative, works well for the text-vs-binary distinction.
        public static bool IsBinary(string path, int sampleSize = 8192)
        {
            try
            {
                using var s = File.OpenRead(path);
                int len = (int)Math.Min(s.Length, sampleSize);
                if (len == 0) return false;
                Span<byte> buf = stackalloc byte[Math.Min(len, 8192)];
                int read = s.Read(buf);
                for (int i = 0; i < read; i++)
                    if (buf[i] == 0) return true;
                return false;
            }
            catch
            {
                // If we can't read it we treat it as binary (so it's skipped).
                return true;
            }
        }
    }
}
