using FleaMarket.Infrastructure;
using FleaMarket.Infrastructure.Repositories;
using FleaMarket.Models.ViewModels.Components;
using Microsoft.AspNetCore.Mvc;

namespace FleaMarket.Components
{
    public class RolesOptions : ViewComponent      
    {
        private readonly IUnitOfWork _uow;

        public RolesOptions(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IViewComponentResult> InvokeAsync(string selected)
        {
            var roles = await _uow.RolesRepository.GetAllRoles();

            return View(new UserRolesViewModel() { Roles = roles, Selected = selected} );
        }
    }
}
