using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace FleaMarket.Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private AppDbContext _appDbContext;
        public IMarketItemRepository MarketItems { get; }
        public IInspirationItemRepository InspirationItems { get; }
        public IItemCategoryRepository ItemCategories { get; }
        public IUserRepository UserRepository { get; }
        public IRolesRepository RolesRepository { get; }

        public UnitOfWork(AppDbContext appDbContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _appDbContext = appDbContext;
            MarketItems = new MarketItemRepository(appDbContext);
            InspirationItems = new InspirationItemRepository(appDbContext);
            ItemCategories = new ItemCategoryRepository(appDbContext);
            UserRepository = new UserRepository(userManager);
            RolesRepository = new RolesRepository(roleManager);
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
