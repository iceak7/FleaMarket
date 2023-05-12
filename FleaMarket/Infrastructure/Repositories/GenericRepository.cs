using FleaMarket.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FleaMarket.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<T> Create(T entity)
        {
            var en = await _context.Set<T>().AddAsync(entity);
            return en.Entity;
        }

        public async Task<bool> Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            return true;
        }

        public async Task<T?> Find(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FindAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
    }
}
