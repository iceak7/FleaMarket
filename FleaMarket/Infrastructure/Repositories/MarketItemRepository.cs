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

        public async Task<IEnumerable<MarketItem>> GetAllItems(int? categoryid, string search, ItemStatus? itemStatus)
        {
             IQueryable<MarketItem> result = _appDbContext.MarketItems.Include(x => x.Images).Include(x => x.Categories);

            if(itemStatus != null)
            {
                result = result.Where(x => x.Status == itemStatus);
            }
            if(categoryid != null)
            {
                result = result.Where(x => x.Categories.FirstOrDefault(c => c.Id == categoryid) != null);
            }
            if (search != null)
            {
                result = result.Where(x => x.Title.Contains(search));
            }


            return await result.ToListAsync();
        }

        public async Task<IEnumerable<MarketItem>> GetLastPublished(int number)
        {
            var items = await _appDbContext.MarketItems.Include(x => x.Images).Where(x=>x.Status == ItemStatus.Published).OrderByDescending(x=>x.PublicationDate).Take(number).ToListAsync();

            return items;
        }

        public async Task<IEnumerable<string>> GetAllTitles()
        {
            return await _appDbContext.MarketItems.Select(x=>x.Title).ToListAsync();
        }
    }
}
