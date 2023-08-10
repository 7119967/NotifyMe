using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotifyMe.API.ViewModels;
using NotifyMe.Core.Models;
using NotifyMe.Core.Entities;
using NotifyMe.Infrastructure.Context;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.API.Controllers;

public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private DatabaseContext _db;
        private readonly IHostEnvironment _environment; 
        private readonly UploadFileService _uploadFileService; 
        private readonly IMapper _mapper;
        
        public AccountController(UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager, 
            SignInManager<User> signInManager, 
            DatabaseContext db, 
            IHostEnvironment environment,
            IMapper mapper,
            UploadFileService uploadFileService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _db = db;
            _environment = environment;
            _mapper = mapper;
            _uploadFileService = uploadFileService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = Path.Combine(_environment.ContentRootPath,"wwwroot\\images\\");
                _uploadFileService.Upload(path, $"{model.Email}.jpg", model.File!);
                string pathImage = $"/images/{model.Email}.jpg";
                
                User user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Avatar = pathImage
                };
                
                var result = await _userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "user");
                    await _signInManager.SignInAsync(user, false);
                    return View();
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel {ReturnUrl = returnUrl});
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel? model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName!) ?? throw new NullReferenceException();
                    var result = await _signInManager.PasswordSignInAsync(
                        user!,
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
                        
                        return RedirectToAction("Index", "Notifications");
                    }
                }

                ModelState.AddModelError("", "Incorrect login and/or password");
            }

            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

    }