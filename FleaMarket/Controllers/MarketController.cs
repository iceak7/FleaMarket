using FleaMarket.Infrastructure;
using FleaMarket.Models.ViewModels.Market;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace FleaMarket.Controllers
{
    public class MarketController : Controller
    {
        private readonly IUnitOfWork _uow;

        public MarketController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IActionResult> Index(int? category, string sortOrder, string search, int? page)
        {
            try
            {
                var items = await _uow.MarketItems.GetAllItems(category, search, Models.ItemStatus.Published);

                var model = new MarketItemsViewModel()
                {
                    Search = search,
                    SortOrder = sortOrder,
                    Category = category,
                };

                if (items?.Count() > 0)
                {
                    model.PagesCount = (items.Count() - 1) / 10 + 1;

                    int pageToUse = page != null ? page.Value : 1;

                    if (pageToUse > model.PagesCount)
                        pageToUse = model.PagesCount;

                    int startIndex = 10 * (pageToUse - 1);
                    int count = (startIndex + 10 <= items.Count()) ? 10 : (items.Count() - startIndex);


                    switch (sortOrder)
                    {
                        case "newest":
                            items = items.OrderBy(x => x.PublicationDate).ToList();
                            break;
                        case "oldest":
                            items = items.OrderByDescending(x => x.PublicationDate).ToList();
                            break;
                        case "name":
                            items = items.OrderBy(x => x.Title).ToList();
                            break;
                        default:
                            break;
                    }

                    model.Items = items.ToList().GetRange(startIndex, count);
                    model.Page = pageToUse;
                }
                else
                {
                    model.PagesCount = 0;
                    model.Page = 1;
                }

                var categories = await _uow.ItemCategories.GetAll();
                model.Categories = categories.ToList();


                return View(model);
            }
            catch (Exception)
            {
                return View("Error");
            }

        }

        public async Task<IActionResult> Item(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var item = await _uow.MarketItems.GetById(id.Value);

                if (item != null)
                {
                    if (item.Status != Models.ItemStatus.Published && (User.IsInRole("Admin") || User.IsInRole("SuperAdmin")))
                    {
                        return View(item);
                    }
                    else if (item.Status == Models.ItemStatus.Published)
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
            catch (Exception)
            {
                return View("Error");
            }
        }
    }
}
