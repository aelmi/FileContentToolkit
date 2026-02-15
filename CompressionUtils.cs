using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace FileContentToolkit
{
    public static class CompressionUtils
    {
        // -------- GZip <-> Base64 --------
        public static string CompressToBase64(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            byte[] raw = Encoding.UTF8.GetBytes(text);
            using var ms = new MemoryStream();
            using (var gzip = new GZipStream(ms, CompressionMode.Compress, leaveOpen: true))
                gzip.Write(raw, 0, raw.Length);
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string DecompressFromBase64(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            using var ms = new MemoryStream(bytes);
            using var gzip = new GZipStream(ms, CompressionMode.Decompress);
            using var sr = new StreamReader(gzip, Encoding.UTF8);
            return sr.ReadToEnd();
        }

        public static bool TryDecompressFromBase64(string base64, out string text, out string error)
        {
            try
            {
                text = DecompressFromBase64(base64);
                error = string.Empty;
                return true;
            }
            catch (FormatException)
            {
                text = string.Empty;
                error = "The text is not valid Base64.";
                return false;
            }
            catch (InvalidDataException)
            {
                text = string.Empty;
                error = "The data is not valid GZip content.";
                return false;
            }
            catch (Exception ex)
            {
                text = string.Empty;
                error = "Unexpected error: " + ex.Message;
                return false;
            }
        }

        // -------- Compress+Encrypt (AES-GCM) --------
        // Output layout (all Base64 as a single string):
        //  [salt(16)] [nonce(12)] [tag(16)] [ciphertext(n)]
        public static string CompressAndEncryptToBase64(string text, string password)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (password == null) password = string.Empty;

            // 1) Compress
            string base64 = CompressToBase64(text); // compressed bytes in base64 for stability
            byte[] plain = Encoding.UTF8.GetBytes(base64);

            // 2) Derive key
            byte[] salt = RandomBytes(16);
            using var kdf = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] key = kdf.GetBytes(32);

            // 3) Encrypt (AES-GCM)
            byte[] nonce = RandomBytes(12);
            byte[] cipher = new byte[plain.Length];
            byte[] tag = new byte[16];
            using var aes = new AesGcm(key);
            aes.Encrypt(nonce, plain, cipher, tag);

            // 4) Pack
            byte[] output = new byte[salt.Length + nonce.Length + tag.Length + cipher.Length];
            Buffer.BlockCopy(salt, 0, output, 0, salt.Length);
            Buffer.BlockCopy(nonce, 0, output, salt.Length, nonce.Length);
            Buffer.BlockCopy(tag, 0, output, salt.Length + nonce.Length, tag.Length);
            Buffer.BlockCopy(cipher, 0, output, salt.Length + nonce.Length + tag.Length, cipher.Length);
            return Convert.ToBase64String(output);
        }

        public static bool TryDecryptAndDecompressFromBase64(string base64, string password, out string text, out string error)
        {
            try
            {
                byte[] blob = Convert.FromBase64String(base64);
                if (blob.Length < 16 + 12 + 16) throw new InvalidDataException("Payload too short.");

                int offset = 0;
                byte[] salt = new byte[16]; Buffer.BlockCopy(blob, offset, salt, 0, salt.Length); offset += salt.Length;
                byte[] nonce = new byte[12]; Buffer.BlockCopy(blob, offset, nonce, 0, nonce.Length); offset += nonce.Length;
                byte[] tag = new byte[16]; Buffer.BlockCopy(blob, offset, tag, 0, tag.Length); offset += tag.Length;
                byte[] cipher = new byte[blob.Length - offset]; Buffer.BlockCopy(blob, offset, cipher, 0, cipher.Length);

                using var kdf = new Rfc2898DeriveBytes(password ?? string.Empty, salt, 100_000, HashAlgorithmName.SHA256);
                byte[] key = kdf.GetBytes(32);

                byte[] plain = new byte[cipher.Length];
                using (var aes = new AesGcm(key))
                {
                    aes.Decrypt(nonce, cipher, tag, plain);
                }

                string compressedBase64 = Encoding.UTF8.GetString(plain);
                if (!TryDecompressFromBase64(compressedBase64, out string decompressed, out string innerErr))
                    throw new InvalidDataException(innerErr);

                text = decompressed;
                error = string.Empty;
                return true;
            }
            catch (CryptographicException)
            {
                text = string.Empty;
                error = "Invalid password or corrupted encrypted data.";
                return false;
            }
            catch (FormatException)
            {
                text = string.Empty;
                error = "The text is not valid Base64.";
                return false;
            }
            catch (Exception ex)
            {
                text = string.Empty;
                error = "Unexpected error: " + ex.Message;
                return false;
            }
        }

        private static byte[] RandomBytes(int n)
        {
            byte[] b = new byte[n];
            RandomNumberGenerator.Fill(b);
            return b;
        }
    }
}