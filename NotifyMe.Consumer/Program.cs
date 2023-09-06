using Microsoft.EntityFrameworkCore;
using NotifyMe.Infrastructure.Context;
using NotifyMe.Infrastructure.Services;
using NotifyMe.IoC.Configuration.DI;

namespace NotifyMe.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            IConfiguration configuration = configurationBuilder.Build();
            
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHostedService<RabbitMqListener>();

            var connection = configuration?.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                options
                    .UseSqlServer(connection, op => op.MigrationsAssembly("NotifyMe.API"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .EnableSensitiveDataLogging()
                    .UseLazyLoadingProxies();
            });
            
            // Services
            builder.Services.ConfigureBusinessServices(configuration);

            var app = builder.Build();

            app.RabbitMqConsumer();
            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
