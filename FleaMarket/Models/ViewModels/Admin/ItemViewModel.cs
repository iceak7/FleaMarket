using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FleaMarket.Models.ViewModels.Admin
{
    public class ItemViewModel
    {
        public int? Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [Range(0.00, 1000000.00)]
        [Precision(18, 2)]
        public decimal? Price { get; set; }

        public List<ItemCategory> Categories { get; set; }
        public List<Image> Images { get; set; }
        public List<InspirationItem> InspirationItems { get; set; }
    }
}
