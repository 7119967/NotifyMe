using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.Infrastructure.Repositories
{
    public class ChangeRepository : Repository<Change>, IChangeRepository
    {
        public ChangeRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
