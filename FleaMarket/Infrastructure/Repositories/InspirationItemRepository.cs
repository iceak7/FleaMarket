using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace FleaMarket.Infrastructure.Repositories
{
    public class InspirationItemRepository : GenericRepository<InspirationItem>, IInspirationItemRepository
    {
        private readonly AppDbContext _appDbContext;    
        public InspirationItemRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public override async Task<IEnumerable<InspirationItem>> GetAll()
        {
            return await _appDbContext.InspirationItems.Include(x=>x.Image).ToListAsync();
        }

        public override async Task<InspirationItem> GetById(int id)
        {
            return await _appDbContext.InspirationItems.Include(x=> x.Image).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
