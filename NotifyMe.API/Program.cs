using Microsoft.EntityFrameworkCore;

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

            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"), 
                    b => b.MigrationsAssembly("NotifyMe.API"));
            });

            // Services
            builder.Services.AddScoped<IEventLogger, EventLogger>();
            builder.Services.AddSingleton<RabbitMQService>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var rabbitMQHost = configuration["ConnectionStrings:RabbitMQHost"] ?? throw new NullReferenceException();
                var rabbitMQUsername = configuration["ConnectionStrings:RabbitMQUsername"] ?? throw new NullReferenceException();
                var rabbitMQPassword = configuration["ConnectionStrings:RabbitMQPassword"] ?? throw new NullReferenceException();
                return new RabbitMQService(rabbitMQHost, rabbitMQUsername, rabbitMQPassword, "notification_queue");
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

            var rabbitMQService = app.Services.GetRequiredService<RabbitMQService>();
            rabbitMQService.StartListening();

            app.Run();
        }
    }
}