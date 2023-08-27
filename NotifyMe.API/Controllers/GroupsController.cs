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
        // await Task.Yield();
        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
        return View();
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
                var group = _mapper.Map<GroupCreateViewModel, Group>(model);
                await _groupService.CreateAsync(group);
            }
            var data = _mapper.Map<List<GroupIndexViewModel>>(_groupService.GetAllAsync().Result);
            return View("Index", data);
        }
        catch
        {
            return Json(model);
        }
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Details(int entityId)
    {
        var group = _groupService.GetAllAsync().Result.FirstOrDefault(group => group.Id == entityId);
        if (group is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        var model = _mapper.Map<Group, GroupDetailsViewModel>(group);

        await Task.Yield();
        return View(model);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit(int entityId)
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
}