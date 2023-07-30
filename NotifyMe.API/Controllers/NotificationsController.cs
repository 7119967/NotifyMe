using Microsoft.AspNetCore.Mvc;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.API.Controllers;

public class NotificationsController : Controller
{
    private readonly ILogger<NotificationsController> _logger;
    private readonly IEventLogger _eventLogger;
    private readonly NotificationService _notificationService;

    public NotificationsController(
        ILogger<NotificationsController> logger,
        IEventLogger eventLogger,
        NotificationService notificationService)
    {
        _logger = logger;
        _eventLogger = eventLogger;
        _notificationService = notificationService;
    }

    [HttpPost]
    public IActionResult SendNotification(Notification notification)
    {
        _eventLogger.LogEvent(notification);
        _notificationService.SendNotification(notification);
        
        return RedirectToAction("");
    }
}