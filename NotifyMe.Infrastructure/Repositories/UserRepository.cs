using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.Infrastructure.Repositories
{
    public class UserRepository: Repository<User>, IUserRepository
    {
        public UserRepository(DatabaseContext context) : base(context) { }
    }
}
