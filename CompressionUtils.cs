using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace FileContentToolkit
{
    public static class CompressionUtils
    {
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
            catch (Exception ex)
            {
                text = string.Empty;
                error = ex.Message;
                return false;
            }
        }

        public static string CompressAndEncryptToBase64(string text, string password)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            string base64 = CompressToBase64(text);
            byte[] plain = Encoding.UTF8.GetBytes(base64);
            byte[] salt = RandomBytes(16);
            using var kdf = new Rfc2898DeriveBytes(password ?? "", salt, 100_000, HashAlgorithmName.SHA256);
            byte[] key = kdf.GetBytes(32);
            byte[] nonce = RandomBytes(12);
            byte[] cipher = new byte[plain.Length];
            byte[] tag = new byte[16];
            using var aes = new AesGcm(key);
            aes.Encrypt(nonce, plain, cipher, tag);
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
                int offset = 0;
                byte[] salt = new byte[16]; Buffer.BlockCopy(blob, offset, salt, 0, 16); offset += 16;
                byte[] nonce = new byte[12]; Buffer.BlockCopy(blob, offset, nonce, 0, 12); offset += 12;
                byte[] tag = new byte[16]; Buffer.BlockCopy(blob, offset, tag, 0, 16); offset += 16;
                byte[] cipher = new byte[blob.Length - offset]; Buffer.BlockCopy(blob, offset, cipher, 0, cipher.Length);
                using var kdf = new Rfc2898DeriveBytes(password ?? "", salt, 100_000, HashAlgorithmName.SHA256);
                byte[] key = kdf.GetBytes(32);
                byte[] plain = new byte[cipher.Length];
                using (var aes = new AesGcm(key)) aes.Decrypt(nonce, cipher, tag, plain);
                return TryDecompressFromBase64(Encoding.UTF8.GetString(plain), out text, out error);
            }
            catch (Exception ex)
            {
                text = string.Empty;
                error = ex.Message;
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