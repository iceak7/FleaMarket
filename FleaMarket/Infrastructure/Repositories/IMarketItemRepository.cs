using FleaMarket.Models;

namespace FleaMarket.Infrastructure.Repositories
{
    public interface IMarketItemRepository : IGenericRepository<MarketItem>
    {
        Task<IEnumerable<MarketItem>> GetAllItems(int? categoryid, string search, ItemStatus? itemStatus);
        Task<List<MarketItem>> GetByIds(IEnumerable<int> ids);
        Task<IEnumerable<string>> GetAllTitles();
    }
}
