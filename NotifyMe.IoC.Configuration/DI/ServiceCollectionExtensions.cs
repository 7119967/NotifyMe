using System.Reflection;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
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
        var connection = configuration?.GetConnectionString("DefaultConnection");

        services.AddDbContext<DatabaseContext>(options =>
        {
            options
                .UseSqlServer(connection, op => op.MigrationsAssembly("NotifyMe.API"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableSensitiveDataLogging()
                .UseLazyLoadingProxies();
        });

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
        services.AddTransient<IEventLogger, EventLogger>();
        services.AddTransient<IEventService, EventService>();
        services.AddTransient<IGroupService, GroupService>();
        services.AddTransient<IChangeService, ChangeService>();
        services.AddTransient<IMessageService, MessageService>();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IConfigurationService, ConfigurationService>();
        services.AddTransient<INotificationUserService, NotificationUserService>();
        // services.AddTransient<IHostedService, EventMonitor>();
        services.AddTransient<EmailService>();
        services.AddTransient<UploadFileService>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddSingleton<ServicesMappingProfile>();
        services.AddSingleton<IConnectionFactory, ConnectionFactory>();
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
        services.AddHostedService<RabbitMqConsumer>();
        services.AddHostedService<EventMonitor>();
    }
}