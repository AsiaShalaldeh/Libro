using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Libro.Tests.Libro.Infrastructure.Tests
{
    
    public class UserRepositoryTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<ILogger<UserRepository>> _loggerMock;
        private readonly IUserRepository _userRepository;

        public UserRepositoryTests()
        {
            _userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _loggerMock = new Mock<ILogger<UserRepository>>();

            _userRepository = new UserRepository(
                _userManagerMock.Object,
                _httpContextAccessorMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task UpdateUserAsync_ValidUser_UpdateSuccessful()
        {
            // Arrange
            var userId = "123";
            var name = "John Doe";
            var email = "john.doe@example.com";

            var existingUser = new IdentityUser
            {
                Id = userId,
                UserName = "johndoe",
                Email = "johndoe@example.com"
            };

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(existingUser)
                .Verifiable();

            _userManagerMock.Setup(m => m.UpdateAsync(existingUser))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            // Act
            await _userRepository.UpdateUserAsync(userId, name, email);

            // Assert
            _userManagerMock.Verify(m => m.FindByIdAsync(userId), Times.Once);
            _userManagerMock.Verify(m => m.UpdateAsync(existingUser), Times.Once);
            _loggerMock.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteUserAsync_ValidUser_DeletionSuccessful()
        {
            // Arrange
            var userId = "123";

            var existingUser = new IdentityUser
            {
                Id = userId,
                UserName = "johndoe",
                Email = "johndoe@example.com"
            };

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(existingUser)
                .Verifiable();

            _userManagerMock.Setup(m => m.DeleteAsync(existingUser))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            // Act
            await _userRepository.DeleteUserAsync(userId);

            // Assert
            _userManagerMock.Verify(m => m.FindByIdAsync(userId), Times.Once);
            _userManagerMock.Verify(m => m.DeleteAsync(existingUser), Times.Once);
            _loggerMock.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetCurrentUserIdAsync_ValidUser_RetrievalSuccessful()
        {
            // Arrange
            var userId = "123";

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

            _httpContextAccessorMock.Setup(m => m.HttpContext)
                .Returns(httpContext);

            // Act
            var result = await _userRepository.GetCurrentUserIdAsync();

            // Assert
            Assert.Equal(userId, result);
            _loggerMock.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }
    }
}
