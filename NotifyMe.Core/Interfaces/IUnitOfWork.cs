using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<EventMonitoring> EventMonitoringRepository { get; }
        IRepository<Notification> NotificationRepository { get; }
        IRepository<User> UserRepository { get; }

        void Complete();
    }
}
