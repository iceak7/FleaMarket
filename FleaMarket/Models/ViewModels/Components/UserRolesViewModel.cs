using Microsoft.AspNetCore.Identity;

namespace FleaMarket.Models.ViewModels.Components
{
    public class UserRolesViewModel
    {
        public IEnumerable<IdentityRole> Roles { get; set; }
        public string Selected { get; set; }

    }
}
