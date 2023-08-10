using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Infrastructure.Context;
using NotifyMe.Infrastructure.Repositories;
using NotifyMe.Infrastructure.Services;
using RabbitMQ.Client;

namespace NotifyMe.IoC.Configuration.DI;

public static class ServiceCollectionExtensions
{
    public static void ConfigureBusinessServices(this IServiceCollection services, 
        IConfiguration configuration,
        ILogger logger)
    {
        
        var connection = configuration.GetConnectionString("DefaultConnection");

        services
            .AddDbContext<DatabaseContext>(options =>
            {
                options
                    .UseSqlServer(connection, op => op.MigrationsAssembly("NotifyMe.API"))
                    .UseLazyLoadingProxies();
            })
            .AddIdentity<User, IdentityRole>(option =>
            {
                option.Password.RequireDigit = false;
                option.Password.RequiredLength = 3;
                option.Password.RequireLowercase = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<DatabaseContext>();
        
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
            });
        
        services.AddTransient<UploadFileService>();
        services.AddScoped<IEventLogger, EventLogger>();
        services.AddSingleton<RabbitMQService>(sp =>
        {
            var rabbitMqHost = configuration["ConnectionStrings:RabbitMQHost"] ?? throw new NullReferenceException();
            var rabbitMqUsername = configuration["ConnectionStrings:RabbitMQUsername"] ?? throw new NullReferenceException();
            var rabbitMqPassword = configuration["ConnectionStrings:RabbitMQPassword"] ?? throw new NullReferenceException();
            return new RabbitMQService(rabbitMqHost, rabbitMqUsername, rabbitMqPassword, "notification_queue");
        });

        services.AddScoped<IEventMonitoringRepository, EventMonitoringRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IGroupService, GroupService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<EmailService>();

        services.AddSingleton<IConnectionFactory, ConnectionFactory>();
        
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}