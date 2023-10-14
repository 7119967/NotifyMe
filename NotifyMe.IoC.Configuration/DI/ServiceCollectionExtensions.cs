using System.Reflection;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Infrastructure.Context;
using NotifyMe.Infrastructure.Repositories;
using NotifyMe.Infrastructure.Services;
using NotifyMe.IoC.Configuration.AutoMapper;

using RabbitMQ.Client;

namespace NotifyMe.IoC.Configuration.DI;

public static class ServiceCollectionExtensions
{
    public static void ConfigureBusinessServices(this IServiceCollection services, IConfiguration? configuration)
    {
        var serviceProvider = services.BuildServiceProvider();
        var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

        services.AddDbContext<DatabaseContext>(options =>
        {
            options
                .UseSqlServer(GetDbConnection(configuration!, env),
                    op => op.MigrationsAssembly("NotifyMe.API"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableSensitiveDataLogging()
                .UseLazyLoadingProxies();
        });

        // var assembly = Assembly.GetEntryAssembly();
        // var projectName = assembly!.GetName().Name;
        // var dataProtectionBuilder = services.AddDataProtection();
        // dataProtectionBuilder.SetApplicationName($"{projectName}");
        // dataProtectionBuilder.ProtectKeysWithDpapi(protectToLocalMachine: false);
        
        services.AddIdentity<User, IdentityRole>(option =>
            {
                option.Password.RequireDigit = false;
                option.Password.RequiredLength = 3;
                option.Password.RequireLowercase = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<DatabaseContext>();

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
            });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IEventService, EventService>();
        services.AddTransient<IGroupService, GroupService>();
        services.AddTransient<IChangeService, ChangeService>();
        services.AddTransient<IMessageService, MessageService>();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IConfigurationService, ConfigurationService>();
        services.AddTransient<INotificationUserService, NotificationUserService>();
  
        services.AddTransient<EmailService>();
        services.AddTransient<UploadFileService>();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddSingleton<ServicesMappingProfile>();

        services.AddSingleton<IConnectionFactory, ConnectionFactory>();
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

        services.AddHostedService<RabbitMqConsumer>();
        services.AddHostedService<EventMonitor>();
        
        services.AddControllersWithViews();
        services.AddSwaggerGen();
    }
    
    private static string GetDbConnection(IConfiguration configuration, IWebHostEnvironment env)
    {
        return (env.IsDevelopment()
            ? configuration.GetConnectionString("LocalConnection")
            : configuration.GetConnectionString("ServerConnection"))!;
    }
}