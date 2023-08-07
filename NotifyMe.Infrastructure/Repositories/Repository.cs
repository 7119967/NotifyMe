using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Interfaces;
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
        
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id) ?? throw new NullReferenceException();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }
    }
}
