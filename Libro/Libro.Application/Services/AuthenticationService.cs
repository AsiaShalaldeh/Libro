using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Libro.Application.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Libro.Domain.Exceptions;

namespace Libro.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IUserRepository userRepository, 
            IConfiguration configuration, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> Register(string username, string email, string password)
        {
            //var userExists = await _userManager.FindByNameAsync(username);
            //if (userExists != null)
            //    throw new Exception(); // new Response { Status = "Error", Message = "User already exists!" }

            IdentityUser user = new()
            {
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = username
            };

            // Assign the default role Patron to the user
            if (!await _roleManager.RoleExistsAsync(UserRole.Patron.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRole.Patron.ToString()));
            }
            await _userManager.AddToRoleAsync(user, UserRole.Patron.ToString());

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return false;

            return true; 
        }

        public async Task<string> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                var token = await GenerateAuthToken(user);
                return token;
            }

            // Store the token in the user session
            // HttpContextAccessor.HttpContext.Session.SetString("AuthToken", token);
            return "";
        }

        private async Task<string> GenerateAuthToken(IdentityUser user)
        {
            var secretKey = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var userRole = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, userRole.FirstOrDefault()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> AssignRole(string userId, string role)
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
                return false;
            }

            return true;
        }
    }
}
