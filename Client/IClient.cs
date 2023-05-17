using System;
using Client.DataType;
using System.Threading.Tasks;


namespace Client.Interface
{
    interface IUser
    {
        /// <summary>
        /// ログイン
        /// </summary>
        /// <param name="username">ユーザネーム</param>
        /// <param name="password">パスワード</param>
        /// <returns></returns>
        public Task<Response> Login(string username, string password);

        /// <summary>
        /// 登録
        /// </summary>
        /// <param name="username">ユーザネーム</param>
        /// <param name="password">パスワード</param>
        /// <returns></returns>
        public Task<Response> Register(string username, string password);

        /// <summary>
        /// メッセージを送る
        /// </summary>
        public Response SendMsg(string username, string msg);
        public Response AddFriend(string targetUsername);
        public Response GetFriendList();
        public Response DelFriend(string targetUsername);
        public Response JoinGroup(string groupNum);
        public Response GetGroupList();
        public Response LeaveGroup();

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
}
