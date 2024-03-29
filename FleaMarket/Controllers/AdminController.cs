﻿using FleaMarket.Infrastructure;
using FleaMarket.Models;
using FleaMarket.Models.Items;
using FleaMarket.Models.ViewModels;
using FleaMarket.Models.ViewModels.Admin;
using FleaMarket.Models.ViewModels.Market;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace FleaMarket.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AdminController> _logger;
        public AdminController(IUnitOfWork uow, IWebHostEnvironment webHostEnvironment, ILogger<AdminController> logger)
        {
            _uow = uow;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        //Dashboard
        [HttpGet]
        public async Task<IActionResult> Index(ItemRequestStatus? status)
        {
            try
            {
                var requests = await _uow.ItemRequestRepository.GetItemByStatus(status);

                var item = new DashboardViewModel()
                {
                    ItemRequests = requests.OrderByDescending(x => x.Status),
                    Status = status
                };

                return View(item);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateRequestStatus(int id, ItemRequestStatus status)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var itemRequest = await _uow.ItemRequestRepository.GetById(id);

                    if (itemRequest != null)
                    {
                        if (itemRequest.Status == status)
                        {
                            return new JsonResult(new { Success = true });
                        }

                        itemRequest.Status = status;
                        await _uow.SaveAsync();

                        return new JsonResult(new { Success = true });
                    }

                    return new JsonResult(new { Success = false });
                }

                return new JsonResult(new { Success = false });
            }
            catch (Exception)
            {

                return new JsonResult(new { Success = false });
            }

        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Users()
        {
            try
            {
                var users = await _uow.UserRepository.GetAll();

                var userViewModels = users.Select(x => new UserViewModel
                {
                    Email = x.Email,
                    Id = x.Id,
                    UserName = x.UserName,
                    Role = _uow.UserRepository.GetRoleByUser(x).Result
                }
                );

                return View(userViewModels);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ett fel uppstod.";
                return View("Error");
            }

        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> EditUserRole(string userId, string roleSelected)
        {
            try
            {
                var res = await _uow.UserRepository.UpdateUserRole(userId, roleSelected);

                if (res)
                {
                    if (roleSelected != null)
                        TempData["SuccessMessage"] = $"Roll uppdaterad till '{roleSelected}'.";
                    else
                        TempData["SuccessMessage"] = $"Rollen togs bort.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Det gick inte att skapa rollen.";
                }

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ett fel uppstod.";
            }

            return RedirectToAction("Users");

        }
        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            try
            {
                var categories = await _uow.ItemCategories.GetAll();

                return View(categories);
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(ItemCategory category)
        {
            try
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ett fel uppstod vid skapandet av kategorin.";

                return RedirectToAction("Categories");
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                if (id != 0)
                {
                    var item = await _uow.ItemCategories.GetById(id);
                    if (item != null)
                    {
                        await _uow.ItemCategories.Delete(item);
                        await _uow.SaveAsync();

                        TempData["SuccessMessage"] = "Raderingen av kategorin lyckades.";

                        return RedirectToAction("Categories");
                    }
                }
                TempData["ErrorMessage"] = "Det gick inte att radera kategorin.";
                return RedirectToAction("Categories");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ett el uppstod..";
                return RedirectToAction("Categories");
            }

        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(int? id, ItemCategory item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id != null)
                    {
                        var cat = await _uow.ItemCategories.GetById(id.Value);

                        if (cat != null)
                        {
                            cat.Name = item.Name;
                            await _uow.SaveAsync();

                            return Json(new { Success = true, Message = "Uppdateringen av kategorin lyckades." });
                        }
                    }

                    return Json(new { Success = false, Message = "Ett fel uppstod." });
                }

                var mes = "Ett fel uppstod vid uppdateringen av kategorin. " + ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage)).Aggregate("", (current, s) => current + (s + " ")); ;
                return Json(new { Success = false, Message = mes });
            }
            catch (Exception ex)
            {
                var mes = "Ett fel uppstod vid uppdateringen av kategorin. ";
                return Json(new { Success = false, Message = mes });
            }


        }

        [HttpGet]
        public async Task<IActionResult> MarketItems(int? category, string sortOrder, string search, int? page, ItemStatus? status)
        {
            try
            {
                var items = await _uow.MarketItems.GetAllItems(category, search, status);

                var model = new MarketItemsViewModel()
                {
                    Search = search,
                    SortOrder = sortOrder,
                    Category = category,
                    Status = status
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
                            items = items.OrderByDescending(x => x.Id).ToList();
                            break;
                        case "oldest":
                            items = items.OrderBy(x => x.Id).ToList();
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
            catch
            {
                return View("Error");
            }

        }

        [HttpGet]
        public async Task<IActionResult> AddOrEditItem(int? id)
        {
            try
            {
                if (id == null)
                {
                    var item = new ItemViewModel()
                    {
                        Categories = new List<int>(),
                        Images = new List<Image>(),
                        InspirationItems = new List<InspirationItem>()
                    };

                    var categories = await _uow.ItemCategories.GetAll();
                    item.CategoriesToChoose = categories.Select(x => new SelectItem() { label = x.Name, value = x.Id, selected = false }).ToList();

                    return View(item);
                }
                else
                {
                    var item = await _uow.MarketItems.GetById(id.Value);

                    if (item != null)
                    {
                        var categories = await _uow.ItemCategories.GetAll();

                        return View(new ItemViewModel()
                        {
                            Id = id.Value,
                            CategoriesToChoose = categories.Select(x => new SelectItem()
                            {
                                label = x.Name,
                                value = x.Id,
                                selected = item.Categories.Contains(x)
                            }).ToList(),
                            Images = item.Images.ToList(),
                            Description = item.Description,
                            InspirationItems = item.InspirationItems.ToList(),
                            Price = item.Price,
                            Title = item.Title,
                            Status = item.Status
                        });
                    }
                    return NotFound();
                }
            }
            catch 
            { 
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditItem(ItemViewModel item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (item.Id != null)
                    {
                        var itemToUpdate = await _uow.MarketItems.GetById(item.Id.Value);

                        if (itemToUpdate != null)
                        {
                            itemToUpdate.Price = item.Price;
                            itemToUpdate.Title = item.Title;
                            itemToUpdate.Description = item.Description;

                            if (itemToUpdate.Status != ItemStatus.Published && item.Status == ItemStatus.Published)
                            {
                                itemToUpdate.PublicationDate = DateTime.Now;
                            }
                            else if(itemToUpdate.Status == ItemStatus.Published && item.Status == ItemStatus.Unpublished)
                            {
                                itemToUpdate.PublicationDate = null;
                            }


                            itemToUpdate.Status = item.Status;

                            itemToUpdate.Categories = await _uow.ItemCategories.GetByIds(item.Categories);
                            itemToUpdate.Images = await _uow.ImageRepository.GetByIds(item.Images?.Select(x => x.Id));

                            await _uow.SaveAsync();

                            TempData["SuccessMessage"] = "Uppdateringen av föremålet lyckades.";

                            return RedirectToAction("MarketItems");
                        }
                    }
                    else
                    {
                        MarketItem marketItem = new MarketItem()
                        {
                            Description = item.Description,
                            Title = item.Title,
                            Price = item.Price,
                            Status = item.Status,
                            PublicationDate = item.Status == ItemStatus.Published ? DateTime.Now : null,
                            Images = await _uow.ImageRepository.GetByIds(item.Images?.Select(x => x.Id)),
                            Categories = await _uow.ItemCategories.GetByIds(item.Categories)
                        };


                        var res = await _uow.MarketItems.Create(marketItem);
                        await _uow.SaveAsync();

                        TempData["SuccessMessage"] = "Skapandet av föremålet lyckades.";

                        return RedirectToAction("MarketItems");
                    }
                }
                TempData["ErrorMessage"] = "Kunde inte uppdatera föremålet.";

                return View(item);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ett fel uppstod.";
                return View(item);
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                if (id != 0)
                {
                    var item = await _uow.MarketItems.GetById(id);
                    if (item != null)
                    {
                        await _uow.MarketItems.Delete(item);
                        await _uow.SaveAsync();

                        TempData["SuccessMessage"] = "Raderingen av föremålet lyckades.";

                        return RedirectToAction("MarketItems");
                    }
                }
                TempData["ErrorMessage"] = "Kunde inte radera föremålet.";
                return RedirectToAction("MarketItems");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ett fel uppstod.";
                return RedirectToAction("MarketItems");
            }

        }

        [HttpPost]
        public async Task<JsonResult> AddImage(IFormFile image)
        {
            _logger.LogInformation($"Adding image with filename '{image.FileName}'." );

            try
            {
                if (image != null)
                {
                    var extension = Path.GetExtension(image.FileName).ToUpper();
                    var allowedExtensions = new string[] { ".JPG", ".JPEG", ".PNG", ".HEIC" };

                    if (image.Length < 6 * 1024 * 1024 && allowedExtensions.Contains(extension))
                    {
                        string folder = "images/uploads/";

                        var url = await UploadImage(folder, image);

                        var createdImage = await _uow.ImageRepository.Create(new Image()
                        {
                            Url = url
                        });
                        await _uow.SaveAsync();

                        return new JsonResult(new { Success = true, createdImage.Url, createdImage.Id });
                    }

                }

                return new JsonResult(new { Success = false, Message = "Bilden är för stor eller är inte i formatet JPG, JPEG, PNG eller HEIC." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not upload image.");
                return new JsonResult(new { Success = false, Message="There was an error" });
            }

        }

        [HttpPost]
        public async Task<JsonResult> DeleteImage(int? id)
        {
            try
            {
                if (id != null)
                {
                    var imageToDelete = await _uow.ImageRepository.GetById(id.Value);
                    if (imageToDelete != null)
                    {
                        var path = imageToDelete.Url;

                        var isDeleted = await _uow.ImageRepository.Delete(imageToDelete);
                        await _uow.SaveAsync();

                        if (isDeleted)
                        {
                            var res = DeleteImage(path);
                            return new JsonResult(new { Success = true });
                        }
                    }
                }

                return Json(new { Success = false });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false });
            }
        }
        [HttpGet]
        public async Task<IActionResult> InspirationItems()
        {
            try
            {
                var items = await _uow.InspirationItems.GetAll();

                return View(items);
            }
            catch (Exception)
            {

                return View("Error");
            }

        }
        [HttpGet]
        public async Task<IActionResult> AddOrEditInspirationItem(int? id)
        {
            try
            {
                if (id == null)
                {
                    InspirationItemViewModel item = new InspirationItemViewModel();

                    var marketItem = await _uow.MarketItems.GetAll();

                    item.MarketItemOptions = marketItem.Select(x => new MarketItemOptionViewModel()
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Selected = false,
                        Title = x.Title,
                        Price = x.Price
                    }).ToList();

                    return View(item);
                }
                else
                {
                    var itemToUpdate = await _uow.InspirationItems.GetById(id.Value);

                    if (itemToUpdate != null)
                    {
                        var item = new InspirationItemViewModel();

                        item.Title = itemToUpdate.Title;
                        item.Description = itemToUpdate.Description;
                        item.Image = itemToUpdate.Image;
                        item.Id = itemToUpdate.Id;
                        item.Status = itemToUpdate.Status;

                        var marketItem = await _uow.MarketItems.GetAll();

                        item.MarketItemOptions = marketItem.Select(x => new MarketItemOptionViewModel()
                        {
                            Id = x.Id,
                            Description = x.Description,
                            Selected = itemToUpdate.MarketItems.Contains(x),
                            Title = x.Title,
                            Price = x.Price
                        }).ToList();

                        return View(item);
                    }
                    return NotFound();

                }
            }
            catch (Exception)
            {

                return View("Error");
            }

        }
        [HttpPost]
        public async Task<IActionResult> AddOrEditInspirationItem(InspirationItemViewModel item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (item.Id != null)
                    {
                        var itemToUpdate = await _uow.InspirationItems.GetById(item.Id.Value);

                        if (itemToUpdate == null)
                        {
                            TempData["ErrorMessage"] = "Kunde inte hitta föremålet.";
                            return View(item);
                        }


                        itemToUpdate.Title = item.Title;
                        itemToUpdate.Description = item.Description;
                        itemToUpdate.Status = item.Status;
                        itemToUpdate.MarketItems = await _uow.MarketItems.GetByIds(item.SelectedItems);

                        if (itemToUpdate.Image.Id != item.Image.Id)
                        {
                            var imageToDelete = itemToUpdate.Image;
                            var isDeleted = await _uow.ImageRepository.Delete(imageToDelete);
                            if (isDeleted)
                            {
                                itemToUpdate.Image = await _uow.ImageRepository.GetById(item.Image.Id);
                                DeleteImage(imageToDelete.Url);
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Ett fel uppstod vid ändring av bilden.";
                                return View(item);
                            }
                        }

                        await _uow.SaveAsync();

                        TempData["SuccessMessage"] = "Uppdateringen lyckades.";
                        return RedirectToAction("InspirationItems");
                    }
                    else
                    {
                        var newItem = new InspirationItem()
                        {
                            Description = item.Description,
                            Image = await _uow.ImageRepository.GetById(item.Image.Id),
                            Status = item.Status,
                            Title = item.Title,
                            MarketItems = await _uow.MarketItems.GetByIds(item.SelectedItems)
                        };

                        var createdItem = await _uow.InspirationItems.Create(newItem);
                        await _uow.SaveAsync();

                        TempData["SuccessMessage"] = "Skapandet av föremålet lyckades.";

                        return RedirectToAction("InspirationItems");
                    }
                }
                TempData["ErrorMessage"] = "Kunde inte uppdatera föremålet.";
                return View(item);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ett fel uppstod.";
                return View(item);
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteInspirationItem(int id)
        {
            try
            {
                if (id != 0)
                {
                    var item = await _uow.InspirationItems.GetById(id);
                    if (item != null)
                    {
                        await _uow.InspirationItems.Delete(item);
                        await _uow.SaveAsync();

                        TempData["SuccessMessage"] = "Raderingen av föremålet lyckades.";

                        return RedirectToAction("InspirationItems");
                    }
                }
                TempData["ErrorMessage"] = "Kunde inte radera föremålet.";
                return RedirectToAction("MarketItems");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ett fel uppstod vid raderingen av föremålet.";
                return RedirectToAction("InspirationItems");
            }

        }

        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {
            
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            _logger.LogInformation($"Trying to upload image with folderpath '{folderPath}' and webRootPath '{_webHostEnvironment.WebRootPath}'.");

            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

            _logger.LogInformation($"Serverfolder combined: '{serverFolder}'.");

            using var filestream = new FileStream(serverFolder, FileMode.Create);
            await file.CopyToAsync(filestream);
            filestream.Close();

            return "/" + folderPath;
        }

        private bool DeleteImage(string path)
        {
            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath + path);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return true;
            };

            return false;
        }


    }
}
