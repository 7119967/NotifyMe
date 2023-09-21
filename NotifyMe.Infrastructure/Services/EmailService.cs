using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Infrastructure.Context;

using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
namespace NotifyMe.Infrastructure.Services;

public class EmailService
{
    private readonly DatabaseContext? _dbContext;
    private readonly IConfigurationService? _configurationService;
    private readonly IGroupService? _groupService;
    private readonly INotificationService? _notificationService;
    private readonly INotificationUserService? _notificationUserService;
    private readonly IEventService? _eventService;
    private readonly IMessageService? _messageService;
    
    public EmailService(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.GetService<IServiceScopeFactory>()?.CreateScope();
        _dbContext = scope?.ServiceProvider.GetRequiredService<DatabaseContext>();
        _configurationService = scope?.ServiceProvider.GetRequiredService<IConfigurationService>();
        _groupService = scope?.ServiceProvider.GetRequiredService<IGroupService>();
        _notificationService = scope?.ServiceProvider.GetRequiredService<INotificationService>();
        _notificationUserService = scope?.ServiceProvider.GetRequiredService<INotificationUserService>();
        _eventService = scope?.ServiceProvider.GetRequiredService<IEventService>();
        _messageService = scope?.ServiceProvider.GetRequiredService<IMessageService>();
    }
    
    public async Task SendEmail(string eventId)
    {
        var originalEvent = _dbContext?.Events.FirstOrDefault(e => e.Id == eventId);
                
        var configuration = await _configurationService!
                .AsQueryable()
                .FirstOrDefaultAsync(t => t.Id == originalEvent!.ConfigurationId); 
        
        var group = await _groupService!
                .AsQueryable()
                .Include(g => g.Users)
                .FirstOrDefaultAsync(t => t.Id == configuration!.Id);
        
        // var originalMessage = CreateMessage(originalEvent!);
        // var receivers = new List<string>();

        var message = new MailMessage();
        foreach (var receiver in group!.Users)
        {
            message.To.Add(receiver.Email ?? throw new InvalidOperationException());
        }
       
        message.From = new MailAddress("7119967@mail.ru");
        message.Subject = "Subject";
        message.Body = "Content";

        Console.WriteLine("Enter the password of your email");
        var password = Console.ReadLine();
        
        using (var smtp = new SmtpClient())
        {
            smtp.Host = "smtp.mail.ru";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("7119967@mail.ru", $"{password}");
            await smtp.SendMailAsync(message);
            smtp.Dispose();
        }

        await Task.CompletedTask;
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

        var sequence = _messageService!.AsEnumerable();
        var newId = (sequence?.Any() == true) ? (sequence.Max(e => Convert.ToInt32(e.Id)) + 1) : 1;
        model.Id = newId.ToString();

        var existingEntity = _messageService?
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefault(m => m.Id == model.Id)
            ;
       
        if (existingEntity != null)
        {
            return _messageService!.Update(model).Entity;
        }
        
        return _messageService!.Create(model).Entity;
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
    
    public void SendNotification(string eventId)
    {
        var originalEvent = _eventService!
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefault(e => e.Id == eventId)
            ;
                
        var configuration = _configurationService!
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefault(t => t.Id == originalEvent!.ConfigurationId)
            ; 
        
        var group = _groupService!
            .AsQueryable()
            .AsNoTracking()
            .Include(g => g.Users)
            .FirstOrDefault(t => t.Id == configuration!.Id)
            ;
        
        CreateNotification(group!, originalEvent!);
    }
    
    private void CreateNotification(Group group, Event eventItem)
    {
        var sequence = _notificationService!
            .AsQueryable()
            .AsNoTracking()
            .ToList();
        
        var newId = (sequence?.Any() == true) ? (sequence.Max(e => Convert.ToInt32(e.Id)) + 1) : 1;

        var notification = new Notification
        {
            Id = newId.ToString(),
            Message = eventItem.EventDescription,
            EventId = eventItem.Id
        };

        _notificationService?.Create(notification);
        
        var usersToReceiveNotification = group.Users.ToList();
        
        foreach (var user in usersToReceiveNotification)
        {
            var sequence2 = _notificationUserService!
                .AsQueryable()
                .AsNoTracking()
                .ToList();
            
            var newId2 = (sequence2?.Any() == true) ? (sequence2.Max(e => Convert.ToInt32(e.Id)) + 1) : 1;
            
            var notificationUser = new NotificationUser
            {
                Id = newId2.ToString(),
                UserId = user.Id,
                NotificationId = notification.Id
            };
            
            _notificationUserService?.Create(notificationUser);
        }
    }
}