using Common;

namespace Server.Models
{
    public class UserProfile
    {
        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Password { get; set; }

        public List<int> FriendsList { get; set; }

        public List<int> BlockList { get; set; }

        public List<int> GroupList { get; set; }
    }
}
