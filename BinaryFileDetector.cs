using System;
using System.IO;

namespace CodeShuttle.Filters
{
    /// <summary>Why a file could not be treated as readable text.</summary>
    public enum FileReadability
    {
        Text,
        Binary,
        AccessDenied,
        IoError
    }

    public static class BinaryFileDetector
    {
        /// <summary>
        /// Heuristic: read the first N bytes; if any null byte appears, treat as binary.
        /// Cheap, conservative, works well for the text-vs-binary distinction.
        /// </summary>
        public static bool IsBinary(string path, int sampleSize = 8192)
            => Classify(path, sampleSize) != FileReadability.Text;

        /// <summary>
        /// Distinguishes "this is binary" from "I could not read it". Collapsing the two meant an
        /// access-denied or locked file was reported to the user as binary — or not reported at
        /// all — and the export quietly went out incomplete.
        /// </summary>
        public static FileReadability Classify(string path, int sampleSize = 8192)
        {
            try
            {
                using var s = File.OpenRead(path);
                int len = (int)Math.Min(s.Length, sampleSize);
                if (len == 0) return FileReadability.Text;
                Span<byte> buf = stackalloc byte[Math.Min(len, 8192)];
                int read = s.Read(buf);
                for (int i = 0; i < read; i++)
                    if (buf[i] == 0) return FileReadability.Binary;
                return FileReadability.Text;
            }
            catch (UnauthorizedAccessException)
            {
                return FileReadability.AccessDenied;
            }
            catch (IOException)
            {
                return FileReadability.IoError;
            }
            catch
            {
                return FileReadability.IoError;
            }
        }
    }
}
