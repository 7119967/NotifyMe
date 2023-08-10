using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<EventMonitoring> EventMonitoringRepository { get; }
        IRepository<Notification> NotificationRepository { get; }
        IRepository<Group> GroupRepository { get; }
        IRepository<User> UserRepository { get; }

        Task CommitAsync();
    }
}
