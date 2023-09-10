using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces.Services;

public interface IRabbitMqService
{
    void SendMessage(object obj);
    void SendMessage(Message message);
}