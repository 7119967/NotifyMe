using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.API.Controllers;

[Authorize(Roles = "admin, user")]
public class NotificationsController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<NotificationsController> _logger;
    private readonly INotificationService? _notificationService;
    private readonly INotificationUserService? _notificationUserService;

    public NotificationsController(IServiceProvider serviceProvider,
        UserManager<User> userManager,
        ILogger<NotificationsController> logger
        )
    {
        _userManager = userManager;
        _logger = logger;
        var scope = serviceProvider.GetService<IServiceScopeFactory>()?.CreateScope();
        _notificationService = scope?.ServiceProvider.GetRequiredService<INotificationService>();
        _notificationUserService = scope?.ServiceProvider.GetRequiredService<INotificationUserService>();
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User)!);
        var notifications = await GetListNotificationsAsync();
        var notificationUsers = await GetListNotificationUsersAsync();

        if (await _userManager.IsInRoleAsync(user!, "admin"))
        {
            return View(notifications);
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = from m in notifications
            join u in notificationUsers on m.Id equals u.NotificationId
            where u.UserId == userId
            select m;
        
        return View(result);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        return PartialView("PartialViews/CreatePartialView", new Notification());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Notification? model)
    {
        try
        {
            if (model != null)
            {
                var sequence = await GetListNotificationsAsync();
                var newId = Helpers.GetNewIdEntity(sequence);
                model.Id = newId.ToString();
                await _notificationService!.CreateAsync(model);
            }
            
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message, e);
            return RedirectToAction("Index");
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> Details(string entityId)
    {
        var entity = await GetNotificationAsync(entityId);
        if (entity is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        return PartialView("PartialViews/DetailsPartialView", entity);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(string entityId)
    {
        var entity = await GetNotificationAsync(entityId);
        if (entity == null)
        {
            return NotFound();
        }

        return PartialView("PartialViews/EditPartialView", entity);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(Notification model)
    {
        try
        {
            await _notificationService!.UpdateAsync(model);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string entityId)
    {
        var entity = await GetNotificationAsync(entityId);
        if (entity == null)
        {
            return NotFound();
        }

        return PartialView("PartialViews/DeletePartialView", entity);
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(Notification model)
    {
        var entity = await GetNotificationAsync(model.Id);
        if (entity == null)
        {
            return NotFound();
        }

        try
        {
            await _notificationService!.DeleteAsync(entity.Id);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }

    private Task<Notification?> GetNotificationAsync(string entityId)
    {
        return _notificationService!
            .AsQueryable()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    private Task<List<Notification>> GetListNotificationsAsync()
    {
        return _notificationService!
            .AsQueryable()
            .ToListAsync();
    }

    private Task<List<NotificationUser>> GetListNotificationUsersAsync()
    {
        return _notificationUserService!
            .AsQueryable()
            .ToListAsync();
    }
}