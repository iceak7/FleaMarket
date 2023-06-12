using FleaMarket.Infrastructure;
using FleaMarket.Models.ViewModels;
using FleaMarket.Models.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FleaMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _uow;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = new HomeViewModel();
                viewModel.MarketItems = await _uow.MarketItems.GetLastPublished(5);

                return View(viewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}