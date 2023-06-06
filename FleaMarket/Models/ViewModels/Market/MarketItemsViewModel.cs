namespace FleaMarket.Models.ViewModels.Market
{
    public class MarketItemsViewModel
    {
        public IEnumerable<MarketItem> Items { get; set; }
        public List<ItemCategory> Categories { get; set; }
        public int? Category { get; set; }
        public string SortOrder { get; set; }
        public string Search { get; set; }

        public int Page { get; set; }
        public int PagesCount { get; set; }
        public ItemStatus? Status { get; set; }
    }
}
