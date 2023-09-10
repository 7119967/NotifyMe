using System.Linq.Expressions;
using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces
{
    public interface IConfigurationService
    {
        Task<ICollection<Configuration>> GetListEntitiesAsync(Expression<Func<Configuration, bool>> filter);
        Task<ICollection<Configuration>> GetAllAsync();
        Task<Configuration> GetEntityAsync(Expression<Func<Configuration, bool>> filter);
        Task<Configuration?> GetByIdAsync(string entityId);
        Task CreateAsync(Configuration entity);
        Task UpdateAsync(Configuration entity);
        Task DeleteAsync(string entityId);
    }
}
