using Microsoft.AspNetCore.Identity;

namespace FleaMarket.Infrastructure.Repositories
{
    public interface IUserRepository 
    {
        Task<IEnumerable<IdentityUser>> GetAll();
        Task<string> GetRoleByUser(IdentityUser user);
        Task<bool> UpdateUserRole(string userId, string role);
    }
}
