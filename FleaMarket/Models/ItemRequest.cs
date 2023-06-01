using System.ComponentModel.DataAnnotations;

namespace FleaMarket.Models
{
    public class ItemRequest
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        [Required]
        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNr { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }
        [Required]
        public MarketItem MarketItem { get; set; }

    }
}
