using System;
using System.Security.Cryptography;

namespace Common.Encryption
{
    public static class RsaEncryptor
    {
        #region Fields
        private static HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;

        private static RSASignaturePadding _RSAEncryptionPadding = RSASignaturePadding.Pss;
        #endregion

        #region Properties
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
                    encryptedData = rsa.Encrypt(Text.GetBytes(strData), true);
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
                    decryptedData = rsa.Decrypt(Text.GetBytes(strData), true);
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
                    signature = rsa.SignData(Text.GetBytes(strData), DefaultHashAlgorithmName, DefaultRsaSignaturePadding);
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
