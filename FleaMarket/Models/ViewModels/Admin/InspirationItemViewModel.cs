using System.ComponentModel.DataAnnotations;

namespace FleaMarket.Models.ViewModels.Admin
{
    public class InspirationItemViewModel
    {
        public int? Id { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]


        public Image Image { get; set; }
        public List<MarketItem> MarketItems { get; set; }
    }
}
