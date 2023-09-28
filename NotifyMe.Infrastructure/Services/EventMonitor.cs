using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
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
    private readonly IConfigurationService? _configurationService;

    public EventMonitor(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.GetService<IServiceScopeFactory>()?.CreateScope();
        _dbContext = scope?.ServiceProvider.GetRequiredService<DatabaseContext>();
        _rabbitMqService = scope?.ServiceProvider.GetRequiredService<IRabbitMqService>();
        _changeService = scope?.ServiceProvider.GetRequiredService<IChangeService>();
        _eventService = scope?.ServiceProvider.GetRequiredService<IEventService>();
        _configurationService = scope?.ServiceProvider.GetRequiredService<IConfigurationService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(15000, stoppingToken);

            try
            {
                var changes = _changeService?
                                  .AsQueryable()
                                  .AsNoTracking()
                                  .ToList() ??
                              throw new NullReferenceException();
                
                if (!changes.Any())
                {
                    Console.WriteLine("There are no changes. Please, create at least one change or more");
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

                var configurations = _configurationService?
                                         .AsQueryable()
                                         .AsNoTracking()
                                         .ToList() ??
                                     throw new NullReferenceException();

                if (!configurations.Any())
                {
                    await Console.Out.WriteLineAsync("There are no configurations. Please, create at least one configuration or more");
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
        var eventItem = await CreateEvent(configuration, counter);
        eventItem = await AddRelationShipToRelativeEntities(configuration, eventItem);
        // var jsonString = JsonConvert.SerializeObject(eventItem, Formatting.Indented);
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var jsonString = JsonConvert.SerializeObject(eventItem, settings);
        _rabbitMqService?.SendMessage(jsonString);
    }

    private async Task<Event> AddRelationShipToRelativeEntities(Configuration configuration, Event eventItem)
    {
        var collection = _changeService!
            .AsQueryable()
            .AsNoTracking()
            .Where(t => 
                t.ChangeType == configuration.ChangeType && 
                t.Timestamp.Date == DateTime.Now.Date &&
                t.EventId == null)
            .ToList();
        
        foreach (var item in collection!)
        {
            item.EventId = eventItem.Id;
            item.Event = eventItem;
            await _changeService!.UpdateAsync(item);
        }

        var existEvent = await _eventService!
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventItem.Id);

        existEvent!.ConfigurationId = configuration.Id;
        existEvent.Configuration = configuration;

        var entry = _eventService!.Update(existEvent);
        return entry.Entity;
    }


    private async Task<Event> CreateEvent(Configuration configuration, int currentValue)
    {
        var sequence = _eventService!
            .AsQueryable()
            .AsNoTracking()
            .ToList();
        
        // var newId = sequence.Any() ? sequence.Max(e => Convert.ToInt32(e.Id)) + 1 : 1;
        var newId = Helpers.GetNewIdEntity(sequence);

        var model = new Event
        {
            EventName = $"{configuration.ChangeType} exceeded threshold",
            EventDescription = $"{configuration.Message}. \nCurrent value: {currentValue}, Threshold: {configuration.Threshold}",
            CurrentThreshold = currentValue,
            Id = newId.ToString()
        };

        try
        {
            var entity = await Task.Run(() => _eventService?.Create(model));
            return entity!.Entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}