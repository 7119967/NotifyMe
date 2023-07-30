namespace NotifyMe.Core.Entities
{
    public class UserGroupUser
    {
        public int UserGroupId { get; set; } 
        public UserGroup? UserGroup { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
