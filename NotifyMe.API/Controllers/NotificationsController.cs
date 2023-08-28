using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.API.Controllers;

public class NotificationsController : Controller
{
    private readonly ILogger<NotificationsController> _logger;
    private readonly IEventLogger _eventLogger;
    private readonly INotificationService _notificationService;
    private DatabaseContext _db;

    public NotificationsController(
        ILogger<NotificationsController> logger,
        IEventLogger eventLogger,
        INotificationService notificationService,
        DatabaseContext db)
    {
        _logger = logger;
        _eventLogger = eventLogger;
        _notificationService = notificationService;
        _db = db;
    }

    [HttpPost]
    public IActionResult SendNotification(Notification notification)
    {
        _eventLogger.LogEvent(notification);
        _notificationService.SendNotification(notification);

        return RedirectToAction("");
    }

    [Authorize]
    public IActionResult Index()
    {
        // string currentUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        List<Notification> notifications = _db.Notifications.ToList();
        ViewBag.Notifications = notifications;
        return View(notifications);
    }

    // [Authorize]
    // public IActionResult Create(string idUser)
    // {
    //
    //     return View();
    // }
}