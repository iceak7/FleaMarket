using FleaMarket.Models;

namespace FleaMarket.Infrastructure.Repositories
{
    public interface IImageRepository : IGenericRepository<Image>
    {
        Task<List<Image>> GetByIds(IEnumerable<int> ids);
    }
}
