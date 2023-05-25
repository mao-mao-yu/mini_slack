using System;
using System.Collections.Generic;
using System.Net;
using Server.SocketAsyncCore;
using Server.Data;
using Server.Encryption;
using Server.Interface;

namespace Server.ServerCore
{
    public class Service : SocketAsyncTcpServer
    {
        

        private static string Key;
        public static string KEY
        {
            get
            {
                if (Key == null)
                {
                    throw new Exception("Key is null...");
                }
                return Key;
            }
            set
            {
                Key = value;
            }
        }

        #region ctor
        public Service(int listenPort, int maxClient) : base(IPAddress.Any, listenPort, maxClient)
        {
        }

        public Service(IPAddress localIPAddress, int listenPort, int maxClient) : base(localIPAddress, listenPort, maxClient)
        {
        }

        public Service(IPEndPoint localEP, int maxClient) : base(localEP, maxClient)
        {
        }
        #endregion

        protected override void HandleMessage(string data)
        {
            //// Create a request obj
            //Request request = new Request(data);

            //// Get action
            //string action = request.Get("action");
            //if (action == null)                         // Action is null
            //{
            //    return;
            //}

            //action = action.ToLower();                  // 小文字に変換

            //if (methodDict.ContainsKey(action))         // 存在判断
            //{
            //    MyDelegete method = methodDict[action];
            //    method(request);
            //}
        }

        //private static void LoginAction(Request request)
        //{
        //    string username = request.Get("username");
        //    string encryptedPassword = @"";
        //    HashEncrypter.Verify(
        //        AesEncrypter.Decrypt(request.Get("password"), Convert.FromBase64String(KEY)),
        //        encryptedPassword);
        //}

        private static void RegisterAction(Request request)
        {

        }

        private static void PrivateChatAction(Request request)
        {

        }

        private static void GroupChatAction(Request request)
        {

        }

        private static void GetFriendsListAction(Request request)
        {

        }

        private static void GetGroupMemberAction(Request request)
        {

        }

        private static void BlockAction(Request request)
        {

        }

        private UserData GetUserData()
        {
            return null;
        }
    }
}
