using FleaMarket.Models;

namespace FleaMarket.Infrastructure.Repositories
{
    public interface IMarketItemRepository : IGenericRepository<MarketItem>
    {
        Task<IEnumerable<MarketItem>> GetPublishedItems();
        Task<List<MarketItem>> GetByIds(IEnumerable<int> ids);
    }
}
