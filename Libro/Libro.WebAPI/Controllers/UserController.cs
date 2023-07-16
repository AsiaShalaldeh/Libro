using Libro.Domain.Models;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FluentValidation;
using Libro.Application.Validators;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Dtos;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IAuthenticationService authenticationService,
            ILogger<UserController> logger, IUserRepository userRepository)
        {
            _authenticationService = authenticationService;
            _logger = logger;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="registerModel">The registration data.</param>
        /// <returns>The registration response.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            try
            {
                if (registerModel.Username.Contains(" "))
                {
                    return BadRequest(new Response
                    {
                        Status = "Error",
                        Message = "Username should not contain spaces."
                    });
                }
                RegisterModelValidator validator = new RegisterModelValidator();
                validator.ValidateAndThrow(registerModel);

                Response response = await _authenticationService.Register(registerModel.Username,
                    registerModel.Email, registerModel.Password);

                if (response.Status.Equals("Success"))
                {
                    _logger.LogInformation("User registered successfully: {Username}", registerModel.Username);
                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning("User registration failed");
                    return BadRequest(response);
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "User registration failed");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Authenticate user and generate an access token.
        /// </summary>
        /// <param name="loginModel">The login credentials.</param>
        /// <returns>The access token.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                LoginModelValidator validator = new LoginModelValidator();
                validator.ValidateAndThrow(loginModel);
                string token = await _authenticationService.Login(loginModel.Username, loginModel.Password);
                if (!token.Equals(""))
                {
                    _logger.LogInformation("User Logged in successfully: {Username}", loginModel.Username);
                    return Ok(token);
                }

                _logger.LogWarning("Invalid username or password");
                return Unauthorized("Invalid username or password");
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "User Log in failed");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user login");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Assign a role to a user.
        /// </summary>
        /// <param name="request">The role assignment data.</param>
        /// <returns>The assignment response.</returns>
        [HttpPost("assign-role")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AssignRoleToUser(UserRoleModel request)
        {
            try
            {
                UserRoleModelValidator validator = new UserRoleModelValidator();
                validator.ValidateAndThrow(request);

                var response = await _authenticationService.AssignRole(request.UserId, request.Role);
                if (response.Status.Equals("Error"))
                {
                    _logger.LogWarning("Error occurred while assigning role to the user");
                    return BadRequest(response);
                }
                _logger.LogInformation("Role assigned successfully : {Role}", request.Role);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Role assigned failed");
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, "Resource not found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while assigning role to the user");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns>A list of users.</returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator, Librarian")]
        [ProducesResponseType(typeof(List<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Retrieving all Users.");

                var users = await _userRepository.GetAllUsersAsync();

                _logger.LogInformation("Successfully retrieved all users.");

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}