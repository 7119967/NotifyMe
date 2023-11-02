using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services;

public class MessageService : Service<Message>, IMessageService
{
    public MessageService(IUnitOfWork unitOfWork) : base(unitOfWork) { }
}