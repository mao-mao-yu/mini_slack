using System;
using System.Net;
using Server.SocketAsyncCore;
using Server.Models;
using Common.Encryption;
using Server.ServerInterface;
using Common.Log;

namespace Server.ServerCore
{
    public class Service : SocketAsyncTcpServer, IMessageHandler
    {
        #region ctor
        public Service() : base()
        {
        }
        #endregion

        protected override void HandleMessage(string data)
        {
            // Create a request obj
            Request request = new Request(data);

            // Get action
            string action = request.Get("action");
            if (action == null)                         // Action is null
            {
                Logger.FWARNING("Received no action json data...");
                return;
            }

            action = action.ToLower();                  // 小文字に変換

            //if (methodDict.ContainsKey(action))         // 存在判断
            //{
            //    MyDelegete method = methodDict[action];
            //    method(request);
            //}
        }

        public void Broadcast(int groupID)
        {
            throw new NotImplementedException();
        }

        void IMessageHandler.HandleBlockSomeone(Request request)
        {
            throw new NotImplementedException();
        }

        void IMessageHandler.HandleGetBlockList(Request request)
        {
            throw new NotImplementedException();
        }

        void IMessageHandler.HandleGetFriendList(Request request)
        {
            throw new NotImplementedException();
        }

        void IMessageHandler.HandleGetGroupMemberList(Request request)
        {
            throw new NotImplementedException();
        }

        void IMessageHandler.HandleGetUserData(Request request)
        {
            throw new NotImplementedException();
        }

        void IMessageHandler.HandleGroupChat(Request request)
        {
            throw new NotImplementedException();
        }

        void IMessageHandler.HandleLogin(Request request)
        {
            throw new NotImplementedException();
        }

        void IMessageHandler.HandlePrivateChat(Request request)
        {
            throw new NotImplementedException();
        }

        void IMessageHandler.HandleRegist(Request request)
        {
            throw new NotImplementedException();
        }

        void IMessageHandler.HandleUnblockSomeone(Request request)
        {
            throw new NotImplementedException();
        }

        void IMessageHandler.Start()
        {
            throw new NotImplementedException();
        }

        void IMessageHandler.Stop()
        {
            throw new NotImplementedException();
        }
    }
}
