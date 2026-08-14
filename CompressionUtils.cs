using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeShuttle
{
    public static class CompressionUtils
    {
        /// <summary>
        /// Hard ceiling on decompressed output. GZip reaches roughly 1000:1, so a 2 MB payload can
        /// expand to ~2 GB; without a cap that is a hang followed by an OutOfMemoryException.
        /// </summary>
        public const long DefaultMaxOutputBytes = 512L * 1024 * 1024;

        /// <summary>Largest base64 input accepted at all, so a pathological paste is refused up front.</summary>
        public const int MaxInputBase64Chars = 256 * 1024 * 1024;

        /// <summary>Magic prefix identifying the versioned CodeShuttle encrypted format.</summary>
        private static readonly byte[] Magic = { (byte)'C', (byte)'S', (byte)'H', (byte)'T' };

        private const byte FormatVersion = 1;
        private const byte Flags = 0;

        /// <summary>
        /// PBKDF2 iteration count for newly-produced blobs. It is written into the header as a
        /// stored field, so raising it later does not break anything already encrypted.
        /// </summary>
        public const int Pbkdf2Iterations = 100_000;

        private const int SaltLength = 16;
        private const int NonceLength = 12;
        private const int TagLength = 16;
        private const int HeaderLength = 4 + 1 + 1 + 4; // magic + version + flags + iterations

        // -------------------- compression --------------------

        public static string CompressToBase64(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            byte[] raw = Encoding.UTF8.GetBytes(text);
            using var ms = new MemoryStream();
            using (var gzip = new GZipStream(ms, CompressionMode.Compress, leaveOpen: true))
                gzip.Write(raw, 0, raw.Length);
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decompresses with a hard output budget. Exceeding the budget raises
        /// <see cref="InvalidDataException"/> with a message the UI can show, rather than
        /// exhausting memory.
        /// </summary>
        public static string DecompressFromBase64(string base64, long maxOutputBytes = DefaultMaxOutputBytes)
        {
            if (string.IsNullOrEmpty(base64)) return string.Empty;
            if (base64.Length > MaxInputBase64Chars)
                throw new InvalidDataException(
                    $"Input is too large to decompress ({base64.Length:N0} characters; the limit is {MaxInputBase64Chars:N0}).");

            byte[] bytes = Convert.FromBase64String(base64);
            return DecompressBytes(bytes, maxOutputBytes);
        }

        private static string DecompressBytes(byte[] bytes, long maxOutputBytes)
        {
            using var ms = new MemoryStream(bytes);
            using var gzip = new GZipStream(ms, CompressionMode.Decompress);
            using var output = new MemoryStream();

            var buffer = new byte[81920];
            long total = 0;
            int read;
            while ((read = gzip.Read(buffer, 0, buffer.Length)) > 0)
            {
                total += read;
                if (total > maxOutputBytes)
                    throw new InvalidDataException(
                        $"Decompressed content exceeds the {maxOutputBytes / (1024 * 1024):N0} MB safety limit. " +
                        "The input may be a decompression bomb, or simply too large for this tool.");
                output.Write(buffer, 0, read);
            }

            return Encoding.UTF8.GetString(output.GetBuffer(), 0, (int)output.Length);
        }

        public static bool TryDecompressFromBase64(string base64, out string text, out string error)
            => TryDecompressFromBase64(base64, DefaultMaxOutputBytes, out text, out error);

        public static bool TryDecompressFromBase64(string base64, long maxOutputBytes, out string text, out string error)
        {
            try
            {
                text = DecompressFromBase64(base64, maxOutputBytes);
                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                text = string.Empty;
                error = ex.Message;
                return false;
            }
        }

        /// <summary>Base64 of the gzip header (1f 8b 08 00) that every compressed blob starts with.</summary>
        private const string GzipBase64Prefix = "H4sI";

        /// <summary>Base64 of the "CSHT" magic that every encrypted blob starts with.</summary>
        private const string EncryptedBase64Prefix = "Q1NIV";

        /// <summary>
        /// True when the text is one of this tool's own compressed blobs rather than a bundle.
        /// </summary>
        /// <remarks>
        /// Purely so the UI can say "decompress this first" instead of the parse failing with a
        /// count of zero entries and no explanation — which is indistinguishable, from the user's
        /// side, from the tool being broken.
        /// </remarks>
        public static bool LooksLikeCompressedBase64(string? text) =>
            HasPrefix(text, GzipBase64Prefix);

        /// <summary>True when the text is one of this tool's own encrypted blobs.</summary>
        public static bool LooksLikeEncryptedBase64(string? text) =>
            HasPrefix(text, EncryptedBase64Prefix);

        private static bool HasPrefix(string? text, string prefix) =>
            text is not null && text.TrimStart().StartsWith(prefix, StringComparison.Ordinal);

        // -------------------- encryption --------------------

        /// <summary>
        /// GZip, base64, then AES-GCM. The output carries a versioned header recording the format
        /// version and the KDF iteration count, so the parameters can be raised later without
        /// stranding existing blobs.
        /// </summary>
        public static string CompressAndEncryptToBase64(string text, string password)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            string base64 = CompressToBase64(text);
            byte[] plain = Encoding.UTF8.GetBytes(base64);
            byte[] salt = RandomBytes(SaltLength);
            using var kdf = new Rfc2898DeriveBytes(password ?? "", salt, Pbkdf2Iterations, HashAlgorithmName.SHA256);
            byte[] key = kdf.GetBytes(32);
            byte[] nonce = RandomBytes(NonceLength);
            byte[] cipher = new byte[plain.Length];
            byte[] tag = new byte[TagLength];
            using var aes = new AesGcm(key, AesGcm.TagByteSizes.MaxSize);
            aes.Encrypt(nonce, plain, cipher, tag);

            byte[] output = new byte[HeaderLength + salt.Length + nonce.Length + tag.Length + cipher.Length];
            int offset = 0;
            Buffer.BlockCopy(Magic, 0, output, offset, Magic.Length); offset += Magic.Length;
            output[offset++] = FormatVersion;
            output[offset++] = Flags;
            BitConverter.TryWriteBytes(output.AsSpan(offset, 4), Pbkdf2Iterations);
            if (!BitConverter.IsLittleEndian) Array.Reverse(output, offset, 4);
            offset += 4;
            Buffer.BlockCopy(salt, 0, output, offset, salt.Length); offset += salt.Length;
            Buffer.BlockCopy(nonce, 0, output, offset, nonce.Length); offset += nonce.Length;
            Buffer.BlockCopy(tag, 0, output, offset, tag.Length); offset += tag.Length;
            Buffer.BlockCopy(cipher, 0, output, offset, cipher.Length);
            return Convert.ToBase64String(output);
        }

        public static bool TryDecryptAndDecompressFromBase64(string base64, string password, out string text, out string error)
            => TryDecryptAndDecompressFromBase64(base64, password, DefaultMaxOutputBytes, out text, out error);

        public static bool TryDecryptAndDecompressFromBase64(
            string base64, string password, long maxOutputBytes, out string text, out string error)
        {
            text = string.Empty;
            error = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(base64))
                {
                    error = "There is nothing to decrypt.";
                    return false;
                }
                if (base64.Length > MaxInputBase64Chars)
                {
                    error = $"Input is too large to decrypt ({base64.Length:N0} characters).";
                    return false;
                }

                byte[] blob = Convert.FromBase64String(base64);

                int iterations = Pbkdf2Iterations;
                int offset = 0;
                if (HasMagic(blob))
                {
                    if (blob.Length < HeaderLength + SaltLength + NonceLength + TagLength)
                    {
                        error = "The encrypted data is truncated.";
                        return false;
                    }
                    offset = Magic.Length;
                    byte version = blob[offset++];
                    offset++; // flags, reserved
                    if (version != FormatVersion)
                    {
                        error = $"This blob uses format version {version}, which this build of CodeShuttle does not understand.";
                        return false;
                    }
                    var iterationBytes = new byte[4];
                    Buffer.BlockCopy(blob, offset, iterationBytes, 0, 4);
                    if (!BitConverter.IsLittleEndian) Array.Reverse(iterationBytes);
                    iterations = BitConverter.ToInt32(iterationBytes, 0);
                    offset += 4;

                    if (iterations < 1 || iterations > 10_000_000)
                    {
                        error = "The encrypted data declares an implausible key-derivation cost and was rejected.";
                        return false;
                    }
                }
                else if (blob.Length < SaltLength + NonceLength + TagLength)
                {
                    error = "The encrypted data is truncated.";
                    return false;
                }

                byte[] salt = new byte[SaltLength]; Buffer.BlockCopy(blob, offset, salt, 0, SaltLength); offset += SaltLength;
                byte[] nonce = new byte[NonceLength]; Buffer.BlockCopy(blob, offset, nonce, 0, NonceLength); offset += NonceLength;
                byte[] tag = new byte[TagLength]; Buffer.BlockCopy(blob, offset, tag, 0, TagLength); offset += TagLength;
                byte[] cipher = new byte[blob.Length - offset];
                Buffer.BlockCopy(blob, offset, cipher, 0, cipher.Length);

                using var kdf = new Rfc2898DeriveBytes(password ?? "", salt, iterations, HashAlgorithmName.SHA256);
                byte[] key = kdf.GetBytes(32);
                byte[] plain = new byte[cipher.Length];
                using (var aes = new AesGcm(key, AesGcm.TagByteSizes.MaxSize))
                    aes.Decrypt(nonce, cipher, tag, plain);

                return TryDecompressFromBase64(Encoding.UTF8.GetString(plain), maxOutputBytes, out text, out error);
            }
            catch (AuthenticationTagMismatchException)
            {
                error = "Wrong password, or the data has been altered since it was encrypted.";
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static bool HasMagic(byte[] blob)
        {
            if (blob.Length < Magic.Length) return false;
            for (int i = 0; i < Magic.Length; i++)
                if (blob[i] != Magic[i]) return false;
            return true;
        }

        // -------------------- async entry points (keep the UI thread free) --------------------

        public static Task<string> CompressToBase64Async(string text, CancellationToken ct = default)
            => Task.Run(() => CompressToBase64(text), ct);

        public static Task<string> CompressAndEncryptToBase64Async(string text, string password, CancellationToken ct = default)
            => Task.Run(() => CompressAndEncryptToBase64(text, password), ct);

        public static Task<(bool Ok, string Text, string Error)> TryDecompressAsync(
            string base64, CancellationToken ct = default)
            => Task.Run(() =>
            {
                bool ok = TryDecompressFromBase64(base64, out var text, out var error);
                return (ok, text, error);
            }, ct);

        public static Task<(bool Ok, string Text, string Error)> TryDecryptAndDecompressAsync(
            string base64, string password, CancellationToken ct = default)
            => Task.Run(() =>
            {
                bool ok = TryDecryptAndDecompressFromBase64(base64, password, out var text, out var error);
                return (ok, text, error);
            }, ct);

        private static byte[] RandomBytes(int n)
        {
            byte[] b = new byte[n];
            RandomNumberGenerator.Fill(b);
            return b;
        }
    }
}
