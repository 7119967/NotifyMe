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
    
    public EmailService(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.GetService<IServiceScopeFactory>()?.CreateScope();
        _dbContext = scope?.ServiceProvider.GetRequiredService<DatabaseContext>();
        _configurationService = scope?.ServiceProvider.GetRequiredService<IConfigurationService>();
        _groupService = scope?.ServiceProvider.GetRequiredService<IGroupService>();
        _notificationService = scope?.ServiceProvider.GetRequiredService<INotificationService>();
        _notificationUserService = scope?.ServiceProvider.GetRequiredService<INotificationUserService>();
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
        var originalEvent = _dbContext?.Events.AsNoTracking().FirstOrDefault(e => e.Id == eventId);
                
        var configuration = _configurationService!
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == originalEvent!.ConfigurationId).Result; 
        
        var group = _groupService!
            .AsQueryable()
            .AsNoTracking()
            .Include(g => g.Users)
            .FirstOrDefaultAsync(t => t.Id == configuration!.Id)
            .Result
            ;
        
        CreateNotification(group!, originalEvent!);
        
        // await Task.CompletedTask;
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
            // Message = eventItem.EventDescription,
            // EventId = eventItem.Id,
            // Event = eventItem
        };

        _notificationService?.Create(notification);
        
        var notificationToUpdate = _notificationService!
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == newId.ToString()).Result;

        if (notificationToUpdate != null)
        {
            notificationToUpdate.Message = eventItem.EventDescription;
            notificationToUpdate.EventId = eventItem.Id;
            notificationToUpdate.Event = eventItem;
            
            _notificationService?.Update(notificationToUpdate);
        }
        
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
                // User = user,
                // UserId = user.Id,
                // NotificationId = notification.Id,
                // Notification = notification
            };
            
            _notificationUserService?.Create(notificationUser);
            
            var notificationUserToUpdate = _notificationUserService!
                .AsQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == newId2.ToString()).Result;

            if (notificationUserToUpdate != null)
            {
                notificationUserToUpdate.User = user;
                notificationUserToUpdate.UserId = user.Id;
                notificationUserToUpdate.NotificationId = notification.Id;
                notificationUserToUpdate.Notification = notification;
                
                _notificationUserService?.Update(notificationUser);
            }
        }
        
        
        // notification.NotificationUsers!.Add(new NotificationUser
        // {
        //     Id = newId2.ToString(),
        //     User = user,
        //     UserId = user.Id,
        //     NotificationId = notification.Id,
        //     Notification = notification
        // });
    }
}