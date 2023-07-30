using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces
{
    public interface INotificationRepository
    {
        void SendNotification(Notification notification);
    }
}
