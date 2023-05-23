using System;
using System.Collections.Generic;
using Server.SocketCore;

namespace Server.ServerCore
{
    public class ActionHandler: SocketAsyncTcpServer
    {
        delegate void MyDelegete(Request request);

        private readonly Dictionary<string, MyDelegete> methodDict = new Dictionary<string, MyDelegete>()
        {
            {"login",           LoginAction},
            {"register",        RegisterAction},
            {"privatechat",     PrivateChatAction},
            {"groupchat",       GroupChatAction},
            {"getfriendslist",  GetFriendsListAction},
            {"getgroupmember",  GetGroupMemberAction},
            {"block",           BlockAction},
        };
    }
}
