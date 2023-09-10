using System.Linq.Expressions;
using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces
{
    public interface IEventService
    {
        void LogEvent(string name, string description);
        Task<ICollection<Event>> GetListEntitiesAsync(Expression<Func<Event, bool>> filter);
        Task<ICollection<Event>> GetAllAsync();
        Task<Event> GetEntityAsync(Expression<Func<Event, bool>> filter);
        Task<Event?> GetByIdAsync(string entityId);
        Task CreateAsync(Event entity);
        Task UpdateAsync(Event entity);
        Task DeleteAsync(string entityId);
    }
}
