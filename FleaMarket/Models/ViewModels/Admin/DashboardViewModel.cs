﻿namespace FleaMarket.Models.ViewModels.Admin
{
    public class DashboardViewModel
    {
        public IEnumerable<ItemRequest> ItemRequests { get; set; }
        public ItemRequestStatus? Status { get; set; }
    }
}
