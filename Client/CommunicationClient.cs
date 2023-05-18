using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client.CommunicationClient
{
    class MyTcpClient
    {
        private async Task CloseWebSocketClient()
        {
            try
            {
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing the connection", CancellationToken.None);
                Console.WriteLine("ClientWebsocket closed...");
            }
            catch (Exception e)
            {
                Console.WriteLine("ClientWebsocket closed..." + e.Message);
            }
        }

        private async Task CreateWebSocketClient()
        {
            Uri uri = new Uri($"ws://{serverEndPoint.Address}:8080");
            ClientWebSocket clientWebSocket = new ClientWebSocket();
            try
            {
                await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
                Console.WriteLine("Connected to " + uri.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Connect error... " + e.Message);
            }
        }


        private async Task TcpSendStr(string msgStr)
        {
            ArraySegment<byte> segement = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msgStr));
            try
            {
                await clientWebSocket.SendAsync(segement, WebSocketMessageType.Binary, true, CancellationToken.None);
                Console.WriteLine("Msg sended...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Msg send error..." + e.Message);
            }
        }

        private async Task<string> TcpRecvStr()
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[2048]);
            try
            {
                await clientWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                return Encoding.UTF8.GetString(buffer);
            }
            catch (Exception e)
            {
                Console.WriteLine("Receive data error..." + e.Message);
            }

            return string.Empty;
        }


    }

    public abstract class MyUdpClient
    {
        private readonly IPEndPoint _serverEndPoint;
        private readonly UdpClient _client;
        private IPEndPoint _remoteEndPoint;
        private IPEndPoint _localEndPoint;
        private IPAddress _localIPAddress;
        private string GetIPAddress()
        {
            var Addresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var Address in Addresses)
            {
                if (Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return Address.ToString();
                }
            }
            return "127.0.0.1";
        }

        /// <summary>
        /// Create UDPClient
        /// </summary>
        /// <param name="ip">Target IP</param>
        /// <param name="port">Target port</param>
        public MyUdpClient(string ip, int port)
        {
            _localIPAddress = IPAddress.Parse(GetIPAddress());
            _serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _client = new UdpClient();
            _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }

        public void StartRecv(int port)
        {
            _localEndPoint = new IPEndPoint(_localIPAddress, port);
            Thread udpListener = new Thread(RecvData);
            udpListener.Start();
        }

        private void RecvData()
        {
            byte[] receivedData;
            while (true)
            {
                try
                {
                    receivedData = _client.Receive(ref _remoteEndPoint);
                    ProcessReceivedData(receivedData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        //private void Callback(IAsyncResult result)
        //{
        //    try
        //    {
        //        byte[] buffer = _client.EndReceive(result, ref _remoteEndPoint);
        //        ProcessReceivedData(buffer);

        //        _ = _client.BeginReceive(Callback, null);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        _ = _client.BeginReceive(Callback, null);
        //    }
        //}

        public void ProcessReceivedData(byte[] receivedData)
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

        public abstract void ActionHandler(Dictionary<string, string> jsonDict);

        /// <summary>
        /// 发送包
        /// </summary>
        /// <param name="message"></param>
        public void SendPacket(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                Console.WriteLine("Error: Message cannot be null or empty.");
                return;
            }

            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                int messageLength = messageBytes.Length;

                byte[] packetData = new byte[sizeof(int) + messageLength];
                Buffer.BlockCopy(BitConverter.GetBytes(messageLength), 0, packetData, 0, sizeof(int));
                Buffer.BlockCopy(messageBytes, 0, packetData, sizeof(int), messageLength);

                _ = _client.Send(packetData, packetData.Length, _serverEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error sending UDP packet: " + e.Message);
            }

        }
    }
}
