using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FleaMarket.Models
{
    public class MarketItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [Range(0, 1000000)]
        [Precision(18, 2)]
        public decimal? Price { get; set; }

        public ItemStatus Status { get; set; }


        public ICollection<ItemCategory> Categories { get; set; }
        public ICollection<Image> Images { get; set; }
        public ICollection<InspirationItem> InspirationItems { get; set; }
        public ICollection<ItemRequest> ItemRequests{ get; set; }
    }

    public enum ItemStatus
    {
        Unpublished, 
        Published,
        Sold
    }
}
