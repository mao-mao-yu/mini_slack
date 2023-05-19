using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Server.ServerCommunication
{
    public abstract class BaseServer
    {
        protected IPAddress _localIPAddress;
        protected IPEndPoint _remoteIPEndPoint;

        public BaseServer()
        {
            _localIPAddress = GetLocalIPAddress();
        }

        public abstract void UdpStartReceiving();

        public abstract void TcpStartListening();

        protected abstract void TcpListener();

        protected abstract void UdpRecv();

        protected abstract void ProcessTcpClient(TcpClient tcpClient);

        protected abstract void ProcessReceivedData(byte[] receivedData);

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
    }

    public class MyServer : BaseServer
    {
        private readonly TcpListener _tcpListener;
        private readonly UdpClient _udpListener;
        private readonly ConcurrentQueue<Dictionary<string, string>> msgDictQ = new ConcurrentQueue<Dictionary<string, string>>();
        private readonly int _tcpPort;
        private readonly int _udpPort;
        private CancellationTokenSource _udpTokenSource;
        private CancellationTokenSource _tcpTokenSource;

        protected void EnqueueMessage(Dictionary<string, string> message)
        {
            msgDictQ.Enqueue(message);
        }

        protected Dictionary<string, string> DequeueMessage()
        {
            return msgDictQ.TryDequeue(out Dictionary<string, string> message) ? message : null;
        }


        public MyServer(int udpPort, int tcpPort) : base()
        {
            _udpPort = udpPort;
            _tcpPort = tcpPort;
            IPEndPoint localTcpEndPoint = new IPEndPoint(_localIPAddress, _tcpPort);
            IPEndPoint localUdpEndPoing = new IPEndPoint(_localIPAddress, _udpPort);
            _tcpListener = new TcpListener(localTcpEndPoint);
            _udpListener = new UdpClient(localUdpEndPoing);
        }

        public override void UdpStartReceiving()
        {
            Console.WriteLine($"Udp service start receiving on port: {_udpPort}");
            _udpTokenSource = new CancellationTokenSource();
            _ = ThreadPool.QueueUserWorkItem(_ => UdpRecv());
        }

        protected override void UdpRecv()
        {
            while (!_udpTokenSource.IsCancellationRequested)
            {
                try
                {
                    byte[] receivedData = _udpListener.Receive(ref _remoteIPEndPoint);
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
                Thread.Sleep(20);
            }
        }

        public override void TcpStartListening()
        {
            Console.WriteLine($"Tcp service start listening on port: {_tcpPort}");
            _tcpListener.Start();
            _tcpTokenSource = new CancellationTokenSource();
            _ = ThreadPool.QueueUserWorkItem(_ => TcpListener());
        }

        protected override void TcpListener()
        {
            while (!_tcpTokenSource.IsCancellationRequested)
            {
                try
                {
                    TcpClient tcpClient = _tcpListener.AcceptTcpClient();
                    if (tcpClient.Connected)
                    {
                        ThreadPool.QueueUserWorkItem(_ => ProcessTcpClient(tcpClient));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
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
                foreach (var (key, value) in jsonDict)
                {
                    Console.WriteLine(key, value);
                }
                msgDictQ.Enqueue(jsonDict);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deserializing JSON: " + ex.Message);
            }
        }

        protected override void ProcessTcpClient(TcpClient tcpClient)
        {
            using NetworkStream networkStream = tcpClient.GetStream();
            while (tcpClient.Connected)
            {
                try
                {
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
                    break;
                }
            }
        }
    }
}
