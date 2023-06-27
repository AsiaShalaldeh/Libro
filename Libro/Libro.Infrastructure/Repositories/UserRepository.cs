using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Libro.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        //public async Task<IdentityUser> GetUserByIdAsync(string userId)
        //{
        //    return await _userManager.FindByIdAsync(userId);
        //}

        //public async Task<IdentityUser> GetUserByNameAsync(string username)
        //{
        //    return await _userManager.FindByNameAsync(username);
        //}

        public async Task UpdateUserAsync(string userId, string name, string email)
        {
            IdentityUser existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser != null)
            {
                if (!email.Equals(""))
                    existingUser.Email = email;
                if (!name.Equals(""))
                    existingUser.UserName = name;
                await _userManager.UpdateAsync(existingUser);
            }
        }

        public async Task DeleteUserAsync(string userId)
        {
            IdentityUser existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser != null)
            {
                await _userManager.DeleteAsync(existingUser);
            }
        }
        public async Task<string> GetCurrentUserIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }

    }

}
