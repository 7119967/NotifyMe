using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Core.Models;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.API.Controllers;

public class UsersController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHostEnvironment _environment;
    private readonly UploadFileService _uploadFileService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UsersController(UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<User> signInManager,
        IHostEnvironment environment,
        IMapper mapper,
        IUserService userService,
        UploadFileService uploadFileService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _environment = environment;
        _mapper = mapper;
        _userService = userService;
        _uploadFileService = uploadFileService;
    }

    [Authorize]
    [HttpGet]
    public IActionResult Index()
    {
        var users = _mapper.Map<List<UserIndexViewModel>>(_userService.GetAllAsync().Result);

        return View(users);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Search(string search)
    {
        var searchUsers = _userService.GetAllAsync().Result.Where(t =>
            t.UserName!.Contains(search)
            || t.FirstName!.Contains(search)
            || t.LastName!.Contains(search)
            || t.Email!.Contains(search)
            || t.PhoneNumber!.Contains(search)
            || t.Info!.Contains(search)).ToList();

        var users = _mapper.Map<List<UserIndexViewModel>>(searchUsers);

        return RedirectToAction("Index", users);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Create(string entityId)
    {
        var user = _userService.GetAllAsync().Result.FirstOrDefault(u => u.Id == entityId);
        if (user is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }
        
        var model = new UserCreateViewModel
        {
            
        };

        return Json(model);
    }
    
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UserCreateViewModel model)
    {
        try
        {
            if (model != null)
            {
                var entity = _mapper.Map<UserCreateViewModel, User>(model);
                _userService.CreateAsync(entity);
            }
            var data = _mapper.Map<List<UserIndexViewModel>>(_userService.GetAllAsync().Result);
            return View("Index", data);
        }
        catch
        {
            return Json(model);
        }
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult Details(string entityId)
    {
        var user = _userService.GetAllAsync().Result.FirstOrDefault(u => u.Id == entityId);
        if (user is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        var model = _mapper.Map<User, UserDetailsViewModel>(user);

        return View(model);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Edit(string entityId)
    {
        var user = _userService.GetAllAsync().Result.FirstOrDefault(u => u.Id == entityId);
        if (user == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<User, EditProfileViewModel>(user);

        return View("PartialViews/EditPartialView", model);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Edit(EditProfileViewModel model)
    {
        var source = new User{};
        
        if (ModelState.IsValid)
        {
            if (model.UserName != null)
            {
                var user = _userManager.FindByNameAsync(model.UserName).Result;
                if (user == null)
                {
                    return NotFound();
                }

                var editUser = _mapper.Map<User, EditProfileViewModel>(user);
                var target = _mapper.Map<EditProfileViewModel, User>(editUser);
                source = _mapper.Map<EditProfileViewModel, User>(model);

                _userService.ApplyChanges(source, target);

                try
                {
                    _userService.Update(target);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return BadRequest();
                }
            }
        }
        
        var obj = _mapper.Map<User, EditProfileViewModel>(source);
        return View("PartialViews/EditPartialView", obj);
    }
    

    [Authorize]
    [HttpDelete]
    public IActionResult Delete(string entityId)
    {
        var user = _userService.GetAllAsync().Result.FirstOrDefault(u => u.Id == entityId);
        if (user == null)
        {
            return NotFound();
        }

        try
        {
            _userService.DeleteAsync(entityId);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }
}