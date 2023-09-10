namespace NotifyMe.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IChangeRepository ChangeRepository { get; }
        IEventRepository EventRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IConfigurationRepository ConfigurationRepository { get; }
        IMessageRepository MessageRepository { get; }
        IGroupRepository GroupRepository { get; }
        IUserRepository UserRepository { get; }

        Task CommitAsync();
        void Commit();
    }
}
