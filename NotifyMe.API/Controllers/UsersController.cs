using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using NotifyMe.API.ViewModels;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Core.Models;
using NotifyMe.Infrastructure.Context;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.API.Controllers;

public class UsersController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private DatabaseContext _db;
    private readonly IHostEnvironment _environment;
    private readonly UploadFileService _uploadFileService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UsersController(UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<User> signInManager,
        DatabaseContext db,
        IHostEnvironment environment,
        IMapper mapper,
        IUserService userService,
        UploadFileService uploadFileService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _db = db;
        _environment = environment;
        _mapper = mapper;
        _userService = userService;
        _uploadFileService = uploadFileService;
    }

    [Authorize]
    [HttpGet]
    public IActionResult Index(string search)
    {
        List<User> users = _userService.GetAllAsync().Result.Where(t =>
            t.UserName!.Contains(search)
            || t.FirstName!.Contains(search)
            || t.LastName!.Contains(search)
            || t.Email!.Contains(search)
            || t.PhoneNumber!.Contains(search)
            || t.Info!.Contains(search)).ToList(); ;

        return View(users);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Profile(string idUser)
    {
        var user = _userService.GetAllAsync().Result.FirstOrDefault(u => u.Id == idUser);
        if (user is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        var model = _mapper.Map<User, ProfileViewModel>(user);

        return View(model);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Edit(string idCurrentUser)
    {
        var user = _userService.GetAllAsync().Result.FirstOrDefault(u => u.Id == idCurrentUser);
        var model = _mapper.Map<User, EditProfileViewModel>(user);

        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.UserName != null)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var editUser = _mapper.Map<EditProfileViewModel, User>(model);

                var result = await _userManager.UpdateAsync(editUser);
                if (result.Succeeded)
                {
                    return RedirectToAction("Profile", "Users");
                }
            }
        }

        return View(model);
    }
}