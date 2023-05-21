using FleaMarket.Models;
using System.Linq.Expressions;

namespace FleaMarket.Infrastructure.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<T> Find(Expression<Func<T, bool>> predicate);
        Task<T> Create(T entity);
        Task<bool> Delete(T entity);

    }
}
