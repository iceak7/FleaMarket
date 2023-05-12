using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Infrastructure.Repositories;

namespace FleaMarket.Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private AppDbContext _appDbContext;
        public IMarketItemRepository MarketItems { get; }
        public IInspirationItemRepository InspirationItems { get; }
        public IItemCategoryRepository ItemCategories { get; }

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            MarketItems = new MarketItemRepository(appDbContext);
            InspirationItems = new InspirationItemRepository(appDbContext);
            ItemCategories = new ItemCategoryRepository(appDbContext);
        }


        public void Dispose()
        {
            _appDbContext.Dispose();
        }

        public async Task<int> SaveAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }
    }
}
