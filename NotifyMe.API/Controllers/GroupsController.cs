using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Core.Models;

namespace NotifyMe.API.Controllers;

public class GroupsController : Controller
{
    private readonly IMapper _mapper;
    private readonly IGroupService _groupService;
    private readonly UserManager<User> _userManager;
    public GroupsController(UserManager<User> userManager, IMapper mapper, IGroupService groupService)
    {
        _userManager = userManager;
        _groupService = groupService;
        _mapper = mapper;
    }
    public async Task<IActionResult> Index()
    {
        var groups = await Task.Run(() => _groupService.GetAllAsync().Result);
        var model = _mapper.Map<List<GroupIndexViewModel>>(groups);
        await Task.Yield();
        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
        return PartialView("PartialViews/CreatePartialView", new GroupCreateViewModel());
    }
    
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GroupCreateViewModel? model)
    {
        try
        {
            if (model != null)
            {
                var maxId = _groupService.GetAllAsync().Result.Max(g => g.Id) ;
                var group = _mapper.Map<GroupCreateViewModel, Group>(model);
                if (maxId is null)
                {
                    group.Id = "1";
                }
                else 
                {
                    var newId = Convert.ToInt32(maxId) + 1;
                    group.Id = newId.ToString(); 
                }
               
                await _groupService.CreateAsync(group);
            }
            
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message, e);
            return RedirectToAction("Index");
        }
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Details(string entityId)
    {
        var group = _groupService.GetAllAsync().Result.FirstOrDefault(group => group.Id == entityId);
        if (group is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        var model = _mapper.Map<Group, GroupDetailsViewModel>(group);

        await Task.Yield();
        return PartialView("PartialViews/DetailsPartialView", model);
    }
    

    [Authorize]    
    [HttpGet]
    public async Task<IActionResult> Edit(string entityId)
    {
        var group = _groupService.GetAllAsync().Result.FirstOrDefault(group => group.Id == entityId);
        if (group == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<Group, GroupEditViewModel>(group);
        
        await Task.Yield();
        return PartialView("PartialViews/EditPartialView", model);
    }
    
    [Authorize] 
    [HttpPost]
    public async Task<IActionResult> Edit(GroupEditViewModel model)
    {
        var target = _mapper.Map<GroupEditViewModel, Group>(model);
        try
        {
            await _groupService.UpdateAsync(target);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }
   
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Delete(string entityId)
    {
        var group = _groupService.GetAllAsync().Result.FirstOrDefault(group => group.Id == entityId);
        if (group == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<Group, GroupDeleteViewModel>(group);
        
        await Task.Yield();
        return PartialView("PartialViews/DeletePartialView", model);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(GroupDeleteViewModel model)
    {
        var group = _groupService.GetAllAsync().Result.FirstOrDefault(group => group.Id == model.Id);
        if (group == null)
        {
            return NotFound();
        }

        try
        {
            await _groupService.DeleteAsync(group.Id);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }
}