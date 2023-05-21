using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Models;

namespace FleaMarket.Infrastructure.Repositories
{
    public class ImageRepository : GenericRepository<Image>, IImageRepository
    {
        public ImageRepository(AppDbContext context) : base(context)
        {
        }
    }
}
