using System;
using Client.DataType;
using System.Threading.Tasks;


namespace Client.IClient
{
    interface IUser
    {
        /// <summary>
        /// ログイン
        /// </summary>
        /// <param name="username">ユーザネーム</param>
        /// <param name="password">パスワード</param>
        /// <returns></returns>
        public Task PerformLogin(string username, string password);

        /// <summary>
        /// 登録
        /// </summary>
        /// <param name="username">ユーザネーム</param>
        /// <param name="password">パスワード</param>
        /// <returns></returns>
        public Task Register(string username, string password);

        /// <summary>
        /// メッセージを送る
        /// </summary>
        //public Task<Response> SendMsg(string username, string msg);
        //public Task<Response> AddFriend(string targetUsername);
        //public Task<Response> GetFriendList();
        //public Task<Response> DelFriend(string targetUsername);
        //public Task<Response> JoinGroup(string groupNum);
        //public Task<Response> GetGroupList();
        //public Task<Response> LeaveGroup();

    }

    interface IAdmin:IUser
    {
        public Response ChangePassword(string username);

        public Response ChangeNickname(string username);

        public Response ChangeUsername(string username);

        public Response GetChatHistory(string username);

        public Response AddUserToGroup(string username, string groupID);

        public Response DelUserFromGroup(string username, string groupID);
    }

    interface ISocket
    {
        public void Send();
        public void Recv();
    }
}
