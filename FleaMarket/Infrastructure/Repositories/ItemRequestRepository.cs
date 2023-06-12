using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleaMarket.Infrastructure.Repositories
{
    public class ItemRequestRepository : GenericRepository<ItemRequest>, IItemRequestRepository
    {
        private readonly AppDbContext _appDbContext;
        public ItemRequestRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public override async Task<IEnumerable<ItemRequest>> GetAll()
        {
            return await _appDbContext.ItemRequests.Include(x => x.MarketItem).ThenInclude(x => x.Images).ToListAsync();
        }

        public async Task<IEnumerable<ItemRequest>> GetItemByStatus(ItemRequestStatus? status)
        {
            IQueryable<ItemRequest> items = _appDbContext.ItemRequests.Include(x => x.MarketItem).ThenInclude(x => x.Images); 

            if(status != null)
            {
                items = items.Where(x => x.Status == status);
            }

            return await items.ToListAsync();
        }
    }
}
