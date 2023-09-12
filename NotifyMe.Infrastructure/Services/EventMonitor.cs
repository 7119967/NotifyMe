using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Enums;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.Infrastructure.Services;

public class EventMonitor : BackgroundService
{
    private readonly DatabaseContext? _dbContext;
    private readonly IRabbitMqService? _rabbitMqService;

    public EventMonitor(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.GetService<IServiceScopeFactory>()?.CreateScope();
        _dbContext = scope?.ServiceProvider.GetRequiredService<DatabaseContext>();
        _rabbitMqService = scope?.ServiceProvider.GetRequiredService<IRabbitMqService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);

            try
            {
                var changes = _dbContext?.Changes.ToListAsync(cancellationToken: stoppingToken).Result ?? throw new NullReferenceException();
                
                if (changes.Count() == 0) 
                {
                    await Console.Out.WriteLineAsync("There is no changes. Create a change");
                    continue;
                }

                var currentCounterCreation = changes.Count(t => t.ChangeType == ChangeType.Creation && t.Timestamp == DateTime.UtcNow);
                var currentCounterUpdate = changes.Count(t => t.ChangeType == ChangeType.Update && t.Timestamp == DateTime.UtcNow);
                var currentCounterDeletion = changes.Count(t => t.ChangeType == ChangeType.Deletion && t.Timestamp == DateTime.UtcNow);
                var currentCounterView = changes.Count(t => t.ChangeType == ChangeType.View && t.Timestamp == DateTime.UtcNow);

                var configurations = _dbContext?.Configurations.ToListAsync(cancellationToken: stoppingToken).Result ?? throw new NullReferenceException();

                if (configurations.Count() == 0)
                {
                    await Console.Out.WriteLineAsync("There is no configurations. Create a Configuration");
                    continue;
                }

                foreach (var configuration in configurations)
                {
                    switch (configuration.ChangeType)
                    {
                        case ChangeType.Creation:
                            if (currentCounterCreation >= configuration.Threshold)
                            {
                                var eventCreation = CreateEvent(configuration, currentCounterCreation);
                                _dbContext?.Events.AddAsync(eventCreation);
                                AddEventToRelativeChanges(changes, currentCounterCreation, eventCreation);
                                await SendAlertAsync(ChangeType.Creation, currentCounterCreation, configuration.Threshold);
                                var message = CreateMessage(ChangeType.Creation, currentCounterCreation, configuration.Threshold);
                                _rabbitMqService?.SendMessage(message);
                            }
                            break;
                        
                        case ChangeType.Update:
                            if (currentCounterUpdate >= configuration.Threshold)
                            {
                                var eventCreation = CreateEvent(configuration, currentCounterUpdate);
                                _dbContext?.Events.AddAsync(eventCreation);
                                AddEventToRelativeChanges(changes, currentCounterUpdate, eventCreation);
                                await SendAlertAsync(ChangeType.Update, currentCounterUpdate, configuration.Threshold);
                                var message = CreateMessage(ChangeType.Update, currentCounterUpdate, configuration.Threshold);
                                _rabbitMqService?.SendMessage(message);
                            }
                            break;
                        
                        case ChangeType.Deletion:
                            if (currentCounterDeletion >= configuration.Threshold)
                            {
                                var eventCreation = CreateEvent(configuration, currentCounterDeletion);
                                _dbContext?.Events.AddAsync(eventCreation);
                                AddEventToRelativeChanges(changes, currentCounterDeletion, eventCreation);
                                await SendAlertAsync(ChangeType.Deletion, currentCounterDeletion, configuration.Threshold);
                                var message = CreateMessage(ChangeType.Deletion, currentCounterDeletion, configuration.Threshold);
                                _rabbitMqService?.SendMessage(message);
                            }
                            break;
                        
                        case ChangeType.View:
                            if (currentCounterView >= configuration.Threshold)
                            {
                                var eventCreation = CreateEvent(configuration, currentCounterView);
                                _dbContext?.Events.AddAsync(eventCreation);
                                AddEventToRelativeChanges(changes, currentCounterView, eventCreation);
                                await SendAlertAsync(ChangeType.View, currentCounterView, configuration.Threshold);
                                var message = CreateMessage(ChangeType.View, currentCounterView, configuration.Threshold);
                                _rabbitMqService?.SendMessage(message);
                            }
                            break;
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    
    private void AddEventToRelativeChanges (List<Change> changes, int currentValue, Event eventItem)
    {
        var collectionCreation = changes.Take(currentValue);
                    
        foreach (var item in collectionCreation)
        {
            item.EventId = eventItem.Id;
            _dbContext?.Changes.Update(item);
        }
    }
    private Message CreateMessage (Configuration configuration, int currentValue)
    {
        var receivers = new List<User>();
        foreach (var user in configuration.Group!.Users)
        {
            receivers.Add(user.Email);
        }

        return new Message
        {
            Sender = "",
            Receivers = receivers,

            Subject = $"ALERT: {configuration.ChangeType} exceeded threshold",
            ContentBody =
                $"ALERT: {configuration.ChangeType} exceeded threshold. Current value: {currentValue}, Threshold: {configuration.Threshold}"
        };
    }
    
    private Event CreateEvent (Configuration configuration, int currentValue)
    {
        return new Event
        {
            EventName = $"{configuration.ChangeType} exceeded threshold",
            EventDescription = $"{configuration.Message}. \nCurrent value: {currentValue}, Threshold: {configuration.Threshold}",
            ConfigurationId = configuration.Id
        };
    }
    
    private async Task SendAlertAsync(ChangeType changeType, int? currentValue, double thresholdValue)
    {
        await Task.Run(() =>
            Console.WriteLine(
                $"ALERT: {changeType} exceeded threshold. Current value: {currentValue}, Threshold: {thresholdValue}"));
    }
}