using System;
using System.IO;
using System.Text;

namespace CodeShuttle
{
    /// <summary>
    /// Write-then-rename file writes. The temp file is always created in the *destination*
    /// directory so the final <see cref="File.Move(string,string,bool)"/> stays on one volume and
    /// is therefore atomic; a crash mid-write can never leave a truncated target behind.
    /// </summary>
    public static class AtomicFile
    {
        public static void WriteAllBytes(string path, byte[] bytes)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path is required.", nameof(path));
            ArgumentNullException.ThrowIfNull(bytes);

            var dir = Path.GetDirectoryName(Path.GetFullPath(path));
            if (string.IsNullOrEmpty(dir)) throw new ArgumentException("Path has no directory.", nameof(path));
            Directory.CreateDirectory(dir);

            var temp = Path.Combine(dir, Path.GetRandomFileName() + ".cstmp");
            try
            {
                using (var fs = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush(flushToDisk: true);
                }
                File.Move(temp, path, overwrite: true);
                temp = null!;
            }
            finally
            {
                if (temp != null)
                {
                    try { if (File.Exists(temp)) File.Delete(temp); } catch { /* best effort */ }
                }
            }
        }

        public static void WriteAllText(string path, string text, Encoding encoding)
        {
            ArgumentNullException.ThrowIfNull(encoding);
            var preamble = encoding.GetPreamble();
            var body = encoding.GetBytes(text ?? string.Empty);
            if (preamble.Length == 0)
            {
                WriteAllBytes(path, body);
                return;
            }
            var all = new byte[preamble.Length + body.Length];
            Buffer.BlockCopy(preamble, 0, all, 0, preamble.Length);
            Buffer.BlockCopy(body, 0, all, preamble.Length, body.Length);
            WriteAllBytes(path, all);
        }
    }
}
