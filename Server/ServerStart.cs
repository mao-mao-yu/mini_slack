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
using Server.SocketAsyncCore;
using Server.Encryption;

internal class ServerStart
{
    private static void Main(string[] args)
    {
        // udp,tcp
        //AppServer server = new AppServer(11000, 8888);
        //server.Start();
        //while (true) ;
        //Logger lg = new Logger();
        //for (int i = 0; i < 10; i++)
        //{
        //    lg.FDEBUG(i.ToString());
        //}

        //try
        //{
        //    //IPAddress IP = IPAddress.Parse("192.168.0.248");
        //    IPAddress IP = IPAddress.Parse("192.168.10.111");
        //    int parallelNum = 5000;
        //    int port = 8888;

        //    AppServer server = new AppServer(port, parallelNum);
        //    server.Start();
        //    Console.WriteLine("Server is started...");
        //    Console.ReadLine();
        //}
        //catch (Exception e)
        //{
        //    Console.WriteLine(e.Message);
        //}

        string password = "XfkldptY4327";
        // Bytes password
        byte[] bytesPassword = Encoding.UTF8.GetBytes(password);
        (string pubKey, string priKey) = RsaKeyGenerator.GenerateKeys();
        // Aes key
        string aesKey = "I*FA#kgn.24(=13";
        // 加密公钥
        string aesEncryptedPubKey = AesEncrypter.Encrypt(pubKey, aesKey);
        // 解密公钥
        string aesDecryptedPubkey = AesEncrypter.Decrypt(aesEncryptedPubKey, aesKey);
        // 设置编码
        RsaEncrypter.DefaultEncoding = Encoding.UTF8;
        byte[] encryptedPassword = RsaEncrypter.Encrypt(bytesPassword, aesDecryptedPubkey);
        Console.WriteLine(encryptedPassword.Length);
        string decryptedPassword = RsaEncrypter.Decrypt(encryptedPassword, priKey);
        if (password.Equals(decryptedPassword))
        {
            Console.WriteLine("Login successful...");
        }
    }

}