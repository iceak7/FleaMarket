using System.ComponentModel.DataAnnotations;

namespace FleaMarket.Models
{
    public class ItemCategory
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }


        public ICollection<MarketItem> Items { get; set; }
    }
}
