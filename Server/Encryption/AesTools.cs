using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Server.Encryption
{
    public static class AesEncrypter
    {
        public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;
        public static int KeySize { get; }

        /// <summary>
        /// Random Key
        /// </summary>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public static byte[] GenerateRandomKey(int keySize = 256)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = keySize;
                aes.GenerateKey();
                return aes.Key;
            }
        }

        /// <summary>
        /// Generate RandomIV
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateRandomIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return aes.IV;
            }
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="plainText">Text</param>
        /// <param name="key">Aes key</param>
        /// <param name="iv">Vector iv</param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] plainBytes, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(plainBytes, 0, plainBytes.Length);
                        cs.FlushFinalBlock();
                    }

                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="cipherData"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv)
        {
            byte[] decrypted;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(cipherData))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream decryptedMs = new MemoryStream())
                        {
                            cs.CopyTo(decryptedMs);
                            decrypted = decryptedMs.ToArray();
                        }
                    }
                }
                return decrypted;
            }
        }
    }
}

