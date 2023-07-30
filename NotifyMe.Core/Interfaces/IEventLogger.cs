using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces
{
    public interface IEventLogger
    {
        void LogEvent<TEvent>(TEvent eventData);
    }
}
