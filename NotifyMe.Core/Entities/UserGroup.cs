using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyMe.Core.Entities
{
    public class UserGroup
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<UserGroupUser> Users { get; set; } = new List<UserGroupUser>();
    }
}