﻿namespace NotifyMe.Core.Models.Notification;

public class NotificationListViewModel
{
    public string? Id { get; set; }
    public string? Recipient { get; set; }
    public string? Message { get; set; }
    public string? ChangedElements { get; set; }
}