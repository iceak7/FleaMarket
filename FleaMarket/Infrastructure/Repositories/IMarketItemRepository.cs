using FleaMarket.Models;

namespace FleaMarket.Infrastructure.Repositories
{
    public interface IMarketItemRepository : IGenericRepository<MarketItem>
    {
        Task<IEnumerable<MarketItem>> GetPublishedItems();
    }
}
