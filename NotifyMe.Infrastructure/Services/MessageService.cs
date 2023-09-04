using System.Linq.Expressions;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services;

public class MessageService: IMessageService
{
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ICollection<Message>> GetListEntitiesAsync(Expression<Func<Message, bool>> filter)
    {
        return await _unitOfWork.MessageRepository.GetAllAsync();
    }

    public async Task<ICollection<Message>> GetAllAsync()
    {
        return await _unitOfWork.MessageRepository.GetAllAsync();
    }

    public async Task<Message> GetEntityAsync(Expression<Func<Message, bool>> filter)
    {
        return await _unitOfWork.MessageRepository.GetEntityAsync(filter) ?? throw new Exception();
    }

    public async Task<Message?> GetByIdAsync(string entityId)
    {
        Expression<Func<Message, bool>> filter = i => i.Id == entityId;
        return await _unitOfWork.MessageRepository.GetEntityAsync(filter);
    }

    public async Task CreateAsync(Message entity)
    {
        await _unitOfWork.MessageRepository.CreateAsync(entity);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(Message entity)
    {
        await _unitOfWork.MessageRepository.UpdateAsync(entity);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(string entityId)
    {
        await _unitOfWork.MessageRepository.DeleteAsync(entityId);
        await _unitOfWork.CommitAsync();
    }
}