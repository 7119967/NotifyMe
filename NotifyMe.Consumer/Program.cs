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
            // builder.Services.AddHostedService<RabbitMqListener>(_ =>
            // {
            //     var rabbitMqHost = configuration!["ConnectionStrings:RabbitMQHost"] ?? throw new NullReferenceException();
            //     var rabbitMqUsername = configuration["ConnectionStrings:RabbitMQUsername"] ?? throw new NullReferenceException();
            //     var rabbitMqPassword = configuration["ConnectionStrings:RabbitMQPassword"] ?? throw new NullReferenceException();
            //     var rabbitMqQueueName = configuration["ConnectionStrings:RabbitMQQueueName"] ?? throw new NullReferenceException();
            //     return new RabbitMqListener(rabbitMqHost, rabbitMqQueueName);
            // });  
            
            builder.Services.AddHostedService<RabbitMqListener>();
            builder.Services.ConfigureBusinessServices(configuration);

            var app = builder.Build();

            app.RabbitMqConsumer();
            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}