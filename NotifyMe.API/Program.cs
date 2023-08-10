using Microsoft.AspNetCore.Identity;
using NotifyMe.Core.Entities;
using NotifyMe.Infrastructure.Services;
using NotifyMe.IoC.Configuration.DI;

namespace NotifyMe.API
{
    public class Program
    {
        private static readonly IConfiguration _configuration; 
        private static readonly ILogger _logger;
        
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Services
            builder.Services.ConfigureBusinessServices(builder.Configuration, _logger);

            // Controllers and Views
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            var rabbitMqService = app.Services.GetRequiredService<RabbitMQService>();
            rabbitMqService.StartListening();

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var userManager = services.GetRequiredService<UserManager<User>>();
                var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                Task.Run(()=> AdminInitializer.SeedAdminUser(rolesManager, userManager)) ;
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
            
            app.Run();
        }
    }
}