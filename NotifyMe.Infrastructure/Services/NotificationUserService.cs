using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;
using RabbitMQ.Client;

namespace NotifyMe.Infrastructure.Services;

public class NotificationUserService : INotificationUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConnectionFactory _connectionFactory;

    public NotificationUserService(IUnitOfWork unitOfWork, IConnectionFactory connectionFactory)
    {
        _unitOfWork = unitOfWork;
        _connectionFactory = connectionFactory;
    }

    public async Task<ICollection<NotificationUser>> GetListEntitiesAsync(
        Expression<Func<NotificationUser, bool>> filter)
    {
        return await _unitOfWork.NotificationUserRepository.GetAllAsync();
    }

    public async Task<ICollection<NotificationUser>> GetAllAsync()
    {
        return await _unitOfWork.NotificationUserRepository.GetAllAsync();
    }

    public async Task<NotificationUser> GetEntityAsync(Expression<Func<NotificationUser, bool>> filter)
    {
        return await _unitOfWork.NotificationUserRepository.GetEntityAsync(filter) ?? throw new Exception();
    }

    public async Task<NotificationUser?> GetByIdAsync(string entityId)
    {
        Expression<Func<NotificationUser, bool>> filter = i => i.Id == entityId;
        return await _unitOfWork.NotificationUserRepository.GetEntityAsync(filter);
    }

    public async Task CreateAsync(NotificationUser entity)
    {
        await _unitOfWork.NotificationUserRepository.CreateAsync(entity);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(NotificationUser entity)
    {
        await _unitOfWork.NotificationUserRepository.UpdateAsync(entity);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(string entityId)
    {
        await _unitOfWork.NotificationUserRepository.DeleteAsync(entityId);
        await _unitOfWork.CommitAsync();
    }

    public EntityEntry<NotificationUser> Create(NotificationUser entity)
    {
        var entityEntry = _unitOfWork.NotificationUserRepository.Create(entity);
        _unitOfWork.CommitAsync().Wait();
        return entityEntry;
    }

    public IEnumerable<NotificationUser> AsEnumerable()
    {
        return _unitOfWork.NotificationUserRepository.AsEnumerable();
    }

    public IQueryable<NotificationUser> AsQueryable()
    {
        return _unitOfWork.NotificationUserRepository.AsQueryable();
    }

    public EntityEntry<NotificationUser> Update(NotificationUser entity)
    {
        var entityEntry = _unitOfWork.NotificationUserRepository.Update(entity);
        _unitOfWork.CommitAsync().Wait();
        return entityEntry;
    }
}