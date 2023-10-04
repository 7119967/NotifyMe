using Microsoft.AspNetCore.Identity;

using NotifyMe.Core.Entities;
using NotifyMe.Infrastructure.Services;
using NotifyMe.IoC.Configuration.DI;

namespace NotifyMe.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Services
            builder.Services.ConfigureBusinessServices(configuration);

            // Controllers and Views
            builder.Services.AddControllersWithViews();
            builder.Services.AddSwaggerGen();
            
            var app = builder.Build();
            var logger = app.Services.GetService<ILogger<Program>>();

            app.InitializeDatabase(logger);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var userManager = services.GetRequiredService<UserManager<User>>();
                var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                Task.Run(() => AdminInitializer.SeedAdminUser(rolesManager, userManager));
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while seeding the database.");
            }

            app.Run();
        }
    }
}