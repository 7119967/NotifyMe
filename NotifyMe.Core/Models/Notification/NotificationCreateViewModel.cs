namespace NotifyMe.Core.Models.Notification;

public class NotificationCreateViewModel
{
    public string? Recipient { get; set; }
    public string? Message { get; set; }
    public string? ChangedElements { get; set; }
}