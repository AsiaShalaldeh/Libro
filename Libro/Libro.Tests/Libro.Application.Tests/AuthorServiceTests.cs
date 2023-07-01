using AutoMapper;
using Libro.Application.Services;
using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Tests.Libro.Application.Tests
{
    public class AuthorServiceTests
    {
        private readonly Mock<IAuthorRepository> _authorRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<AuthorService>> _loggerMock;
        private readonly IAuthorService _authorService;

        public AuthorServiceTests()
        {
            _authorRepositoryMock = new Mock<IAuthorRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<AuthorService>>();
            _authorService = new AuthorService(
                _authorRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAllAuthorsAsync_ReturnsPaginatedResultOfAuthors()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var paginatedResult = new PaginatedResult<Author>(
                new List<Author> { new Author { AuthorId = 1, Name = "John Doe" } },
                1,
                pageNumber,
                pageSize
            );
            _authorRepositoryMock.Setup(repo => repo.GetAllAuthorsAsync(pageNumber, pageSize))
                .ReturnsAsync(paginatedResult);

            //_mapperMock.Setup(m => m.Map<IEnumerable<Author>, List<Author>>(It.IsAny<IEnumerable<Author>>()))
            //        .Returns((IEnumerable<Author> Items) => Items);

            // Act
            var result = await _authorService.GetAllAuthorsAsync(pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginatedResult.Items, result.Items);
            Assert.Equal(paginatedResult.TotalCount, result.TotalCount);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
        }

        [Fact]
        public async Task GetAuthorByIdAsync_ExistingAuthorId_ReturnsAuthor()
        {
            // Arrange
            int authorId = 1;
            var author = new Author { AuthorId = authorId, Name = "John Doe" };
            _authorRepositoryMock.Setup(repo => repo.GetAuthorByIdAsync(authorId))
                .ReturnsAsync(author);

            // Act
            var result = await _authorService.GetAuthorByIdAsync(authorId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(authorId, result.AuthorId);
            Assert.Equal(author.Name, result.Name);
        }

        [Fact]
        public async Task GetAuthorByIdAsync_NonExistingAuthorId_ShouldReturnNull()
        {
            // Arrange
            int nonExistingAuthorId = 2;
            _authorRepositoryMock.Setup(repo => repo.GetAuthorByIdAsync(nonExistingAuthorId))
                .ReturnsAsync((Author)null);

            // Act
            Author result = await _authorService.GetAuthorByIdAsync(nonExistingAuthorId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAuthorAsync_ValidAuthorDto_ReturnsAddedAuthor()
        {
            // Arrange
            var authorDto = new AuthorDto { Name = "John Doe" };
            var author = new Author { AuthorId = 1, Name = "John Doe" };
            _mapperMock.Setup(mapper => mapper.Map<Author>(authorDto))
                .Returns(author);
            _authorRepositoryMock.Setup(repo => repo.AddAuthorAsync(author))
                .ReturnsAsync(author);

            // Act
            var result = await _authorService.AddAuthorAsync(authorDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(author.AuthorId, result.AuthorId);
            Assert.Equal(author.Name, result.Name);
        }

        [Fact]
        public async Task UpdateAuthorAsync_ExistingAuthorId_ShouldUpdateAuthor()
        {
            // Arrange
            int authorId = 1;
            var authorDto = new AuthorDto { Name = "John Doe" };
            var existingAuthor = new Author { AuthorId = authorId, Name = "Old Name" };
            _authorRepositoryMock.Setup(repo => repo.GetAuthorByIdAsync(authorId))
                .ReturnsAsync(existingAuthor);

            // Act
            await _authorService.UpdateAuthorAsync(authorId, authorDto);

            // Assert
            Assert.Equal(authorDto.Name, existingAuthor.Name);
            _authorRepositoryMock.Verify(repo => repo.UpdateAuthorAsync(existingAuthor), Times.Once);
        }

        [Fact]
        public async Task UpdateAuthorAsync_NonExistingAuthorId_ThrowsResourceNotFoundException()
        {
            // Arrange
            int authorId = 1;
            var authorDto = new AuthorDto { Name = "John Doe" };
            _authorRepositoryMock.Setup(repo => repo.GetAuthorByIdAsync(authorId))
                .ReturnsAsync((Author)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _authorService.UpdateAuthorAsync(authorId, authorDto));
        }

        [Fact]
        public async Task DeleteAuthorAsync_ExistingAuthorId_ShouldDeleteAuthor()
        {
            // Arrange
            int authorId = 1;
            var existingAuthor = new Author { AuthorId = authorId, Name = "John Doe" };
            _authorRepositoryMock.Setup(repo => repo.GetAuthorByIdAsync(authorId))
                .ReturnsAsync(existingAuthor);

            // Act
            await _authorService.DeleteAuthorAsync(authorId);

            // Assert
            _authorRepositoryMock.Verify(repo => repo.DeleteAuthorAsync(existingAuthor), Times.Once);
        }

        [Fact]
        public async Task DeleteAuthorAsync_NonExistingAuthorId_ThrowsResourceNotFoundException()
        {
            // Arrange
            int authorId = 1;
            _authorRepositoryMock.Setup(repo => repo.GetAuthorByIdAsync(authorId))
                .ReturnsAsync((Author)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _authorService.DeleteAuthorAsync(authorId));
        }

        // Clean up resources after each test
        public void Dispose()
        {
            _authorRepositoryMock.Reset();
            _mapperMock.Reset();
            _loggerMock.Reset();
        }
    }
}
