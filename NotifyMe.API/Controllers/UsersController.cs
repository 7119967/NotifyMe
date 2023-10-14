using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Core.Models;
using NotifyMe.Core.Models.User;
using NotifyMe.Infrastructure.Context;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.API.Controllers;

[Authorize(Roles = "admin")]
public class UsersController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHostEnvironment _environment;
    private readonly UploadFileService _uploadFileService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    // private readonly IUnitOfWork? _unitOfWork;
    private readonly DatabaseContext _databaseContext;

  
    public UsersController(UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<User> signInManager,
        IHostEnvironment environment,
        IMapper mapper,
        IUserService userService,
        UploadFileService uploadFileService,
        DatabaseContext databaseContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _environment = environment;
        _mapper = mapper;
        _userService = userService;
        _uploadFileService = uploadFileService;
        _databaseContext = databaseContext;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // var entities = _mapper.Map<List<UserListViewModel>>(_userService.GetAllAsync().Result);
        var entities = _databaseContext.Users.Include(p => p.Group).ToList();
        await Task.Yield();
        return View(entities);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var groups = new SelectList(_databaseContext.Groups, "Id", "Name");
        ViewBag.Groups = groups;
        
        //var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
        await Task.Yield();
        return PartialView("PartialViews/CreatePartialView", new UserCreateViewModel());
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult Search(string search)
    {
        var searchUsers = _userService.GetAllAsync().Result.Where(t =>
            t.UserName!.Contains(search) || 
            t.FirstName!.Contains(search) || 
            t.LastName!.Contains(search) || 
            t.Email!.Contains(search) || 
            t.PhoneNumber!.Contains(search) || 
            t.Info!.Contains(search)).ToList();

        var users = _mapper.Map<List<UserListViewModel>>(searchUsers);

        return RedirectToAction("Index", users);
    }
    
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateViewModel? model)
    {
        try
        {
            if (model == null) return RedirectToAction("Index");
            var user = _userService.GetAllAsync().Result.FirstOrDefault(e => e.UserName == model.UserName);
            if (user != null)
            {
                BadRequest($"User {model.UserName} exists. Change the UserName");
                return RedirectToAction("Index");
            }
                
            var entity = _mapper.Map<UserCreateViewModel, User>(model);
            entity.Id = Guid.NewGuid().ToString();
            entity.Avatar =  model.File != null ? GetPathImage(model) : string.Empty;
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
        var entity = _userService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }
        
        var groups = new SelectList(_databaseContext.Groups, "Id", "Name");
        ViewBag.Groups = groups;

        var model = _mapper.Map<User, UserDetailsViewModel>(entity);

        await Task.Yield();
        return PartialView("PartialViews/DetailsPartialView", model);
    }

    [Authorize]    
    [HttpGet]
    public async Task<IActionResult> Edit(string entityId)
    {
        var entity = _userService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity == null)
        {
            return NotFound();
        }
        
        var groups = new SelectList(_databaseContext.Groups, "Id", "Name");
        ViewBag.Groups = groups;

        var model = _mapper.Map<User, UserEditViewModel>(entity);

        await Task.Yield();
        return PartialView("PartialViews/EditPartialView", model);
    }

    [Authorize] 
    [HttpPost]
    public async Task<IActionResult> Edit(UserEditViewModel model)
    {
        model.Avatar = model.File != null ? GetPathImage(model) : string.Empty;
        var user = _userManager.FindByNameAsync(model.UserName!).Result;
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
        var entity = _userService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity == null)
        {
            return NotFound();
        }
        var model = _mapper.Map<User, UserDeleteViewModel>(entity);
        
        await Task.Yield();
        return PartialView("PartialViews/DeletePartialView", model);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(UserDeleteViewModel model)
    {
        var entity = _userService.GetAllAsync().Result.FirstOrDefault(e => e.Id == model.Id);
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

    private string GetPathImage(object argument)
    {
        UserAvator model = new();
        var path = Path.Combine(_environment.ContentRootPath, "wwwroot/img/avators/");

        if (argument is UserCreateViewModel)
            model = _mapper.Map<UserCreateViewModel, UserAvator>((UserCreateViewModel)argument);
        
        if (argument is UserEditViewModel)
            model = _mapper.Map<UserEditViewModel, UserAvator>((UserEditViewModel)argument);
        
        _uploadFileService.Upload(path, $"{model!.Email}.jpg", model.File!);
        return $"/img/avators/{model.Email}.jpg";
    }
}