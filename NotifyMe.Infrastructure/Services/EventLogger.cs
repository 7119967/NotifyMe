using Microsoft.Extensions.Logging;

using NotifyMe.Core.Interfaces;

namespace NotifyMe.Infrastructure.Services
{
    public class EventLogger : IEventLogger
    {
        private readonly ILogger<EventLogger> _logger;

        public EventLogger(ILogger<EventLogger> logger)
        {
            _logger = logger;
        }

        public void LogEvent<TEvent>(TEvent eventData)
        {
            _logger.LogInformation($"Event: {typeof(TEvent).Name} - Data: {eventData}");
        }
    }
}
