using System.Text;

using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using NotifyMe.Core.Entities;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotifyMe.Infrastructure.Services;

public class NotificationWorker : BackgroundService
{
    private readonly IModel _channel;
    private readonly EmailService _emailService;

    public NotificationWorker(IModel channel, EmailService emailService)
    {
        _channel = channel;
        _emailService = emailService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (sender, e) =>
        {
            var notification = JsonConvert.DeserializeObject<Notification>(Encoding.UTF8.GetString(e.Body.ToArray()));

            if (notification != null)
            {
                Task.Run(() => _emailService.SendEmailNotification(notification));
                Task.Run(() => _emailService.SendSmsNotification(notification));
            }

            _channel.BasicAck(e.DeliveryTag, false);
        };

        _channel.BasicConsume("notifications", false, consumer);

        return Task.CompletedTask;
    }


}