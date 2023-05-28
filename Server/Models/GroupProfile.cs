namespace Server.Models
{
    class GroupProfile
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public Common.List<int> MemberList { get; set; }
    }
}
