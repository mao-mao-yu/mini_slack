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
        public async Task StartListening()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext listenerContext = await listener.GetContextAsync();
                if (listenerContext.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext webSocketContext = await listenerContext.AcceptWebSocketAsync(null);
                    WebSocket webSocket = webSocketContext.WebSocket;
                    Console.WriteLine($"Connected to {webSocket.State}");
                    await HandleClient(webSocket);
                }
                else
                {
                    listenerContext.Response.StatusCode = 400;
                    listenerContext.Response.Close();
                }
            }
        }

        public async Task HandleClient(WebSocket webSocket)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result;
            try
            {
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedData = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                        // 解析 JSON
                        Request request = new Request(receivedData);
                        Response response = ActionHandler(request);
                        // 如果需要，你可以向客户端发送响应：
                        //byte[] bytesResponse = Encoding.UTF8.GetBytes(response.GetJsonStr());
                        //ArraySegment<byte> sendBuffer = new ArraySegment<byte>(bytesResponse);
                        //await webSocket.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                while (!result.EndOfMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        public Response ActionHandler(Request request)
        {
            Dictionary<string, string> requestDict = request.Get();
            foreach (var value in requestDict.Values)
            {
                Console.WriteLine(value);
            }
            string action = requestDict["action"];
            bool flg;

            if (action.Equals("login"))
            {
                foreach (var value in requestDict.Values)
                {
                    Console.WriteLine(value);
                }
            }

            return null;

        }
        public bool CheckPassword()
        {
            Console.WriteLine("密码正确");
            return true;
        }

        public bool CheckUsername()
        {
            Console.WriteLine("用户名不存在");
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
