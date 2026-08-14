using System;
using System.Text;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// Highest-priority suite: encryption with a user password means any defect here is silent,
    /// permanent data loss, and this is the component the format change lands on.
    /// </summary>
    public class CompressionUtilsTests
    {
        [Fact]
        public void CompressDecompress_RoundTripsAscii()
        {
            const string original = "hello world\nsecond line\n";
            var compressed = CompressionUtils.CompressToBase64(original);
            Assert.Equal(original, CompressionUtils.DecompressFromBase64(compressed));
        }

        [Fact]
        public void CompressDecompress_RoundTripsUnicode()
        {
            const string original = "日本語 · Ελληνικά · Кириллица · emoji 🚀 · accents éàü";
            var compressed = CompressionUtils.CompressToBase64(original);
            Assert.Equal(original, CompressionUtils.DecompressFromBase64(compressed));
        }

        [Fact]
        public void CompressDecompress_RoundTripsLargeInput()
        {
            var original = string.Concat(new string('x', 1000), string.Join("\n", System.Linq.Enumerable.Range(0, 20000)));
            var compressed = CompressionUtils.CompressToBase64(original);
            Assert.Equal(original, CompressionUtils.DecompressFromBase64(compressed));
        }

        [Fact]
        public void CompressToBase64_EmptyInputReturnsEmpty()
        {
            Assert.Equal(string.Empty, CompressionUtils.CompressToBase64(""));
        }

        [Fact]
        public void TryDecompress_CorruptBase64FailsCleanly()
        {
            Assert.False(CompressionUtils.TryDecompressFromBase64("not valid base64 !!", out var text, out var error));
            Assert.Equal(string.Empty, text);
            Assert.NotEmpty(error);
        }

        [Fact]
        public void TryDecompress_WellFormedBase64ThatIsNotGzipFailsCleanly()
        {
            var notGzip = Convert.ToBase64String(Encoding.UTF8.GetBytes("this is plain text, not a gzip stream"));

            Assert.False(CompressionUtils.TryDecompressFromBase64(notGzip, out var text, out var error));
            Assert.Equal(string.Empty, text);
            Assert.NotEmpty(error);
        }

        [Fact]
        public void TryDecompress_CorruptedGzipPayloadFailsCleanly()
        {
            var bytes = Convert.FromBase64String(CompressionUtils.CompressToBase64("a reasonable amount of text to compress"));
            for (int i = 10; i < bytes.Length - 4; i++) bytes[i] ^= 0x5A; // wreck the deflate stream

            Assert.False(CompressionUtils.TryDecompressFromBase64(Convert.ToBase64String(bytes), out _, out var error));
            Assert.NotEmpty(error);
        }

        /// <summary>
        /// P0-4: a gzip bomb must produce a clean, explanatory failure rather than consuming all
        /// available memory. The budget is a parameter precisely so this is testable in
        /// milliseconds instead of by actually allocating 512 MB.
        /// </summary>
        [Fact]
        public void TryDecompress_ExceedingOutputBudgetFailsCleanlyNotWithOom()
        {
            // ~4 MB of highly compressible data behind a 64 KB budget.
            var bomb = CompressionUtils.CompressToBase64(new string('A', 4 * 1024 * 1024));

            var ok = CompressionUtils.TryDecompressFromBase64(bomb, 64 * 1024, out var text, out var error);

            Assert.False(ok);
            Assert.Equal(string.Empty, text);
            Assert.Contains("safety limit", error, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Decompress_OverInputLengthCapIsRejectedBeforeAllocating()
        {
            var oversized = new string('A', CompressionUtils.MaxInputBase64Chars + 4);
            var ex = Assert.Throws<System.IO.InvalidDataException>(() => CompressionUtils.DecompressFromBase64(oversized));
            Assert.Contains("too large", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void EncryptDecrypt_RoundTrips()
        {
            const string original = "secret content\nwith 日本語 and a trailing newline\n";
            var sealedText = CompressionUtils.CompressAndEncryptToBase64(original, "correct horse battery");

            Assert.True(CompressionUtils.TryDecryptAndDecompressFromBase64(
                sealedText, "correct horse battery", out var text, out var error));
            Assert.Equal(original, text);
            Assert.Equal(string.Empty, error);
        }

        [Fact]
        public void Decrypt_WrongPasswordFailsCleanlyWithoutThrowing()
        {
            var sealedText = CompressionUtils.CompressAndEncryptToBase64("payload", "the right password");

            var ok = CompressionUtils.TryDecryptAndDecompressFromBase64(
                sealedText, "the wrong password", out var text, out var error);

            Assert.False(ok);
            Assert.Equal(string.Empty, text);
            Assert.NotEmpty(error);
        }

        [Fact]
        public void Decrypt_EmptyPasswordPathIsSupported()
        {
            // CompressAndEncrypt coalesces a null password to "", so that path must round-trip.
            var sealedText = CompressionUtils.CompressAndEncryptToBase64("payload", "");
            Assert.True(CompressionUtils.TryDecryptAndDecompressFromBase64(sealedText, "", out var text, out _));
            Assert.Equal("payload", text);
        }

        [Fact]
        public void Decrypt_TamperedCiphertextFailsTheAuthenticationTag()
        {
            var sealedText = CompressionUtils.CompressAndEncryptToBase64("payload worth protecting", "pw12345678");
            var bytes = Convert.FromBase64String(sealedText);
            bytes[bytes.Length - 1] ^= 0xFF; // flip a bit in the ciphertext
            var tampered = Convert.ToBase64String(bytes);

            Assert.False(CompressionUtils.TryDecryptAndDecompressFromBase64(tampered, "pw12345678", out _, out var error));
            Assert.NotEmpty(error);
        }

        [Fact]
        public void Decrypt_TruncatedBlobFailsCleanly()
        {
            var sealedText = CompressionUtils.CompressAndEncryptToBase64("payload", "pw12345678");
            var bytes = Convert.FromBase64String(sealedText);
            var truncated = Convert.ToBase64String(bytes, 0, 8);

            Assert.False(CompressionUtils.TryDecryptAndDecompressFromBase64(truncated, "pw12345678", out _, out var error));
            Assert.NotEmpty(error);
        }

        /// <summary>
        /// The KDF cost is now a stored field so it can be raised without breaking existing
        /// blobs. Pinning it here means a change to the default is a deliberate act, not a
        /// silent one that strands every blob produced before it.
        /// </summary>
        [Fact]
        public void EncryptedBlob_StoresTheKdfIterationCountInItsHeader()
        {
            var bytes = Convert.FromBase64String(CompressionUtils.CompressAndEncryptToBase64("x", "pw12345678"));

            Assert.Equal((byte)'C', bytes[0]);
            Assert.Equal((byte)'S', bytes[1]);
            Assert.Equal((byte)'H', bytes[2]);
            Assert.Equal((byte)'T', bytes[3]);
            Assert.Equal(1, bytes[4]); // format version

            var iterations = BitConverter.ToInt32(bytes, 6);
            Assert.Equal(CompressionUtils.Pbkdf2Iterations, iterations);
            Assert.Equal(100_000, iterations);
        }

        /// <summary>
        /// Blobs produced before the header existed had no magic bytes and a fixed 100k cost.
        /// They must keep decrypting, or the format change destroys data on upgrade.
        /// </summary>
        [Fact]
        public void Decrypt_LegacyHeaderlessBlobStillDecrypts()
        {
            var legacy = LegacyEncrypt("legacy payload\nsecond line", "pw12345678");

            Assert.True(CompressionUtils.TryDecryptAndDecompressFromBase64(legacy, "pw12345678", out var text, out var error));
            Assert.Equal("legacy payload\nsecond line", text);
            Assert.Equal(string.Empty, error);
        }

        /// <summary>Reproduces the pre-header on-disk layout: salt | nonce | tag | ciphertext.</summary>
        private static string LegacyEncrypt(string text, string password)
        {
            var base64 = CompressionUtils.CompressToBase64(text);
            var plain = Encoding.UTF8.GetBytes(base64);
            var salt = new byte[16];
            var nonce = new byte[12];
            System.Security.Cryptography.RandomNumberGenerator.Fill(salt);
            System.Security.Cryptography.RandomNumberGenerator.Fill(nonce);

            using var kdf = new System.Security.Cryptography.Rfc2898DeriveBytes(
                password, salt, 100_000, System.Security.Cryptography.HashAlgorithmName.SHA256);
            var key = kdf.GetBytes(32);
            var cipher = new byte[plain.Length];
            var tag = new byte[16];
            using (var aes = new System.Security.Cryptography.AesGcm(key, 16))
                aes.Encrypt(nonce, plain, cipher, tag);

            var output = new byte[salt.Length + nonce.Length + tag.Length + cipher.Length];
            Buffer.BlockCopy(salt, 0, output, 0, salt.Length);
            Buffer.BlockCopy(nonce, 0, output, salt.Length, nonce.Length);
            Buffer.BlockCopy(tag, 0, output, salt.Length + nonce.Length, tag.Length);
            Buffer.BlockCopy(cipher, 0, output, salt.Length + nonce.Length + tag.Length, cipher.Length);
            return Convert.ToBase64String(output);
        }
    }
}
