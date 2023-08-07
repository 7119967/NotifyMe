namespace NotifyMe.Core.Entities
{
    public class User : BaseEntity
    {
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Info { get; set; }
        public string? Avatar { get; set; }
        public List<UserGroupUser> Groups { get; set; } = new List<UserGroupUser>();
    }
}
