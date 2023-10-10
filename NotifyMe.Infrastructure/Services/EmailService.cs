using System.Net;
using System.Net.Mail;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
namespace NotifyMe.Infrastructure.Services;

public class EmailService
{
    private readonly IMapper _mapper;
    private readonly IGroupService? _groupService;
    private readonly IEventService? _eventService;
    private readonly IMessageService? _messageService;
    private readonly INotificationService? _notificationService;
    private readonly IConfigurationService? _configurationService;
    private readonly INotificationUserService? _notificationUserService;
    
    public EmailService(IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
        _groupService = scope.ServiceProvider.GetRequiredService<IGroupService>();
        _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        _notificationUserService = scope.ServiceProvider.GetRequiredService<INotificationUserService>();
        _eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        _messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
        _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    }
    
    public async Task SendEmail(string eventId)
    {
        var message = await CreateMailMessageAsync(eventId);
        var password = $"07H9Sd7mJ5kNvCsyuA9F";
        
        using (var smtp = new SmtpClient())
        {
            smtp.Host = "smtp.mail.ru";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("7119967@mail.ru", $"{password}");
            var result = smtp.SendMailAsync(message).Status;
            if (result == TaskStatus.Faulted)
            {
                Console.WriteLine(result);
            }

            smtp.Dispose();
        }
    }
    
    private async Task<MailMessage> CreateMailMessageAsync(string eventId)
    {
        var originalEvent = await _eventService!.AsQueryable()
            .Include(e => e.Configuration)
            .FirstOrDefaultAsync(e => e.Id == eventId) ?? new Event();

        var configuration = await _configurationService!.AsQueryable()
            .FirstOrDefaultAsync(t => t.Id == originalEvent.ConfigurationId) ?? new Configuration();

        var group = await _groupService!.AsQueryable()
            .Include(g => g.Users)
            .FirstOrDefaultAsync(t => t.Id == configuration.Id) ?? new Group();

        var mailMessage = new MailMessage
        {
            From = new MailAddress("7119967@mail.ru"),
            Subject = $"ALERT: {originalEvent.Configuration!.ChangeType} exceeded threshold",
            Body = $"{originalEvent.Configuration.Message}. \nCurrent value: {originalEvent.CurrentThreshold}, Threshold: {originalEvent.Configuration.Threshold}"
        };

        foreach (var user in group.Users)
        {
            mailMessage.To.Add(user.Email ?? throw new InvalidOperationException());
        }

        CreateMessage(originalEvent, mailMessage);

        return mailMessage;
    }

    private void CreateMessage(Event originalEvent, MailMessage mailMessage)
    {
        var message = _mapper.Map<MailMessage, Message>(mailMessage);
        var sequence = _messageService!.GetAllAsync().Result;
        var newId = Helpers.GetNewIdEntity(sequence);
        message.Id = newId.ToString();

        _messageService!.Create(message);

        var existingEntity = _messageService?
            .AsQueryable()
            .FirstOrDefault(m => m.Id == message.Id);

        if (existingEntity != null)
        {
            existingEntity.EventId = originalEvent.Id;
            existingEntity.Event = originalEvent;
            _messageService!.Update(existingEntity);
        }
    }

    public void SendNotification(string eventId)
    {
        var originalEvent = _eventService!
            .AsQueryable()
            .FirstOrDefault(e => e.Id == eventId);
                
        var configuration = _configurationService!
            .AsQueryable()
            .FirstOrDefault(c => c.Id == originalEvent!.ConfigurationId); 
        
        var group = _groupService!
            .AsQueryable()
            .Include(g => g.Users)
            .FirstOrDefault(g => g.Id == configuration!.Id);
        
        CreateNotification(originalEvent!, out string notificationId);
        CreateNotificationUser(group!, notificationId);
    }
    
    private void CreateNotification(Event eventItem, out string notificationId)
    {
        var sequence = _notificationService!
            .GetAllAsync()
            .Result.ToList();
        
        var newId = Helpers.GetNewIdEntity(sequence);
        var notification = new Notification
        {
            Id = newId.ToString(),
            Message = eventItem.EventDescription,
            EventId = eventItem.Id
        };

        _notificationService?.Create(notification);
        notificationId = notification.Id;
    }

    private void CreateNotificationUser(Group group, string notificationId)
    {
        foreach (var user in group.Users.ToList())
        {
            var sequence = _notificationUserService!.GetAllAsync().Result;
            
            var newId = Helpers.GetNewIdEntity(sequence);
            var notificationUser = new NotificationUser
            {
                Id = newId.ToString(), 
                UserId = user.Id,
                NotificationId = notificationId
            };
            
            _notificationUserService?.Create(notificationUser);
        }
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
}
