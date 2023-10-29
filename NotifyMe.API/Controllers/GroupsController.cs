using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Enums;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Core.Models.Group;
using NotifyMe.Infrastructure.Context;
using NotifyMe.Infrastructure.Services;


namespace NotifyMe.API.Controllers;

[Authorize(Roles = "admin")]
public class GroupsController : Controller
{
    private readonly IMapper _mapper;
    private readonly IGroupService _groupService;
    private readonly UserManager<User> _userManager;
    private readonly DatabaseContext _databaseContext;
    public GroupsController(UserManager<User> userManager, 
        IMapper mapper, 
        IGroupService groupService,
        DatabaseContext databaseContext)
    {
        _userManager = userManager;
        _groupService = groupService;
        _mapper = mapper;
        _databaseContext = databaseContext;
    }
    public async Task<IActionResult> Index()
    {
        var entities = await GetListGroups();
        var model = _mapper.Map<List<GroupListViewModel>>(entities);
        return View(model);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Priorities = GetPriorities();
        return PartialView("PartialViews/CreatePartialView", new GroupCreateViewModel());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GroupCreateViewModel? model)
    {
        try
        {
            if (model != null)
            {
                var sequence = await GetListGroups();
                var newId = Helpers.GetNewIdEntity(sequence);
                model.Id = newId.ToString();
                var entity = _mapper.Map<GroupCreateViewModel, Group>(model);
                await _groupService.CreateAsync(entity);
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
        var entity = await GetGroup(entityId);
        if (entity is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        ViewBag.Priorities = GetPriorities();
        return PartialView("PartialViews/DetailsPartialView", entity);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(string entityId)
    {
        var entity = await GetGroup(entityId);
        if (entity == null)
        {
            return NotFound();
        }

        ViewBag.Priorities = GetPriorities();
        var model = _mapper.Map<Group, GroupEditViewModel>(entity);
        return PartialView("PartialViews/EditPartialView", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(GroupEditViewModel model)
    {
        var entity = _mapper.Map<GroupEditViewModel, Group>(model);
        try
        {
            await _groupService.UpdateAsync(entity);
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
        var entity = await GetGroup(entityId);
        if (entity == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<Group, GroupDeleteViewModel>(entity);
        return PartialView("PartialViews/DeletePartialView", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(GroupDeleteViewModel model)
    {
        var entity = await GetGroup(model.Id!);
        if (entity == null)
        {
            return NotFound();
        }

        try
        {
            await _groupService.DeleteAsync(entity.Id);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }

    private Task<List<Group>> GetListGroups()
    {
        return _groupService!
            .AsQueryable()
            .ToListAsync();
    }

    private Task<Group?> GetGroup(string entityId)
    {
        return _groupService!
            .AsQueryable()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    private static SelectList GetPriorities()
    {
        return new SelectList(GetPriorityTypeValues());
    }

    private static IEnumerable<PriorityType> GetPriorityTypeValues()
    {
        return Enum.GetValues(typeof(PriorityType)).Cast<PriorityType>();
    }
}