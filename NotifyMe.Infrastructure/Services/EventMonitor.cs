using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Enums;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services;

public class EventMonitor : BackgroundService
{
    private readonly IRabbitMqPublisher? _rabbitMqPublisher;
    private readonly IChangeService? _changeService;
    private readonly IEventService? _eventService;
    private readonly IConfigurationService? _configurationService;
    private readonly IServiceScopeFactory? _scopeFactory;

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
            { ChangeType.Update, InvokeProcessing },
            { ChangeType.Deletion, InvokeProcessing },
            { ChangeType.View, InvokeProcessing }
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10000, stoppingToken);
            try
            {
                var changes = _changeService?.AsQueryable().ToList() ?? throw new NullReferenceException();

                if (!changes.Any())
                {
                    Console.WriteLine("There are no changes. Please, create at least one change or more");
                    continue;
                }

                var configurations = _configurationService!.AsQueryable().ToList() ?? throw new NullReferenceException();

                if (!configurations.Any())
                {
                    Console.WriteLine("There are no configurations. Please, create at least one configuration or more");
                    continue;
                }

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

    private void ProcessingEvent(Configuration configuration, int counter)
    {
        var eventItem = CreateEvent(configuration, counter).Result;
        eventItem = UpdateRelatedEntities(configuration, eventItem).Result;
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var jsonString = JsonConvert.SerializeObject(eventItem, settings);
        _rabbitMqPublisher?.PublishMessage(jsonString);
    }

    private async Task<Event> UpdateRelatedEntities(Configuration configuration, Event eventItem)
    {
        var currentDate = DateTime.Now.Date;
        
        await UpdateEventChanges(configuration, eventItem, currentDate);
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
           using (var scope = _scopeFactory?.CreateScope())
           {
               var dbContext = scope?.ServiceProvider.GetRequiredService<IEventService>();
               var entity = dbContext?.Create(model);
               return entity!.Entity;
           }
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
        var thread = new Thread(() => ProcessingEvent(configuration, counter));
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
    
    private async Task UpdateEventChanges(Configuration configuration, Event eventItem, DateTime currentDate)
    {
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
}