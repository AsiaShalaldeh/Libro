using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Libro.Domain.Models;
using Libro.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.Tests.Libro.WebAPI.Tests
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly Mock<IAuthenticationService> _authenticationServiceMock;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<ILogger<UserController>> _loggerMock;

        public UserControllerTests()
        {
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _loggerMock = new Mock<ILogger<UserController>>();
            _userRepository = new Mock<IUserRepository>();
            _controller = new UserController(
                _authenticationServiceMock.Object,
                _loggerMock.Object,
                _userRepository.Object
            );
        }

        [Fact]
        public async Task Register_ValidModel_ShouldReturnOkResult()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "testpassword"
            };
            var response = new Response { Status = "Success" };
            _authenticationServiceMock.Setup(service => service.Register(registerModel.Username, registerModel.Email, registerModel.Password))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Register_InvalidModel_ShouldReturnBadRequestResult()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "testpassword"
            };
            var response = new Response { Status = "Error" };
            _authenticationServiceMock.Setup(service => service.Register(registerModel.Username, registerModel.Email, registerModel.Password))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Register_InternalServerError_ShouldReturnInternalServerErrorResult()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "testpassword"
            };
            var exceptionMessage = "An error occurred.";
            _authenticationServiceMock.Setup(service => service.Register(registerModel.Username, registerModel.Email, registerModel.Password))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var internalServerErrorResult = result.Should().BeOfType<ObjectResult>().Subject;
            internalServerErrorResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            internalServerErrorResult.Value.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task Login_ValidModel_ShouldReturnOkResult()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Username = "testuser",
                Password = "testpassword"
            };
            var token = "testtoken";
            _authenticationServiceMock.Setup(service => service.Login(loginModel.Username, loginModel.Password))
                .ReturnsAsync(token);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(token);
        }

        [Fact]
        public async Task Login_InvalidModel_ShouldReturnUnauthorizedResult()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Username = "testuser",
                Password = "testpassword"
            };
            var emptyToken = "";
            _authenticationServiceMock.Setup(service => service.Login(loginModel.Username, loginModel.Password))
                .ReturnsAsync(emptyToken);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.Value.Should().Be("Invalid username or password");
        }

        [Fact]
        public async Task Login_InternalServerError_ShouldReturnInternalServerErrorResult()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Username = "testuser",
                Password = "testpassword"
            };
            var exceptionMessage = "An error occurred.";
            _authenticationServiceMock.Setup(service => service.Login(loginModel.Username, loginModel.Password))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var internalServerErrorResult = result.Should().BeOfType<ObjectResult>().Subject;
            internalServerErrorResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            internalServerErrorResult.Value.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task AssignRoleToUser_ValidRequest_ShouldReturnOkResult()
        {
            // Arrange
            var request = new UserRoleModel
            {
                UserId = "123",
                Role = "Administrator"
            };
            var response = new Response { Status = "Success" };
            _authenticationServiceMock.Setup(service => service.AssignRole(request.UserId, request.Role))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.AssignRoleToUser(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task AssignRoleToUser_InvalidRequest_ShouldReturnBadRequestResult()
        {
            // Arrange
            var request = new UserRoleModel
            {
                UserId = "123",
                Role = "Administrator"
            };
            var response = new Response { Status = "Error" };
            _authenticationServiceMock.Setup(service => service.AssignRole(request.UserId, request.Role))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.AssignRoleToUser(request);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task AssignRoleToUser_ResourceNotFoundException_ShouldReturnNotFoundResult()
        {
            // Arrange
            var request = new UserRoleModel
            {
                UserId = "123",
                Role = "Administrator"
            };
            var exceptionMessage = "No User found with ID = " + request.UserId;
            _authenticationServiceMock.Setup(service => service.AssignRole(request.UserId, request.Role))
                .Throws(new ResourceNotFoundException("User", "ID", request.UserId));

            // Act
            var result = await _controller.AssignRoleToUser(request);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task AssignRoleToUser_InternalServerError_ShouldReturnInternalServerErrorResult()
        {
            // Arrange
            var request = new UserRoleModel
            {
                UserId = "123",
                Role = "Administrator"
            };
            var exceptionMessage = "An error occurred.";
            _authenticationServiceMock.Setup(service => service.AssignRole(request.UserId, request.Role))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = await _controller.AssignRoleToUser(request);

            // Assert
            var internalServerErrorResult = result.Should().BeOfType<ObjectResult>().Subject;
            internalServerErrorResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            internalServerErrorResult.Value.Should().Be(exceptionMessage);
        }
    }
}
