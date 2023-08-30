using System.Linq.Expressions;
using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces
{
    public interface INotificationService
    {
        void SendNotification(Notification notification);
        Task<ICollection<Notification>> GetListEntitiesAsync(Expression<Func<Notification, bool>> filter);
        Task<ICollection<Notification>> GetAllAsync();
        Task<Notification> GetEntityAsync(Expression<Func<Notification, bool>> filter);
        Task<Notification?> GetByIdAsync(string entityId);
        Task CreateAsync(Notification entity);
        Task UpdateAsync(Notification entity);
        Task DeleteAsync(string entityId);
    }
}
