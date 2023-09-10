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
            
            // Services
            builder.Services.AddHostedService<RabbitMqListener>();
            builder.Services.ConfigureBusinessServices(configuration);

            var app = builder.Build();

            app.RabbitMqConsumer();
            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}