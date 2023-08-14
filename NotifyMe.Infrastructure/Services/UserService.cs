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

    public Task<ICollection<User>> GetListEntitiesAsync(Expression<Func<User, bool>> filter)
    {
        return _unitOfWork.UserRepository.GetAllAsync();
    }

    public Task<ICollection<User>> GetAllAsync()
    {
        return _unitOfWork.UserRepository.GetAllAsync();
    }

    public async Task<User> GetEntityAsync(Expression<Func<User, bool>> filter)
    {
        return await _unitOfWork.UserRepository.GetEntityAsync(filter) ?? throw new NullReferenceException();
    }

    public Task<User?> GetByIdAsync(int id)
    {
        Expression<Func<User, bool>> filter = i => i.Id == id.ToString();
        return _unitOfWork.UserRepository.GetEntityAsync(filter);
    }

    public async Task CreateAsync(User entity)
    {
        await _unitOfWork.UserRepository.CreateAsync(entity);
        await _unitOfWork.CommitAsync();
    }

    public Task UpdateAsync(User entity)
    {
        _unitOfWork.UserRepository.UpdateAsync(entity);
        _unitOfWork.CommitAsync();

        return Task.CompletedTask;
    }

    public void Update(User entity)
    {
        _unitOfWork.UserRepository.Update(entity);
        _unitOfWork.Commit();
    }

    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.UserRepository.DeleteAsync(id);
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