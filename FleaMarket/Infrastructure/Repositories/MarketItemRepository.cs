using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Models;

namespace FleaMarket.Infrastructure.Repositories
{
    public class MarketItemRepository : GenericRepository<MarketItem>, IMarketItemRepository
    {
        public MarketItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
