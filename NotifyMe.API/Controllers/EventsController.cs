using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.API.Controllers;

[Authorize(Roles = "admin")]
public class EventsController : Controller
{
    private readonly IEventService _eventService;
    
    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async Task<IActionResult> Index()
    {
        var entities = await GetEventViaConfiguration();
        return View(entities);
    }


    [HttpGet]
    public async Task<IActionResult> Details(string entityId)
    {
        var entity = await GetEvent(entityId);
        if (entity is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        return PartialView("PartialViews/DetailsPartialView", entity);
    }

    private Task<Event?> GetEvent(string entityId)
    {
        return _eventService!
            .AsQueryable()
            .Include(e => e.Changes)
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    private Task<List<Event>> GetEventViaConfiguration()
    {
        return _eventService
           .AsQueryable()
           .Include(e => e.Configuration)
           .ToListAsync();
    }
}