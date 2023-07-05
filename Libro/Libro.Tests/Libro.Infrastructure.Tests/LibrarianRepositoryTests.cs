using Libro.Domain.Entities;
using Libro.Infrastructure.Data;
using Libro.Tests.MockData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Libro.Infrastructure.Repositories.Tests
{
    public class LibrarianRepositoryTests
    {
        private readonly LibrarianRepository _librarianRepository;
        private readonly Mock<IConfiguration> _configuration;
        private readonly LibroDbContext _dbContext;

        public LibrarianRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibroDB")
                .Options;
            _configuration = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<LibrarianRepository>>();
            _dbContext = new LibroDbContext(options, _configuration.Object);

            LibrarianMockData.InitializeTestData(_dbContext);
            _librarianRepository = new LibrarianRepository(_dbContext, loggerMock.Object);
        }

        [Fact]
        public async Task GetAllLibrariansAsync_ReturnsAllLibrarians()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            int total = _dbContext.Librarians.Count();

            // Act
            var result = await _librarianRepository.GetAllLibrariansAsync(pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(total, result.TotalCount);
            Assert.Equal(_dbContext.Librarians, result.Items);
        }

        [Fact]
        public async Task GetLibrarianByIdAsync_ValidId_ReturnsLibrarian()
        {
            // Arrange
            string librarianId = "1";

            // Act
            Librarian librarian = await _librarianRepository.GetLibrarianByIdAsync(librarianId);

            // Assert
            Assert.NotNull(librarian);
            Assert.Equal(librarianId, librarian.LibrarianId);
        }

        [Fact]
        public async Task AddLibrarianAsync_AddsNewLibrarianToDatabase()
        {
            // Arrange
            var librarian = new Librarian { LibrarianId = "4", Name = "Librarian 4" };

            // Act
            var result = await _librarianRepository.AddLibrarianAsync(librarian);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(librarian.LibrarianId, result.LibrarianId);

            // Check if the librarian is actually added to the database
            var addedLibrarian = await _librarianRepository.GetLibrarianByIdAsync(librarian.LibrarianId);
            Assert.NotNull(addedLibrarian);
            Assert.Equal(librarian.Name, addedLibrarian.Name);
        }

        [Fact]
        public async Task UpdateLibrarianAsync_UpdatesExistingLibrarianInDatabase()
        {
            // Arrange
            string librarianId = "1";
            var librarian = await _librarianRepository.GetLibrarianByIdAsync(librarianId);
            librarian.Name = "Updated Librarian";

            // Act
            await _librarianRepository.UpdateLibrarianAsync(librarian);
            var updatedLibrarian = await _librarianRepository.GetLibrarianByIdAsync(librarianId);

            // Assert
            Assert.NotNull(updatedLibrarian);
            Assert.Equal(librarianId, updatedLibrarian.LibrarianId);
            Assert.Equal("Updated Librarian", updatedLibrarian.Name);
        }

        [Fact]
        public async Task DeleteLibrarianAsync_DeletesLibrarianFromDatabase()
        {
            // Arrange
            string librarianId = "1";
            var librarian = await _librarianRepository.GetLibrarianByIdAsync(librarianId);

            // Act
            await _librarianRepository.DeleteLibrarianAsync(librarian);
            var deletedLibrarian = await _librarianRepository.GetLibrarianByIdAsync(librarianId);

            // Assert
            Assert.Null(deletedLibrarian);
        }

        public void Dispose()
        {
            // Clean up the in-memory database after each test
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
