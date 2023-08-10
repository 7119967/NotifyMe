namespace NotifyMe.Core.Entities
{
    public class NotificationGroup
    {
        public int Id { get; set; }
        public int NotificationId { get; set; } // Foreign key to Notification model
        public int GroupId { get; set; } // Foreign key to UserGroup model
    }
}
