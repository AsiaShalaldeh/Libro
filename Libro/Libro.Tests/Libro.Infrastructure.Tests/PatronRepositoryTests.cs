using Libro.Domain.Entities;
using Libro.Infrastructure.Data;
using Libro.Infrastructure.Repositories;
using Libro.Tests.MockData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Libro.Tests.Libro.Infrastructure.Tests
{
    public class PatronRepositoryTests
    {
        private readonly PatronRepository _patronRepository;
        private readonly Mock<IConfiguration> _configuration;
        private readonly LibroDbContext _dbContext;

        public PatronRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibroDB")
                .Options;
            _configuration = new Mock<IConfiguration>();
            _dbContext = new LibroDbContext(options, _configuration.Object);
            PatronMockData.InitializeTestData(_dbContext);
            _patronRepository = new PatronRepository(_dbContext, Mock.Of<ILogger<PatronRepository>>());
        }

        [Fact]
        public async Task GetPatronByIdAsync_ValidId_ReturnsPatronWithRelatedEntities()
        {
            // Arrange
            var patronId = "1";
            var patron = PatronMockData.GetPatron(patronId);

            // Act
            var result = await _patronRepository.GetPatronByIdAsync(patronId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patron.PatronId, result.PatronId);
            Assert.Equal(patron.Name, result.Name);
            Assert.Equal(patron.Email, result.Email);

            Assert.Equal(patron.Reviews.Count, result.Reviews.Count);
            Assert.Equal(patron.Reviews.Select(r => r.ReviewId), result.Reviews.Select(r => r.ReviewId));

            Assert.Equal(patron.ReservedBooks.Count, result.ReservedBooks.Count);
            Assert.Equal(patron.ReservedBooks.Select(b => b.BookId), result.ReservedBooks.Select(b => b.BookId));

            Assert.Equal(patron.CheckedoutBooks.Count, result.CheckedoutBooks.Count);
            Assert.Equal(patron.CheckedoutBooks.Select(b => b.BookId), result.CheckedoutBooks.Select(b => b.BookId));

            Assert.Equal(patron.ReadingLists.Count, result.ReadingLists.Count);
            Assert.Equal(patron.ReadingLists.Select(rl => rl.ReadingListId), result.ReadingLists.Select(rl => rl.ReadingListId));
        }

        [Fact]
        public async Task AddPatronAsync_AddsNewPatronToDatabase()
        {
            // Arrange
            var patron = new Patron { PatronId = "3", Name = "Jane Smith", Email = "John@gmail.com"};

            // Act
            await _patronRepository.AddPatronAsync(patron);
            var result = await _dbContext.Patrons.FirstOrDefaultAsync(p => p.PatronId.Equals(patron.PatronId));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patron.PatronId, result.PatronId);
            Assert.Equal(patron.Name, result.Name);
        }

        [Fact]
        public async Task UpdatePatronAsync_UpdatesExistingPatronInDatabase()
        {
            // Arrange
            string patronId = "2";
            Patron patron = await _patronRepository.GetPatronByIdAsync(patronId);
            patron.Name = "Updated Patron";
            patron.Email = "person@gmail.com";

            // Act
            await _patronRepository.UpdatePatronAsync(patron);
            Patron updatedPatron = await _dbContext.Patrons.FirstOrDefaultAsync(p => p.PatronId.Equals(patronId));

            // Assert
            Assert.NotNull(updatedPatron);
            Assert.Equal(patronId, updatedPatron.PatronId);
            Assert.Equal("Updated Patron", updatedPatron.Name);
        }
        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
