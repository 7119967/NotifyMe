using Microsoft.AspNetCore.Identity;

namespace NotifyMe.Core.Entities
{
    public class User : IdentityUser
    {
        public override string?  UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public override string? Email { get; set; }
        public override string? PhoneNumber { get; set; }
        public string? Info { get; set; }
        public string? Avatar { get; set; }
        
        //public virtual List<UserGroupUser> UserGroupUser { get; set; } = new List<UserGroupUser>();
        
        public virtual List<Group> Groups { get; set; } = new();
    }
}
