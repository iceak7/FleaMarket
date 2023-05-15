using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleaMarket.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserRepository(UserManager<IdentityUser> um)
        {
            _userManager = um;
        }
        public async Task<IEnumerable<IdentityUser>> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();
            
            return users;
        }

        public async Task<string> GetRoleByUser(IdentityUser user)
        {
            
            var roles = await _userManager.GetRolesAsync(user);

            return roles.FirstOrDefault();
        }

        public async Task<bool> UpdateUserRole(string userId, string role)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);

            if(user == null) 
                return false;

            var currentRole = await _userManager.GetRolesAsync(user);

            if(currentRole.Count > 0) 
                await _userManager.RemoveFromRoleAsync(user, currentRole.FirstOrDefault());

            if(role != null)
            {
                var res = await _userManager.AddToRoleAsync(user, role);

                if (res.Succeeded)
                    return true;
                else
                    return false;
            }
              
            return true;
        }
    }
}
