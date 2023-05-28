using FleaMarket.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace FleaMarket.Controllers
{
    public class MarketController : Controller
    {
        private readonly IUnitOfWork _uow;

        public MarketController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IActionResult> Index()
        {
            var items = await _uow.MarketItems.GetPublishedItems();

            return View(items);
        }

        public async Task<IActionResult> Item(int? id)
        {
            if(id == null)
                return NotFound();

            var item = await _uow.MarketItems.GetById(id.Value);

            if(item != null)
            {
                if(item.Status != Models.ItemStatus.Published && ( User.IsInRole("Admin") || User.IsInRole("SuperAdmin")))
                {
                    return View(item);
                }
                else if(item.Status == Models.ItemStatus.Published)
                {
                    return View(item);
                }
                else
                {
                    return NotFound();
                }
            }
            
            return NotFound();
        }
    }
}
