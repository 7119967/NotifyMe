using System.Linq.Expressions;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
using NotifyMe.Core.Interfaces.Repositories;

namespace NotifyMe.Infrastructure.Services
{
    public class ConfigurationService: IConfigurationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfigurationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ICollection<Configuration>> GetListEntitiesAsync(Expression<Func<Configuration, bool>> filter)
        {
            return await _unitOfWork.ConfigurationRepository.GetAllAsync();
        }

        public async Task<ICollection<Configuration>> GetAllAsync()
        {
            return await _unitOfWork.ConfigurationRepository.GetAllAsync();
        }

        public async Task<Configuration> GetEntityAsync(Expression<Func<Configuration, bool>> filter)
        {
            return await _unitOfWork.ConfigurationRepository.GetEntityAsync(filter) ?? throw new Exception();
        }

        public async Task<Configuration?> GetByIdAsync(string entityId)
        {
            Expression<Func<Configuration, bool>> filter = i => i.Id == entityId;
            return await _unitOfWork.ConfigurationRepository.GetEntityAsync(filter);
        }

        public async Task CreateAsync(Configuration entity)
        {
            await _unitOfWork.ConfigurationRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(Configuration entity)
        {
            await _unitOfWork.ConfigurationRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(string entityId)
        {
            await _unitOfWork.ConfigurationRepository.DeleteAsync(entityId);
            await _unitOfWork.CommitAsync();
        }
    }
}
