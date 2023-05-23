using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Client.Encryption
{
    public static class RsaEncryptor
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
        /// <param name="plainData"></param>
        /// <param name="publicKey"></param>
        /// <param name="toString"></param>
        /// <returns></returns>
        public static byte[] Encrypt<T>(T plainData, string publicKey)
            where T : class
        {
            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                if (plainData is string)
                {
                    string strData = plainData as string;
                    encryptedData = rsa.Encrypt(DefaultEncoding.GetBytes(strData), true);
                }
                else if (plainData is byte[])
                {
                    byte[] byteData = plainData as byte[];
                    encryptedData = rsa.Encrypt(byteData, true);
                }
                else
                {
                    throw new ArgumentException("Invalid argument type. Only string and byte array are allowed.", nameof(plainData));
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
        /// <param name="cipherData">Data</param>
        /// <param name="privateKey">Private key</param>
        /// <returns></returns>
        public static byte[] Decrypt<T>(T cipherData, string privateKey)
            where T : class
        {
            byte[] decryptedData;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                if (cipherData is string)
                {
                    string strData = cipherData as string;
                    decryptedData = rsa.Decrypt(DefaultEncoding.GetBytes(strData), true);
                }
                else if (cipherData is byte[])
                {
                    byte[] bytesData = cipherData as byte[];
                    decryptedData = rsa.Decrypt(bytesData, true);
                }
                else
                {
                    throw new ArgumentException("Invalid argument type. Only string and byte array are allowed.", nameof(cipherData));
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
