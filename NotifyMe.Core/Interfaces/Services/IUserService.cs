using System.Linq.Expressions;

using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<ICollection<User>> GetListEntitiesAsync(Expression<Func<User, bool>> filter);
        Task<ICollection<User>> GetAllAsync();
        Task<User> GetEntityAsync(Expression<Func<User, bool>> filter);
        Task<User?> GetByIdAsync(string entityId);
        Task CreateAsync(User entity);
        Task UpdateAsync(User entity);
        void Update(User entity);
        Task DeleteAsync(string entityId);
        void ApplyChanges(Object source, Object target);
    }
}
