using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotifyMe.Infrastructure.Services;

public class RabbitMqConsumer: BackgroundService, IRabbitMqConsumer
{
    private readonly EmailService _emailService;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _rabbitMqQueueName;
    
    public RabbitMqConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        _emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

        var config = configuration;
        var rabbitMqHost = "";

        if (env.IsProduction())
        {
            rabbitMqHost = config["RabbitMq:ServerHost"] ?? throw new NullReferenceException();
        }
        else 
        {
            rabbitMqHost = config["RabbitMq:LocalHost"] ?? throw new NullReferenceException();
        }
     
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
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //
        //     await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        // }
        
        await Task.Run(StartConsuming, stoppingToken);
    }

    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received message: {message}");
            
            var newEvent = JsonConvert.DeserializeObject<Event>(message);
            var json = JsonConvert.SerializeObject(newEvent, Formatting.Indented);
            Console.WriteLine(" [x] Received {0}", json);
            try
            {
                if (newEvent != null)
                {
                    _emailService.SendEmail(newEvent.Id).Wait();
                    _emailService.SendNotification(newEvent.Id);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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