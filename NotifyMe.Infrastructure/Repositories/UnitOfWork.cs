using Microsoft.EntityFrameworkCore;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool disposed = false;
        private readonly DatabaseContext _dbContext;
        
        private readonly IEventMonitoringRepository _eventMonitoringRepository = null!;
        private readonly INotificationRepository _notificationRepository = null!;
        private readonly IGroupRepository _groupRepository = null!;
        private readonly IUserRepository _userRepository = null!;
        
        public IEventMonitoringRepository EventMonitoringRepository => _eventMonitoringRepository ?? new EventMonitoringRepository(_dbContext);
        public INotificationRepository NotificationRepository => _notificationRepository ?? new NotificationRepository(_dbContext);
        public IGroupRepository GroupRepository => _groupRepository ?? new GroupRepository(_dbContext);
        public IUserRepository UserRepository => _userRepository ?? new UserRepository(_dbContext);

        public UnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task CommitAsync()
        {
            try
            {
                _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
            }
            
            return Task.CompletedTask;
        }
        
        public void Commit()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            
            catch (DbUpdateConcurrencyException  e)
            {
                Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
                this.disposed = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
