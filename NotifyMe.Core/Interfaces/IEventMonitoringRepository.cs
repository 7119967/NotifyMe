using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces
{
    public interface IEventMonitoringRepository
    {
        EventMonitoring Get(int id);
        void Add(EventMonitoring eventMonitoring);
    }
}