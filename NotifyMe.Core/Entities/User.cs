namespace NotifyMe.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        // Navigation property to UserGroupUsers
        public ICollection<UserGroupUser>? UserGroupUsers { get; set; }
    }
   
}
