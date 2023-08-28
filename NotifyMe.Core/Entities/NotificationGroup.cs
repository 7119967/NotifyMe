namespace NotifyMe.Core.Entities
{
    public class NotificationGroup
    {
        public int Id { get; set; }
        public int NotificationId { get; set; } // Foreign key to Notification model
        public string? GroupId { get; set; } // Foreign key to UserGroup model
    }
}
