using Libro.Domain.Dtos;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public UserController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserDto request)
        {
            bool isRegistered = _authenticationService.Register(request.UserName, request.Password);

            if (isRegistered)
            {
                return Ok("Registration successful");
            }

            return BadRequest("Registration failed");
        }

        [HttpPost("login")]
        public IActionResult Login(UserDto request)
        {
            string token = _authenticationService.Login(request.UserName, request.Password);

            if (token != null)
            {
                return Ok(token);
            }

            return Unauthorized("Invalid username or password");
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Administrator")]
        public IActionResult AssignRole(UserRoleDto request)
        {
            _authenticationService.AssignRole(request.UserId, request.Role);
            return Ok("Role assigned successfully");
        }
    }
}
