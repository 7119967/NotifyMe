using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Enums;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services;

public class EventMonitor : BackgroundService
{
    private readonly IEventService? _eventService;
    private readonly ILogger<EventMonitor> _logger;
    private readonly IChangeService? _changeService;
    private readonly IServiceScopeFactory? _scopeFactory;
    private readonly IRabbitMqPublisher? _rabbitMqPublisher;
    private readonly IConfigurationService? _configurationService;

    public EventMonitor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        var scope = scopeFactory.CreateScope();
        _eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<EventMonitor>>();
        _changeService = scope.ServiceProvider.GetRequiredService<IChangeService>();
        _rabbitMqPublisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();
        _configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var processingActions = new Dictionary<ChangeType, Action<int, Configuration>>
        {
            { ChangeType.Creation, InvokeProcessingAsync },
            { ChangeType.Deletion, InvokeProcessingAsync },
            { ChangeType.Update, InvokeProcessingAsync },
            { ChangeType.View, InvokeProcessingAsync }
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10000, stoppingToken);
            try
            {
                var changes = await GetListChangesAsync(_changeService!);
                if (changes == null) throw new ArgumentNullException(nameof(changes));
                
                var configurations = await GetConfigurationsAsListAsync(_configurationService!);
                if (configurations == null) throw new ArgumentNullException(nameof(configurations));

                foreach (var configuration in configurations)
                {
                    if (processingActions.TryGetValue(configuration.ChangeType, out var action))
                    {
                        action(await GetCounterByChangeTypeAsync(changes, configuration.ChangeType), configuration);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"{ex.Message}");
            }
        }
    }

    private void InvokeProcessingAsync(int counter, Configuration configuration)
    {
        if (counter < configuration.Threshold) return;
        // var thread = new Thread(() => ProcessingEventAsync(configuration, counter).Wait());
        // _logger.LogDebug($"Thread name: {thread.Name}, Thread id: {thread.ManagedThreadId}");
        // thread.Start();
        ProcessingEventAsync(configuration, counter).Wait();
    }
    
    private async Task ProcessingEventAsync(Configuration configuration, int counter)
    {
        var eventItem = await CreateEventAsync(configuration, counter);
        eventItem = await UpdateRelatedEntitiesAsync(configuration, eventItem!);
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var jsonString = JsonConvert.SerializeObject(eventItem, settings);
        _rabbitMqPublisher?.PublishMessage(jsonString);
    }

    private async Task<Event> UpdateRelatedEntitiesAsync(Configuration configuration, Event eventItem)
    {
        await UpdateEventChangesAsync(configuration, eventItem);
        await UpdateEventConfigurationAsync(configuration, eventItem);

        return eventItem;
    }

    private async Task<Event?> CreateEventAsync(Configuration configuration, int currentValue)
    {
        var sequence = await _eventService!.GetAllAsync();
        var newId = Helpers.GetNewIdEntity(sequence);
        var model = new Event
        {
            Id = newId.ToString(),
            EventName = $"{configuration.ChangeType} exceeded threshold",
            EventDescription = $"{configuration.Message}. \n" +
                               $"Current value: {currentValue}, Threshold: {configuration.Threshold}",
            CurrentThreshold = currentValue,
        };

        try
        {
            using var scope = _scopeFactory?.CreateScope();
            var dbContext = scope?.ServiceProvider.GetRequiredService<IEventService>();
            var entity = dbContext?.Create(model);
            return entity!.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogDebug($"{ex.Message}");
            return null;
        }
    }
    
    private static async Task<int> GetCounterByChangeTypeAsync(IEnumerable<Change> changes, ChangeType type)
    {
        int counter;
        
        switch (type)
        {
            case ChangeType.View:
                counter = await Task.Run(() => CountChangesByType(changes, ChangeType.View));
                return counter;
            case ChangeType.Update:
                counter = await Task.Run(() => CountChangesByType(changes, ChangeType.Update));
                return counter;
            case ChangeType.Deletion:
                counter = await Task.Run(() => CountChangesByType(changes, ChangeType.Deletion));
                return counter;
            case ChangeType.Creation:
                counter = await Task.Run(() => CountChangesByType(changes, ChangeType.Creation));
                return counter;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private static int CountChangesByType(IEnumerable<Change> changes, ChangeType type)
    {
        return changes.Count(change =>
            change.ChangeType == type &&
            change.Timestamp.Date == DateTime.Now.Date &&
            string.IsNullOrEmpty(change.EventId));
    }
    
    private async Task UpdateEventChangesAsync(Configuration configuration, Event eventItem)
    {
        var currentDate = DateTime.Now.Date;
        var changes = (await _changeService!.GetAllAsync()).Where(c => 
            c.ChangeType == configuration.ChangeType &&
            c.Timestamp.Date == currentDate &&
            c.EventId == null);

        foreach (var change in changes)
        {
            change.EventId = eventItem.Id;
            change.Event = eventItem;
            _changeService!.Update(change);
        }
    }

    private async Task UpdateEventConfigurationAsync(Configuration configuration, Event eventItem)
    {
        var existEvent = await _eventService!
            .AsQueryable()
            .FirstOrDefaultAsync(e => e.Id == eventItem.Id);

        if (existEvent != null)
        {
            existEvent.ConfigurationId = configuration.Id;
            existEvent.Configuration = configuration;
            await Task.Run(() => _eventService!.Update(existEvent));
        }
    }

    private async Task<List<Change>> GetListChangesAsync(IChangeService changeService)
    {
        var changes = await changeService.AsQueryable().ToListAsync();
        if (changes.Any()) return changes;
        _logger.LogDebug("There are no changes. Please, create at least one change or more");
        return changes;
    }
    
    private async Task<List<Configuration>> GetConfigurationsAsListAsync(IConfigurationService configurationService)
    {
        var configurations = await configurationService.AsQueryable().ToListAsync();
        if (configurations.Any()) return configurations;
        _logger.LogDebug("There are no configurations. Please, create at least one configuration or more");
        return configurations;

    }
}