using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _dbContext;
        public IRepository<EventMonitoring> EventMonitoringRepository { get; private set; }
        public IRepository<Notification> NotificationRepository { get; private set; }
        public IRepository<Group> GroupRepository { get; private set; }
        public IRepository<User> UserRepository { get; private set; }

        public UnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            EventMonitoringRepository = new Repository<EventMonitoring>(_dbContext);
            NotificationRepository = new Repository<Notification>(_dbContext);
            GroupRepository = new Repository<Group>(_dbContext);
            UserRepository = new Repository<User>(_dbContext);
        }

        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
