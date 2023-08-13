namespace NotifyMe.Core.Entities
{
    public class Group: BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        //public virtual List<UserGroupUser> UserGroupUser { get; set; } = new List<UserGroupUser>();
        
        // public virtual List<User> Users { get; set; } = new();
    }
}