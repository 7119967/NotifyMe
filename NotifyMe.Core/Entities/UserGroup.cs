namespace NotifyMe.Core.Entities
{
    public class UserGroup
    {
        public int UserGroupId { get; set; }

        public string Name { get; set; }

        // Navigation property to UserGroupUsers
        public ICollection<UserGroupUser> UserGroupUsers { get; set; }
    }
}