using Libro.Application.Services;
using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Tests.Libro.Application.Tests
{
    public class LibrarianServiceTests
    {
        private readonly Mock<ILibrarianRepository> _librarianRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<LibrarianService>> _loggerMock;
        private readonly ILibrarianService _librarianService;

        public LibrarianServiceTests()
        {
            _librarianRepositoryMock = new Mock<ILibrarianRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<LibrarianService>>();
            _librarianService = new LibrarianService(
                _librarianRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAllLibrariansAsync_ReturnsPaginatedResultOfLibrarians()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var paginatedResult = new PaginatedResult<Librarian>(
                new List<Librarian> { new Librarian { LibrarianId = "1", Name = "John Doe" } },
                1,
                pageNumber,
                pageSize
            );
            _librarianRepositoryMock.Setup(repo => repo.GetAllLibrariansAsync(pageNumber, pageSize))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _librarianService.GetAllLibrariansAsync(pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginatedResult.Items, result.Items);
            Assert.Equal(paginatedResult.TotalCount, result.TotalCount);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
        }

        [Fact]
        public async Task GetLibrarianByIdAsync_ExistingLibrarianId_ShouldReturnLibrarian()
        {
            // Arrange
            string librarianId = "1";
            var librarian = new Librarian { LibrarianId = librarianId, Name = "John Doe" };
            _librarianRepositoryMock.Setup(repo => repo.GetLibrarianByIdAsync(librarianId))
                .ReturnsAsync(librarian);

            // Act
            var result = await _librarianService.GetLibrarianByIdAsync(librarianId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(librarianId, result.LibrarianId);
            Assert.Equal(librarian.Name, result.Name);
        }

        [Fact]
        public async Task GetLibrarianByIdAsync_NonExistingLibrarianId_ShouldReturnNull()
        {
            // Arrange
            string nonExistingLibrarianId = "2";
            _librarianRepositoryMock.Setup(repo => repo.GetLibrarianByIdAsync(nonExistingLibrarianId))
                .ReturnsAsync((Librarian)null);

            // Act
            var result = await _librarianService.GetLibrarianByIdAsync(nonExistingLibrarianId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddLibrarianAsync_ReturnsAddedLibrarian()
        {
            // Arrange
            string librarianId = "1";
            string name = "John Doe";
            var librarianDto = new LibrarianDto { Name = name };
            var addedLibrarian = new Librarian { LibrarianId = librarianId, Name = name };
            _librarianRepositoryMock.Setup(repo => repo.AddLibrarianAsync(It.IsAny<Librarian>()))
                .ReturnsAsync(addedLibrarian);

            // Act
            var result = await _librarianService.AddLibrarianAsync(librarianId, name);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(librarianId, result.LibrarianId);
            Assert.Equal(name, result.Name);
        }

        [Fact]
        public async Task UpdateLibrarianAsync_ExistingLibrarianId_ShouldReturnUpdatedLibrarian()
        {
            // Arrange
            string librarianId = "1";
            var librarianDto = new LibrarianDto { Name = "Updated Name" };
            var existingLibrarian = new Librarian { LibrarianId = librarianId, Name = "Original Name" };
            _librarianRepositoryMock.Setup(repo => repo.GetLibrarianByIdAsync(librarianId))
                .ReturnsAsync(existingLibrarian);
            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(librarianId, librarianDto.Name, ""))
                .Returns(Task.CompletedTask);
            _librarianRepositoryMock.Setup(repo => repo.UpdateLibrarianAsync(existingLibrarian))
                .ReturnsAsync(existingLibrarian);

            // Act
            var result = await _librarianService.UpdateLibrarianAsync(librarianId, librarianDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(librarianId, result.LibrarianId);
            Assert.Equal(librarianDto.Name, result.Name);
        }

        [Fact]
        public async Task UpdateLibrarianAsync_NonExistingLibrarianId_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            string nonExistingLibrarianId = "2";
            var librarianDto = new LibrarianDto { Name = "Updated Name" };
            _librarianRepositoryMock.Setup(repo => repo.GetLibrarianByIdAsync(nonExistingLibrarianId))
                .ReturnsAsync((Librarian)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _librarianService.UpdateLibrarianAsync(nonExistingLibrarianId, librarianDto));
        }

        [Fact]
        public async Task DeleteLibrarianAsync_ExistingLibrarianId_ShouldDeleteLibrarian()
        {
            // Arrange
            string librarianId = "1";
            var existingLibrarian = new Librarian { LibrarianId = librarianId, Name = "John Doe" };
            _librarianRepositoryMock.Setup(repo => repo.GetLibrarianByIdAsync(librarianId))
                .ReturnsAsync(existingLibrarian);
            _librarianRepositoryMock.Setup(repo => repo.DeleteLibrarianAsync(existingLibrarian))
                .Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(repo => repo.DeleteUserAsync(librarianId))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await _librarianService.DeleteLibrarianAsync(librarianId);

            // Assert
            _librarianRepositoryMock.Verify(repo => repo.DeleteLibrarianAsync(existingLibrarian), Times.Once);
            _userRepositoryMock.Verify(repo => repo.DeleteUserAsync(librarianId), Times.Once);
        }

        [Fact]
        public async Task DeleteLibrarianAsync_NonExistingLibrarianId_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            string nonExistingLibrarianId = "2";
            _librarianRepositoryMock.Setup(repo => repo.GetLibrarianByIdAsync(nonExistingLibrarianId))
                .ReturnsAsync((Librarian)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _librarianService.DeleteLibrarianAsync(nonExistingLibrarianId));
        }
    }
}
