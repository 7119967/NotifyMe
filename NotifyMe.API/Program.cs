using Microsoft.AspNetCore.Identity;

using NotifyMe.Core.Entities;
using NotifyMe.Infrastructure.Services;
using NotifyMe.IoC.Configuration.DI;

namespace NotifyMe.API
{
    public class Program
    {
        private static IConfiguration? _configuration;
        // private static readonly ILogger? _logger;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            _configuration = builder.Configuration;

            // Services
            builder.Services.ConfigureBusinessServices(_configuration);

            // Controllers and Views
            builder.Services.AddControllersWithViews();
            builder.Services.AddSwaggerGen();
            
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
            
            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Users}/{action=Index}/{id?}");

            var rabbitMqService = app.Services.GetRequiredService<RabbitMQService1>();
            rabbitMqService.StartListening();

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
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }

            app.Run();
        }
    }
}