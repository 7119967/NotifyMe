using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces.Services;

public interface IMessageService: IService<Message>
{
    // Task<ICollection<Message>> GetListEntitiesAsync(Expression<Func<Message, bool>> filter);
    // Task<ICollection<Message>> GetAllAsync();
    // Task<Message> GetEntityAsync(Expression<Func<Message, bool>> filter);
    // Task<Message?> GetByIdAsync(string entityId);
    // Task CreateAsync(Message entity);
    // Task UpdateAsync(Message entity);
    // Task DeleteAsync(string entityId);
}