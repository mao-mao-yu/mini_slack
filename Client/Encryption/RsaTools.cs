using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Server.Encryption
{
    /// <summary>
    /// Rsa key generater
    /// </summary>
    /// <param name="keySize"></param>
    /// <returns></returns>
    public static class RsaKeyGenerator
    {
        /// <summary>
        /// Generate keys
        /// </summary>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public static (string publicKey, string privateKey) GenerateKeys(int keySize = 2048)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize))
            {
                rsa.PersistKeyInCsp = false;
                string publicKey = rsa.ToXmlString(false);
                string privateKey = rsa.ToXmlString(true);

                return (publicKey, privateKey);
            }
        }
    }

    public static class EncryptionRsa
    {
        /// <summary>
        /// Rsa encrypt
        /// </summary>
        /// <param name="data"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static byte[] EncryptToBytes(string data, string publicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                return rsa.Encrypt(Encoding.UTF8.GetBytes(data), true);
            }
        }

        public static byte[] EncryptToBytes(byte[] bytesData, string publicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                return rsa.Encrypt(bytesData, true);
            }
        }
        /// <summary>
        /// Rsa Decrypt
        /// </summary>
        /// <param name="byteData"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string DecryptToString(byte[] byteData, string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                return Encoding.UTF8.GetString(rsa.Decrypt(byteData, true));
            }
        }

        public static string DecryptToString(string data, string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                byte[] bytesData = Encoding.UTF8.GetBytes(data);
                rsa.FromXmlString(privateKey);
                return Encoding.UTF8.GetString(rsa.Decrypt(bytesData, true));
            }
        }

        public static byte[] DecryptToBytes(byte[] bytesData, string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                return rsa.Decrypt(bytesData, true);
            }
        }

        public static byte[] DecryptToBytes(string data, string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                byte[] bytesData = Encoding.UTF8.GetBytes(data);
                rsa.FromXmlString(privateKey);
                return rsa.Decrypt(bytesData, true);
            }
        }

        public static T Encrypt<T>(T data, string publicKey, bool toString = false)
        {
            byte[] encryptedBytes;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                if (data is string strData)
                {
                    encryptedBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(strData), true);
                }
                else if (data is byte[] byteData)
                {
                    encryptedBytes = rsa.Encrypt(byteData, true);
                }
                else
                {
                    throw new ArgumentException("Invalid argument type. Only string and byte array are allowed.", nameof(data));
                }
            }

            if (toString)
            {
                return (T)(object)Encoding.UTF8.GetString(encryptedBytes);
            }
            else
            {
                return (T)(object)encryptedBytes;
            }
        }
    }
}
