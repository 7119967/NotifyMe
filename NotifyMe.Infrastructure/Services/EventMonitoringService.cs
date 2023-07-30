using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;

namespace NotifyMe.Infrastructure.Services
{
    public class EventMonitoringService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventMonitoringRepository _repository;
  
        public EventMonitoringService(IUnitOfWork unitOfWork, IEventMonitoringRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public void LogEvent(string name, string description) 
        {
            var eventItem = new EventMonitoring
            {
                EventName = name, 
                EventDescription = description
            };
            _repository.Add(eventItem);
            _unitOfWork.Complete();
        }
    }
}
