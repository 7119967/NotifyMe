using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services
{
    public class ChangeService: IChangeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ICollection<Change>> GetListEntitiesAsync(Expression<Func<Change, bool>> filter)
        {
            return await _unitOfWork.ChangeRepository.GetAllAsync();
        }

        public async Task<ICollection<Change>> GetAllAsync()
        {
            return await _unitOfWork.ChangeRepository.GetAllAsync();
        }

        public async Task<Change> GetEntityAsync(Expression<Func<Change, bool>> filter)
        {
            return await _unitOfWork.ChangeRepository.GetEntityAsync(filter) ?? throw new Exception();
        }

        public async Task<Change?> GetByIdAsync(string entityId)
        {
            Expression<Func<Change, bool>> filter = i => i.Id == entityId;
            return await _unitOfWork.ChangeRepository.GetEntityAsync(filter);
        }

        public async Task CreateAsync(Change entity)
        {
            await _unitOfWork.ChangeRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(Change entity)
        {
            await _unitOfWork.ChangeRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(string entityId)
        {
            await _unitOfWork.ChangeRepository.DeleteAsync(entityId);
            await _unitOfWork.CommitAsync();
        }

        public IEnumerable<Change> AsEnumerable()
        {
            return _unitOfWork.ChangeRepository.AsEnumerable();
        }

        public IQueryable<Change> AsQueryable()
        {
            return _unitOfWork.ChangeRepository.AsQueryable();
        }
                
        public EntityEntry<Change> Create(Change entity)
        {
            var entityEntry = _unitOfWork.ChangeRepository.Create(entity);
            _unitOfWork.CommitAsync().Wait();
            return entityEntry;
        }
        
        public EntityEntry<Change> Update(Change entity)
        {
            var entityEntry = _unitOfWork.ChangeRepository.Update(entity);
            _unitOfWork.CommitAsync().Wait();
            return entityEntry;
        }
    }
}
