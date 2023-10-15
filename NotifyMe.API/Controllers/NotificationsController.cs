using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        // var model = _mapper.Map<List<NotificationListViewModel>>(entities);
        await Task.Yield();
        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User)!);
        if (await _userManager.IsInRoleAsync(user!, "admin"))
        {
            var entities = _notificationService!.AsEnumerable().ToList();
            return View(entities);
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var notifications = from m in _notificationService!.AsEnumerable().ToList()
            join u in _notificationUserService!.AsEnumerable().ToList() on m.Id equals u.NotificationId
            where u.UserId == userId
            select m;
        
        return View(notifications);
    }
    
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        // var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User)!);
        await Task.CompletedTask;
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
                var sequence = _notificationService!.GetAllAsync().Result;
                var newId = Helpers.GetNewIdEntity(sequence);
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