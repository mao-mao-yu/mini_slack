using System;
using System.Collections.Generic;
using System.Text;
using Client.IClient;
using Client.FileHandler;
using System.Threading.Tasks;
using Client.DataType;
using Client.Encryption;
using System.Net.WebSockets;
using System.Threading;
using Newtonsoft.Json;

namespace Client
{
    class AppClient : IUser
    {
        //private readonly Dictionary<string, object> jsonData = JsonFileHandler.LoadJsonObjFromFile(@"UserData.json");
        private readonly Dictionary<string, object> jsonData = new Dictionary<string, object>() {};
        private string username;
        private string password;
        private bool isLogged = false;
        private Response Response { get; set; }
        private Uri ServerUri { get; set; }
        private ClientWebSocket ClientWebSocket { get; set; }

        public AppClient(Uri serverUri)
        {
            //username = (string)jsonData["username"];
            //password = (string)jsonData["password"];
            //isLogged = (bool)jsonData["isLogged"];
            ServerUri = serverUri;
        }

        public async Task Connect()
        {
            ClientWebSocket = new ClientWebSocket();
            try
            {
                await ClientWebSocket.ConnectAsync(ServerUri, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task StartListening()
        {
            byte[] receiveBuffer = new byte[1024];

            while (ClientWebSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await ClientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string jsonStr = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                    Response response = new Response(jsonStr);
                    Dictionary<string, string> responseDict = response.Get();
                    ResponseHandler responseHandler = new ResponseHandler(responseDict);
                }
            }
        }

        public async Task SendJson(Request request)
        {
            string jsonStr = JsonConvert.SerializeObject(request.Get());
            byte[] msgBytes = Encoding.UTF8.GetBytes(jsonStr);
            Console.WriteLine($"msgBytes长度为{msgBytes}");
            await ClientWebSocket.SendAsync(new ArraySegment<byte>(msgBytes), WebSocketMessageType.Binary, true, CancellationToken.None);

        }

        /// <summary>
        /// Perform login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task PerformLogin(string username, string password)
        {
            if (isLogged == true)
            {
                return;
            }

            password = PasswordEncryption.Encrypt(password);
            Request request = new Request("login", username, password);
            await SendJson(request);
        }

        public async Task Register(string username, string password)
        {
            string EncryptedPassword = PasswordEncryption.Encrypt(password);
            Request request = new Request("register", username, EncryptedPassword);
            await SendJson(request);
        }

        //public Response SendMsg(string username, string msg)
        //{
        //}
        //public Response AddFriend()
        //{
        //}
        //public Response GetFriendList()
        //{
        //}
        //public Response DelFriend()
        //{
        //}
        //public Response JoinGroup()
        //{
        //}
        //public Response LeaveGroup()
        //{
        //}

    }
    public class ResponseHandler : ActionHandler
    {
        public ResponseHandler(Dictionary<string, string> responseDict) : base(responseDict) 
        {
            ActionProcessing();
        }


    }

    public class ActionHandler
    {
        protected Dictionary<string, string> ResponseDict { get; set; }
        private readonly Dictionary<string, object> ActionDict = new Dictionary<string, object>() { };

        public ActionHandler(Dictionary<string, string> responseDict)
        {
            ResponseDict = responseDict;
        }

        public void ActionProcessing()
        {
            string action = ResponseDict["action"];
            if (action.Equals("login"))
            {
                Console.WriteLine("登录成功");
            }
        }
    }
}
