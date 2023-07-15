using Libro.Application.Services;
using Libro.Domain.Interfaces.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

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
            Assert.Equal("Error", result.Status);
            Assert.Equal("User already exists! The UserName Should Be Unique", result.Message);
            _userManagerMock.Verify(mock => mock.FindByNameAsync(username), Times.Once);
            _userManagerMock.Verify(mock => mock.CreateAsync(It.IsAny<IdentityUser>(), password), Times.Never);
        }

        [Fact]
        public async Task Login_ValidCredentials_Success()
        {
            // Arrange
            string username = "john";
            string password = "password";
            var user = new IdentityUser
            {
                Id = "123",
                UserName = "john",
                Email = "john@example.com"
            };

            _userManagerMock.Setup(mock => mock.FindByNameAsync(username))
                .ReturnsAsync(user);
            _userManagerMock.Setup(mock => mock.CheckPasswordAsync(user, password))
                .ReturnsAsync(true);
            _userManagerMock.Setup(mock => mock.RemoveAuthenticationTokenAsync(user, "Libro", "RefreshToken"))
                .ReturnsAsync(IdentityResult.Success);

            var generateAuthTokenMethod = _authenticationService.GetType()
                .GetMethod("GenerateAuthToken", BindingFlags.NonPublic | BindingFlags.Instance);
            var generateAuthTokenDelegate = (Func<IdentityUser, Task<string>>)Delegate.CreateDelegate(typeof(Func<IdentityUser, Task<string>>), _authenticationService, generateAuthTokenMethod);
            var generateAuthTokenResult = Task.FromResult("sampleToken");
            _userManagerMock.Setup(mock => mock.GenerateUserTokenAsync(user, "Libro", "RefreshToken"))
                .ReturnsAsync("refreshToken");

            _userManagerMock.Setup(mock => mock.SetAuthenticationTokenAsync(user, "Libro", "RefreshToken", "refreshToken"))
            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(mock => mock.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Role1", "Role2" });

            _configurationMock.Setup(x => x.GetSection("Jwt:Key").Value).Returns("QCzh7iT3Pwc4N7jDBHy2QCzh7iTGPwc4N7jDBHy2");
            var secretKey = _configurationMock.Object.GetSection("Jwt:Key").Value;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "Role1"),
                    new Claim(ClaimTypes.Role, "Role2")
                }),
                Expires = DateTime.UtcNow.AddDays(5),
                SigningCredentials = signingCredentials
            };

            var jwtHandler = new JwtSecurityTokenHandler();
            var token = jwtHandler.CreateToken(tokenDescriptor);
            var tokenString = jwtHandler.WriteToken(token);

            _userManagerMock.Setup(mock => mock.AddClaimAsync(user, It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(mock => mock.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Role1", "Role2" });

            // Act
            var result = await _authenticationService.Login(username, password);

            // Assert
            Assert.Equal("sampleToken", result);
            _userManagerMock.Verify(mock => mock.FindByNameAsync(username), Times.Once);
            _userManagerMock.Verify(mock => mock.CheckPasswordAsync(user, password), Times.Once);
            _userManagerMock.Verify(mock => mock.RemoveAuthenticationTokenAsync(user, "Libro", "RefreshToken"), Times.Once);
            _userManagerMock.Verify(mock => mock.GenerateUserTokenAsync(user, "Libro", "RefreshToken"), Times.Once);
            _userManagerMock.Verify(mock => mock.SetAuthenticationTokenAsync(user, "Libro", "RefreshToken", "refreshToken"), Times.Once);
            _userManagerMock.Verify(mock => mock.AddClaimAsync(user, It.IsAny<Claim>()), Times.Exactly(3));
            _userManagerMock.Verify(mock => mock.GetRolesAsync(user), Times.Once);
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
