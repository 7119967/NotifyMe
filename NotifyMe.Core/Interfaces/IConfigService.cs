using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces
{
    public interface IConfigService
    {
        List<Configuration> GetAllConfigurations();
    }
}
