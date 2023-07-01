using Libro.Application.Services;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Libro.Tests.MockData;

namespace Libro.Tests.Libro.Application.Tests
{
    public class PatronServiceTests
    {
        private readonly Mock<IPatronRepository> _patronRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<PatronService>> _loggerMock;
        private readonly IPatronService _patronService;

        public PatronServiceTests()
        {
            _patronRepositoryMock = new Mock<IPatronRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<PatronService>>();
            _patronService = new PatronService(
                _patronRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetPatronAsync_ExistingPatronId_ShouldReturnPatron()
        {
            // Arrange
            string patronId = "1";
            var expectedPatron = new Patron { PatronId = patronId, Name = "John Doe" };
            _patronRepositoryMock.Setup(repo => repo.GetPatronByIdAsync(patronId))
                .ReturnsAsync(expectedPatron);

            // Act
            var result = await _patronService.GetPatronAsync(patronId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patronId, result.PatronId);
            Assert.Equal(expectedPatron.Name, result.Name);
        }

        [Fact]
        public async Task GetPatronAsync_NonExistingPatronId_ShouldReturnNull()
        {
            // Arrange
            string nonExistingPatronId = "2";
            _patronRepositoryMock.Setup(repo => repo.GetPatronByIdAsync(nonExistingPatronId))
                .ReturnsAsync((Patron)null);

            // Act
            var result = await _patronService.GetPatronAsync(nonExistingPatronId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddPatronAsync_ShouldAddPatron()
        {
            // Arrange
            string patronId = "1";
            string name = "John Doe";
            string email = "john.doe@example.com";

            // Act
            await _patronService.AddPatronAsync(patronId, name, email);

            // Assert
            _patronRepositoryMock.Verify(repo => repo.AddPatronAsync(It.Is<Patron>(p =>
                p.PatronId == patronId && p.Name == name && p.Email == email)), Times.Once);
        }

        [Fact]
        public async Task UpdatePatronAsync_ExistingPatronId_ShouldReturnUpdatedPatron()
        {
            // Arrange
            string patronId = "1";
            var patronDto = new PatronDto { Name = "Updated Name", Email = "updated.email@example.com" };
            var existingPatron = new Patron { PatronId = patronId, Name = "Original Name", Email = "original.email@example.com" };
            _patronRepositoryMock.Setup(repo => repo.GetPatronByIdAsync(patronId))
                .ReturnsAsync(existingPatron);
            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(patronId, patronDto.Name, patronDto.Email))
                .Returns(Task.CompletedTask);
            _patronRepositoryMock.Setup(repo => repo.UpdatePatronAsync(existingPatron))
                .ReturnsAsync(existingPatron);

            // Act
            var result = await _patronService.UpdatePatronAsync(patronId, patronDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingPatron.PatronId, result.PatronId);
            Assert.Equal(patronDto.Name, result.Name);
            Assert.Equal(patronDto.Email, result.Email);
        }

        [Fact]
        public async Task UpdatePatronAsync_NonExistingPatronId_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            string nonExistingPatronId = "2";
            var patronDto = new PatronDto { Name = "Updated Name", Email = "updated.email@example.com" };
            _patronRepositoryMock.Setup(repo => repo.GetPatronByIdAsync(nonExistingPatronId))
                .ReturnsAsync((Patron)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _patronService.UpdatePatronAsync(nonExistingPatronId, patronDto));
        }

        [Fact]
        public async Task GetBorrowingHistoryAsync_ExistingPatronId_ShouldReturnBorrowingHistory()
        {
            // Arrange
            string patronId = "2";
            var patron = PatronMockData.ReturnPatron();
            _patronRepositoryMock.Setup(repo => repo.GetPatronByIdAsync(patronId))
                .ReturnsAsync(patron);

            // Act
            var result = await _patronService.GetBorrowingHistoryAsync(patronId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patron.CheckedoutBooks, result);
        }

        [Fact]
        public async Task GetBorrowingHistoryAsync_NonExistingPatronId_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            string nonExistingPatronId = "3";
            _patronRepositoryMock.Setup(repo => repo.GetPatronByIdAsync(nonExistingPatronId))
                .ReturnsAsync((Patron)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _patronService.GetBorrowingHistoryAsync(nonExistingPatronId));
        }
    }
}
