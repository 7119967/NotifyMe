using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services
{
    public class ConfigurationService : Service<Configuration>, IConfigurationService
    {
        public ConfigurationService(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
