using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotifyMe.Infrastructure.Services;

public class RabbitMqConsumer: BackgroundService, IRabbitMqConsumer
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly string _rabbitMqQueueName;
    private readonly EmailService _emailService;
    private readonly ILogger<RabbitMqConsumer> _logger;
    
    public RabbitMqConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        _emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<RabbitMqConsumer>>();

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
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(StartConsuming, stoppingToken);
    }

    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogDebug($"Received message: {message}");
            
            var newEvent = JsonConvert.DeserializeObject<Event>(message);
            if (newEvent == null) return;
            var json = JsonConvert.SerializeObject(newEvent, Formatting.Indented);
            _logger.LogDebug("[x] Received {0}", json);
            try
            {
                // Task.Run(() => _emailService.SendEmail(newEvent.Id));
                // Task.Run(() => _emailService.SendNotification(newEvent.Id));
                // var thread = new Thread(() =>
                // {
                //     _emailService.SendEmail(newEvent.Id);
                //     _emailService.SendNotification(newEvent.Id);
                // });
                // _logger.LogDebug($"Thread name: {thread.Name}, Thread id: {thread.ManagedThreadId}");
                // thread.Start();
                _emailService.SendEmail(newEvent.Id);
                _emailService.SendNotification(newEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"{ex.Message}");
            }
        };

        _channel.BasicConsume(queue: _rabbitMqQueueName, autoAck: true, consumer: consumer);
    }
    
    public void CloseConnection()
    {
        _channel.Close();
        _connection.Close();
    }
}