namespace NotifyMe.Core.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<UserGroupUser> Groups { get; set; } = new List<UserGroupUser>();
    }
}
