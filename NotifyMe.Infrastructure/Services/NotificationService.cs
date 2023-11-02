using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services
{
    public class NotificationService : Service<Notification>, INotificationService
    {
        public NotificationService(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
