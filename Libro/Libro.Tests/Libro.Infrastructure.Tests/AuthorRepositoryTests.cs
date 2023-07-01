using Libro.Domain.Entities;
using Libro.Infrastructure.Data;
using Libro.Infrastructure.Tests.MockData;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Repositories.Tests
{
    public class AuthorRepositoryTests
    {
        private readonly AuthorRepository _authorRepository;
        private readonly LibroDbContext _dbContext;
        public readonly DbContextOptions<LibroDbContext> dbContextOptions;

        public AuthorRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<LibroDbContext>()
            .UseInMemoryDatabase(databaseName: "TestLibroDB")
            .Options;
            var loggerMock = new Mock<ILogger<AuthorRepository>>();

            _dbContext = new LibroDbContext(options);
            AuthorMockData.InitializeTestData(_dbContext);
            _authorRepository = new AuthorRepository(_dbContext, loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAuthorsAsync_ReturnsAllAuthors()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            int total = _dbContext.Authors.Count();
            var authors = _dbContext.Authors;

            // Act
            var result = await _authorRepository.GetAllAuthorsAsync(pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(total, result.TotalCount);
            Assert.Equal(authors, result.Items);
        }

        [Fact]
        public async Task GetAuthorByIdAsync_ValidId_ReturnsAuthorWithBooks()
        {
            // Arrange
            int authorId = 1;

            // Act
            Author author = await _authorRepository.GetAuthorByIdAsync(authorId);

            // Assert
            Assert.NotNull(author);
            Assert.Equal(authorId, author.AuthorId);
        }

        [Fact]
        public async Task AddAuthorAsync_AddsNewAuthorToDatabase()
        {
            // Arrange
            var author = new Author { AuthorId = 4, Name = "Author 4" };

            // Act
            var result = await _authorRepository.AddAuthorAsync(author);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(author.AuthorId, result.AuthorId);
        }

        [Fact]
        public async Task UpdateAuthorAsync_UpdatesExistingAuthorInDatabase()
        {
            // Arrange
            int authorId = 1;
            var author = await _authorRepository.GetAuthorByIdAsync(authorId);
            author.Name = "Updated Author";

            // Act
            await _authorRepository.UpdateAuthorAsync(author);
            var updatedAuthor = await _authorRepository.GetAuthorByIdAsync(authorId);

            // Assert
            Assert.NotNull(updatedAuthor);
            Assert.Equal(authorId, updatedAuthor.AuthorId);
            Assert.Equal("Updated Author", updatedAuthor.Name);
        }

        [Fact]
        public async Task DeleteAuthorAsync_DeletesAuthorFromDatabase()
        {
            // Arrange
            int authorId = 1;
            var author = await _authorRepository.GetAuthorByIdAsync(authorId);

            // Act
            await _authorRepository.DeleteAuthorAsync(author);
            var deletedAuthor = await _authorRepository.GetAuthorByIdAsync(authorId);

            // Assert
            Assert.Null(deletedAuthor);
        }

        public void Dispose()
        {
            // Clean up the in-memory database after each test
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
