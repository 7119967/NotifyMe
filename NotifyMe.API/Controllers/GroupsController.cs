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
        return PartialView("PartialViews/DetailsPartialView", model);
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
    
    [HttpPost]
    [Authorize]
    public IActionResult Edit(EditProfileViewModel model)
    {
        var source = new Group{};
        
        if (ModelState.IsValid)
        {
            if (model.UserName != null)
            {
                var user = _userManager.FindByNameAsync(model.UserName).Result;
                if (user == null)
                {
                    return NotFound();
                }

                // var editUser = _mapper.Map<User, EditProfileViewModel>(user);
                // var target = _mapper.Map<EditProfileViewModel, User>(editUser);
                // source = _mapper.Map<EditProfileViewModel, User>(model);
                //
                // _userService.ApplyChanges(source, target);

                try
                {
                    // _userService.Update(target);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return BadRequest();
                }
            }
        }
                
        var obj = _mapper.Map<Group, GroupEditViewModel>(source);
        return View("PartialViews/EditPartialView", obj);
    }
   
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Delete(int entityId)
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
    [HttpDelete]
    public async Task<IActionResult> Delete(GroupDeleteViewModel model)
    {
        var group = _groupService.GetAllAsync().Result.FirstOrDefault(group => group.Id == model.Id);
        if (group == null)
        {
            return NotFound();
        }

        try
        {
            await _groupService.DeleteAsync(group.Id.ToString());
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }
}