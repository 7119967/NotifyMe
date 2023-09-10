using System.Text;
using Newtonsoft.Json;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;
using RabbitMQ.Client;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NotifyMe.Infrastructure.Services;

public class RabbitMqService : IRabbitMqService
{
    public void SendMessage(object obj)
    {
        var message = JsonSerializer.Serialize(obj);
        SendMessage(message);
    }

    public void SendMessage(Message message)
    {
        // Serialize the message to a JSON string
        string messageBody = JsonConvert.SerializeObject(message);
        
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "MyQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(messageBody);

        channel.BasicPublish(exchange: "",
            routingKey: "MyQueue",
            basicProperties: null,
            body: body);
    }
}