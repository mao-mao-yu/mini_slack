using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Server.Encryption.HashTool
{
    public static class PasswordEncryptionHash
    {
        private const int SaltSize = 16; // 加盐大小，以字节为单位
        private const int Iterations = 10000; // 迭代次数，用于增加计算成本

        public static string Encrypt(string password)
        {
            // 生成随机的盐值
            byte[] salt = GenerateSalt();

            // 创建密码哈希
            byte[] hash = GenerateHash(password, salt);

            // 将盐值和哈希值合并为一个字符串
            string saltString = Convert.ToBase64String(salt);
            string hashString = Convert.ToBase64String(hash);
            string encryptedPassword = $"{saltString}:{hashString}";

            return encryptedPassword;
        }

        public static bool Verify(string password, string encryptedPassword)
        {
            // 从加密的密码中提取盐值和哈希值
            string[] parts = encryptedPassword.Split(':');
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] hash = Convert.FromBase64String(parts[1]);

            // 生成哈希并与提供的哈希进行比较
            byte[] generatedHash = GenerateHash(password, salt);

            return SlowEquals(hash, generatedHash);
        }

        public static bool IsPasswordStrong(string password)
        {
            // 密码不能为纯数字，且长度不能少于8位
            return !IsNumeric(password) && password.Length >= 8;
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[SaltSize];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        private static byte[] GenerateHash(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                return pbkdf2.GetBytes(SaltSize);
            }
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;

            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }

            return diff == 0;
        }

        private static bool IsNumeric(string input)
        {
            return int.TryParse(input, out _);
        }   
    }

    public class FileEncryption
    {

    }

}
