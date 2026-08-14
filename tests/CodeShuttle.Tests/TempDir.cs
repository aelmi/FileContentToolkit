using System;
using System.IO;

namespace CodeShuttle.Tests
{
    /// <summary>A scratch directory that deletes itself at the end of a test.</summary>
    public sealed class TempDir : IDisposable
    {
        public string Path { get; }

        public TempDir()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "codeshuttle-tests", System.IO.Path.GetRandomFileName());
            Directory.CreateDirectory(Path);
        }

        public string File(string relative) => System.IO.Path.Combine(Path, relative);

        public string WriteBytes(string relative, byte[] bytes)
        {
            var full = File(relative);
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(full)!);
            System.IO.File.WriteAllBytes(full, bytes);
            return full;
        }

        public string WriteText(string relative, string text)
            => WriteBytes(relative, System.Text.Encoding.UTF8.GetBytes(text));

        public void Dispose()
        {
            try { if (Directory.Exists(Path)) Directory.Delete(Path, recursive: true); }
            catch { /* a locked file must not fail the test */ }
        }
    }
}
