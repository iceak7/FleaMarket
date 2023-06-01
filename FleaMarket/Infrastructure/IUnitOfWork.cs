using FleaMarket.Infrastructure.Repositories;

namespace FleaMarket.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IMarketItemRepository MarketItems { get; }
        IInspirationItemRepository InspirationItems { get; }
        IItemCategoryRepository ItemCategories { get; }
        IUserRepository UserRepository { get; }
        IRolesRepository RolesRepository { get; }
        IImageRepository ImageRepository { get; }
        IItemRequestRepository ItemRequestRepository { get; }
        Task<int> SaveAsync();
    }
}
