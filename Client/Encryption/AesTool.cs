using System.Text;
using System.Security.Cryptography;
using System.IO;
using Client.Log;
using System;

namespace Client.Encryption
{
    public static class AesEncryptor
    {
        public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;
        public static int KeySize { get; } = 256;

        /// <summary>
        /// Random Key
        /// </summary>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public static byte[] GenerateRandomKey(int keySize)
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
        public static byte[] Encrypt<T>(T plainData, byte[] key, byte[] iv)
        {
            byte[] plainBytes;
            if (plainData is string)
            {
                string plainStr = plainData as string;
                plainBytes = DefaultEncoding.GetBytes(plainStr);
            }
            else if (plainData is byte[])
            {
                plainBytes = plainData as byte[];
            }
            else
            {
                throw new ArgumentException("Invalid argument type. Only string and byte array are allowed.", nameof(plainData));
            }

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
        public static byte[] Decrypt<T>(T cipherData, byte[] key, byte[] iv)
        {
            byte[] decryptedBytes;
            byte[] cipherBytes;
            if (cipherData is string)
            {
                string cipherStr = cipherData as string;
                cipherBytes = DefaultEncoding.GetBytes(cipherStr);
            }
            else if (cipherData is byte[])
            {
                cipherBytes = cipherData as byte[];
            }
            else
            {
                throw new ArgumentException("Invalid argument type. Only string and byte array are allowed.", nameof(cipherData));
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream decryptedMs = new MemoryStream())
                        {
                            cs.CopyTo(decryptedMs);
                            decryptedBytes = decryptedMs.ToArray();
                        }
                    }
                }
                return decryptedBytes;
            }
        }
    }
}

