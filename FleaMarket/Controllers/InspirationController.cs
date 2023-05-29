using FleaMarket.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace FleaMarket.Controllers
{
    public class InspirationController : Controller
    {
        private readonly IUnitOfWork _uow;
        public InspirationController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IActionResult> Index()
        {
            var items = await _uow.InspirationItems.GetPublishedItems();

            return View(items);
        }

        public async Task<IActionResult> Item(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var item = await _uow.InspirationItems.GetById(id.Value);
             return View(item);
        }
    }
}
