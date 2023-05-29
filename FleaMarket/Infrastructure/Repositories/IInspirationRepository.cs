using FleaMarket.Models;

namespace FleaMarket.Infrastructure.Repositories
{
    public interface IInspirationItemRepository : IGenericRepository<InspirationItem>
    {
        Task<IEnumerable<InspirationItem>> GetPublishedItems();
    }
}
