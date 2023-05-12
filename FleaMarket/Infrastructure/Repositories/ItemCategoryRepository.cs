using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Models;

namespace FleaMarket.Infrastructure.Repositories
{
    public class ItemCategoryRepository : GenericRepository<ItemCategory>, IItemCategoryRepository
    {
        public ItemCategoryRepository(AppDbContext context) : base(context)
        {

        }
    }
}
