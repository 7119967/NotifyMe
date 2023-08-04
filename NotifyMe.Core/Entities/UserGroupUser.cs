namespace NotifyMe.Core.Entities
{
    public class UserGroupUser: BaseEntity
    {
        public int UserGroupUserId { get; set; }

        // Foreign key for User
        public int UserId { get; set; }
        public User? User { get; set; }

        // Foreign key for UserGroup
        public int UserGroupId { get; set; }
        public UserGroup? UserGroup { get; set; }
    }
}
