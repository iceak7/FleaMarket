using Microsoft.AspNetCore.Identity;

namespace FleaMarket.Infrastructure.Repositories
{
    public interface IRolesRepository
    {
        Task<IEnumerable<IdentityRole>> GetAllRoles();
    }
}
