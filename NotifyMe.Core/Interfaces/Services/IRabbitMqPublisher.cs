namespace NotifyMe.Core.Interfaces.Services;

public interface IRabbitMqPublisher
{
    void PublishMessage(string message);
}