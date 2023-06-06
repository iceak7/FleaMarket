using FleaMarket.Infrastructure;
using FleaMarket.Models;
using Microsoft.AspNetCore.Mvc;

namespace FleaMarket.Controllers
{
    public class ItemRequestController : Controller
    {
        private readonly IUnitOfWork _uow;
        public ItemRequestController(IUnitOfWork uow)
        {
                _uow = uow;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? itemId)
        {
            if (itemId == null)
            {
                return NotFound();
            }
            else
            {
                var marketItem = await _uow.MarketItems.GetById(itemId.Value);
                if (marketItem == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(new ItemRequest()
                    {
                        MarketItem = marketItem
                    });
                }

            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ItemRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var marketItem = await _uow.MarketItems.GetById(request.MarketItem.Id);

                    if (marketItem != null)
                    {
                        request.MarketItem = marketItem;
                        request.Created = DateTime.Now;
                        request.Status = ItemRequestStatus.New;

                        await _uow.ItemRequestRepository.Create(request);
                        await _uow.SaveAsync();

                        return RedirectToAction("Successfull");

                    }
                }
                ViewData["ErrorMessage"] = "There was an error when making the request.";

                return View(request);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "There was an error when making the request.";

                return View(request);
            }
            
        }


        [HttpGet] 
        public async Task<IActionResult> Successfull()
        {
            return View();
        }

    }
}
