using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleaMarket.Infrastructure.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesRepository(RoleManager<IdentityRole> rm)
        {
            _roleManager = rm;
        }

        public async Task<IEnumerable<IdentityRole>> GetAllRoles()
        {
            return await _roleManager.Roles.ToListAsync();
        }


    }
}
