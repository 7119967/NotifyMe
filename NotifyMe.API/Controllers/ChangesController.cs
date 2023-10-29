using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Enums;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.API.Controllers;

[Authorize(Roles = "admin, user")]
public class ChangesController  : Controller
{
    private readonly IChangeService _changeService;
    
    public ChangesController(IChangeService changeService)
    {
        _changeService = changeService;
    }

    public async Task<IActionResult> Index()
    {
        var entities = await GetListChangeEventsAsync();
        return View(entities);
    }
    
    [HttpGet]
    public async Task<IActionResult> Details(string entityId)
    {
        var entity = await GetChangeByIdAsync(entityId);
        if (entity is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        return PartialView("PartialViews/DetailsPartialView", entity);
    }


    [HttpGet]
    public IActionResult Create()
    {
        var changeTypeValues = Enum.GetValues(typeof(ChangeType)).Cast<ChangeType>();
        var changeTypes = new SelectList(changeTypeValues);
        ViewBag.ChangeTypes = changeTypes;

        return PartialView("PartialViews/CreatePartialView", new Change());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Change? model)
    {
        try
        {
            if (model != null)
            {
                var sequence = await GetListChangesAsync();
                var newId = Helpers.GetNewIdEntity(sequence);
                model.Id = newId.ToString();
                await _changeService.CreateAsync(model);
            }
            
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message, e);
            return RedirectToAction("Index");
        }
    }

    private Task<Change?> GetChangeByIdAsync(string entityId)
    {
        return _changeService!
            .AsQueryable()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    private Task<List<Change>> GetListChangesAsync()
    {
        return _changeService!
            .AsQueryable()
            .ToListAsync();
    }

    private Task<List<Change>> GetListChangeEventsAsync()
    {
        return _changeService!
            .AsQueryable()
            .Include(e => e.Event)
            .ToListAsync();
    }
}