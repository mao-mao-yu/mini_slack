using Server.ServerInterface;
using Server.Models;

namespace Server.ServerMessageHandler
{
    public class MessageHandler : MessageHandleMethods
    {
        public void HandleMessage(Request request)
        {
            GetAndInvoke(request);
        }

        public override void HandleLogin(Request request)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleRegist(Request request)
        {
            throw new System.NotImplementedException();
        }

        public override void HandlePrivateChat(Request request)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleGroupChat(Request request)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleGetFriendList(Request request)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleGetGroupMemberList(Request request)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleBlockSomeone(Request request)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleUnblockSomeone(Request request)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleGetUserData(Request request)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleGetBlockList(Request request)
        {
            throw new System.NotImplementedException();
        }

        public override void Broadcast(int groupID)
        {
            throw new System.NotImplementedException();
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }

        public override void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}
