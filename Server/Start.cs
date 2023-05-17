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

class Start
{
    static void Main(string[] args)
    {

        Request request = new Request("login");
        request.Add("flg", "false");
        request.Add("msg", "ユーザは存在しない");

        try
        {
            var server = new WebSocketServer();
            server.Start(8080).Wait();
        }
        catch (Exception)
        {

            throw;
        }


    }
    static async Task ProcessWebSocketRequest(TcpClient tcpClient)
    {
        WebSocket webSocket = await AcceptWebSocketAsync(tcpClient);

        if (webSocket != null)
        {
            try
            {
                // 处理WebSocket连接
                Console.WriteLine("WebSocket client connected.");

                byte[] receiveBuffer = new byte[1024];
                WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                while (!receiveResult.CloseStatus.HasValue)
                {
                    string message = Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);
                    Console.WriteLine("Received message from client: " + message);

                    string responseMessage = "This is a response message.";
                    byte[] responseBuffer = Encoding.UTF8.GetBytes(responseMessage);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                    // 继续接收下一个消息
                    receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                }

                await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
                Console.WriteLine("WebSocket client disconnected.");
            }
            finally
            {
                webSocket.Dispose();
            }
        }
    }

    static async Task<WebSocket> AcceptWebSocketAsync(TcpClient tcpClient)
    {
        Stream stream = tcpClient.GetStream();
        HttpListenerContext listenerContext = (HttpListenerContext)await new HttpListener().GetContextAsync();

        if (listenerContext.Request.IsWebSocketRequest)
        {
            WebSocketContext webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: null);
            return webSocketContext.WebSocket;
        }

        return null;
    }

}