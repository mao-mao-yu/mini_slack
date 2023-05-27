using System;
using Server.Data;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.IO;
using System.Net;
using Server;
using Server.ServerCore;
using Server.Encryption;
using Server.Setting;

internal class ServerStart
{
    #region Main
    private static void Main(string[] args)
    {
        // udp,tcp
        //AppServer server = new AppServer(11000, 8888);
        // server.Start();
        // while (true) ;
        // Logger lg = new Logger();
        // for (int i = 0; i < 10; i++)
        // {
        //     lg.FDEBUG(i.ToString());
        // }



        try
        {
            Service server = new Service();
            server.Start();
            Console.WriteLine("Server is started...");
            Console.ReadLine();
        }
        catch (TypeInitializationException ex)
        {
            Console.WriteLine("Failed to initialize type: " + ex.TypeName);
            Console.WriteLine("The original exception: " + ex.InnerException);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }





        //string password = "XfkldptY4327";
        //// Bytes password
        //byte[] bytesPassword = Encoding.UTF8.GetBytes(password);
        //(string pubKey, string priKey) = RsaKeyGenerator.GenerateKeys();
        //// Aes key
        //string aesKey = "I*FA#kgn.24(=13";
        //// 加密公钥
        //string aesEncryptedPubKey = AesEncrypter.Encrypt(pubKey, aesKey);
        //// 解密公钥
        //string aesDecryptedPubkey = AesEncrypter.Decrypt(aesEncryptedPubKey, aesKey);
        //// 设置编码
        //RsaEncryptor.DefaultEncoding = Encoding.UTF8;
        //byte[] encryptedPassword = RsaEncryptor.Encrypt(bytesPassword, aesDecryptedPubkey);
        //Console.WriteLine(encryptedPassword.Length);
        //string decryptedPassword = RsaEncryptor.Decrypt(encryptedPassword, priKey);
        //if (password.Equals(decryptedPassword))
        //{
        //    Console.WriteLine("Login successful...");
        //}
        #endregion
    }
}