using Libro.Application.Services;
using Libro.Domain.Interfaces.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Libro.Tests.Libro.Application.Tests
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IPatronRepository> _patronRepositoryMock;
        private readonly Mock<ILibrarianRepository> _librarianRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<AuthenticationService>> _loggerMock;
        private readonly AuthenticationService _authenticationService;

        public AuthenticationServiceTests()
        {
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
            _patronRepositoryMock = new Mock<IPatronRepository>();
            _librarianRepositoryMock = new Mock<ILibrarianRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<AuthenticationService>>();

            _authenticationService = new AuthenticationService(
                _configurationMock.Object,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _loggerMock.Object,
                _patronRepositoryMock.Object,
                _librarianRepositoryMock.Object);
        }

        [Fact]
        public async Task Register_NewUser_Success()
        {
            // Arrange
            string username = "john";
            string email = "john@example.com";
            string password = "password";

            _userManagerMock.Setup(mock => mock.FindByNameAsync(username))
                .ReturnsAsync((IdentityUser)null);
            _userManagerMock.Setup(mock => mock.CreateAsync(It.IsAny<IdentityUser>(), password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authenticationService.Register(username, email, password);

            // Assert
            Assert.Equal("Success", result.Status);
            Assert.Equal("User created successfully!", result.Message);
            _userManagerMock.Verify(mock => mock.FindByNameAsync(username), Times.Once);
            _userManagerMock.Verify(mock => mock.CreateAsync(It.IsAny<IdentityUser>(), password), Times.Once);
        }

        [Fact]
        public async Task Register_ExistingUser_Error()
        {
            // Arrange
            string username = "john";
            string email = "john@example.com";
            string password = "password";
            var existingUser = new IdentityUser();

            _userManagerMock.Setup(mock => mock.FindByNameAsync(username))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _authenticationService.Register(username, email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Error", result.Status);
            Assert.Equal("User already exists! The username should be unique.", result.Message);
            _userManagerMock.Verify(mock => mock.FindByNameAsync(username), Times.Once);
            _userManagerMock.Verify(mock => mock.CreateAsync(It.IsAny<IdentityUser>(), password), Times.Never);
        }

        [Fact]
        public async Task Login_ValidCredentials_EmptyToken()
        {
            // Arrange
            string username = "john";
            string password = "password";
            var user = new IdentityUser();

            _userManagerMock.Setup(mock => mock.FindByNameAsync(username))
                .ReturnsAsync(user);

            // Act
            var token = await _authenticationService.Login(username, password);

            // Assert
            Assert.Equal("", token);
            _userManagerMock.Verify(mock => mock.FindByNameAsync(username), Times.Once);
            _userManagerMock.Verify(mock => mock.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Login_InvalidCredentials_EmptyToken()
        {
            // Arrange
            string username = "john";
            string password = "password";

            _userManagerMock.Setup(mock => mock.FindByNameAsync(username))
                .ReturnsAsync((IdentityUser)null);

            // Act
            var token = await _authenticationService.Login(username, password);

            // Assert
            Assert.Equal("", token);
            _userManagerMock.Verify(mock => mock.FindByNameAsync(username), Times.Once);
            _userManagerMock.Verify(mock => mock.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
            _userManagerMock.Verify(mock => mock.RemoveAuthenticationTokenAsync(It.IsAny<IdentityUser>(), "Libro", "RefreshToken"), Times.Never);
        }

    }

}
