namespace FleaMarket.Models.ViewModels.Admin
{
    public class MarketItemOptionViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal? Price{ get; set; }
        public string Description { get; set; }
        public bool Selected { get; set; }
    }
}
