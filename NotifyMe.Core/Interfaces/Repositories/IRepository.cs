using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace NotifyMe.Core.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<ICollection<T>> GetListEntitiesAsync(Expression<Func<T, bool>> filter);
        Task<ICollection<T>> GetAllAsync();
        Task<T?> GetEntityAsync(Expression<Func<T, bool>> filter);
        Task<T> GetByIdAsync(string entityId);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        EntityEntry<T> Update(T entity);
        Task DeleteAsync(string entityId);
        EntityEntry<T> Create(T entity);
        IEnumerable<T> AsEnumerable();
        IQueryable<T> AsQueryable();
    }
}
