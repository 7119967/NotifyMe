using System.Linq.Expressions;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
using NotifyMe.Core.Interfaces.Repositories;

namespace NotifyMe.Infrastructure.Services
{
    public class EventService: IEventService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public void LogEvent(string name, string description)
        {
            var eventItem = new Event
            {
                EventName = name,
                EventDescription = description
            };
            _unitOfWork.EventRepository.CreateAsync(eventItem);
            _unitOfWork.CommitAsync();
        }

        public async Task<ICollection<Event>> GetListEntitiesAsync(Expression<Func<Event, bool>> filter)
        {
            return await _unitOfWork.EventRepository.GetAllAsync();
        }

        public async Task<ICollection<Event>> GetAllAsync()
        {
            return await _unitOfWork.EventRepository.GetAllAsync();
        }

        public async Task<Event> GetEntityAsync(Expression<Func<Event, bool>> filter)
        {
            return await _unitOfWork.EventRepository.GetEntityAsync(filter) ?? throw new Exception();
        }

        public async Task<Event?> GetByIdAsync(string entityId)
        {
            Expression<Func<Event, bool>> filter = i => i.Id == entityId;
            return await _unitOfWork.EventRepository.GetEntityAsync(filter);
        }

        public async Task CreateAsync(Event entity)
        {
            await _unitOfWork.EventRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(Event entity)
        {
            await _unitOfWork.EventRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(string entityId)
        {
            await _unitOfWork.EventRepository.DeleteAsync(entityId);
            await _unitOfWork.CommitAsync();
        }
    }
}
