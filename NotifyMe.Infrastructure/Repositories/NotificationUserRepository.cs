using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.Infrastructure.Repositories;

public class NotificationUserRepository: Repository<NotificationUser>, INotificationUserRepository
{
    public NotificationUserRepository(DatabaseContext context) : base(context) { }
}