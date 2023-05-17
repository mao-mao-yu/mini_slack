using System;
using System.Collections.Generic;
using System.Text;
using Client.Interface;
using Client.FileHandler;
using System.Threading.Tasks;
using Client.DataType;
using Client.Encryption;
using System.Net.WebSockets;
using System.Threading;
using Newtonsoft.Json;

namespace Client
{
    class Client : IUser, IAdmin
    {
        private readonly Dictionary<string, object> jsonData = JsonFileHandler.LoadJsonObjFromFile(@"UserData.json");
        private string username;
        private string password;
        private bool isLogged;
        private Response Response { get; set; }
        private Uri ServerUri { get; set; }
        private ClientWebSocket ClientWebSocket { get; set; }

        public Client(Uri serverUri)
        {
            username = (string)jsonData["username"];
            password = (string)jsonData["password"];
            isLogged = (bool)jsonData["isLogged"];
            ServerUri = serverUri;
        }

        public async Task Connect()
        {
            try
            {
                await ClientWebSocket.ConnectAsync(ServerUri, CancellationToken.None);
            }
            catch (Exception)
            {
                throw new Exception("Connect error");
            }
        }

        public async Task Listener()
        {
            byte[] receiveBuffer = new byte[1024];

            while (ClientWebSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await ClientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string jsonStr = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                    Response response = new Response(jsonStr);
                    ResponseHandler responseHandler = new ResponseHandler(response.Get());
                }
            }
        }

        public async Task SendJson(Request request)
        {
            string jsonStr = JsonConvert.SerializeObject(request.Get());
            byte[] msgBytes = Encoding.UTF8.GetBytes(jsonStr);
            await ClientWebSocket.SendAsync(new ArraySegment<byte>(msgBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task Login(string username, string password)
        {
            // ログインチェック、trueの時ログインしない
            if (isLogged == true) return;
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

        public void SendMsg(string username, string msg)
        {

        }

        public void AddFriend()
        {
        }
        public void GetFriendList()
        {

        }
        public void DelFriend()
        {

        }

        public void JoinGroup()
        {

        }

        public void LeaveGroup()
        {

        }

    }
    public class ResponseHandler : ActionHandler
    {
        public ResponseHandler(Dictionary<string, string> responseDict) : base(responseDict) 
        { 

        }


    }

    public class ActionHandler
    {
        protected Dictionary<string, string> ResponseDict { get; set; }
        private readonly Dictionary<string, object> ActionDict = new Dictionary<string, object>()
        {
            {"login",  },
        }

        public ActionHandler(Dictionary<string, string> responseDict)
        {
            ResponseDict = responseDict;
        }

        public void ActionProcessing()
        {
            string action = ResponseDict["action"];
            
        }

        public Show
    }
}
