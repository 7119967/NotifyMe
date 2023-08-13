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
            return await _entities.AsQueryable().ToListAsync() ?? throw new NullReferenceException();
        }

        public async Task<T?> GetEntityAsync(Expression<Func<T, bool>> filter)
        {
            return await _entities.FirstOrDefaultAsync(filter);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id) ?? throw new NullReferenceException();
        }

        public async Task CreateAsync(T entity)
        {
            _entities.Entry(entity).State = EntityState.Added;
            await _entities.AddAsync(entity);
        }

        public Task UpdateAsync(T entity)
        {
            // _entities.Entry(entity).State = EntityState.Modified;
            _entities.Update(entity);

            return Task.CompletedTask;
        }
        
        public void Update(T entity)
        {
            // _context.Attach(entity);
            // entity.ConcurrencyStamp = Guid.NewGuid().ToString();
            //
            // var entityEntry = _entities.Entry(entity);
            //
            // if (entity.GetType() == typeof(User))
            // {
            //     var user = _mapper.Map<User>(entity);
            //     var track = _entities.Attach(user);
            //     track.ConcurrencyStamp = Guid.NewGuid().ToString();
            //     track.State = EntityState.Modified;
            // }
  
            
            // _context.SaveChanges();

            // var entityEntry = _entities.Entry(entity);
            // if (entityEntry.State != EntityState.Detached)
            // {
            //     entityEntry.State = EntityState.Detached;
            // }
            _entities.Update(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _entities.FindAsync(id);
            if (entity == null)
            {
                throw new NullReferenceException();
            }

            _entities.Entry(entity).State = EntityState.Deleted;
            await Task.Run(() => _entities.Remove(entity));
        }
        
        public void Commit()
        {
            _context.SaveChanges();
        }
    }
}
