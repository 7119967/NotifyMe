using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // Initialize other repositories as needed
        }

        public IRepository<EventMonitoring> EventMonitoringRepository { get; private set; }
        public IRepository<Notification> NotificationRepository { get; private set; }
        // Other repositories

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
