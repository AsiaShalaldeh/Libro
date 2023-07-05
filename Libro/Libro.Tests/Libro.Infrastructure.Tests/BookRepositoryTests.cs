using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Infrastructure.Data;
using Libro.Infrastructure.Repositories;
using Libro.Tests.MockData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Libro.Tests.Libro.Infrastructure.Tests
{
    public class BookRepositoryTests 
    {
        private readonly BookRepository _bookRepository;
        private readonly Mock<IConfiguration> _configuration;
        private readonly LibroDbContext _dbContext;

        public BookRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibroDB")
                .Options;
            _configuration = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<BookRepository>>();
            _dbContext = new LibroDbContext(options, _configuration.Object);
            BookMockData.InitializeTestData(_dbContext);
            _bookRepository = new BookRepository(_dbContext, loggerMock.Object);
        }

        [Fact]
        public async Task GetBookByISBNAsync_ValidISBN_ReturnsBook()
        {
            // Arrange
            string isbn = "1234567890";

            // Act
            Book book = await _bookRepository.GetBookByISBNAsync(isbn);

            // Assert
            Assert.NotNull(book);
            Assert.Equal(isbn, book.ISBN);
        }

        [Fact]
        public async Task GetBookByISBNAsync_InvalidISBN_ReturnsNull()
        {
            // Arrange
            string isbn = "0000000000";

            // Act
            Book book = await _bookRepository.GetBookByISBNAsync(isbn);

            // Assert
            Assert.Null(book);
        }

        [Fact]
        public async Task SearchBooksAsync_ReturnsMatchingBooks()
        {
            // Arrange
            string title = "Book 1";
            string author = "John";
            string genre = "ScienceFiction";
            int pageNumber = 1;
            int pageSize = 10;

            // Act
            var result = await _bookRepository.SearchBooksAsync(title, author, genre, pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Items.Count() > 0);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
        }

        [Fact]
        public async Task GetAllBooksAsync_ReturnsAllBooks()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;

            // Act
            var result = await _bookRepository.GetAllBooksAsync(pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Items.Count() > 0);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
        }

        [Fact]
        public async Task AddBookAsync_AddsNewBookToDatabase()
        {
            // Arrange
            var book = new Book
            {
                ISBN = "9876543210",
                Title = "New Book",
                Author = new Author { Name = "New Author" },
                Genre = Genre.Romance
            };

            // Act
            await _bookRepository.AddBookAsync(book, book.Author);
            var addedBook = await _bookRepository.GetBookByISBNAsync(book.ISBN);

            // Assert
            Assert.NotNull(addedBook);
            Assert.Equal(book.ISBN, addedBook.ISBN);
            Assert.Equal(book.Title, addedBook.Title);
            Assert.Equal(book.Author.Name, addedBook.Author.Name);
            Assert.Equal(book.Genre, addedBook.Genre);
        }

        [Fact]
        public async Task UpdateBookAsync_UpdatesExistingBookInDatabase()
        {
            // Arrange
            string isbn = "1234027890";
            var book = await _bookRepository.GetBookByISBNAsync(isbn);
            book.Title = "Updated Book";

            // Act
            await _bookRepository.UpdateBookAsync(book);
            var updatedBook = await _bookRepository.GetBookByISBNAsync(isbn);

            // Assert
            Assert.NotNull(updatedBook);
            Assert.Equal(isbn, updatedBook.ISBN);
            Assert.Equal("Updated Book", updatedBook.Title);
        }

        [Fact]
        public async Task DeleteBookAsync_DeletesBookFromDatabase()
        {
            // Arrange
            string isbn = "1234567890";
            var book = await _bookRepository.GetBookByISBNAsync(isbn);

            // Act
            await _bookRepository.DeleteBookAsync(book);
            var deletedBook = await _bookRepository.GetBookByISBNAsync(isbn);

            // Assert
            Assert.Null(deletedBook);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
