using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool disposed = false;
        private readonly DatabaseContext _dbContext;

        private readonly IChangeRepository _changeRepository = null!;
        private readonly IEventRepository _eventRepository = null!;
        private readonly INotificationRepository _notificationRepository = null!;
        private readonly IConfigurationRepository _configurationRepository = null!;
        private readonly IMessageRepository _messageRepository = null!;
        private readonly IGroupRepository _groupRepository = null!;
        private readonly IUserRepository _userRepository = null!;
        private readonly INotificationUserRepository _notificationUserRepository = null!;

        public IChangeRepository ChangeRepository => _changeRepository ?? new ChangeRepository(_dbContext);
        public IEventRepository EventRepository => _eventRepository ?? new EventRepository(_dbContext);
        public INotificationRepository NotificationRepository => _notificationRepository ?? new NotificationRepository(_dbContext);
        public IConfigurationRepository ConfigurationRepository => _configurationRepository ?? new ConfigurationRepository(_dbContext);
        public IMessageRepository MessageRepository => _messageRepository ?? new MessageRepository(_dbContext);
        public IGroupRepository GroupRepository => _groupRepository ?? new GroupRepository(_dbContext);
        public IUserRepository UserRepository => _userRepository ?? new UserRepository(_dbContext);
        public INotificationUserRepository NotificationUserRepository => _notificationUserRepository ?? new NotificationUserRepository(_dbContext);

        public UnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CommitAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
            }
        }

        public void Commit()
        {
            try
            {
                _dbContext.SaveChanges();
            }

            catch (DbUpdateConcurrencyException e)
            {
                Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
