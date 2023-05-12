using System.ComponentModel.DataAnnotations;

namespace FleaMarket.Models
{
    public class InspirationItem
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]


        public Image Image { get; set; }
        public ICollection<MarketItem> MarketItems { get; set; }
    }
}
