using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace FleaMarket.Infrastructure.Repositories
{
    public class ItemCategoryRepository : GenericRepository<ItemCategory>, IItemCategoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public ItemCategoryRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public async Task<List<ItemCategory>> GetByIds(List<int> ids)
        {
            List<ItemCategory> categories = new List<ItemCategory>();

            if( ids?.Count > 0 )
            {
                categories = await _appDbContext.ItemCategories.Where(x => ids.Contains(x.Id)).ToListAsync();
            }
            return categories;
        }


    }
}
