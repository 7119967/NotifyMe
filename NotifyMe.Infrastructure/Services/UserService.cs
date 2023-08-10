using System.Linq.Expressions;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services;

public class UserService: IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ICollection<User>> GetListEntitiesAsync(Expression<Func<User, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<User>> GetAllAsync()
    {
        return await _unitOfWork.UserRepository.GetAllAsync();
    }

    public async Task<User> GetEntityAsync(Expression<Func<User, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        Expression<Func<User, bool>> filter = i => i.Id == id.ToString();
        return await _unitOfWork.UserRepository.GetEntityAsync(filter);
    }

    public async Task CreateAsync(User entity)
    {
        await _unitOfWork.UserRepository.CreateAsync(entity);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        await _unitOfWork.UserRepository.UpdateAsync(entity);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.UserRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}