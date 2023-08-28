namespace NotifyMe.Core.Entities
{
    public class GroupUser : BaseEntity
    {
        public string? UserId { get; set; }
        // public virtual User User { get; set; }

        public string GroupId { get; set; }
        // public virtual Group Group { get; set; }

        //public virtual List<User> Users { get; set; } = new List<User>(); 
        //public virtual List<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    }
}
