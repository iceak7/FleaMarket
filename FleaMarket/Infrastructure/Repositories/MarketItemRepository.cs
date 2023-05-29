using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;

namespace FleaMarket.Infrastructure.Repositories
{
    public class MarketItemRepository : GenericRepository<MarketItem>, IMarketItemRepository
    {
        private readonly AppDbContext _appDbContext;
        public MarketItemRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public override async Task<MarketItem> GetById(int id)
        {
            var result = await _appDbContext.MarketItems.Where(x => x.Id == id).Include(x => x.InspirationItems).Include(x => x.Categories).Include(x => x.Images).FirstOrDefaultAsync();

            return result;
        }

        public async Task<List<MarketItem>> GetByIds(IEnumerable<int> ids)
        {
            List<MarketItem> items = new List<MarketItem>();

            if (ids?.Count() > 0)
            {
                items = await _context.MarketItems.Where(x => ids.Contains(x.Id)).ToListAsync();
            }
            return items;
        }

        public async Task<IEnumerable<MarketItem>> GetPublishedItems()
        {
            var result = await _appDbContext.MarketItems.Where(x => x.Status == ItemStatus.Published).Include(x => x.Images).Include(x => x.Categories).ToListAsync();

            return result;
        }

    }
}
