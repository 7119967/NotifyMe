using System.Text;
using Microsoft.Extensions.Configuration;
using NotifyMe.Core.Interfaces.Services;
using RabbitMQ.Client;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NotifyMe.Infrastructure.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly string _rabbitMqHost;
    private readonly string _rabbitMqUsername;
    private readonly string _rabbitMqPassword;
    private readonly string _rabbitMqQueueName;
    public RabbitMqService(IConfiguration configuration)
    {
        var config = configuration;
        
        _rabbitMqHost = config!["RabbitMq:Host"] ?? throw new NullReferenceException();
        _rabbitMqUsername = config["RabbitMq:Username"] ?? throw new NullReferenceException();
        _rabbitMqPassword = config["RabbitMq:Password"] ?? throw new NullReferenceException();
        _rabbitMqQueueName = config["RabbitMq:QueueName"] ?? throw new NullReferenceException();
    }
    
    public void SendMessage(object obj)
    {
        var message = JsonSerializer.Serialize(obj);
        SendMessage(message);
    }

    public void SendMessage(string message)
    {
        // JsonSerializerSettings settings = new JsonSerializerSettings
        // {
        //     ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        // };
        // string messageBody = JsonConvert.SerializeObject(message, settings);
        
        // var factory = new ConnectionFactory { HostName = "localhost" };
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqHost,
            UserName = _rabbitMqUsername,
            Password = _rabbitMqPassword,
            //VirtualHost = "rabbitmq",
            Port = 5672
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: _rabbitMqQueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
            routingKey: _rabbitMqQueueName,
            basicProperties: null,
            body: body);
    }
}