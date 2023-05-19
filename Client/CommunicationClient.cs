using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Client.Commons;

namespace Client.ClientCommunication
{
    /// <summary>
    /// 客户端基类 
    /// Client base class 
    /// クライアント基本クラス
    /// </summary>
    public abstract class BaseClient
    {
        protected Queue<Dictionary<string, string>> msgDictQ = new Queue<Dictionary<string, string>>();
        private IPAddress _localIPAddress;            // 本地IP Local ip
        protected IPAddress LocalIPAddress => _localIPAddress;

        public BaseClient()
        {
            _localIPAddress = GetLocalIPAddress();
        }

        protected void EnqueueMessage(Dictionary<string, string> message)
        {
            msgDictQ.Enqueue(message);
        }

        protected Dictionary<string, string> DequeueMessage()
        {
            return msgDictQ.Dequeue();
        }

        private static IPAddress GetLocalIPAddress()
        {
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address;
                }
            }
            return IPAddress.Parse("127.0.0.1");
        }

        protected abstract void Connect();

        protected abstract void Listening();

        public abstract Task Send(string message);

        protected abstract void Recv();

        protected abstract void StartRecv();

        protected abstract void ProcessReceivedData(byte[] receivedData);
    }
    public abstract class TClient : BaseClient
    {
        private readonly TcpClient _tcpClient;
        private readonly IPAddress _serverIPAddress;
        private readonly IPEndPoint _serverIPEndPoint;

        public TClient(string serverIP, int tcpPort)
        {
            try
            {
                _serverIPAddress = IPAddress.Parse(serverIP);
            }
            catch (FormatException e)
            {
                throw new FormatException(e.Message);
            }

            _serverIPEndPoint = new IPEndPoint(_serverIPAddress, tcpPort);
            _tcpClient = new TcpClient();
        }

        protected override void Connect()
        {
            try
            {
                _tcpClient.Connect(_serverIPEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public override async Task Send(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Error: Message cannot be null or empty.");
            }

            try
            {
                byte[] bytesMsg = Encoding.UTF8.GetBytes(message);
                int msgLength = bytesMsg.Length;
                byte[] lengthData = BitConverter.GetBytes(msgLength);

                using NetworkStream networkStream = _tcpClient.GetStream();
                await networkStream.WriteAsync(lengthData, 0, sizeof(int));
                await networkStream.WriteAsync(bytesMsg, sizeof(int), msgLength);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        protected override void Recv()
        {
            while (true)
            {
                NetworkStream networkStream = null;
                try
                {
                    networkStream = _tcpClient.GetStream();
                    byte[] dataLength = new byte[sizeof(int)];
                    _ = networkStream.Read(dataLength, 0, sizeof(int));
                    int msgLength = BitConverter.ToInt32(dataLength);
                    byte[] receivedData = new byte[msgLength];
                    _ = networkStream.Read(receivedData, sizeof(int), msgLength);
                    ProcessReceivedData(receivedData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    networkStream?.Close();
                }
            }

        }

        protected override void StartRecv()
        {
            Thread tcpListener = new Thread(Recv);
            tcpListener.Start();
        }

        protected override void ProcessReceivedData(byte[] receivedData)
        {
            // 转string并打印
            string packetMessage = Encoding.UTF8.GetString(receivedData);
            Dictionary<string, string> jsonDict;

            try
            {
                jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(packetMessage);
                msgDictQ.Enqueue(jsonDict);
                Console.WriteLine($"Received data {packetMessage} to Q");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deserializing JSON: " + ex.Message);
            }
        }
    }
    public class UClient : BaseClient
    {
        private readonly UdpClient _client;                                                 // UdpClient
        private const int HeaderLength = sizeof(int);                                       // Header length
        private readonly IPAddress _serverIPAddress;                                        // Server IPv4 address
        private readonly IPEndPoint _serverIPEndPoint;                                      // Server Ip end point
        private IPEndPoint _remoteIPEndPoint;                                               // Remote Ip end point
        private readonly IPEndPoint _localIPEndPoint;                                       // Local Ip end point

        public UClient(string serverIp, int localListeningUdpPort, int serverUdpPort)
        {
            try
            {
                _serverIPAddress = IPAddress.Parse(serverIp);                               // Server IP 
                _serverIPEndPoint = new IPEndPoint(_serverIPAddress, serverUdpPort);        // Server Ip end Point
            }
            catch (FormatException e)
            {
                throw new FormatException(e.Message);
            }

            _localIPEndPoint = new IPEndPoint(LocalIPAddress, localListeningUdpPort);       // Local IP end point

            _remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);                           // Any remote point

            _client = new UdpClient(_localIPEndPoint);                                      // Instance a udp client bind local IP end point
        }

        protected override void StartRecv()
        {
            Thread udpListener = new Thread(Recv);
            udpListener.Start();
        }

        protected override void Recv()
        {
            while (true)
            {
                try
                {
                    byte[] receivedData = _client.Receive(ref _remoteIPEndPoint);
                    if (receivedData.Length > 0)
                    {
                        ProcessReceivedData(receivedData);  // 处理数据 Process data
                    }
                    else
                    {
                        Console.WriteLine("Error: Received empty data packet.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public override async Task Send(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Error: Message cannot be null or empty.");
            }

            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                int messageLength = messageBytes.Length;

                byte[] packetData = new byte[sizeof(int) + messageLength];
                Buffer.BlockCopy(BitConverter.GetBytes(messageLength), 0, packetData, 0, sizeof(int));
                Buffer.BlockCopy(messageBytes, 0, packetData, sizeof(int), messageLength);
                using UdpClient udpClient = new UdpClient(12000);
                int SendedDataLength = await udpClient.SendAsync(packetData, packetData.Length, _serverIPEndPoint);
                Console.WriteLine($"Sended {SendedDataLength} bytes...");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static byte[] CreatePacket(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                throw new ArgumentException("Error: jsonData cannot be null or empty.");
            }

            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);
            byte[] headerBytes = BitConverter.GetBytes(dataBytes.Length);
            byte[] packet = new byte[HeaderLength + dataBytes.Length];

            Buffer.BlockCopy(headerBytes, 0, packet, 0, HeaderLength);
            Buffer.BlockCopy(dataBytes, 0, packet, HeaderLength, dataBytes.Length);

            return packet;
        }

        protected override void ProcessReceivedData(byte[] receivedData)
        {
            int offset = 0;

            if (receivedData.Length < sizeof(int))
            {
                Console.WriteLine("Received data is incomplete.");
                return;
            }

            // 从receivedData取4个节转int
            int packetLength = BitConverter.ToInt32(receivedData, offset);
            offset += sizeof(int);

            if (receivedData.Length < offset + packetLength)
            {
                Console.WriteLine("Received data is incomplete.");
                return;
            }

            byte[] packetData = new byte[packetLength];
            Buffer.BlockCopy(receivedData, offset, packetData, 0, packetLength);

            string packetMessage = Encoding.UTF8.GetString(packetData);
            Dictionary<string, string> jsonDict;

            try
            {
                jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(packetMessage);
                msgDictQ.Enqueue(jsonDict);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deserializing JSON: " + ex.Message);
            }
        }

        protected override void Connect()
        {
            throw new NotImplementedException();
        }

        protected override void Listening()
        {
            throw new NotImplementedException();
        }
    }
}
