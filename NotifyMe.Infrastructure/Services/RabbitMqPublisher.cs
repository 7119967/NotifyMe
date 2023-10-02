using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NotifyMe.Core.Interfaces.Services;
using RabbitMQ.Client;

namespace NotifyMe.Infrastructure.Services;

public class RabbitMqPublisher: BackgroundService, IRabbitMqPublisher
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly string _rabbitMqQueueName;
    private readonly ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();

    
    public RabbitMqPublisher(IConfiguration configuration)
    {
        var config = configuration;
        
        var rabbitMqHost = config["RabbitMq:Host"] ?? throw new NullReferenceException();
        var rabbitMqUsername = config["RabbitMq:Username"] ?? throw new NullReferenceException();
        var rabbitMqPassword = config["RabbitMq:Password"] ?? throw new NullReferenceException();
        _rabbitMqQueueName = config["RabbitMq:QueueName"] ?? throw new NullReferenceException();
        
        var factory = new ConnectionFactory
        {
            HostName = rabbitMqHost,
            UserName = rabbitMqUsername,
            Password = rabbitMqPassword,
            //VirtualHost = "rabbitmq",
            Port = 5672
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: _rabbitMqQueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_messageQueue.TryDequeue(out var message))
            {
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(exchange: "",
                    routingKey: _rabbitMqQueueName,
                    basicProperties: null,
                    body: body);
            }

            await Task.Delay(100, stoppingToken);
        }
    }

    public void PublishMessage(string message)
    {
        // _messageQueue.Enqueue(message);
        
        var body = Encoding.UTF8.GetBytes(message);
        
        _channel.BasicPublish(exchange: "",
            routingKey: _rabbitMqQueueName,
            basicProperties: null,
            body: body);
    }
    
    public void CloseConnection()
    {
        _channel.Close();
        _connection.Close();
    }
}