using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;
using RabbitMQ.Client;

namespace NotifyMe.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConnectionFactory _connectionFactory;
        private const string QueueName = "notification_queue";

        public NotificationService(IUnitOfWork unitOfWork, IConnectionFactory connectionFactory)
        {
            _unitOfWork = unitOfWork;
            _connectionFactory = connectionFactory;
        }

        public void SendNotification(Notification notification)
        {
            // Serialize the notification content to send as a message
            string message = JsonSerializer.Serialize(notification);

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declare a queue for notifications
                channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                // Convert the message to bytes
                var body = Encoding.UTF8.GetBytes(message);

                // Publish the message to the queue
                channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: body);
            }
        }
        
        public async Task<ICollection<Notification>> GetListEntitiesAsync(Expression<Func<Notification, bool>> filter)
        {
            return await _unitOfWork.NotificationRepository.GetAllAsync();
        }

        public async Task<ICollection<Notification>> GetAllAsync()
        {
            return await _unitOfWork.NotificationRepository.GetAllAsync();
        }

        public async Task<Notification> GetEntityAsync(Expression<Func<Notification, bool>> filter)
        {
            return await _unitOfWork.NotificationRepository.GetEntityAsync(filter) ?? throw new Exception();
        }

        public async Task<Notification?> GetByIdAsync(string entityId)
        {
            Expression<Func<Notification, bool>> filter = i => i.Id == entityId;
            return await _unitOfWork.NotificationRepository.GetEntityAsync(filter);
        }

        public async Task CreateAsync(Notification entity)
        {
            await _unitOfWork.NotificationRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(Notification entity)
        {
            await _unitOfWork.NotificationRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(string entityId)
        {
            await _unitOfWork.NotificationRepository.DeleteAsync(entityId);
            await _unitOfWork.CommitAsync();
        }

        public EntityEntry<Notification> Create(Notification entity)
        {
            var entityEntry = _unitOfWork.NotificationRepository.Create(entity);
            _unitOfWork.CommitAsync().Wait();
            return entityEntry;
        }

        public IEnumerable<Notification> AsEnumerable()
        {
            return _unitOfWork.NotificationRepository.AsEnumerable();
        }

        public IQueryable<Notification> AsQueryable()
        {
            return _unitOfWork.NotificationRepository.AsQueryable();
        }

        public EntityEntry<Notification> Update(Notification entity)
        {
            var entityEntry = _unitOfWork.NotificationRepository.Update(entity);
            _unitOfWork.CommitAsync().Wait();
            return entityEntry;
        }
    }
}
