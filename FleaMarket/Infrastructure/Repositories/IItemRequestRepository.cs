using FleaMarket.Models;

namespace FleaMarket.Infrastructure.Repositories
{
    public interface IItemRequestRepository : IGenericRepository<ItemRequest>
    {
        Task<IEnumerable<ItemRequest>> GetItemByStatus(ItemRequestStatus? status);
    }
}
