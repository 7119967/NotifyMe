using System.Linq.Expressions;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services;

public class GroupService : IGroupService
{
    private readonly IUnitOfWork _unitOfWork;

    public GroupService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ICollection<Group>> GetListEntitiesAsync(Expression<Func<Group, bool>> filter)
    {
        return await _unitOfWork.GroupRepository.GetAllAsync();
    }

    public async Task<ICollection<Group>> GetAllAsync()
    {
        return await _unitOfWork.GroupRepository.GetAllAsync();
    }

    public async Task<Group> GetEntityAsync(Expression<Func<Group, bool>> filter)
    {
        return await _unitOfWork.GroupRepository.GetEntityAsync(filter) ?? throw new Exception();
    }

    public async Task<Group?> GetByIdAsync(string entityId)
    {
        Expression<Func<Group, bool>> filter = i => i.Id.ToString() == entityId;
        return await _unitOfWork.GroupRepository.GetEntityAsync(filter);
    }

    public async Task CreateAsync(Group entity)
    {
        await _unitOfWork.GroupRepository.CreateAsync(entity);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(Group entity)
    {
        await _unitOfWork.GroupRepository.UpdateAsync(entity);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(string entityId)
    {
        await _unitOfWork.GroupRepository.DeleteAsync(entityId);
        await _unitOfWork.CommitAsync();
    }
}