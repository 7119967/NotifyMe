using System.Linq.Expressions;
using System.Reflection;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ICollection<User>> GetListEntitiesAsync(Expression<Func<User, bool>> filter)
    {
        return await _unitOfWork.UserRepository.GetAllAsync();
    }

    public async Task<ICollection<User>> GetAllAsync()
    {
        return await _unitOfWork.UserRepository.GetAllAsync();
    }

    public async Task<User> GetEntityAsync(Expression<Func<User, bool>> filter)
    {
        return await _unitOfWork.UserRepository.GetEntityAsync(filter) ?? throw new NullReferenceException();
    }

    public async Task<User?> GetByIdAsync(string entityId)
    {
        Expression<Func<User, bool>> filter = i => i.Id == entityId;
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

    public void Update(User entity)
    {
        _unitOfWork.UserRepository.Update(entity);
        _unitOfWork.Commit();
    }

    public async Task DeleteAsync(string entityId)
    {
        await _unitOfWork.UserRepository.DeleteAsync(entityId);
        await _unitOfWork.CommitAsync();
    }

    public void ApplyChanges(Object source, Object target)
    {
        PropertyInfo[] properties = typeof(User).GetProperties();

        foreach (PropertyInfo property in properties)
        {
            var sourceValue = property.GetValue(source);
            var targetValue = property.GetValue(target);
            
            if (sourceValue == null && targetValue == null)
            {
                continue;
            }
            
            switch (property.Name)
            {
                case "UserName":
                case "FirstName":
                case "LastName":
                case "Email":
                case "PhoneNumber":
                case "Avatar":
                case "Info":
                    property.SetValue(target, sourceValue);
                    break;
            }
        }
    }
}