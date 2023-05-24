using System;
using Client;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Client.Encryption;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

class Program
{
    static public int messageNum;
    static public IPAddress ip;
    static Dictionary<string, string> test = new Dictionary<string, string>()
    {
        {"action", "login"},
        {"username", "1228315965"},
        {"password", "gasgsaaagasgasg" }
    };

    static string FormatTimeSpan(TimeSpan timeSpan)
    {
        return $"{timeSpan.Hours:D2}H{timeSpan.Minutes:D2}M{timeSpan.Seconds:D2}S";
    }

    static void Main(string[] args)
    {

        //string password = "XfkldptY4327";
        //byte[] key = AesEncryptor.GenerateRandomKey(256);
        //byte[] iv = AesEncryptor.GenerateRandomIV();
        //byte[] encryptedPassword = AesEncryptor.Encrypt(password, key, iv);
        //Console.WriteLine(Convert.ToBase64String(key));
        //Console.WriteLine(Convert.ToBase64String(iv));
        //Console.WriteLine(Convert.ToBase64String(encryptedPassword));
        //byte[] decryptedPassword = AesEncryptor.Decrypt(encryptedPassword, key, iv);
        //Console.WriteLine(Encoding.UTF8.GetString(decryptedPassword));
        List<Thread> threads = new List<Thread>();
        int threadNum = 1;
        messageNum = 20;
        //ip = IPAddress.Parse("192.168.10.111");
        ip = IPAddress.Parse("127.0.0.1");
        //createConnect(test);
        //Console.ReadKey();

        DateTime startTime = DateTime.Now;
        for (int i = 0; i < threadNum; i++)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(createConnect));
            threads.Add(thread);
            thread.Start(test);
        }
        foreach (Thread thread in threads)
        {
            thread.Join();
        }
        DateTime endTime = DateTime.Now;
        TimeSpan duration = endTime - startTime;

        string formattedDuration = FormatTimeSpan(duration);
        Console.WriteLine($"{threadNum}个客户端全部发送完毕.耗时{formattedDuration}");
        Console.WriteLine("按任意键退出...");
        Console.ReadKey(); // 防止主线程退出
    }

    public static void createConnect(object testData)
    {
        Dictionary<string, string> testDict = (Dictionary<string, string>)testData;

        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            int Port = 8888;
            clientSocket.Connect(new IPEndPoint(ip, Port));
            byte[] data = new byte[2048];
            int receivedBytes = clientSocket.Receive(data);
            string receivedMessage = Encoding.UTF8.GetString(data, 0, receivedBytes);
            Console.WriteLine($"收到UserToken：{receivedMessage}");
            Console.WriteLine("连接服务器成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"连接服务器失败，请按回车键退出！{ex.Message}");
            return;
        }

        string jsonStr;
        try
        {
            jsonStr = JsonConvert.SerializeObject(testDict);
        }
        catch (Exception)
        {
            Console.WriteLine("json字符串解析失败");
            throw;
        }

        byte[] dataLength = BitConverter.GetBytes(jsonStr.Length);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonStr);
        byte[] allData = new byte[dataLength.Length + jsonBytes.Length];
        Array.Copy(dataLength, 0, allData, 0, dataLength.Length);
        Array.Copy(jsonBytes, 0, allData, dataLength.Length, jsonBytes.Length);
        int totalBytes = 0;
        for (int i = 0; i < messageNum; i++)
        {
            totalBytes += clientSocket.Send(allData);
        }
        Console.WriteLine(totalBytes);
        //    for (int i = 0; i < messageNum; i++)
        //    {
        //        try
        //        {
        //            Thread.Sleep(1000);
        //            clientSocket.Send(Encoding.UTF8.GetBytes(jsonStr), SocketFlags.None);
        //            Console.WriteLine($"向服务器发送消息：{jsonStr}");
        //        }
        //        catch (Exception e)
        //        {
        //            clientSocket.Close();
        //            Console.WriteLine(e.Message, e.StackTrace);
        //            break;
        //        }
        //    }
        //    Console.WriteLine($"{messageNum}条消息发送完成");
        //    clientSocket.Close();
    }
}

