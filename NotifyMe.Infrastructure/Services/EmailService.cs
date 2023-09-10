using System.Net.Mail;

using NotifyMe.Core.Entities;

using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;

namespace NotifyMe.Infrastructure.Services;

public class EmailService
{
    public Task SendEmailNotification(Notification notification)
    {
        var message = new MailMessage();
        foreach (var receiver in new List<Group>())
        {
            message.To.Add(receiver.Name ?? throw new InvalidOperationException());
        }
       
        message.From = new MailAddress("alerts@example.com");
        message.Subject = "Alert notification";
        message.Body = notification.Message;

        using (var smtp = new SmtpClient())
        {
            smtp.Host = "smtp.example.com";
            smtp.SendMailAsync(message);
            smtp.Dispose();
        }

        return Task.CompletedTask;
    }

    public Task SendSmsNotification(Notification notification)
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

        return Task.CompletedTask;
    }
}