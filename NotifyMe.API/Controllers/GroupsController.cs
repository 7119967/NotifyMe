using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Enums;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Core.Models.Group;
using NotifyMe.Infrastructure.Context;


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
        var entities = await Task.Run(() => _groupService.GetAllAsync().Result);
        var model = _mapper.Map<List<GroupListViewModel>>(entities);
        await Task.Yield();
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var priorityTypeValues = Enum.GetValues(typeof(PriorityType)).Cast<PriorityType>();
        var priorities = new SelectList(priorityTypeValues);
        ViewBag.Priorities = priorities;
        
        //var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
        await Task.Yield();
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
                var seequence = _groupService.GetAllAsync().Result;
                var size = seequence.Count();
                int[] anArray = new int[size];
                if (size == 0)
                {
                    model.Id = "1";
                }
                else
                {
                    var index = 0;
                    foreach (var configuration in seequence)
                    {
                        anArray[index] = Convert.ToInt32(configuration.Id);
                        index++;
                    }

                    int maxValue = anArray.Max();
                    var newId = Convert.ToInt32(maxValue) + 1;
                    model.Id = newId.ToString();
                }

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
        var entity = _groupService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        var priorityTypeValues = Enum.GetValues(typeof(PriorityType)).Cast<PriorityType>();
        var priorities = new SelectList(priorityTypeValues);
        ViewBag.Priorities = priorities;

        //var model = _mapper.Map<Group, GroupDetailsViewModel>(entity);
        entity.Users = _databaseContext.Users.Where(e => e.GroupId == entityId).ToList();

        await Task.Yield();
        return PartialView("PartialViews/DetailsPartialView", entity);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(string entityId)
    {
        var entity = _groupService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity == null)
        {
            return NotFound();
        }
        
        var priorityTypeValues = Enum.GetValues(typeof(PriorityType)).Cast<PriorityType>();
        var priorities = new SelectList(priorityTypeValues);
        ViewBag.Priorities = priorities;
        
        var model = _mapper.Map<Group, GroupEditViewModel>(entity);
        
        await Task.Yield();
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
        var entity = _groupService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<Group, GroupDeleteViewModel>(entity);
        
        await Task.Yield();
        return PartialView("PartialViews/DeletePartialView", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(GroupDeleteViewModel model)
    {
        var entity = _groupService.GetAllAsync().Result.FirstOrDefault(e => e.Id == model.Id);
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
}