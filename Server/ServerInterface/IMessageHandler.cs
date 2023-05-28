using Server.Models;

namespace Server.ServerInterface
{
    public interface IMessageHandler
    {
        // 登录
        public void HandleLogin(Request request);

        // 注册
        public void HandleRegist(Request request);

        // 私聊
        public void HandlePrivateChat(Request request);

        // 群聊
        public void HandleGroupChat(Request request);

        // 获取好友列表
        public void HandleGetFriendList(Request request);

        // 获取群员列表
        public void HandleGetGroupMemberList(Request request);

        // 添加到黑名单
        public void HandleBlockSomeone(Request request);

        // 移除出黑名单
        public void HandleUnblockSomeone(Request request);

        // 获取用户数据
        public void HandleGetUserData(Request request);

        // 获取黑名单
        public void HandleGetBlockList(Request request);

        // 广播
        public void Broadcast(int groupID);

        // 开启服务端
        public void Start();

        // 关闭服务端
        public void Stop();
    }
}
