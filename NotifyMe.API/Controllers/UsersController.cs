using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Core.Models.User;
using NotifyMe.Infrastructure.Context;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.API.Controllers;

[Authorize(Roles = "admin")]
public class UsersController : Controller
{
    private readonly IMapper _mapper;
    private readonly IHostEnvironment _env;
    private readonly IUserService _userService;
    private readonly UploadFileService _uploader;
    private readonly UserManager<User> _userManager;
    private readonly DatabaseContext _databaseContext;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

  
    public UsersController(UserManager<User> userManager,
        IMapper mapper,
        IUserService userService,
        IHostEnvironment environment,
        SignInManager<User> signInManager,
        UploadFileService uploadFileService,
        RoleManager<IdentityRole> roleManager,
        DatabaseContext databaseContext)
    {
        _mapper = mapper;
        _env = environment;
        _userService = userService;
        _userManager = userManager;
        _roleManager = roleManager;
        _uploader = uploadFileService;
        _signInManager = signInManager;
        _databaseContext = databaseContext;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var entities = await GetUsersViaGroup();
        return View(entities);
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Groups = GetGroups();
        return PartialView("PartialViews/CreatePartialView", new UserCreateViewModel());
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Search(string search)
    {
        var searchUsers = await GetSearchedUsers(search);
        var users = _mapper.Map<List<UserListViewModel>>(searchUsers);
        return RedirectToAction("Index", users);
    }


    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateViewModel model)
    {
        try
        {
            if (model == null) return RedirectToAction("Index");
            var user = await GetUserByUserNameAsync(model.UserName!);
            if (user != null)
            {
                BadRequest($"User {model.UserName} exists. Change the UserName");
                return RedirectToAction("Index");
            }
                
            var entity = _mapper.Map<UserCreateViewModel, User>(model);
            entity.Id = Guid.NewGuid().ToString();
            entity.Avatar = GetAvatorPath(model);
            var result = await _userManager.CreateAsync(entity, model.Password!);
            if (!result.Succeeded) return RedirectToAction("Index");
            await _userManager.AddToRoleAsync(entity, "user");

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
        var entity = await GetUserByIdAsync(entityId);
        if (entity is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        ViewBag.Groups = GetGroups();

        var model = _mapper.Map<User, UserDetailsViewModel>(entity);
        return PartialView("PartialViews/DetailsPartialView", model);
    }

    [Authorize]    
    [HttpGet]
    public async Task<IActionResult> Edit(string entityId)
    {
        var entity = await GetUserByIdAsync(entityId);
        if (entity == null)
        {
            return NotFound();
        }

        ViewBag.Groups = GetGroups();

        var model = _mapper.Map<User, UserEditViewModel>(entity);
        return PartialView("PartialViews/EditPartialView", model);
    }

    [Authorize] 
    [HttpPost]
    public async Task<IActionResult> Edit(UserEditViewModel model)
    {
        model.Avatar = GetAvatorPath(model);
        var user = await _userManager.FindByNameAsync(model.UserName!);
        if (user == null) return NotFound();
        var editUser = _mapper.Map<User, UserEditViewModel>(user);
        var target = _mapper.Map<UserEditViewModel, User>(editUser);
        var source = _mapper.Map<UserEditViewModel, User>(model);
        _userService.ApplyChanges(source, target);
        try
        {
            await _userService.UpdateAsync(target);
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
        var entity = await GetUserByIdAsync(entityId);
        if (entity == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<User, UserDeleteViewModel>(entity);
        return PartialView("PartialViews/DeletePartialView", model);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(UserDeleteViewModel model)
    {
        var entity = await GetUserByIdAsync(model.Id!);
        if (entity == null)
        {
            return NotFound();
        }

        try
        {
            await _userService.DeleteAsync(entity.Id);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }

    private SelectList GetGroups()
    {
        return new SelectList(_databaseContext.Groups, "Id", "Name");
    }
    
    private Task<User?> GetUserByIdAsync(string entityId)
    {
        return _userService!
            .AsQueryable()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }   
    
    private Task<User?> GetUserByUserNameAsync(string userName)
    {
        return _userService!
            .AsQueryable()
            .FirstOrDefaultAsync(e => e.UserName == userName);
    }

    private Task<List<User>> GetListUsersAsync()
    {
        return _userService!
            .AsQueryable()
            .ToListAsync();
    }

    private Task<List<User>> GetUsersViaGroup()
    {
        return _userService!
            .AsQueryable()
            .Include(e => e.Group)
            .ToListAsync();
    }


    private Task<List<User>> GetSearchedUsers(string search)
    {
        return _userService
            .AsQueryable()
            .Where(t =>
                    t.UserName!.Contains(search) ||
                    t.FirstName!.Contains(search) ||
                    t.LastName!.Contains(search) ||
                    t.Email!.Contains(search) ||
                    t.PhoneNumber!.Contains(search) ||
                    t.Info!.Contains(search))
            .ToListAsync();
    }

    private string GetAvatorPath(object item)
    {
        switch (item)
        {
            case UserEditViewModel model:
                return model.File != null ?
                   Helpers.GetPathImage(_env, _mapper, _uploader, model) :
                   string.Empty;
            case UserCreateViewModel model:
                return model.File != null ?
                   Helpers.GetPathImage(_env, _mapper, _uploader, model) :
                   string.Empty;
            default:
                return string.Empty;
      
        }
    }
}