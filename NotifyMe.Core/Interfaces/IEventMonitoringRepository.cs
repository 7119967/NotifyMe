using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces
{
    public interface IEventMonitoringRepository
    {
        EventMonitoring GetById(int id);
        void Add(EventMonitoring eventMonitoring);
    }
}