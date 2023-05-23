using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Data
{
    public class GroupData
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public List<int> MemberList { get; set; }
    }
}
