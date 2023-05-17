using FleaMarket.Infrastructure;
using FleaMarket.Models;
using FleaMarket.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleaMarket.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _uow;
        public AdminController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Users()
        {
            var users = await _uow.UserRepository.GetAll();

            var userViewModels = users.Select( x => new UserViewModel
                {
                    Email = x.Email,
                    Id = x.Id,
                    UserName = x.UserName,
                    Role = _uow.UserRepository.GetRoleByUser(x).Result
                }
            );

            return View(userViewModels);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult>EditUserRole(string userId, string roleSelected)
        {
            try
            {
                var res = await _uow.UserRepository.UpdateUserRole(userId, roleSelected);

                if (res)
                {
                    if (roleSelected != null)
                        TempData["SuccessMessage"] = $"Successfully set role to '{roleSelected}'.";
                    else
                        TempData["SuccessMessage"] = $"Successfully removed role.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to set role";
                }

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error seting role";
            }

            return RedirectToAction("Users");

        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _uow.ItemCategories.GetAll();

            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(ItemCategory category)
        {
            if (ModelState.IsValid)
            {
                await _uow.ItemCategories.Create(category);
                await _uow.SaveAsync();
            }
            else
            {
                TempData["ValidationError"] = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage)).Aggregate("", (current, s) => current + (s + " "));
            }


            return RedirectToAction("Categories");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if(id != 0)
            {
                var item = await _uow.ItemCategories.GetById(id);
                if (item != null)
                {
                    await _uow.ItemCategories.Delete(item);
                    await _uow.SaveAsync();

                    TempData["SuccessMessage"] = "Successfully deleted category.";

                    return RedirectToAction("Categories");
                }
            }
            TempData["ErrorMessage"] = "Unable to delete category.";
            return RedirectToAction("Categories");
        }

        [HttpGet]
        public async Task<IActionResult> MarketItems()
        {
            var items = await _uow.MarketItems.GetAll();

            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEditItem(int? id)
        {
            if(id == null)
            {
                var item = new ItemViewModel()
                {
                    Categories = new List<ItemCategory>(),
                    Images = new List<Image>(),
                    InspirationItems = new List<InspirationItem>()
                };

                return View(new ItemViewModel());
            }
            else
            {
                var item = await _uow.MarketItems.GetById(id.Value);

                if(item != null)
                {
                    return View(new ItemViewModel()
                    {
                        Id = id.Value,
                        Categories = item.Categories.ToList(),
                        Images = item.Images.ToList(),
                        Description = item.Description,
                        InspirationItems = item.InspirationItems.ToList(),
                        Price = item.Price,
                        Title = item.Title
                    });
                }
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditItem(ItemViewModel item)
        {
            if(ModelState.IsValid)
            {
                if(item.Id != null)
                {
                    var itemToUpdate = await _uow.MarketItems.GetById(item.Id.Value);

                    if (itemToUpdate != null)
                    {
                        itemToUpdate.Price = item.Price;
                        itemToUpdate.Title = item.Title;
                        itemToUpdate.Description = item.Description;

                        await _uow.SaveAsync();

                        TempData["SuccessMessage"] = "Successfully edited market item.";

                        return RedirectToAction("MarketItems");
                    }
                }
                else
                {
                    MarketItem marketItem = new MarketItem()
                    {
                        Description = item.Description,
                        Title = item.Title,
                        Price = item.Price
                    };

                    var res = await _uow.MarketItems.Create(marketItem);
                    await _uow.SaveAsync();

                    TempData["SuccessMessage"] = "Successfully added market item.";

                    return RedirectToAction("MarketItems");
                }
            }

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id)
        {
            if (id != 0)
            {
                var item = await _uow.MarketItems.GetById(id);
                if (item != null)
                {
                    await _uow.MarketItems.Delete(item);
                    await _uow.SaveAsync();

                    TempData["SuccessMessage"] = "Successfully deleted market item.";

                    return RedirectToAction("Categories");
                }
            }
            TempData["ErrorMessage"] = "Unable to delete market item.";
            return RedirectToAction("MarketItems");
        }


    }
}
