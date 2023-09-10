using Moq;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.Tests
{
    public class EventMonitoringServiceTests
    {
        private readonly EventService _service;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEventRepository> _repositoryMock;

        public EventMonitoringServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _repositoryMock = new Mock<IEventRepository>();
            _service = new EventService(_unitOfWorkMock.Object);
        }

        [Fact]
        public void LogEvent_ShouldCallRepository()
        {

            _service.LogEvent("Test", "Details");

            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Event>()), Times.Once());

        }

        [Fact]
        public void LogEvent_ShouldCreateEvent()
        {

            _service.LogEvent("Test", "Details");

            _repositoryMock.Verify(r => r.CreateAsync(It.Is<Event>(
                e => e.EventName == "Test" && e.EventDescription == "Details"
            )), Times.Once());

        }
    }
}
