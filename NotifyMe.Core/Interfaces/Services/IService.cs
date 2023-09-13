using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace NotifyMe.Core.Interfaces.Services;

public interface IService<T> where T : class
{
    Task<ICollection<T>> GetListEntitiesAsync(Expression<Func<T, bool>> filter);
    Task<ICollection<T>> GetAllAsync();
    Task<T> GetEntityAsync(Expression<Func<T, bool>> filter);
    Task<T?> GetByIdAsync(string entityId);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(string entityId);
    EntityEntry<T> Create(T entity);
}