using System;
using System.Linq;
using System.Text;
using CodeShuttle.Filters;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// These two gate the correctness of the primary output: misdetecting an encoding corrupts
    /// every byte downstream, and misclassifying a file quietly drops it from the export.
    /// </summary>
    public class EncodingDetectorTests
    {
        private static readonly Encoding Fallback = Encoding.Latin1;

        [Fact]
        public void DetectsUtf8WithBom()
        {
            using var temp = new TempDir();
            var enc = new UTF8Encoding(true);
            var path = temp.WriteBytes("a.txt", enc.GetPreamble().Concat(enc.GetBytes("héllo")).ToArray());

            var detected = EncodingDetector.Detect(path, Fallback);

            Assert.Equal(Encoding.UTF8.CodePage, detected.CodePage);
            Assert.NotEmpty(detected.GetPreamble());
        }

        [Fact]
        public void DetectsUtf8WithoutBom()
        {
            using var temp = new TempDir();
            var path = temp.WriteBytes("a.txt", new UTF8Encoding(false).GetBytes("héllo wörld"));

            var detected = EncodingDetector.Detect(path, Fallback);

            Assert.Equal(Encoding.UTF8.CodePage, detected.CodePage);
            Assert.Empty(detected.GetPreamble());
        }

        [Fact]
        public void DetectsUtf16LittleEndian()
        {
            using var temp = new TempDir();
            var enc = new UnicodeEncoding(false, true);
            var path = temp.WriteBytes("a.txt", enc.GetPreamble().Concat(enc.GetBytes("hello")).ToArray());

            Assert.Equal(Encoding.Unicode.CodePage, EncodingDetector.Detect(path, Fallback).CodePage);
        }

        [Fact]
        public void DetectsUtf16BigEndian()
        {
            using var temp = new TempDir();
            var enc = new UnicodeEncoding(true, true);
            var path = temp.WriteBytes("a.txt", enc.GetPreamble().Concat(enc.GetBytes("hello")).ToArray());

            Assert.Equal(Encoding.BigEndianUnicode.CodePage, EncodingDetector.Detect(path, Fallback).CodePage);
        }

        [Fact]
        public void PlainAsciiIsTreatedAsUtf8()
        {
            using var temp = new TempDir();
            var path = temp.WriteBytes("a.txt", Encoding.ASCII.GetBytes("plain ascii content"));

            Assert.Equal(Encoding.UTF8.CodePage, EncodingDetector.Detect(path, Fallback).CodePage);
        }

        [Fact]
        public void InvalidUtf8FallsBackToTheCallersChoice()
        {
            using var temp = new TempDir();
            // 0xFF 0xFE would be a BOM, so use bytes that are simply not valid UTF-8 lead bytes.
            var path = temp.WriteBytes("a.txt", new byte[] { 0x41, 0xC3, 0x28, 0x42 });

            Assert.Equal(Fallback.CodePage, EncodingDetector.Detect(path, Fallback).CodePage);
        }

        /// <summary>The classic crash: a zero-byte file.</summary>
        [Fact]
        public void ZeroByteFileDoesNotThrow()
        {
            using var temp = new TempDir();
            var path = temp.WriteBytes("empty.txt", Array.Empty<byte>());

            var detected = EncodingDetector.Detect(path, Fallback);

            Assert.NotNull(detected);
            Assert.False(BinaryFileDetector.IsBinary(path));
        }

        [Fact]
        public void MissingFileFallsBackRatherThanThrowing()
        {
            using var temp = new TempDir();
            Assert.Equal(Fallback.CodePage, EncodingDetector.Detect(temp.File("nope.txt"), Fallback).CodePage);
        }

        [Fact]
        public void AllZeroBufferIsClassifiedBinary()
        {
            using var temp = new TempDir();
            var path = temp.WriteBytes("blob.bin", new byte[512]);

            Assert.True(BinaryFileDetector.IsBinary(path));
            Assert.Equal(FileReadability.Binary, BinaryFileDetector.Classify(path));
        }

        [Fact]
        public void TextFileIsClassifiedText()
        {
            using var temp = new TempDir();
            var path = temp.WriteText("a.cs", "class A { }\n");

            Assert.False(BinaryFileDetector.IsBinary(path));
            Assert.Equal(FileReadability.Text, BinaryFileDetector.Classify(path));
        }

        /// <summary>
        /// P1-19: an unreadable file used to be reported as "binary", so the user was told a
        /// perfectly good source file had been skipped for the wrong reason — or not told at all.
        /// </summary>
        [Fact]
        public void UnreadableFileIsDistinguishedFromBinary()
        {
            using var temp = new TempDir();
            var classification = BinaryFileDetector.Classify(temp.File("does-not-exist.cs"));

            Assert.NotEqual(FileReadability.Binary, classification);
            Assert.Equal(FileReadability.IoError, classification);
        }

        /// <summary>
        /// P1-12: reading a UTF-8 file as ASCII used to replace every non-ASCII byte with '?',
        /// and that corrupted text was written back over the user's real files.
        /// </summary>
        [Fact]
        public void StrictDecodingThrowsInsteadOfSilentlyReplacingBadBytes()
        {
            using var temp = new TempDir();
            var path = temp.WriteBytes("a.txt", new UTF8Encoding(false).GetBytes("héllo"));

            var service = new FileContentService { AutoDetectEncoding = false };

            Assert.Throws<DecoderFallbackException>(() => service.ReadFileText(path, Encoding.ASCII));
        }
    }
}
