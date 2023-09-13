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
    private readonly IChangeService? _changeService;

    public EventMonitor(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.GetService<IServiceScopeFactory>()?.CreateScope();
        _dbContext = scope?.ServiceProvider.GetRequiredService<DatabaseContext>();
        _rabbitMqService = scope?.ServiceProvider.GetRequiredService<IRabbitMqService>();
        _changeService = scope?.ServiceProvider.GetRequiredService<IChangeService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(15000, stoppingToken);

            try
            {
                var changes = _dbContext.Changes.ToListAsync(cancellationToken: stoppingToken).Result ?? throw new NullReferenceException();
                
                if (changes.Count() == 0) 
                {
                    await Console.Out.WriteLineAsync("There is no changes. Create a change");
                    continue;
                }

                var currentCounterCreation = changes.Count(t => 
                    t.ChangeType == ChangeType.Creation && 
                    t.Timestamp.Date == DateTime.Now.Date && 
                    string.IsNullOrEmpty(t.EventId));
                var currentCounterUpdate = changes.Count(t => 
                    t.ChangeType == ChangeType.Update &&
                    t.Timestamp.Date == DateTime.Now.Date && 
                    string.IsNullOrEmpty(t.EventId));
                var currentCounterDeletion = changes.Count(t => 
                    t.ChangeType == ChangeType.Deletion && 
                    t.Timestamp.Date == DateTime.Now.Date && 
                    string.IsNullOrEmpty(t.EventId));
                var currentCounterView = changes.Count(t => 
                    t.ChangeType == ChangeType.View && 
                    t.Timestamp.Date == DateTime.Now.Date && 
                    string.IsNullOrEmpty(t.EventId));

                var configurations = _dbContext?.Configurations?.ToListAsync(cancellationToken: stoppingToken).Result ?? throw new NullReferenceException();

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
                                var eventItem = CreateEvent(configuration, currentCounterCreation);
                                await AddEventToRelativeChanges(configuration.ChangeType, eventItem);
                                _rabbitMqService?.SendMessage(eventItem.Id);
                            }
                            break;
                        
                        case ChangeType.Update:
                            if (currentCounterUpdate >= configuration.Threshold)
                            {
                                var eventItem = CreateEvent(configuration, currentCounterUpdate);
                                await AddEventToRelativeChanges(configuration.ChangeType, eventItem);
                                _rabbitMqService?.SendMessage(eventItem.Id);
                            }
                            break;
                        
                        case ChangeType.Deletion:
                            if (currentCounterDeletion >= configuration.Threshold)
                            {
                                var eventItem = CreateEvent(configuration, currentCounterDeletion);
                                await AddEventToRelativeChanges(configuration.ChangeType, eventItem);
                                _rabbitMqService?.SendMessage(eventItem.Id);
                            }
                            break;
                        
                        case ChangeType.View:
                            if (currentCounterView >= configuration.Threshold)
                            {
                                var eventItem = CreateEvent(configuration, currentCounterView);
                                await AddEventToRelativeChanges(configuration.ChangeType, eventItem);
                                //var message = CreateMessage(eventItem);
                                _rabbitMqService?.SendMessage(eventItem.Id);
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
    
    private async Task AddEventToRelativeChanges (ChangeType changeType, Event eventItem)
    {
        var collection = _dbContext?.Changes.Where(t =>
            t.ChangeType == changeType && 
            t.Timestamp.Date == DateTime.Now.Date && 
            string.IsNullOrEmpty(t.EventId));
        
        foreach (var item in collection!)
        {
            item.EventId = eventItem.Id;
            item.Event = eventItem;
            await _changeService!.UpdateAsync(item);
        }

        await Task.CompletedTask;
    }
    
    
    private Event CreateEvent (Configuration configuration, int currentValue)
    {
        var model = new Event
        {
            EventName = $"{configuration.ChangeType} exceeded threshold",
            EventDescription = $"{configuration.Message}. \nCurrent value: {currentValue}, Threshold: {configuration.Threshold}",
            CurrentThreshold = currentValue,
            ConfigurationId = configuration.Id,
            Configuration = configuration
        };
        
        var seequence = _dbContext?.Events.AsEnumerable();
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

        var existingEntity = _dbContext?.Events.Find(model.Id);

        if (existingEntity != null)
        {
            var entity = _dbContext?.Events.Update(model);
            _dbContext?.SaveChanges();
            return entity!.Entity;
        }
        else
        {
            var entity = _dbContext?.Events.AddAsync(model).Result;
            _dbContext?.SaveChanges();
            return entity!.Entity;
        }
    }
}