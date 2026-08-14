using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace CodeShuttle.Tests
{
    public class BundleFormatTests
    {
        [Fact]
        public void FramedRoundTripPreservesPathsAndContent()
        {
            var entries = new[]
            {
                new BundleEntry { Path = @"src\a.cs", Content = "line1\nline2", EndsWithNewline = true, HasMetadata = true },
                new BundleEntry { Path = @"src\b.cs", Content = "", EndsWithNewline = false, HasMetadata = true }
            };

            var parsed = BundleFormat.Parse(BundleFormat.Write(entries));

            Assert.Equal(2, parsed.Count);
            Assert.Equal(@"src\a.cs", parsed[0].Path);
            Assert.Equal("line1\nline2", parsed[0].Content);
            Assert.Equal(@"src\b.cs", parsed[1].Path);
            Assert.Equal("", parsed[1].Content);
        }

        /// <summary>
        /// P1-1: under the old parser, ANY line ending in ':' whose second character was ':'
        /// started a new file — so a source line mentioning a path split the file in two and
        /// wrote the tail to a fabricated location. The framed format states a line count, so
        /// content is never scanned for delimiters.
        /// </summary>
        [Fact]
        public void ContentThatWouldForgeAHeaderUnderTheOldParserIsSafe()
        {
            const string hostile = "int x = 1;\nSee C:\\build\\output\\log.txt:\nint y = 2;";
            var entry = new BundleEntry { Path = @"src\a.cs", Content = hostile, HasMetadata = true, EndsWithNewline = true };

            var parsed = BundleFormat.Parse(BundleFormat.Write(new[] { entry }));

            Assert.Single(parsed);
            Assert.Equal(hostile, parsed[0].Content);
        }

        [Fact]
        public void ContentContainingTheEndSentinelIsStillOneEntry()
        {
            const string tricky = "before\n<<<< end file\n>>>> file: fake.cs\nafter";
            var entry = new BundleEntry { Path = "real.cs", Content = tricky, HasMetadata = true };

            var parsed = BundleFormat.Parse(BundleFormat.Write(new[] { entry }));

            Assert.Single(parsed);
            Assert.Equal("real.cs", parsed[0].Path);
            Assert.Equal(tricky, parsed[0].Content);
        }

        /// <summary>
        /// WS2 acceptance criterion 3: LF endings, no trailing newline, UTF-16LE with a BOM —
        /// through the bundle and back out as BYTE-IDENTICAL content. This is the whole of P0-3
        /// in one assertion.
        /// </summary>
        [Fact]
        public void Utf16LeWithBomLfNoTrailingNewline_RoundTripsByteIdentical()
        {
            using var temp = new TempDir();

            var encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true);
            var text = "first line\nsecond line\nthird line"; // LF, no trailing newline
            var original = encoding.GetPreamble().Concat(encoding.GetBytes(text)).ToArray();
            var path = temp.WriteBytes("sample.reg", original);

            var entry = BundleFormat.FromFile(path, "sample.reg");

            Assert.Equal("utf-16le-bom", entry.EncodingToken);
            Assert.Equal(EolStyle.Lf, entry.Eol);
            Assert.False(entry.EndsWithNewline);

            // Through the serialised bundle, not just the in-memory object.
            var reparsed = BundleFormat.Parse(BundleFormat.Write(new[] { entry })).Single();
            var rendered = BundleFormat.Render(reparsed);

            Assert.Equal(original, rendered);
        }

        [Theory]
        [InlineData("a\r\nb\r\n", "utf-8")]
        [InlineData("a\nb\n", "utf-8")]
        [InlineData("a\rb\r", "utf-8")]
        [InlineData("a\r\nb\nc", "utf-8")]
        [InlineData("no newline at all", "utf-8")]
        [InlineData("", "utf-8")]
        public void EveryLineEndingShapeRoundTripsByteIdentical(string text, string encodingToken)
        {
            var encoding = BundleFormat.ResolveEncoding(encodingToken);
            var original = encoding.GetBytes(text);

            var entry = BundleFormat.FromBytes(original, "x.txt");
            var reparsed = BundleFormat.Parse(BundleFormat.Write(new[] { entry })).Single();

            Assert.Equal(original, BundleFormat.Render(reparsed));
        }

        [Fact]
        public void Utf8BomIsPreserved()
        {
            var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
            var original = encoding.GetPreamble().Concat(encoding.GetBytes("hello\r\n")).ToArray();

            var entry = BundleFormat.FromBytes(original, "x.txt");

            Assert.Equal("utf-8-bom", entry.EncodingToken);
            Assert.Equal(original, BundleFormat.Render(entry));
        }

        [Fact]
        public void Utf8WithoutBomStaysWithoutBom()
        {
            var original = new UTF8Encoding(false).GetBytes("hello\n");
            var entry = BundleFormat.FromBytes(original, "x.txt");

            Assert.Equal("utf-8", entry.EncodingToken);
            Assert.Equal(original, BundleFormat.Render(entry));
        }

        [Fact]
        public void LegacyHeaderFormatStillParses()
        {
            var legacy = "C:\\proj\\a.cs:\nclass A { }\n\n\n\nC:\\proj\\b.cs:\nclass B { }";

            var parsed = BundleFormat.Parse(legacy);

            Assert.Equal(2, parsed.Count);
            Assert.Equal(@"C:\proj\a.cs", parsed[0].Path);
            Assert.Equal("class A { }", parsed[0].Content);
            Assert.Equal("class B { }", parsed[1].Content);
            Assert.All(parsed, e => Assert.False(e.HasMetadata));
        }

        [Fact]
        public void LegacyDotRelativeHeadersParse()
        {
            var parsed = BundleFormat.Parse(".\\src\\a.cs:\ncontent here");

            Assert.Single(parsed);
            Assert.Equal(@".\src\a.cs", parsed[0].Path);
        }

        [Fact]
        public void IsFramed_DistinguishesTheTwoFormats()
        {
            Assert.True(BundleFormat.IsFramed(BundleFormat.Write(new[] { new BundleEntry { Path = "a", Content = "b" } })));
            Assert.False(BundleFormat.IsFramed("C:\\x\\a.cs:\ncontent"));
            Assert.False(BundleFormat.IsFramed(""));
            Assert.False(BundleFormat.IsFramed(null));
        }

        [Fact]
        public void ATruncatedFramedBundleThrowsFormatExceptionRatherThanSilentlyLosingContent()
        {
            var full = BundleFormat.Write(new[] { new BundleEntry { Path = "a.cs", Content = "1\n2\n3\n4\n5" } });
            var truncated = string.Join("\n", full.Split('\n').Take(4));

            Assert.Throws<FormatException>(() => BundleFormat.Parse(truncated));
        }

        [Fact]
        public void ContentEquals_IgnoresLineEndingStyleAndTrailingNewlines()
        {
            Assert.True(BundleFormat.ContentEquals("a\r\nb", "a\nb"));
            Assert.True(BundleFormat.ContentEquals("a\nb\n\n", "a\nb"));
            Assert.False(BundleFormat.ContentEquals("a\nb", "a\nc"));
        }
    }
}
