using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
using NotifyMe.Infrastructure.Context;
using NotifyMe.Infrastructure.Repositories;
using NotifyMe.Infrastructure.Services;

using RabbitMQ.Client;

namespace NotifyMe.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddDbContext<DatabaseContext>(options =>
                {
                    options
                        .UseLazyLoadingProxies()
                        .UseSqlServer(
                        builder.Configuration.GetConnectionString("DefaultConnection"), 
                        b => b.MigrationsAssembly("NotifyMe.API"));
                })
                .AddIdentity<User, IdentityRole>(option =>
                {
                    option.Password.RequireDigit = false;
                    option.Password.RequiredLength = 5;
                    option.Password.RequireLowercase = false;
                    option.Password.RequireUppercase = false;
                    option.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<DatabaseContext>();

            // Services
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                });
            builder.Services.AddTransient<UploadFileService>();
            builder.Services.AddScoped<IEventLogger, EventLogger>();
            builder.Services.AddSingleton<RabbitMQService>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var rabbitMqHost = configuration["ConnectionStrings:RabbitMQHost"] ?? throw new NullReferenceException();
                var rabbitMqUsername = configuration["ConnectionStrings:RabbitMQUsername"] ?? throw new NullReferenceException();
                var rabbitMqPassword = configuration["ConnectionStrings:RabbitMQPassword"] ?? throw new NullReferenceException();
                return new RabbitMQService(rabbitMqHost, rabbitMqUsername, rabbitMqPassword, "notification_queue");
            });
            builder.Services.AddTransient<INotificationService, NotificationService>();
            builder.Services.AddScoped<IEventMonitoringRepository, EventMonitoringRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();

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