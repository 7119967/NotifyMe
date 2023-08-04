using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _dbContext;

        public UnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            EventMonitoringRepository = new Repository<EventMonitoring>(_dbContext);
            NotificationRepository = new Repository<Notification>(_dbContext);
        }

        public IRepository<EventMonitoring> EventMonitoringRepository { get; private set; }
        public IRepository<Notification> NotificationRepository { get; private set; }

        public void Complete()
        {
            _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
