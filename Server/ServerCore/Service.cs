using System;
using System.Net;
using Server.SocketAsyncCore;
using Server.Models;
using Common.Encryption;
using Server.ServerInterface;
using Common.Log;

namespace Server.ServerCore
{
    public class Service : SocketAsyncTcpServer, IServer
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

        void IServer.HandleBlockSomeone(Request request)
        {
            throw new NotImplementedException();
        }

        void IServer.HandleGetBlockList(Request request)
        {
            throw new NotImplementedException();
        }

        void IServer.HandleGetFriendList(Request request)
        {
            throw new NotImplementedException();
        }

        void IServer.HandleGetGroupMemberList(Request request)
        {
            throw new NotImplementedException();
        }

        void IServer.HandleGetUserData(Request request)
        {
            throw new NotImplementedException();
        }

        void IServer.HandleGroupChat(Request request)
        {
            throw new NotImplementedException();
        }

        void IServer.HandleLogin(Request request)
        {
            throw new NotImplementedException();
        }

        void IServer.HandlePrivateChat(Request request)
        {
            throw new NotImplementedException();
        }

        void IServer.HandleRegist(Request request)
        {
            throw new NotImplementedException();
        }

        void IServer.HandleUnblockSomeone(Request request)
        {
            throw new NotImplementedException();
        }

        void IServer.Start()
        {
            throw new NotImplementedException();
        }

        void IServer.Stop()
        {
            throw new NotImplementedException();
        }
    }
}
