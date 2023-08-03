namespace NotifyMe.Core.Entities
{
    public class UserGroupUser : BaseEntity
    {
        public int UserGroupId { get; set; } 
        public virtual UserGroup? UserGroup { get; set; }
        public int UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
