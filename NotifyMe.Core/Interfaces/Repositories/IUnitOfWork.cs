namespace NotifyMe.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IEventMonitoringRepository EventMonitoringRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IGroupRepository GroupRepository { get; }
        IUserRepository UserRepository { get; }

        Task CommitAsync();
        void Commit();
    }
}
