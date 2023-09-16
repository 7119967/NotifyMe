using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Infrastructure.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotifyMe.IoC.Configuration.DI;

public static class RabbitMqExtensions
{
    public static void RabbitMqConsumer(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
        var services = scope.ServiceProvider;
        var unitOfWork = services.GetRequiredService<IUnitOfWork>();
        var emailService = services.GetRequiredService<EmailService>();
        var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
        
        var rabbitMqHost = config!["RabbitMq:Host"] ?? throw new NullReferenceException();
        var rabbitMqQueueName = config["RabbitMq:QueueName"] ?? throw new NullReferenceException();
        
        var factory = new ConnectionFactory { HostName = rabbitMqHost };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: rabbitMqQueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var stringBody = Encoding.UTF8.GetString(body);
      
            var newEvent = JsonConvert.DeserializeObject<Event>(stringBody);
            var json = JsonConvert.SerializeObject(newEvent, Formatting.Indented);
            Console.WriteLine(" [x] Received {0}", json);
            try
            {
                if (newEvent != null)
                {
                    emailService.SendNotification(newEvent.Id);
                    // Task.Run(async() => await emailService.SendNotification(newEvent.Id));
                    // Task.Run(async () => await emailService.SendEmail(newEvent.Id));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        };
        
        channel.BasicConsume(queue:rabbitMqQueueName,
            autoAck: true,
            consumer: consumer);
        
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}