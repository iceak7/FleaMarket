using FleaMarket.Infrastructure;
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


    }
}
