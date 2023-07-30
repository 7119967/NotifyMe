using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.Tests
{
    public class EventMonitoringServiceTests {

        private EventMonitoringService _service;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IEventMonitoringRepository> _repositoryMock;

        public EventMonitoringServiceTests() {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _repositoryMock = new Mock<IEventMonitoringRepository>();
            _service = new EventMonitoringService(_unitOfWorkMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public void LogEvent_ShouldCallRepository() {
    
            _service.LogEvent("Test", "Details");
            
            _repositoryMock.Verify(r => r.Add(It.IsAny<EventMonitoring>()), Times.Once());

        }
 
        [Fact]
        public void LogEvent_ShouldCreateEvent() {
   
            _service.LogEvent("Test", "Details");
            
            _repositoryMock.Verify(r => r.Add(It.Is<EventMonitoring>(
                e => e.EventName == "Test" && e.EventDescription == "Details"
            )), Times.Once());

        }

    }
}
