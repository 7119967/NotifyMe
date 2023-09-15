using System.Text.Json;
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
    private readonly IEventService? _eventService;

    public EventMonitor(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.GetService<IServiceScopeFactory>()?.CreateScope();
        _dbContext = scope?.ServiceProvider.GetRequiredService<DatabaseContext>();
        _rabbitMqService = scope?.ServiceProvider.GetRequiredService<IRabbitMqService>();
        _changeService = scope?.ServiceProvider.GetRequiredService<IChangeService>();
        _eventService = scope?.ServiceProvider.GetRequiredService<IEventService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(15000, stoppingToken);

            try
            {
                var changes = _dbContext.Changes.ToListAsync(cancellationToken: stoppingToken).Result ??
                              throw new NullReferenceException();

                if (changes.Count() == 0)
                {
                    Console.WriteLine("There is no changes. Create a change");
                    continue;
                }

                var counterCreation = changes.Count(t =>
                    t.ChangeType == ChangeType.Creation &&
                    t.Timestamp.Date == DateTime.Now.Date &&
                    string.IsNullOrEmpty(t.EventId));
                var counterUpdate = changes.Count(t =>
                    t.ChangeType == ChangeType.Update &&
                    t.Timestamp.Date == DateTime.Now.Date &&
                    string.IsNullOrEmpty(t.EventId));
                var counterDeletion = changes.Count(t =>
                    t.ChangeType == ChangeType.Deletion &&
                    t.Timestamp.Date == DateTime.Now.Date &&
                    string.IsNullOrEmpty(t.EventId));
                var counterView = changes.Count(t =>
                    t.ChangeType == ChangeType.View &&
                    t.Timestamp.Date == DateTime.Now.Date &&
                    string.IsNullOrEmpty(t.EventId));

                var configurations = _dbContext?.Configurations?.ToListAsync(cancellationToken: stoppingToken).Result ??
                                     throw new NullReferenceException();

                if (!configurations.Any())
                {
                    await Console.Out.WriteLineAsync("There is no configurations. Create a Configuration");
                    continue;
                }

                foreach (var configuration in configurations)
                {
                    switch (configuration.ChangeType)
                    {
                        case ChangeType.Creation:
                            if (counterCreation >= configuration.Threshold)
                                await ProcessingEvent(configuration, counterCreation);
                            break;

                        case ChangeType.Update:
                            if (counterUpdate >= configuration.Threshold)
                                await ProcessingEvent(configuration, counterUpdate);
                            break;

                        case ChangeType.Deletion:
                            if (counterDeletion >= configuration.Threshold)
                                await ProcessingEvent(configuration, counterDeletion);
                            break;

                        case ChangeType.View:
                            if (counterView >= configuration.Threshold)
                                await ProcessingEvent(configuration, counterView);
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

    private async Task ProcessingEvent(Configuration configuration, int counter)
    {
        var eventItem = CreateEvent(configuration, counter);
        await AddEventToRelativeChanges(configuration.ChangeType, eventItem);
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonString = JsonSerializer.Serialize(eventItem, options);
        _rabbitMqService?.SendMessage(jsonString);
    }

    private async Task AddEventToRelativeChanges(ChangeType changeType, Event eventItem)
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


    private Event CreateEvent(Configuration configuration, int currentValue)
    {
        var model = new Event
        {
            EventName = $"{configuration.ChangeType} exceeded threshold",
            EventDescription =
                $"{configuration.Message}. \nCurrent value: {currentValue}, Threshold: {configuration.Threshold}",
            CurrentThreshold = currentValue,
            ConfigurationId = configuration.Id,
            Configuration = configuration
        };

        var seequence = _eventService?.GetAllAsync().Result;
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

        var entity = _eventService?.Create(model);
        return entity!.Entity;
        
        // var existingEntity = _eventService?.GetByIdAsync(model.Id).Result;
        //
        // if (existingEntity != null)
        // {
        //     var entity = _eventService?.UpdateAsync(model);
        //     return entity;
        // }
        // else
        // {
        //   
        // }
    }
}