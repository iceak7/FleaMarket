using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
