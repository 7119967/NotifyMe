using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.Infrastructure.Services
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<T>? _repo;
        
        public Service(IUnitOfWork unitOfWork)    
        {
            _unitOfWork = unitOfWork;
            _repo = GetRepository<T>();
        }

        public IEnumerable<T> AsEnumerable() => _repo!.AsEnumerable();

        public Task<ICollection<T>> GetAllAsync() => _repo!.GetAllAsync();
        
        public IQueryable<T> AsQueryable() => _repo!.AsQueryable().AsNoTracking();

        public Task<T?> GetByIdAsync(string entityId) 
            => _repo!.AsQueryable().FirstOrDefaultAsync(e => e.Equals(entityId));

        public Task<T> GetEntityAsync(Expression<Func<T, bool>> filter) 
            => _repo!.GetEntityAsync(filter)!;
  
        public Task<ICollection<T>> GetListEntitiesAsync(Expression<Func<T, bool>> filter)
         => _repo!.GetAllAsync();

        public EntityEntry<T> Create(T entity)
        {
            var entityEntry = _repo!.Create(entity);
            _unitOfWork.CommitAsync().Wait();
            return entityEntry;
        }

        public async Task CreateAsync(T entity)
        {
            await _repo!.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(string entityId)
        {
            await _repo!.DeleteAsync(entityId);
            await _unitOfWork.CommitAsync();
        }

        public EntityEntry<T> Update(T entity)
        {
            var entityEntry = _repo!.Update(entity);
            _unitOfWork.CommitAsync().Wait();
            return entityEntry;
        }

        public async Task UpdateAsync(T entity)
        {
            await _repo!.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        private IRepository<TEntity>? GetRepository<TEntity>() where TEntity : class
        {
            return typeof(TEntity).Name switch
            {
                nameof(User) => _unitOfWork.UserRepository as IRepository<TEntity>,
                nameof(Event) => _unitOfWork.EventRepository as IRepository<TEntity>,
                nameof(Group) => _unitOfWork.GroupRepository as IRepository<TEntity>,
                nameof(Change) => _unitOfWork.ChangeRepository as IRepository<TEntity>,
                nameof(Message) => _unitOfWork.MessageRepository as IRepository<TEntity>,
                nameof(Notification) => _unitOfWork.NotificationRepository as IRepository<TEntity>,
                nameof(Configuration) => _unitOfWork.ConfigurationRepository as IRepository<TEntity>,
                nameof(NotificationUser) => _unitOfWork.NotificationUserRepository as IRepository<TEntity>,
                _ => null
            };
        }
    }
}
