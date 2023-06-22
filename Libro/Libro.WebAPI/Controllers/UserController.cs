using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Libro.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/users/auth")]
    public class UserController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public UserController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel) 
        {
            try
            {
                bool isRegistered = await _authenticationService.Register(registerModel.Username,
                    registerModel.Email, registerModel.Password);

                if (isRegistered)
                {
                    return Ok(new Response { Status = "Success", Message = "User created successfully!" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response 
                    { Status = "Error", Message = "User creation failed! Please check user " +
                    "details and try again." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                string token = await _authenticationService.Login(loginModel.Username, loginModel.Password);

                if (!token.Equals(""))
                {
                    return Ok(token);
                }
                return Unauthorized("Invalid username or password");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AssignRoleToUser(UserRoleDto request)
        {
            try
            {
                var result = await _authenticationService.AssignRole(request.UserId, request.Role);
                if (!result)
                {
                    throw new Exception("Role assignment failed");
                }
                return Ok("Role assigned successfully");
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }
    }
}
