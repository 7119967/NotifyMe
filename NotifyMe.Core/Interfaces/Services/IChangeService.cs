using System.Linq.Expressions;
using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces
{
    public interface IChangeService
    {
        Task<ICollection<Change>> GetListEntitiesAsync(Expression<Func<Change, bool>> filter);
        Task<ICollection<Change>> GetAllAsync();
        Task<Change> GetEntityAsync(Expression<Func<Change, bool>> filter);
        Task<Change?> GetByIdAsync(string entityId);
        Task CreateAsync(Change entity);
        Task UpdateAsync(Change entity);
        Task DeleteAsync(string entityId);
    }
}
