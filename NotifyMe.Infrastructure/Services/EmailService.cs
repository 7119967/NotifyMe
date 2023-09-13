using System.Net.Mail;

using Microsoft.Extensions.DependencyInjection;

using NotifyMe.Core.Entities;
using NotifyMe.Infrastructure.Context;

using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
namespace NotifyMe.Infrastructure.Services;

public class EmailService
{
    private readonly DatabaseContext? _dbContext;
    public EmailService(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.GetService<IServiceScopeFactory>()?.CreateScope();
        _dbContext = scope?.ServiceProvider.GetRequiredService<DatabaseContext>();
    }
    public Task SendEmail(string eventId)
    {
        var originalEvent = _dbContext?.Events.FirstOrDefault(e => e.Id == eventId);
        var originalMessage = CreateMessage(originalEvent!);
        var receivers = new List<string>();

        var message = new MailMessage();
        foreach (var receiver in originalMessage!.Receivers)
        {
            message.To.Add(receiver.ToString() ?? throw new InvalidOperationException());
        }
       
        message.From = new MailAddress("alerts@example.com");
        message.Subject = originalMessage.Subject;
        message.Body = originalMessage.ContentBody;

        using (var smtp = new SmtpClient())
        {
            smtp.Host = "smtp.example.com";
            smtp.SendMailAsync(message);
            smtp.Dispose();
        }

        return Task.CompletedTask;
    }

    public Task SendSms(Notification notification)
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

    private Message CreateMessage(Event eventItem)
    {
        var receivers = new List<string>();

        foreach (var user in eventItem.Configuration!.Group!.Users)
        {
            if (!string.IsNullOrEmpty(user.Email))
            {
                receivers.Add(user.Email);
            }
        }

        var model = new Message
        {
            Sender = "",
            Receivers = string.Join(";", receivers),
            EventId = eventItem.Id,
            Event = eventItem,
            Subject = $"ALERT: {eventItem.Configuration.ChangeType} exceeded threshold",
            ContentBody = $"{eventItem.Configuration.Message}. \nCurrent value: {eventItem.CurrentThreshold}, Threshold: {eventItem.Configuration.Threshold}"
        };

        var seequence = _dbContext?.Messages.AsEnumerable();
        var size = seequence!.Count();
        int[] anArray = new int[size];
        if (size == 0)
        {
            model.Id = "1";
        }
        else
        {
            var index = 0;
            foreach (var element in seequence!)
            {
                anArray[index] = Convert.ToInt32(element.Id);
                index++;
            }

            int maxValue = anArray.Max();
            var newId = Convert.ToInt32(maxValue) + 1;
            model.Id = newId.ToString();
        }

        var existingEntity = _dbContext?.Messages.Find(model.Id);
        if (existingEntity != null)
        {
            var entity = _dbContext?.Messages.Update(model);
            _dbContext?.SaveChanges();
            return entity!.Entity;
        }
        else
        {
            var entity = _dbContext?.Messages.AddAsync(model).Result;
            _dbContext?.SaveChanges();
            return entity!.Entity;
        }
    }
}