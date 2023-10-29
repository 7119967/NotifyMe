using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Enums;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Infrastructure.Context;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.API.Controllers;

[Authorize(Roles = "admin")]
public class ConfigurationsController : Controller
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ConfigurationsController> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly DatabaseContext _databaseContext;

    public ConfigurationsController(
        UserManager<User> userManager,
        IMapper mapper,
        ILogger<ConfigurationsController> logger,
        IConfigurationService configurationService,
        DatabaseContext databaseContext)
    {
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
        _configurationService = configurationService;
        _databaseContext = databaseContext;
    }

    public async Task<IActionResult> Index()
    {
        var entities = await GetConfigurationViaGroup();
        return View(entities);
    }
    
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var  changeTypeValues = Enum.GetValues(typeof(ChangeType)).Cast<ChangeType>();
        var changeTypes = new SelectList(changeTypeValues);
        ViewBag.ChangeTypes = changeTypes;
        
        var priorityTypeValues = Enum.GetValues(typeof(PriorityType)).Cast<PriorityType>();
        var priorities = new SelectList(priorityTypeValues);
        ViewBag.Priorities = priorities;

        var groups = new SelectList(_databaseContext.Groups, "Id", "Name");
        ViewBag.Groups = groups;

        await Task.CompletedTask;
        return PartialView("PartialViews/CreatePartialView", new Configuration());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Configuration? model)
    {
        try
        {
            if (model != null)
            {
                var sequence = await GetListConfigurations();
                var newId = Helpers.GetNewIdEntity(sequence);
                model.Id = newId.ToString();
                await _configurationService.CreateAsync(model);
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
        var entity = await GetConfigurationViaGroupAndEvent(entityId);
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
        var entity = await GetConfiguration(entityId);
        if (entity == null)
        {
            return NotFound();
        }

        var changeTypeValues = Enum.GetValues(typeof(ChangeType)).Cast<ChangeType>();
        var changeTypes = new SelectList(changeTypeValues);
        ViewBag.ChangeTypes = changeTypes;

        var priorityTypeValues = Enum.GetValues(typeof(PriorityType)).Cast<PriorityType>();
        var priorities = new SelectList(priorityTypeValues);
        ViewBag.Priorities = priorities;

        var groups = new SelectList(_databaseContext.Groups, "Id", "Name");
        ViewBag.Groups = groups;

        return PartialView("PartialViews/EditPartialView", entity);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(Configuration model)
    {
        try
        {
            await _configurationService.UpdateAsync(model);
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
        var entity = await GetConfiguration(entityId);
        if (entity == null)
        {
            return NotFound();
        }

        return PartialView("PartialViews/DeletePartialView", entity);
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(Configuration model)
    {
        var entity = await GetConfiguration(model.Id);
        if (entity == null)
        {
            return NotFound();
        }

        try
        {
            await _configurationService.DeleteAsync(entity.Id);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }

    private Task<Configuration?> GetConfiguration(string entityId)
    {
        return _configurationService!
            .AsQueryable()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    private Task<List<Configuration>> GetListConfigurations()
    {
        return _configurationService!
            .AsQueryable()
            .ToListAsync();
    }

    private Task<List<Configuration>> GetConfigurationViaGroup()
    {
        return _configurationService
            .AsQueryable()
            .Include(e => e.Group)
            .ToListAsync();
    }

    private Task<Configuration?> GetConfigurationViaGroupAndEvent(string entityId)
    {
        return _configurationService!
            .AsQueryable()
            .Include(e => e.Group)
            .Include(e => e.Events)
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }
}