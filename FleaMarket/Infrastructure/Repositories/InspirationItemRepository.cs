using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Models;

namespace FleaMarket.Infrastructure.Repositories
{
    public class InspirationItemRepository : GenericRepository<InspirationItem>, IInspirationItemRepository
    {
        public InspirationItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
