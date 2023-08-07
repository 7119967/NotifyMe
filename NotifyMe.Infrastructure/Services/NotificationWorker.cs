using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NotifyMe.Core.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;

namespace NotifyMe.Infrastructure.Services;

public class NotificationWorker : BackgroundService 
{
    private readonly IModel _channel;

    public NotificationWorker(IModel channel) 
    {
        _channel = channel;
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
                SendEmailNotification(notification);
                SendSMSNotification(notification);
            }

            //Acknowledge message
            _channel.BasicAck(e.DeliveryTag, false);
        };

        _channel.BasicConsume("notifications", false, consumer);

        return Task.CompletedTask;
    }
    
    private void SendEmailNotification(Notification notification)
    {
        var mailMessage = new MailMessage();
        mailMessage.To.Add(notification.Recipient ?? throw new InvalidOperationException());
        mailMessage.From = new MailAddress("alerts@example.com");
        mailMessage.Subject = "Alert notification";
        mailMessage.Body = notification.Message;
        
        using(var smtp = new SmtpClient())
        {
            smtp.Host = "smtp.example.com";
            smtp.Send(mailMessage);
        }
    }

    private void SendSMSNotification(Notification notification)
    {
        const string accountSid = "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
        const string authToken = "your_auth_token";

        TwilioClient.Init(accountSid, authToken);

        try
        {
            var message = MessageResource.Create(
                body: "This is the ship that made the Kessel Run in fourteen parsecs?",
                from: new Twilio.Types.PhoneNumber("+15017122661"),
                to: new Twilio.Types.PhoneNumber("+15558675310")
            );

            Console.WriteLine(message.Sid);
        }
        catch (ApiException e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine($"Twilio Error {e.Code} - {e.MoreInfo}");
        }

        Console.Write("Press any key to continue.");
        Console.ReadKey();
    }
}