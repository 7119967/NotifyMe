using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotifyMe.API.ViewModels;
using NotifyMe.Core.Entities;
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
        
        public UsersController(UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager, 
            SignInManager<User> signInManager, 
            DatabaseContext db, 
            IHostEnvironment environment, 
            UploadFileService uploadFileService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _db = db;
            _environment = environment;
            _uploadFileService = uploadFileService;
        }
               
        [Authorize]
        public IActionResult Index(string search)
        {
            List<User> users = _db.Users.Where(t => 
                t.UserName.Contains(search)
                || t.FirstName.Contains(search) 
                || t.LastName.Contains(search) 
                || t.Email.Contains(search) 
                || t.PhoneNumber.Contains(search) 
                || t.Info.Contains(search)).ToList();;
            
            return View(users);
        }
        
        [Authorize]
        public IActionResult Profile(long idUser)
        {
            User user = _db.Users.FirstOrDefault(u => u.Id == idUser);
            if (user is null)
            {
                NotFound();
                return RedirectToAction("Index");
            }
 
            var model = new ProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                Info = user.Info
            };
            
            return View(model);
        }

        [Authorize]
        public IActionResult Edit(long idCurrentUser)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == idCurrentUser);
            EditProfileViewModel model = new EditProfileViewModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                Info = user.Info
            };
            return View(model);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Info = model.Info;
                user.Avatar = model.Avatar;
                
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Profile", "Users");
                }
            }

            return View(model);
        }
    }