using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client.CommunicationClient
{
    public abstract class MyTcpClient
    {
        // TCP

        private IPEndPoint serverEndPoint;

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

    public abstract class SocketClient
    {
        private readonly IPEndPoint _serverUdpEndPoint;     // 服务端UDP终结点 Server udp endpoint
        private readonly IPEndPoint _serverTcpEndPoint;     // 服务端TCP终结点 serverEndPoint
        private readonly IPAddress _serverIpAddress;        // 服务器IP
        private readonly UdpClient _client;                 // Udp客户端 UdpClient
        private IPEndPoint _remoteEndPoint;                 // 远程终结点 remoteEndPoint
        private IPEndPoint _localEndPoint;                  // 本地终结点 localEndPoint
        private IPAddress _localIPAddress;                  // 本机IPv4IP local Ipv4
        private ClientWebSocket _clientWebSocket;            // 网络套接字客户端

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
        /// <param name="serverIp">Target IP</param>
        /// <param name="udpPort">Target port</param>
        public SocketClient(string serverIp, int udpPort, int tcpPort)
        {
            _serverIpAddress = IPAddress.Parse(serverIp);
            _localIPAddress = IPAddress.Parse(GetIPAddress());
            _serverUdpEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), udpPort);
            _serverTcpEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), tcpPort);
            _client = new UdpClient();
            _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }

        public void UDPStartRecv(int port)
        {
            Thread udpListener = new Thread(UdpRecvData);
            udpListener.Start();
        }

        /// <summary>
        /// 创建websocketclient
        /// </summary>
        /// <returns></returns>
        private async Task CreateWebSocketClient()
        {
            Uri uri = new Uri($"ws://{_serverIpAddress}:8080");
            _clientWebSocket = new ClientWebSocket();
            try
            {
                await _clientWebSocket.ConnectAsync(uri, CancellationToken.None);
                Console.WriteLine("Connected to " + uri.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Connect error... " + e.Message);
            }
        }
        /// <summary>
        /// 关闭websocketclient
        /// </summary>
        /// <returns></returns>
        private async Task CloseWebSocketClient()
        {
            try
            {
                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing the connection", CancellationToken.None);
                Console.WriteLine("ClientWebsocket closed...");
            }
            catch (Exception e)
            {
                Console.WriteLine("ClientWebsocket closed..." + e.Message);
            }
        }

        private void UdpRecvData()
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

                _ = _client.Send(packetData, packetData.Length, _serverUdpEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error sending UDP packet: " + e.Message);
            }

        }
    }
}
