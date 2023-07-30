using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces
{
    public interface INotificationService
    {
        void SendNotification(Notification notification);
    }
}
