using FleaMarket.Infrastructure.DataAccess;
using FleaMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace FleaMarket.Infrastructure.Repositories
{
    public class ImageRepository : GenericRepository<Image>, IImageRepository
    {
        private readonly AppDbContext _context;
        public ImageRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Image>> GetByIds(IEnumerable<int> ids)
        {
            List<Image> images = new List<Image>();

            if(ids?.Count() > 0)
            {
                images = await _context.Images.Where(x => ids.Contains(x.Id)).ToListAsync();
            }
            return images;
        }
    }
}
