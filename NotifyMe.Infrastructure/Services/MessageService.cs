using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore.ChangeTracking;

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

    public Task<ICollection<Message>> GetListEntitiesAsync(Expression<Func<Message, bool>> filter)
    {
        return _unitOfWork.MessageRepository.GetAllAsync();
    }

    public Task<ICollection<Message>> GetAllAsync()
    {
        return _unitOfWork.MessageRepository.GetAllAsync();
    }

    public async Task<Message> GetEntityAsync(Expression<Func<Message, bool>> filter)
    {
        return await _unitOfWork.MessageRepository.GetEntityAsync(filter) ?? throw new Exception();
    }

    public Task<Message?> GetByIdAsync(string entityId)
    {
        Expression<Func<Message, bool>> filter = i => i.Id == entityId;
        return _unitOfWork.MessageRepository.GetEntityAsync(filter);
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

    public IEnumerable<Message> AsEnumerable()
    {
        return _unitOfWork.MessageRepository.AsEnumerable();
    }

    public IQueryable<Message> AsQueryable()
    {
        return _unitOfWork.MessageRepository.AsQueryable();
    }

    public EntityEntry<Message> Create(Message entity)
    {
        var entityEntry = _unitOfWork.MessageRepository.Create(entity);
        _unitOfWork.CommitAsync().Wait();
        return entityEntry;
    }

    public EntityEntry<Message> Update(Message entity)
    {
        var entityEntry = _unitOfWork.MessageRepository.Update(entity);
        _unitOfWork.CommitAsync().Wait();
        return entityEntry;
    }
}