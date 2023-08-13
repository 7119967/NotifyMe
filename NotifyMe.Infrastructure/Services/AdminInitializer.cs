using Microsoft.AspNetCore.Identity;
using NotifyMe.Core.Entities;

namespace NotifyMe.Infrastructure.Services;

public class AdminInitializer
{
    public static async Task SeedAdminUser(
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager)
    {
        string adminLogin = "admin";
        string adminEmail = "admin@admin.com";
        string adminPassword = "pass";
  
        var roles = new []
        {
            "admin", 
            "user"
        };

        foreach (var role in roles)
        {
            if (await roleManager.FindByNameAsync(role) is null)
                await roleManager.CreateAsync(new IdentityRole(role));
        }
        
        if (await userManager.FindByNameAsync(adminEmail) == null)
        {
            User admin = new User 
            {
                UserName = adminLogin,
                Email = adminEmail
            };

            IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "admin");
        }
    }
}