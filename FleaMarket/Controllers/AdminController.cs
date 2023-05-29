using FleaMarket.Infrastructure;
using FleaMarket.Models;
using FleaMarket.Models.Items;
using FleaMarket.Models.ViewModels.Admin;
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
        public AdminController(IUnitOfWork uow, IWebHostEnvironment webHostEnvironment)
        {
            _uow = uow;
            _webHostEnvironment = webHostEnvironment;
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
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Error when adding category.";

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

                        TempData["SuccessMessage"] = "Successfully deleted category.";

                        return RedirectToAction("Categories");
                    }
                }
                TempData["ErrorMessage"] = "Unable to delete category.";
                return RedirectToAction("Categories");
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Error when deleting category.";
                return RedirectToAction("Categories");
            }
               
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(int? id, ItemCategory item)
        {
            if (ModelState.IsValid)
            {
                if (id != null)
                {
                    var cat = await _uow.ItemCategories.GetById(id.Value);

                    if(cat != null)
                    {
                        cat.Name = item.Name;
                        await _uow.SaveAsync();

                        return Json( new {Success = true, Message = "Successfully updated category" } );
                    }
                }

                return Json(new { Success = false, Message = "Error when updating category" });
            }

            var mes = "Error when updating category. "+ ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage)).Aggregate("", (current, s) => current + (s + " ")); ;
            return Json(new { Success = false, Message = mes });

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
                    Categories = new List<int>(),
                    Images = new List<Image>(),
                    InspirationItems = new List<InspirationItem>()
                };

                var categories =  await _uow.ItemCategories.GetAll();
                item.CategoriesToChoose = categories.Select(x=>new SelectItem() { label = x.Name, value=x.Id, selected=false }).ToList();

                return View(item);
            }
            else
            {
                var item = await _uow.MarketItems.GetById(id.Value);

                if(item != null)
                {
                    var categories = await _uow.ItemCategories.GetAll();

                    return View(new ItemViewModel()
                    {
                        Id = id.Value,
                        CategoriesToChoose = categories.Select(x => new SelectItem()
                        {
                            label = x.Name,
                            value = x.Id,
                            selected= item.Categories.Contains(x) 
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
                            itemToUpdate.Status = item.Status;

                            itemToUpdate.Categories = await _uow.ItemCategories.GetByIds(item.Categories);
                            itemToUpdate.Images =  await _uow.ImageRepository.GetByIds(item.Images?.Select(x => x.Id));

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
                            Price = item.Price,
                            Status = item.Status,
                            Images = await _uow.ImageRepository.GetByIds(item.Images?.Select(x => x.Id)),
                            Categories = await _uow.ItemCategories.GetByIds(item.Categories)
                        };

                        var res = await _uow.MarketItems.Create(marketItem);
                        await _uow.SaveAsync();

                        TempData["SuccessMessage"] = "Successfully added market item.";

                        return RedirectToAction("MarketItems");
                    }
                }
                TempData["ErrorMessage"] = "Could not update the item.";

                return View(item);
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred.";
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

                        TempData["SuccessMessage"] = "Successfully deleted market item.";

                        return RedirectToAction("MarketItems");
                    }
                }
                TempData["ErrorMessage"] = "Unable to delete market item.";
                return RedirectToAction("MarketItems");
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Error when deleting market item.";
                return RedirectToAction("MarketItems");
            }

        }

        [HttpPost]
        public async Task<JsonResult> AddImage(IFormFile image)
        {
            try
            {
                if (image != null)
                {
                    var extension = Path.GetExtension(image.FileName).ToUpper();
                    var allowedExtensions = new string[] { ".JPG", ".JPEG", ".PNG", ".HEIC" };

                    if (image.Length < 5*1024*1024 && allowedExtensions.Contains(extension))
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

                return new JsonResult(new { Success = false });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Success = false });
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
            catch(Exception ex)
            {
                return Json(new { Success = false });
            }
        }
        [HttpGet]
        public async Task<IActionResult> InspirationItems()
        {
            var items = await _uow.InspirationItems.GetAll();

            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> AddOrEditInspirationItem(int? id)
        {
            if(id == null)
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

                if(itemToUpdate != null)
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
                            TempData["ErrorMessage"] = "Could not find the item.";
                            return View(item);
                        }


                        itemToUpdate.Title = item.Title;
                        itemToUpdate.Description = item.Description;
                        itemToUpdate.Status = item.Status;
                        itemToUpdate.MarketItems = await _uow.MarketItems.GetByIds(item.SelectedItems);

                        if(itemToUpdate.Image.Id != item.Image.Id)
                        {
                            var imageToDelete = itemToUpdate.Image;
                            var isDeleted = await _uow.ImageRepository.Delete(imageToDelete);
                            if(isDeleted)
                            {
                                itemToUpdate.Image = await _uow.ImageRepository.GetById(item.Image.Id);
                                DeleteImage(imageToDelete.Url);
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Error when changing image.";
                                return View(item);
                            }
                        }

                        await _uow.SaveAsync();

                        TempData["SuccessMessage"] = "Successfully updated inspiration item.";
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

                        TempData["SuccessMessage"] = "Successfully added inspiration item.";

                        return RedirectToAction("InspirationItems");
                    }
                }
                TempData["ErrorMessage"] = "Could not update the item.";
                return View(item);
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred.";
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

                        TempData["SuccessMessage"] = "Successfully deleted inspiration item.";

                        return RedirectToAction("InspirationItems");
                    }
                }
                TempData["ErrorMessage"] = "Unable to delete inspiration item.";
                return RedirectToAction("MarketItems");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error when deleting inspiration item.";
                return RedirectToAction("InspirationItems");
            }

        }

        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

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
