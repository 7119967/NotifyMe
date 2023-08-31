using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<T> _entities;

        public Repository(DatabaseContext context)
        {
            _context = context;
            _entities = _context.Set<T>();
        }

        public async Task<ICollection<T>> GetListEntitiesAsync(Expression<Func<T, bool>> filter)
        {
            return await _entities.AsQueryable().Where(filter).ToListAsync() ?? throw new NullReferenceException();
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            // return await _entities.AsQueryable().ToListAsync() ?? throw new NullReferenceException();
            return await _entities.ToListAsync();
        }

        public async Task<T?> GetEntityAsync(Expression<Func<T, bool>> filter)
        {
            return await _entities.FirstOrDefaultAsync(filter);
        }

        public async Task<T> GetByIdAsync(string entityId)
        {
            return await _entities.FindAsync(entityId) ?? throw new NullReferenceException();
        }

        public async Task CreateAsync(T entity)
        {
            await _entities.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            await Task.Run(() => _entities.Update(entity));
        }

        public void Update(T entity)
        {
            _entities.Update(entity);
        }

        public async Task DeleteAsync(string entityId)
        {
            var entity = await _entities.FindAsync(entityId);
            if (entity == null)
            {
                throw new NullReferenceException();
            }

            _entities.Entry(entity).State = EntityState.Deleted;
            await Task.Run(() => _entities.Remove(entity));
        }
    }
}
