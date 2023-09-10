using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Infrastructure.Context;
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
        var messageService = services.GetRequiredService<IMessageService>();

        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "MyQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = new Message
            {
                ContentBody = Encoding.UTF8.GetString(body)
            };
            
            var maxId = messageService?.GetAllAsync().Result.Max(e => e.Id);
            if (maxId is null)
            {
                message.Id = "1";
            }
            else 
            {
                var newId = Convert.ToInt32(maxId) + 1;
                message.Id = newId.ToString(); 
            }

            try
            {
                messageService?.CreateAsync(message);
                Console.WriteLine(" [x] Received {0}", message);
            }

            // var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
            // var context = scope.ServiceProvider.GetRequiredService<IMessageService>();
            // try
            // {
            //     context.CreateAsync(message);
            //     Console.WriteLine(" [x] Received {0}", message);
            // }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        };
        
        channel.BasicConsume(queue: "MyQueue",
            autoAck: true,
            consumer: consumer);
        
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}