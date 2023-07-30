using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotifyMe.Infrastructure.Services
{
    public class RabbitMQService
    {
        private readonly string _rabbitMQHost;
        private readonly string _queueName;

        private IConnection _connection;
        private IModel _channel;

        public RabbitMQService(string rabbitMQHost, string queueName)
        {
            _rabbitMQHost = rabbitMQHost;
            _queueName = queueName;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory() { HostName = _rabbitMQHost };

            // Create a connection and channel to the RabbitMQ server
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare the queue from which we'll consume messages
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            // Create a consumer and bind it to the queue
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += HandleMessage;

            // Start consuming messages from the queue
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }

        private void HandleMessage(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                // Extract the message content from the event arguments
                var body = e.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());

                // Process the message (you can replace this with your actual message handling logic)
                ProcessMessage(message);

                Console.WriteLine($"Message received: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while processing message: {ex.Message}");
            }
        }

        private void ProcessMessage(string message)
        {
            // Placeholder method for processing the incoming message
            // You should implement the actual message handling logic here
            // For example, you can parse the message, extract data, and call appropriate services for notification processing.
        }

        public void StopListening()
        {
            // Close the channel and connection
            _channel?.Close();
            _connection?.Close();
        }
    }
}
