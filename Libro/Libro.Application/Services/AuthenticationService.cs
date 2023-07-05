using Libro.Domain.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Libro.Domain.Exceptions;
using Libro.Domain.Models;

namespace Libro.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPatronService _patronService;
        private readonly ILibrarianService _librarianService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(IConfiguration configuration, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IPatronService patronService,
            ILibrarianService librarianService, ILogger<AuthenticationService> logger)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _patronService = patronService;
            _librarianService = librarianService;
            _logger = logger;
        }
        public async Task<Response> Register(string username, string email, string password)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(username);
                if (userExists != null)
                    return new Response
                    {
                        Status = "Error",
                        Message = "User already exists! The username should be unique."
                    };

                IdentityUser user = new IdentityUser
                {
                    Email = email,
                    UserName = username
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Assign the "Admin" role to the first registered user
                    if (_userManager.Users.Count() == 1)
                    {
                        await _userManager.AddToRoleAsync(user, "Administrator");
                    }

                    return new Response { Status = "Success", Message = "User created successfully!" };
                }

                return new Response
                {
                    Status = "Error",
                    Message = "User creation failed! Please check user details and try again."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering a user.");
                throw;
            }
        }

        public async Task<string> Login(string username, string password)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user != null && await _userManager.CheckPasswordAsync(user, password))
                {
                    await _userManager.RemoveAuthenticationTokenAsync(user, "Libro", "RefreshToken");
                    var token = await GenerateAuthToken(user);

                    //var newRefreshToken = await _userManager.GenerateUserTokenAsync(user, "Libro", "RefreshToken");

                    // Store the token in the UserTokens table
                    await _userManager.SetAuthenticationTokenAsync(user, "Libro", "RefreshToken", token);

                    return token;
                }

                return "";
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "An error occurred while logging in.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in.");
                throw;
            }
        }

        private async Task<string> GenerateAuthToken(IdentityUser user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User cannot be null.");
                }
                var secretKey = _configuration["Jwt:Key"];
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];

                var userRoles = await _userManager.GetRolesAsync(user);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                foreach (var claim in claims)
                {
                    var result = await _userManager.AddClaimAsync(user, claim);
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddDays(5),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the authentication token.");
                throw;
            }
        }

        public async Task<Response> AssignRole(string userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new ResourceNotFoundException("User", "ID", userId.ToString());
                }

                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    throw new ResourceNotFoundException("Role", "Value", role);
                }

                var result = await _userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    return new Response { Status = "Error", Message = "Role assignment " + result.ToString() };
                }
                if (role.Equals("Patron"))
                {
                    await _patronService.AddPatronAsync(userId, user.UserName, user.Email);
                }
                if (role.Equals("Librarian"))
                {
                    await _librarianService.AddLibrarianAsync(userId, user.UserName);
                }
                return new Response { Status = "Success", Message = "Role assigned successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while assigning a role to a user.");
                throw;
            }
        }
    }
}
