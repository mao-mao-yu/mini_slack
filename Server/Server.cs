using System;
using System.Collections.Generic;
using System.Text;
using Server.IServer;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Server.Data;
using System.Net.WebSockets;
using System.Threading;
using Newtonsoft.Json;

namespace Server
{
    public class AppServer : IAppServer
    {
        //private const int bufferSize = 2048;
        private UdpClient udpClient;
        private IPEndPoint remoteEndPoint;

        public void Start()
        {
            udpClient = new UdpClient(8888);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            _ = udpClient.BeginReceive(ReceiveCallback, null);
            while (true)
            {
                Thread.Sleep(1000);
            }


        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                byte[] receiveData = udpClient.EndReceive(ar, ref remoteEndPoint);
                ProcessReceiveData(receiveData);

                _ = udpClient.BeginReceive(ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error receiving UDP packet: {e.Message}");
            }
        }

        private void ProcessReceiveData(byte[] receivedData)
        {
            int offset = 0;
            while (offset < receivedData.Length)
            {
                if (offset + sizeof(int) > receivedData.Length)
                {
                    Console.WriteLine($"Error: Packet is incomplete. Missing {sizeof(int)} bytes for packet length.");
                    return;
                }
                // 数据长度转int
                int packetLength = BitConverter.ToInt32(receivedData, offset);
                offset += sizeof(int);

                if (offset + packetLength > receivedData.Length)
                {
                    Console.WriteLine($"Error: Packet is incomplete. Missing {offset + packetLength - receivedData.Length} bytes for packet data.");
                    return;
                }
                // 用请求头的数据长取定量数据
                byte[] packetData = new byte[packetLength];
                Buffer.BlockCopy(receivedData, offset, packetData, 0, packetLength);

                // 转string并打印
                string packetMessage = Encoding.UTF8.GetString(packetData);
                Dictionary<string, string> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(packetMessage);
                ActionHandler(jsonDict);
                Console.WriteLine("Received packet: " + packetMessage);

                offset += packetLength;
            }
        }

        //private void ActionHandler(Dictionary<string, string> jsonDict)
        //{
        //    string action = jsonDict["action"];
        //    if (action.Equals("login"))
        //    {
        //        if (CheckUsername(jsonDict["username"]))
        //        {
        //            Console.WriteLine("Username Checked");
        //            if (CheckPassword(jsonDict["password"]))
        //            {
        //                Console.WriteLine("Password  Checked");
        //            }
        //        }
        //    }
        //}

        public bool CheckPassword(string password)
        {
            return true;
        }

        public bool CheckUsername(string username)
        {

            return true;
        }
        public void FileHandler()
        {
            throw new NotImplementedException();
        }

        public void Listening()
        {
            throw new NotImplementedException();
        }

        public void Send()
        {
            throw new NotImplementedException();
        }
    }
}
