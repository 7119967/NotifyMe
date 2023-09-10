using System.Linq.Expressions;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
using NotifyMe.Core.Interfaces.Repositories;

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
    }
}
