using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Models;
using Microsoft.AspNetCore.Mvc;

namespace FleaMarket.Infrastructure.Repositories
{
    public class ItemRequestRepository : GenericRepository<ItemRequest>, IItemRequestRepository
    {
        private readonly AppDbContext _appDbContext;
        public ItemRequestRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }
    }
}
