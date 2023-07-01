using AutoMapper;
using Libro.Application.Services;
using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Tests.Libro.Application.Tests
{
    public class BookServiceTests
    {
        private readonly Mock<IAuthorRepository> _authorRepositoryMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<BookService>> _loggerMock;
        private readonly IBookService _bookService;

        public BookServiceTests()
        {
            _authorRepositoryMock = new Mock<IAuthorRepository>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<BookService>>();
            _bookService = new BookService(
                _bookRepositoryMock.Object,
                _mapperMock.Object,
                _authorRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetBookByISBNAsync_ExistingISBN_ShouldReturnBook()
        {
            // Arrange
            string ISBN = "1234567890";
            var expectedBook = new Book { ISBN = ISBN, Title = "Book Title" };
            _bookRepositoryMock.Setup(repo => repo.GetBookByISBNAsync(ISBN))
                .ReturnsAsync(expectedBook);

            // Act
            var result = await _bookService.GetBookByISBNAsync(ISBN);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ISBN, result.ISBN);
            Assert.Equal(expectedBook.Title, result.Title);
        }

        [Fact]
        public async Task GetBookByISBNAsync_NonExistingISBN_ShouldReturnNull()
        {
            // Arrange
            string nonExistingISBN = "9876543210";
            _bookRepositoryMock.Setup(repo => repo.GetBookByISBNAsync(nonExistingISBN))
                .ReturnsAsync((Book)null);

            // Act
            var result = await _bookService.GetBookByISBNAsync(nonExistingISBN);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnPaginatedResultOfBookDtos()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var paginatedResult = new PaginatedResult<Book>(
                new List<Book>
                {
                    new Book { ISBN = "1", Title = "Book 1" },
                    new Book { ISBN = "2", Title = "Book 2" }
                },
                2,
                pageNumber,
                pageSize
            );
            var expectedBookDtos = new List<BookDto>
            {
                new BookDto { ISBN = "1", Title = "Book 1" },
                new BookDto { ISBN = "2", Title = "Book 2" }
            };
            _bookRepositoryMock.Setup(repo => repo.GetAllBooksAsync(pageNumber, pageSize))
                .ReturnsAsync(paginatedResult);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<BookDto>>(paginatedResult.Items))
                .Returns(expectedBookDtos);

            // Act
            var result = await _bookService.GetAllBooksAsync(pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBookDtos, result.Items);
            Assert.Equal(paginatedResult.TotalCount, result.TotalCount);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
        }

        [Fact]
        public async Task SearchBooksAsync_ShouldReturnPaginatedResultOfBookDtos()
        {
            // Arrange
            string title = "Book";
            string author = "John Doe";
            string genre = "ScienceFiction";
            int pageNumber = 1;
            int pageSize = 10;
            var paginatedResult = new PaginatedResult<Book>(
                new List<Book>
                {
                    new Book { ISBN = "1", Title = "Book 1" },
                    new Book { ISBN = "2", Title = "Book 2" }
                },
                2,
                pageNumber,
                pageSize
            );
            var expectedBookDtos = new List<BookDto>
            {
                new BookDto { ISBN = "1", Title = "Book 1" },
                new BookDto { ISBN = "2", Title = "Book 2" }
            };
            _bookRepositoryMock.Setup(repo => repo.SearchBooksAsync(title, author, genre, pageNumber, pageSize))
                .ReturnsAsync(paginatedResult);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<BookDto>>(paginatedResult.Items))
                .Returns(expectedBookDtos);

            // Act
            var result = await _bookService.SearchBooksAsync(title, author, genre, pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBookDtos, result.Items);
            Assert.Equal(paginatedResult.TotalCount, result.TotalCount);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
        }

        [Fact]
        public async Task AddBookAsync_ExistingAuthorId_ShouldReturnBookDto()
        {
            // Arrange
            int authorId = 1;
            string ISBN = "1234567890";
            var bookRequest = new BookRequest
            {
                Title = "Book Title",
                AuthorId = authorId,
                Genre = "ScienceFiction",
                IsAvailable = true
            };
            var author = new Author { AuthorId = authorId, Name = "John Doe" };
            var book = new Book { ISBN = ISBN, Title = bookRequest.Title };
            var expectedBookDto = new BookDto { ISBN = ISBN, Title = bookRequest.Title };
            _authorRepositoryMock.Setup(repo => repo.GetAuthorByIdAsync(authorId))
                .ReturnsAsync(author);
            _mapperMock.Setup(mapper => mapper.Map<Book>(bookRequest))
                .Returns(book);
            _bookRepositoryMock.Setup(repo => repo.AddBookAsync(book, author))
                .Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map<BookDto>(book))
                .Returns(expectedBookDto);

            // Act
            var result = await _bookService.AddBookAsync(bookRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBookDto, result);
        }

        [Fact]
        public async Task AddBookAsync_NonExistingAuthorId_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            int nonExistingAuthorId = 2;
            var bookRequest = new BookRequest
            {
                Title = "Book Title",
                AuthorId = nonExistingAuthorId,
                Genre = "ScienceFiction",
                IsAvailable = true
            };
            _authorRepositoryMock.Setup(repo => repo.GetAuthorByIdAsync(nonExistingAuthorId))
                .ReturnsAsync((Author)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _bookService.AddBookAsync(bookRequest));
        }

        [Fact]
        public async Task UpdateBookAsync_ExistingISBN_ShouldUpdateBook()
        {
            // Arrange
            string ISBN = "1234567890";
            int authorId = 1;
            var bookRequest = new BookRequest
            {
                Title = "Updated Book Title",
                AuthorId = authorId,
                Genre = "ScienceFiction",
                IsAvailable = false
            };
            var existingBook = new Book
            {
                ISBN = ISBN,
                Title = "Book Title",
                Author = new Author { AuthorId = authorId, Name = "John Doe" },
                Genre = Genre.ScienceFiction,
                IsAvailable = true
            };
            var updatedBook = new Book
            {
                ISBN = ISBN,
                Title = bookRequest.Title,
                Author = existingBook.Author,
                Genre = Enum.Parse<Genre>(bookRequest.Genre, ignoreCase: true),
                IsAvailable = bookRequest.IsAvailable
            };
            _bookRepositoryMock.Setup(repo => repo.GetBookByISBNAsync(ISBN))
                .ReturnsAsync(existingBook);
            _authorRepositoryMock.Setup(repo => repo.GetAuthorByIdAsync(authorId))
                .ReturnsAsync(existingBook.Author);
            _bookRepositoryMock.Setup(repo => repo.UpdateBookAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _bookService.UpdateBookAsync(ISBN, bookRequest);

            // Assert
            Assert.Equal(bookRequest.Title, existingBook.Title);
            Assert.Equal(bookRequest.IsAvailable, existingBook.IsAvailable);
            Assert.Equal(Genre.ScienceFiction, existingBook.Genre);
            _bookRepositoryMock.Verify(repo => repo.UpdateBookAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task UpdateBookAsync_NonExistingISBN_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            string nonExistingISBN = "9876543210";
            var bookRequest = new BookRequest
            {
                Title = "Updated Book Title",
                AuthorId = 1,
                Genre = "Fiction",
                IsAvailable = false
            };
            _bookRepositoryMock.Setup(repo => repo.GetBookByISBNAsync(nonExistingISBN))
                .ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _bookService.UpdateBookAsync(nonExistingISBN, bookRequest));
        }

        [Fact]
        public async Task UpdateBookAsync_NonExistingAuthorId_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            string ISBN = "1234567890";
            int nonExistingAuthorId = 2;
            var bookRequest = new BookRequest
            {
                Title = "Updated Book Title",
                AuthorId = nonExistingAuthorId,
                Genre = "ScienceFiction",
                IsAvailable = false
            };
            var existingBook = new Book
            {
                ISBN = ISBN,
                Title = "Book Title",
                Author = new Author { AuthorId = 1, Name = "John Doe" },
                Genre = Genre.ScienceFiction,
                IsAvailable = true
            };
            _bookRepositoryMock.Setup(repo => repo.GetBookByISBNAsync(ISBN))
                .ReturnsAsync(existingBook);
            _authorRepositoryMock.Setup(repo => repo.GetAuthorByIdAsync(nonExistingAuthorId))
                .ReturnsAsync((Author)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _bookService.UpdateBookAsync(ISBN, bookRequest));
        }

        [Fact]
        public async Task RemoveBookAsync_ExistingISBN_ShouldRemoveBook()
        {
            // Arrange
            string ISBN = "1234567890";
            var existingBook = new Book { ISBN = ISBN, Title = "Book Title" };
            _bookRepositoryMock.Setup(repo => repo.GetBookByISBNAsync(ISBN))
                .ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(repo => repo.DeleteBookAsync(existingBook))
                .Returns(Task.CompletedTask);

            // Act
            await _bookService.RemoveBookAsync(ISBN);

            // Assert
            _bookRepositoryMock.Verify(repo => repo.DeleteBookAsync(existingBook), Times.Once);
        }

        [Fact]
        public async Task RemoveBookAsync_NonExistingISBN_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            string nonExistingISBN = "9876543210";
            _bookRepositoryMock.Setup(repo => repo.GetBookByISBNAsync(nonExistingISBN))
                .ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _bookService.RemoveBookAsync(nonExistingISBN));
        }
    }
}
