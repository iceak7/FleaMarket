using FleaMarket.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FleaMarket.Infrastructure.DataAccess
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Image> Images { get; set; }
        public DbSet<InspirationItem> InspirationItems { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<MarketItem> MarketItems { get; set; }
    }
}
