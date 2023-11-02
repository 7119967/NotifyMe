using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services
{
    public class ChangeService: Service<Change>, IChangeService
    {
        public ChangeService(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
