using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using Common.Setting;
using Client.ClientCore;
using Client.Data;

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
    static int Count = 0;

    static string FormatTimeSpan(TimeSpan timeSpan)
    {
        return $"{timeSpan.Hours:D2}H{timeSpan.Minutes:D2}M{timeSpan.Seconds:D2}S";
    }

    static void Main(string[] args)
    {
        AsyncClient client = new AsyncClient();
        client.StartConnect();
        Console.ReadKey();


        //ClientSetting setting = new ClientSetting();
        //setting.BufferSize = 2048;
        //setting.ServerIP = "127.0.0.1";
        //setting.Port = 8888;
        //setting.SaveSetting(Const.CLIENT_SETTING_PATH);
        //string password = "XfkldptY4327";
        //byte[] key = AesEncryptor.GenerateRandomKey(256);
        //byte[] iv = AesEncryptor.GenerateRandomIV();
        //byte[] encryptedPassword = AesEncryptor.Encrypt(password, key, iv);
        //Console.WriteLine(Convert.ToBase64String(key));
        //Console.WriteLine(Convert.ToBase64String(iv));
        //Console.WriteLine(Convert.ToBase64String(encryptedPassword));
        //byte[] decryptedPassword = AesEncryptor.Decrypt(encryptedPassword, key, iv);
        //Console.WriteLine(Encoding.UTF8.GetString(decryptedPassword));
        //List<Thread> threads = new List<Thread>();
        //int threadNum = 1;
        //messageNum = 1;
        ////ip = IPAddress.Parse("192.168.10.111");
        //ip = IPAddress.Parse("127.0.0.1");
        ////createConnect(test);
        ////Console.ReadKey();

        //DateTime startTime = DateTime.Now;
        //for (int i = 0; i < threadNum; i++)
        //{
        //    Thread thread = new Thread(new ParameterizedThreadStart(createConnect));
        //    threads.Add(thread);
        //    thread.Start(test);
        //}
        //foreach (Thread thread in threads)
        //{
        //    thread.Join();
        //}
        //DateTime endTime = DateTime.Now;
        //TimeSpan duration = endTime - startTime;

        //string formattedDuration = FormatTimeSpan(duration);
        //Console.WriteLine($"{Count} clients have successfully connected...");
        //Console.WriteLine($"{threadNum} clients sended...take {formattedDuration}...");
        //Console.WriteLine("Enter any key to quit...");
        //Console.ReadKey(); // 防止主线程退出
    }

    //public static void createConnect(object testData)
    //{
    //    Dictionary<string, string> testDict = (Dictionary<string, string>)testData;

    //    Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //    try
    //    {
    //        int Port = 8888;
    //        clientSocket.Connect(new IPEndPoint(ip, Port));
    //        if (clientSocket.Connected)
    //        {
    //            Interlocked.Increment(ref Count);
    //        }
    //        byte[] data = new byte[2048];
    //        int receivedBytes = clientSocket.Receive(data);
    //        string receivedMessage = Encoding.UTF8.GetString(data, 4, receivedBytes);
    //        Console.WriteLine($"Received data：{receivedMessage}");
    //        Console.WriteLine("Connect to server successful...");
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Connect to server error...，Enter to quit！{ex.Message}");
    //        return;
    //    }

    //    string jsonStr;
    //    try
    //    {
    //        jsonStr = JsonConvert.SerializeObject(testDict);
    //    }
    //    catch (Exception)
    //    {
    //        Console.WriteLine("Convert json to str error");
    //        throw;
    //    }

    //    byte[] dataLength = BitConverter.GetBytes(jsonStr.Length);
    //    byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonStr);
    //    byte[] allData = new byte[dataLength.Length + jsonBytes.Length];
    //    Array.Copy(dataLength, 0, allData, 0, dataLength.Length);
    //    Array.Copy(jsonBytes, 0, allData, dataLength.Length, jsonBytes.Length);
    //    int totalBytes = 0;
    //    for (int i = 0; i < messageNum; i++)
    //    {
    //        totalBytes += clientSocket.Send(allData);
    //        Console.WriteLine($"Sended {totalBytes} bytes");
    //    }
    //    Console.WriteLine(totalBytes);
    //}

}

