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

    public EventMonitor(IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _rabbitMqPublisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();
        _changeService = scope.ServiceProvider.GetRequiredService<IChangeService>();
        _eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        _configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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

                var counterView = CountChangesByType(changes, ChangeType.View);
                var counterUpdate = CountChangesByType(changes, ChangeType.Update);
                var counterDeletion = CountChangesByType(changes, ChangeType.Deletion);
                var counterCreation = CountChangesByType(changes, ChangeType.Creation);

                var configurations = _configurationService!.AsQueryable().ToList() ?? throw new NullReferenceException();

                if (!configurations.Any())
                {
                    await Console.Out.WriteLineAsync(
                        "There are no configurations. Please, create at least one configuration or more");
                    continue;
                }

                foreach (var configuration in configurations)
                {
                    switch (configuration.ChangeType)
                    {
                        case ChangeType.Creation:
                            await InvokeProcessing(counterCreation, configuration);
                            break;
                        case ChangeType.Update:
                            await InvokeProcessing(counterUpdate, configuration);
                            break;
                        case ChangeType.Deletion:
                            await InvokeProcessing(counterDeletion, configuration);
                            break;
                        case ChangeType.View:
                            await InvokeProcessing(counterView, configuration);
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
        eventItem = UpdateEventAndRelativeEntities(configuration, eventItem);
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var jsonString = JsonConvert.SerializeObject(eventItem, settings);
        await Task.Run(() => _rabbitMqPublisher?.PublishMessage(jsonString));
    }

    private Event UpdateEventAndRelativeEntities(Configuration configuration, Event eventItem)
    {
        var collection = from c in _changeService!.AsEnumerable()
            where c.ChangeType == configuration.ChangeType &&
                  c.Timestamp.Date == DateTime.Now.Date &&
                  c.EventId == null
            select c;

        foreach (var item in collection)
        {
            item.EventId = eventItem.Id;
            item.Event = eventItem;
            _changeService!.UpdateAsync(item);
        }

        var existEvent = _eventService!
            .AsEnumerable()
            .FirstOrDefault(e => e.Id == eventItem.Id);

        existEvent!.ConfigurationId = configuration.Id;
        existEvent.Configuration = configuration;

        var entry = _eventService!.Update(existEvent);
        return entry.Entity;
    }

    private Event CreateEvent(Configuration configuration, int currentValue)
    {
        var sequence = _eventService!.AsEnumerable().ToList();
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
            var entity = _eventService!.Create(model);
            return entity.Entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private int CountChangesByType(IEnumerable<Change> changes, ChangeType changeType)
    {
        return changes.Count(t =>
            t.ChangeType == changeType &&
            t.Timestamp.Date == DateTime.Now.Date &&
            string.IsNullOrEmpty(t.EventId));
    }

    private async Task InvokeProcessing(int counter, Configuration configuration)
    {
        if (counter >= configuration.Threshold)
        {
            await ProcessingEvent(configuration, counter);
        }
    }
}