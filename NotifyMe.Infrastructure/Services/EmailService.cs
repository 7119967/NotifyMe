using System.Net;
using System.Net.Mail;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;

using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
namespace NotifyMe.Infrastructure.Services;

public class EmailService
{
    private readonly IMapper _mapper;
    private readonly string? _username;
    private readonly string? _password;
    private readonly IGroupService? _groupService;
    private readonly IEventService? _eventService;
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMessageService? _messageService;
    private readonly INotificationService? _notificationService;
    private readonly IConfigurationService? _configurationService;
    private readonly INotificationUserService? _notificationUserService;
    
    public EmailService(IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        _groupService = scope.ServiceProvider.GetRequiredService<IGroupService>();
        _eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        _configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<EmailService>>();
        _messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
        _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        _configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
        _notificationUserService = scope.ServiceProvider.GetRequiredService<INotificationUserService>();
        
        _username = _configuration["MailService:Credentials:Username"];
        _password = _configuration["MailService:Credentials:Password"];
    }
    
    public void SendEmail(string eventId)
    {
        try
        {
            var mailMessage = CreateMailMessage(eventId).Result;
            Task.Run(() => DepartureMailMessage(mailMessage, _username!, _password!, _configuration).Wait());
            CreateMessage(eventId, mailMessage).Wait();
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            _logger.LogDebug($"Sending mail failed");
        }
    }

    private Task DepartureMailMessage(MailMessage message, string user, string pass, IConfiguration config)
    {
        using var smtp = new SmtpClient();
        smtp.Credentials = new NetworkCredential(user, pass);
        smtp.Port = Convert.ToInt32(config["MailService:Port"]!);
        smtp.Host = config["MailService:Host"]!;
        smtp.UseDefaultCredentials = false;
        smtp.EnableSsl = true;
        var result = smtp.SendMailAsync(message).Status;
        if (result == TaskStatus.Faulted)
        {
            _logger.LogDebug($"{result}");
        }
        smtp.Dispose();
        return Task.CompletedTask;
    }

    private async Task<MailMessage> CreateMailMessage(string eventId)
    {
        var originalEvent = await GetEventViaConfigurationAsync(eventId);
        var configuration = await GetConfigurationAsync(originalEvent!);
        var group = await GetGroupViaUserAsync(configuration!);
        if (group!.Users.Count == 0)
        {
            _logger.LogDebug($"For configuration id: {configuration!.Id} there are no found correspondents to notify about the Event id:{eventId}");
            throw new ArgumentNullException();
        }
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_username!),
            Subject = $"ALERT: {originalEvent!.Configuration!.ChangeType} exceeded threshold",
            Body = $"{originalEvent.Configuration.Message}. \nCurrent value: {originalEvent.CurrentThreshold}, " +
                   $"Threshold: {originalEvent.Configuration.Threshold}"
        };

        foreach (var user in group.Users)
        {
            mailMessage.To.Add(user.Email ?? throw new InvalidOperationException());
        }

        return mailMessage;
    }

    private async Task CreateMessage(string eventId, MailMessage mailMessage)
    {
        var originalEvent = await GetEventViaConfigurationAsync(eventId);
        var message = _mapper.Map<MailMessage, Message>(mailMessage);
        var sequence = await _messageService!.GetAllAsync();
        var newId = Helpers.GetNewIdEntity(sequence);
        message.Id = newId.ToString();
        _messageService!.Create(message);
        var existingEntity = await GetExistingMessageAsync(message);
        if (existingEntity == null) return;
        existingEntity.EventId = originalEvent!.Id;
        existingEntity.Event = originalEvent;
        _messageService!.Update(existingEntity);
    }
    
    public void SendNotification(string eventId)
    {
        try
        {
            var originalEvent = GetEventAsync(eventId).Result;
            var configuration = GetConfigurationAsync(originalEvent!).Result;
            var group = GetGroupViaUserAsync(configuration!).Result;
            if (group!.Users.Count == 0)
            {
                _logger.LogDebug($"For configuration id: {configuration!.Id} there are no found correspondents to notify about the Event id:{eventId}");
                throw new ArgumentNullException();
            }
            CreateNotification(originalEvent!, out var notificationId);
            CreateNotificationUser(group, notificationId);
        }
        catch (Exception ex) when (ex is ArgumentNullException)
        {
            _logger.LogDebug($"Sending notification failed");
        }
    }
    
    private void CreateNotification(Event eventItem, out string notificationId)
    {
        var sequence = GetListNotifications();
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
            var sequence = GetListNotificationUsers();
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
    
    private async Task<Event?> GetEventAsync(string eventId)
    {
        return await _eventService!
            .AsQueryable()
            .FirstOrDefaultAsync(e => e.Id == eventId);
    }    
    
    private async Task<Event?> GetEventViaConfigurationAsync(string eventId)
    {
        return await _eventService!
            .AsQueryable()
            .Include(e => e.Configuration)
            .FirstOrDefaultAsync(e => e.Id == eventId);
    }
    
    private async Task<Group?> GetGroupViaUserAsync(Configuration item)
    {
        return await _groupService!
            .AsQueryable()
            .Include(g => g.Users)
            .FirstOrDefaultAsync(g => g.Id == item.Id);
    }

    private async Task<Message?> GetExistingMessageAsync(Message item)
    {
        return await _messageService!
            .AsQueryable()
            .FirstOrDefaultAsync(m => m.Id == item.Id);
    }
    
    private async Task<Configuration?> GetConfigurationAsync(Event item)
    {
        return await _configurationService!
            .AsQueryable()
            .FirstOrDefaultAsync(c => c.Id == item.ConfigurationId);
    }

    private List<Notification> GetListNotifications()
    {
        return _notificationService!
            .AsQueryable()
            .ToList();
    }

    private List<NotificationUser> GetListNotificationUsers()
    {
        return _notificationUserService!
            .AsQueryable()
            .ToList();
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
