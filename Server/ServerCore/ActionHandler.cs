using System;
using System.Collections.Generic;
using Server.SocketCore;
using Server.Data;
using System.Net;

namespace Server.ServerCore
{
    public class ActionHandler: SocketAsyncTcpServer
    {
        delegate void MyDelegete(Request request);

        //private readonly Dictionary<string, MyDelegete> methodDict = new Dictionary<string, MyDelegete>()
        //{
        //    {"login",           LoginAction},
        //    {"register",        RegisterAction},
        //    {"privatechat",     PrivateChatAction},
        //    {"groupchat",       GroupChatAction},
        //    {"getfriendslist",  GetFriendsListAction},
        //    {"getgroupmember",  GetGroupMemberAction},
        //    {"block",           BlockAction},
        //};

        #region ctor
        public ActionHandler(int listenPort, int maxClient) : base(IPAddress.Any, listenPort, maxClient)
        {
        }

        public ActionHandler(IPAddress localIPAddress, int listenPort, int maxClient) : base(localIPAddress, listenPort, maxClient)
        {
        }

        public ActionHandler(IPEndPoint localEP, int maxClient) : base(localEP, maxClient)
        {
        }
        #endregion
        protected override void HandleMessage(string data)
        {

        }
    }
}
