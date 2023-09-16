using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

    public EntityEntry<User> Update(User entity)
    {
        var entityEntry = _unitOfWork.UserRepository.Update(entity);
        _unitOfWork.Commit();
        return entityEntry;
    }

    public async Task DeleteAsync(string entityId)
    {
        await _unitOfWork.UserRepository.DeleteAsync(entityId);
        await _unitOfWork.CommitAsync();
    }

    public EntityEntry<User> Create(User entity)
    {
        var entityEntry = _unitOfWork.UserRepository.Create(entity);
        _unitOfWork.CommitAsync();
        return entityEntry;
    }

    public IEnumerable<User> AsEnumerable()
    {
        return _unitOfWork.UserRepository.AsEnumerable();
    }

    public IQueryable<User> AsQueryable()
    {
        return _unitOfWork.UserRepository.AsQueryable();
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
                case "GroupId":
                    property.SetValue(target, sourceValue);
                    break;
            }
        }
    }
}