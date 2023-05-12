using System.ComponentModel.DataAnnotations;

namespace FleaMarket.Models
{
    public class Image
    {
        public int Id { get; set; }
        [Required]
        public string Url { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }


        public ICollection<MarketItem> MarketItems { get; set; }
        public ICollection<InspirationItem> InspirationItems { get; set; }

    }
}
