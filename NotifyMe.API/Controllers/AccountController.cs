using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using NotifyMe.API.ViewModels;
using NotifyMe.Core.Entities;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.API.Controllers;

public class AccountController : Controller
{
    private readonly IMapper _mapper;
    private readonly IHostEnvironment _env;
    private readonly UploadFileService _uploader;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<User> userManager,
        IMapper mapper,
        IHostEnvironment environment,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager,
        UploadFileService uploadFileService)
    {
        _mapper = mapper;
        _env = environment;
        _userManager = userManager;
        _roleManager = roleManager;
        _uploader = uploadFileService;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = model.Email,
            UserName = model.UserName,
            PhoneNumber = model.PhoneNumber,
            File = model.File
        };
        user.Avatar = model.File != null ? Helpers.GetPathImage(_env, _mapper, _uploader, user) : string.Empty;
        var result = await _userManager.CreateAsync(user, model.Password!);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "user");
            await _signInManager.SignInAsync(user, false);
            return View("Login");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel? model)
    {
        if (ModelState.IsValid)
        {
            if (model != null)
            {
                var user = await _userManager.FindByNameAsync(model.UserName!.Trim()) ?? throw new NullReferenceException();
                var result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password!,
                    model.RememberMe,
                    false
                );

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Changes");
                }
            }

            ModelState.AddModelError("", "Incorrect login and/or password");
        }

        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

}