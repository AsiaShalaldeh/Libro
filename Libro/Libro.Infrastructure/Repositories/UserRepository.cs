using Libro.Domain.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Libro.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor,
            ILogger<UserRepository> logger)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task UpdateUserAsync(string userId, string name, string email)
        {
            try
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

                _logger.LogInformation("User updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user.");
                throw; 
            }
        }

        public async Task DeleteUserAsync(string userId)
        {
            try
            {
                IdentityUser existingUser = await _userManager.FindByIdAsync(userId);
                if (existingUser != null)
                {
                    await _userManager.DeleteAsync(existingUser);
                }

                _logger.LogInformation("User deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user.");
                throw; 
            }
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                _logger.LogInformation("Retrieved current user ID successfully.");

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the current user ID.");
                throw; 
            }
        }
    }
}
