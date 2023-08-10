using System.Linq.Expressions;

namespace NotifyMe.Core.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<ICollection<T>> GetListEntitiesAsync(Expression<Func<T, bool>> filter);        
        Task<ICollection<T>> GetAllAsync();
        Task<T?> GetEntityAsync(Expression<Func<T, bool>> filter);
        Task<T> GetByIdAsync(int id); 
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
