using FleaMarket.Infrastructure.Repositories;

namespace FleaMarket.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IMarketItemRepository MarketItems { get; }
        IInspirationItemRepository InspirationItems { get; }
        IItemCategoryRepository ItemCategories { get; }
        Task<int> SaveAsync();
    }
}
