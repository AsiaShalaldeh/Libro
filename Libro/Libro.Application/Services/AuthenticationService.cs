using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Libro.Domain.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Libro.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public bool Register(string username, string password)
        {
            // Perform validation
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username is required.", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.", nameof(password));
            }

            var newUser = new User
            {
                UserName = username,
                Password = password,
                Role = UserRole.Patron // Assign default role as a patron
            };

            var validator = new UserValidator();
            var validationResult = validator.Validate(newUser);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.Errors.First().ErrorMessage);
            }

            _userRepository.Add(newUser);
            return true; 
        }

        public string Login(string username, string password)
        {
            // will be replaced by Auhenticate method 
            var user = _userRepository.GetByUsername(username);

            if (user == null || user.Password != password)
            {
                return null;
                //throw new UnauthorizedAccessException("Invalid username or password");
            }

            var token = GenerateAuthToken(user);

            // Store the token in the user session
            // HttpContextAccessor.HttpContext.Session.SetString("AuthToken", token);

            return token;
        }

        private string GenerateAuthToken(User user)
        {
            var secretKey = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
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

        public void AssignRole(int userId, string role)
        {
            var user = _userRepository.GetById(userId);

            if (user != null)
            {
                user.Role = (UserRole)Enum.Parse(typeof(UserRole), role);
                _userRepository.Update(user);
            }
        }
    }
}
