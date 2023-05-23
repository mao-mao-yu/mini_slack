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
        
    }

    public static class RsaEncrypter
    {
        #region Fields
        /// <summary>
        /// デフォルトコードページ
        /// </summary>
        private static Encoding _defaultEncoding = Encoding.UTF8;

        private static HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;

        private static RSASignaturePadding _RSAEncryptionPadding = RSASignaturePadding.Pss;
        #endregion

        #region Properties
        /// <summary>
        /// デフォルトコードページプロパティ
        /// </summary>
        public static Encoding DefaultEncoding => _defaultEncoding;

        public static HashAlgorithmName DefaultHashAlgorithmName => _hashAlgorithmName;

        public static RSASignaturePadding DefaultRsaSignaturePadding => _RSAEncryptionPadding;
        #endregion

        #region Encrypt
        /// <summary>
        /// Rsa encrypt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="publicKey"></param>
        /// <param name="toString"></param>
        /// <returns></returns>
        public static byte[] Encrypt<T>(T data, string publicKey)
            where T: class
        {
            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                if (data is string)
                {
                    string strData = data as string;
                    encryptedData = rsa.Encrypt(DefaultEncoding.GetBytes(strData), true);
                }
                else if (data is byte[])
                {
                    byte[] byteData = data as byte[];
                    encryptedData = rsa.Encrypt(byteData, true);
                }
                else
                {
                    throw new ArgumentException("Invalid argument type. Only string and byte array are allowed.", nameof(data));
                }
            }
            return encryptedData;
        }
        #endregion

        #region Decrypt
        /// <summary>
        /// Decrypt data(byte[] or string)
        /// </summary>
        /// <typeparam name="T">byte[] or string</typeparam>
        /// <param name="data">Data</param>
        /// <param name="privateKey">Private key</param>
        /// <returns></returns>
        public static byte[] Decrypt<T>(T data, string privateKey)
            where T : class
        {
            byte[] decryptedData;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                if (data is string)
                {
                    string strData = data as string;
                    decryptedData = rsa.Decrypt(DefaultEncoding.GetBytes(strData), true);
                }
                else if (data is byte[])
                {
                    byte[] bytesData = data as byte[];
                    decryptedData = rsa.Decrypt(bytesData, true);
                }
                else
                {
                    throw new ArgumentException("Invalid argument type. Only string and byte array are allowed.", nameof(data));
                }
            }
            return decryptedData;
        }
        #endregion

        #region SignData
        /// <summary>
        /// Sign data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static byte[] SignData<T>(T data, string publicKey)
        {
            byte[] signature;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                if (data is string)
                {
                    string strData = data as string;
                    signature = rsa.SignData(DefaultEncoding.GetBytes(strData), DefaultHashAlgorithmName, DefaultRsaSignaturePadding);
                }
                else if (data is byte[])
                {
                    byte[] byteData = data as byte[];
                    signature = rsa.SignData(byteData, DefaultHashAlgorithmName);
                }
                else
                {
                    throw new ArgumentException("Invalid argument type. Only string and byte array are allowed.", nameof(data));
                }
            }
            return signature;
        }
        #endregion

        #region Verify
        public static bool VerifyData(byte[] bytesData, byte[] signature)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                return rsa.VerifyData(bytesData, signature, DefaultHashAlgorithmName, DefaultRsaSignaturePadding);
            }
        }
        #endregion

        #region Key generater
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
        #endregion
    }
}
