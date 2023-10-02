using NotifyMe.Core.Entities;

namespace NotifyMe.Core.Interfaces.Services
{
    public interface IUserService: IService<User>
    {
        void ApplyChanges(Object source, Object target);
    }
}
