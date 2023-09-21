using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Core.Models.Notification;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.API.Controllers;

[Authorize(Roles = "admin, user")]
public class NotificationsController : Controller
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<NotificationsController> _logger;
    private readonly IEventLogger _eventLogger;
    private readonly INotificationService? _notificationService;
    private readonly INotificationUserService? _notificationUserService;
    // private readonly DatabaseContext _databaseContext;

    public NotificationsController(IServiceProvider serviceProvider,
        UserManager<User> userManager,
        IMapper mapper,
        ILogger<NotificationsController> logger
        // IEventLogger eventLogger,
        // INotificationService notificationService,
        // DatabaseContext databaseContext
        )
    {
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
        // _eventLogger = eventLogger;
        // _notificationService = notificationService;
        // _databaseContext = databaseContext;
        var scope = serviceProvider.GetService<IServiceScopeFactory>()?.CreateScope();
        _notificationService = scope?.ServiceProvider.GetRequiredService<INotificationService>();
        _notificationUserService = scope?.ServiceProvider.GetRequiredService<INotificationUserService>();
    }

    [HttpPost]
    public IActionResult SendNotification(Notification notification)
    {
        _eventLogger.LogEvent(notification);
        // _notificationService.SendNotification(notification);

        return RedirectToAction("");
    }

    public async Task<IActionResult> Index()
    {
        // var model = _mapper.Map<List<NotificationListViewModel>>(entities);
        await Task.Yield();
        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User)!);
        if (await _userManager.IsInRoleAsync(user!, "admin"))
        {
            var entities = await Task.Run(() => _notificationService!.GetAllAsync().Result);
            return View(entities);
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var notifications = from m in _notificationService!.AsQueryable()
            join u in _notificationUserService?.AsQueryable() on m.Id equals u.NotificationId
            where u.UserId == userId
            select m;
        
        return View(notifications);
    }
    
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        // var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User)!);
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
                var sequence = await _notificationService!.GetAllAsync();
                var newId = (sequence?.Any() == true) ? (sequence.Max(e => Convert.ToInt32(e.Id)) + 1) : 1;

                model.Id = newId.ToString();
               
                await _notificationService.CreateAsync(model);
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
        var entity = _notificationService!.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        // var model = _mapper.Map<Notification, NotificationDetailsViewModel>(entity);

        await Task.Yield();
        return PartialView("PartialViews/DetailsPartialView", entity);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(string entityId)
    {
        var entity = _notificationService!.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity == null)
        {
            return NotFound();
        }

        // var model = _mapper.Map<Notification, NotificationEditViewModel>(entity);
        
        await Task.Yield();
        return PartialView("PartialViews/EditPartialView", entity);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(Notification model)
    {
        // var entity = _mapper.Map<NotificationEditViewModel, Notification>(model);
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
        var entity = _notificationService!.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity == null)
        {
            return NotFound();
        }

        // var model = _mapper.Map<Notification, NotificationDeleteViewModel>(entity);
        
        await Task.Yield();
        return PartialView("PartialViews/DeletePartialView", entity);
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(Notification model)
    {
        var entity = _notificationService!.GetAllAsync().Result.FirstOrDefault(e => e.Id == model.Id);
        if (entity == null)
        {
            return NotFound();
        }

        try
        {
            await _notificationService.DeleteAsync(entity.Id);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }
}