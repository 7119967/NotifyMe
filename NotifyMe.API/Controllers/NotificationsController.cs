using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
using NotifyMe.Core.Models.Notification;

namespace NotifyMe.API.Controllers;

public class NotificationsController : Controller
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<NotificationsController> _logger;
    private readonly IEventLogger _eventLogger;
    private readonly INotificationService _notificationService;

    public NotificationsController(
        UserManager<User> userManager,
        IMapper mapper,
        ILogger<NotificationsController> logger,
        IEventLogger eventLogger,
        INotificationService notificationService)
    {
        _userManager = userManager;
        _mapper = mapper;
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

    public async Task<IActionResult> Index()
    {
        var entities = await Task.Run(() => _notificationService.GetAllAsync().Result);
        var model = _mapper.Map<List<NotificationListViewModel>>(entities);
        await Task.Yield();
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
        return PartialView("PartialViews/CreatePartialView", new NotificationCreateViewModel());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NotificationCreateViewModel? model)
    {
        try
        {
            if (model != null)
            {
                var maxId = _notificationService.GetAllAsync().Result.Max(e => e.Id) ;
                var entity = _mapper.Map<NotificationCreateViewModel, Notification>(model);
                if (maxId is null)
                {
                    entity.Id = "1";
                }
                else 
                {
                    var newId = Convert.ToInt32(maxId) + 1;
                    entity.Id = newId.ToString(); 
                }
               
                await _notificationService.CreateAsync(entity);
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
        var entity = _notificationService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        var model = _mapper.Map<Notification, NotificationDetailsViewModel>(entity);

        await Task.Yield();
        return PartialView("PartialViews/DetailsPartialView", model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(string entityId)
    {
        var entity = _notificationService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<Notification, NotificationEditViewModel>(entity);
        
        await Task.Yield();
        return PartialView("PartialViews/EditPartialView", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(NotificationEditViewModel model)
    {
        var entity = _mapper.Map<NotificationEditViewModel, Notification>(model);
        try
        {
            await _notificationService.UpdateAsync(entity);
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
        var entity = _notificationService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<Notification, NotificationDeleteViewModel>(entity);
        
        await Task.Yield();
        return PartialView("PartialViews/DeletePartialView", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(NotificationDeleteViewModel model)
    {
        var entity = _notificationService.GetAllAsync().Result.FirstOrDefault(e => e.Id == model.Id);
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