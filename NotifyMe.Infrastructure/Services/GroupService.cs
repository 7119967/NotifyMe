using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services;

public class GroupService : Service<Group>, IGroupService
{
    public GroupService(IUnitOfWork unitOfWork) : base(unitOfWork) { }
}