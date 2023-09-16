using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotifyMe.Infrastructure.Services;

public class RabbitMqListener : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;
    private EmailService _emailService;

    public RabbitMqListener(IConfiguration configuration, EmailService emailService)
    {
        var config = configuration;
        _emailService = emailService;
        
        var rabbitMqHost = config!["RabbitMq:Host"] ?? throw new NullReferenceException();
        var rabbitMqQueueName = config["RabbitMq:QueueName"] ?? throw new NullReferenceException();
        
        var factory = new ConnectionFactory { HostName = rabbitMqHost };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: rabbitMqQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
      
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            Task.Run(async () => await _emailService.SendEmail(content));
            Debug.WriteLine($"A message received: {content}");
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume("MyQueue", false, consumer);

        return Task.CompletedTask;
    }
	
    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}