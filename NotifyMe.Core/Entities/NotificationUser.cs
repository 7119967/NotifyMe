namespace NotifyMe.Core.Entities;

public class NotificationUser: BaseEntity
{
    public string? NotificationId { get; set; }
    public virtual Notification? Notification { get; set; }

    public string? UserId { get; set; }
    public virtual User? User { get; set; }
}