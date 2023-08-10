using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DatabaseContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task<ICollection<T>> GetListEntitiesAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AsQueryable().Where(filter).ToListAsync() ?? throw new NullReferenceException();
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await _dbSet.AsQueryable().ToListAsync() ?? throw new NullReferenceException();
        }

        public async Task<T?> GetEntityAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.FirstOrDefaultAsync(filter);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id) ?? throw new NullReferenceException();
        }

        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            await Task.Run(() => _dbSet.Update(entity));
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            await Task.Run(() => _dbSet.Remove(entity!));
        }
    }
}
