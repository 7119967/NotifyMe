using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services;

public class NotificationUserService : Service<NotificationUser>, INotificationUserService
{
    public NotificationUserService(IUnitOfWork unitOfWork) : base(unitOfWork) { }
}