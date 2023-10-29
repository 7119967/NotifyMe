using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotifyMe.Core.Interfaces.Services;
using RabbitMQ.Client;

namespace NotifyMe.Infrastructure.Services;

public class RabbitMqPublisher: BackgroundService, IRabbitMqPublisher
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly string _rabbitMqQueueName;
    private readonly ConcurrentQueue<string> _messageQueue = new();
    private readonly ILogger<RabbitMqPublisher> _logger;
    
    public RabbitMqPublisher(IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<RabbitMqPublisher>>();

        var rabbitMqHost = env.IsProduction() ? 
            configuration["RabbitMq:ServerHost"] : 
            configuration["RabbitMq:LocalHost"];
        var rabbitMqUsername = configuration["RabbitMq:Username"];
        var rabbitMqPassword = configuration["RabbitMq:Password"];
        _rabbitMqQueueName = configuration["RabbitMq:QueueName"]!;
        
        var factory = new ConnectionFactory
        {
            HostName = rabbitMqHost,
            UserName = rabbitMqUsername,
            Password = rabbitMqPassword,
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
                _logger.LogDebug($"Got message: {message}");
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