using System.Linq.Expressions;

using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces.Services;

public interface IGroupService
{
    Task<ICollection<Group>> GetListEntitiesAsync(Expression<Func<Group, bool>> filter);
    Task<ICollection<Group>> GetAllAsync();
    Task<Group> GetEntityAsync(Expression<Func<Group, bool>> filter);
    Task<Group?> GetByIdAsync(int id);
    Task CreateAsync(Group entity);
    Task UpdateAsync(Group entity);
    Task DeleteAsync(int id);
}