﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;

using RabbitMQ.Client;

namespace NotifyMe.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConnectionFactory _connectionFactory;
        private const string QueueName = "notification_queue";

        public NotificationService(IConnectionFactory connectionFactory)
        {
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
    }
}
