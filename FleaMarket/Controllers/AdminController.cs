using FleaMarket.Infrastructure;
using FleaMarket.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleaMarket.Controllers
{
    [Authorize(Roles = "SuperAdmin, User")]
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

        [HttpPost]
        public async Task<IActionResult>EditUserRole(string userId, string roleSelected)
        {

            var res = await _uow.UserRepository.UpdateUserRole(userId, roleSelected);



            return RedirectToAction("Users");

        }


    }
}
