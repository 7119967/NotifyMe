using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces
{
    public interface IEventMonitoringService
    {
        void LogEvent(string name, string description);
    }
}
