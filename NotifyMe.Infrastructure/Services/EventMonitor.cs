using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Enums;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services;

public class EventMonitor : BackgroundService
{
    private readonly IEventService? _eventService;
    private readonly IChangeService? _changeService;
    private readonly IServiceScopeFactory? _scopeFactory;
    private readonly IRabbitMqPublisher? _rabbitMqPublisher;
    private readonly IConfigurationService? _configurationService;

    public EventMonitor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        
        var scope = scopeFactory.CreateScope();
        _rabbitMqPublisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();
        _changeService = scope.ServiceProvider.GetRequiredService<IChangeService>();
        _eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        _configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var processingActions = new Dictionary<ChangeType, Action<int, Configuration>>
        {
            { ChangeType.Creation, InvokeProcessing },
            { ChangeType.Deletion, InvokeProcessing },
            { ChangeType.Update, InvokeProcessing },
            { ChangeType.View, InvokeProcessing }
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10000, stoppingToken);
            try
            {
                var changes = await GetChangesAsListAsync(_changeService!);
                if (changes == null) throw new ArgumentNullException(nameof(changes));
                var configurations = await GetConfigurationsAsListAsync(_configurationService!);
                if (configurations == null) throw new ArgumentNullException(nameof(configurations));

                foreach (var configuration in configurations)
                {
                    if (processingActions.TryGetValue(configuration.ChangeType, out var action))
                    {
                        action(GetCounterByChangeType(changes, configuration.ChangeType), configuration);
                    }
                    else
                    {
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
        eventItem = await UpdateRelatedEntities(configuration, eventItem);
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var jsonString = JsonConvert.SerializeObject(eventItem, settings);
        _rabbitMqPublisher?.PublishMessage(jsonString);
    }

    private async Task<Event> UpdateRelatedEntities(Configuration configuration, Event eventItem)
    {
        await UpdateEventChanges(configuration, eventItem);
        await UpdateEventConfiguration(configuration, eventItem);

        return eventItem;
    }

    private async Task<Event> CreateEvent(Configuration configuration, int currentValue)
    {
        var sequence = await _eventService!.GetAllAsync();
        var newId = Helpers.GetNewIdEntity(sequence);
        var model = new Event
        {
            Id = newId.ToString(),
            EventName = $"{configuration.ChangeType} exceeded threshold",
            EventDescription =
                $"{configuration.Message}. \nCurrent value: {currentValue}, Threshold: {configuration.Threshold}",
            CurrentThreshold = currentValue,
        };

        try
        {
            using var scope = _scopeFactory?.CreateScope();
            var dbContext = scope?.ServiceProvider.GetRequiredService<IEventService>();
            var entity = dbContext?.Create(model);
            return entity!.Entity;
            // var entity = _eventService!.Create(model);
            // return entity.Entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private int CountChangesByType(IEnumerable<Change> changes, ChangeType type)
    {
        return changes.Count(t =>
            t.ChangeType == type &&
            t.Timestamp.Date == DateTime.Now.Date &&
            string.IsNullOrEmpty(t.EventId));
    }

    private void InvokeProcessing(int counter, Configuration configuration)
    {
        if (counter < configuration.Threshold) return;
        var thread = new Thread(() => ProcessingEvent(configuration, counter).Wait());
        thread.Start();
        //await Task.Run(() => ProcessingEvent(configuration, counter));
    }
    
    private int GetCounterByChangeType(IEnumerable<Change> changes, ChangeType type)
    {
        int counter;
        
        switch (type)
        {
            case ChangeType.View:
                counter = CountChangesByType(changes, ChangeType.View);
                return counter;
            case ChangeType.Update:
                counter = CountChangesByType(changes, ChangeType.Update);
                return counter;
            case ChangeType.Deletion:
                counter = CountChangesByType(changes, ChangeType.Deletion);
                return counter;
            case ChangeType.Creation:
                counter = CountChangesByType(changes, ChangeType.Creation);
                return counter;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private async Task UpdateEventChanges(Configuration configuration, Event eventItem)
    {
        var currentDate = DateTime.Now.Date;
        var changes = await _changeService!.GetAllAsync();

        foreach (var change in changes.Where(c => 
                     c.ChangeType == configuration.ChangeType &&
                     c.Timestamp.Date == currentDate &&
                     c.EventId == null))
        {
            change.EventId = eventItem.Id;
            change.Event = eventItem;
            _changeService!.Update(change);
        }
    }

    private async Task UpdateEventConfiguration(Configuration configuration, Event eventItem)
    {
        var existEvent = _eventService!
            .AsEnumerable()
            .FirstOrDefault(e => e.Id == eventItem.Id);

        if (existEvent != null)
        {
            existEvent.ConfigurationId = configuration.Id;
            existEvent.Configuration = configuration;
            await Task.Run(() => _eventService!.Update(existEvent));
        }
    }

    private async Task<List<Change>> GetChangesAsListAsync(IChangeService changeService)
    {
        var changes = await Task.Run(() => changeService.AsQueryable().ToList());

        if (changes.Any()) return changes;
        
        Console.WriteLine("There are no changes. Please, create at least one change or more");
        return changes;
    }
    
    private async Task<List<Configuration>> GetConfigurationsAsListAsync(IConfigurationService configurationService)
    {
        var configurations = await Task.Run(() => configurationService.AsQueryable().ToList());

        if (configurations.Any()) return configurations;
        
        Console.WriteLine("There are no configurations. Please, create at least one configuration or more");
        return configurations;

    }
}