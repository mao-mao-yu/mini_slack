using Server.Models;
using Server.ServerInterface;
using System.Reflection;
using Common;
using Common.Log;
using System;

namespace Server.ServerMessageHandler
{
    public abstract class MessageHandleMethods : IMessageHandler
    {
        private readonly Dictionary<string, Action<Request>> messageHandlers;
       
        public MessageHandleMethods()
        {
            messageHandlers = new Dictionary<string, Action<Request>>()
            {
                { "BlockSomeone", HandleBlockSomeone },
                { "GetBlockList", HandleGetBlockList },
                { "GetFriendList", HandleGetFriendList },
                { "GetGroupMemberList", HandleGetGroupMemberList },
                { "GetUserData", HandleGetUserData },
                { "GroupChat", HandleGroupChat },
                { "Login", HandleLogin },
                { "PrivateChat", HandlePrivateChat },
                { "Regist", HandleRegist },
                { "UnblockSomeone", HandleUnblockSomeone }
            };
        }

        public void GetAndInvoke(Request request)
        {
            string methodType = request.Get("action");
            if (messageHandlers.ContainsKey(methodType))
            {
                Action<Request> handler = messageHandlers[methodType];
                handler.Invoke(request);
            }
            else
            {
                Logger.FERROR($"Error message...{request}");
            }
        }

        public abstract void Broadcast(int groupID);

        public abstract void HandleBlockSomeone(Request request);

        public abstract void HandleGetBlockList(Request request);

        public abstract void HandleGetFriendList(Request request);

        public abstract void HandleGetGroupMemberList(Request request);

        public abstract void HandleGetUserData(Request request);

        public abstract void HandleGroupChat(Request request);

        public abstract void HandleLogin(Request request);

        public abstract void HandlePrivateChat(Request request);

        public abstract void HandleRegist(Request request);

        public abstract void HandleUnblockSomeone(Request request);

        public abstract void Start();

        public abstract void Stop();
    }
}
