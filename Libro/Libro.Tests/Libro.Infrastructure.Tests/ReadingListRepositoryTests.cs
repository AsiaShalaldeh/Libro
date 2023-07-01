using Libro.Domain.Entities;
using Libro.Infrastructure.Data;
using Libro.Infrastructure.Repositories;
using Libro.Tests.MockData;
using Microsoft.EntityFrameworkCore;

namespace Libro.Tests.Libro.Infrastructure.Tests
{
    public class ReadingListRepositoryTests
    {
        private readonly ReadingListRepository _readingListRepository;
        private readonly LibroDbContext _dbContext;

        public ReadingListRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibroDB")
                .Options;
            var loggerMock = new Mock<ILogger<ReadingListRepository>>();
            _dbContext = new LibroDbContext(options);

            ReadingListMockData.InitializeTestData(_dbContext);
            _readingListRepository = new ReadingListRepository(_dbContext, loggerMock.Object);
        }

        [Fact]
        public async Task GetReadingListByIdAsync_ValidId_ReturnsReadingList()
        {
            // Arrange
            int listId = 1;
            string patronId = "123";

            // Act
            ReadingList readingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);

            // Assert
            Assert.NotNull(readingList);
            Assert.Equal(listId, readingList.ReadingListId);
            Assert.Equal(patronId, readingList.PatronId);
        }

        [Fact]
        public async Task GetReadingListsByPatronIdAsync_ValidPatronId_ReturnsReadingLists()
        {
            // Arrange
            string patronId = "123";

            // Act
            var readingLists = await _readingListRepository.GetReadingListsByPatronIdAsync(patronId);

            // Assert
            Assert.NotNull(readingLists);
            Assert.NotEmpty(readingLists);
            Assert.All(readingLists, rl => Assert.Equal(patronId, rl.PatronId));
            Assert.Equal(2, readingLists.Count());
        }

        [Fact]
        public async Task CreateReadingListAsync_CreatesNewReadingList()
        {
            // Arrange
            var readingList = new ReadingList { ReadingListId = 3, PatronId = "456", Name = "Classics" };

            // Act
            var result = await _readingListRepository.CreateReadingListAsync(readingList);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(readingList.ReadingListId, result.ReadingListId);

            // Check if the reading list is actually added to the database
            var addedReadingList = await _readingListRepository.GetReadingListByIdAsync(readingList.ReadingListId, readingList.PatronId);
            Assert.NotNull(addedReadingList);
            Assert.Equal(readingList.PatronId, addedReadingList.PatronId);
        }

        [Fact]
        public async Task RemoveReadingListAsync_RemovesReadingListFromDatabase()
        {
            // Arrange
            int listId = 1;
            string patronId = "123";
            var readingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);

            // Act
            await _readingListRepository.RemoveReadingListAsync(readingList);
            var deletedReadingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);

            // Assert
            Assert.Null(deletedReadingList);
        }

        [Fact]
        public async Task GetBooksByReadingListAsync_ValidReadingList_ReturnsBooks()
        {
            // Arrange
            int listId = 1;
            string patronId = "123";
            var readingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);

            // Act // no need to pass patron id here 
            var books = await _readingListRepository.GetBooksByReadingListAsync(readingList);

            // Assert
            Assert.NotNull(books);
            Assert.Equal(2, books.Count());
        }

        [Fact]
        public async Task AddBookToReadingListAsync_AddsBookToList()
        {
            // Arrange
            int listId = 1;
            string patronId = "123";
            var readingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);
            var bookList = new BookList { BookId = "9780487654321", ReadingListId = listId };

            // Act
            await _readingListRepository.AddBookToReadingListAsync(readingList, bookList);
            var updatedReadingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);

            // Assert
            Assert.NotNull(updatedReadingList);
            Assert.Contains(updatedReadingList.BookLists, bl => bl.BookId.Equals(bookList.BookId));
        }

        [Fact]
        public async Task RemoveBookFromReadingListAsync_RemovesBookFromList()
        {
            // Arrange
            int listId = 1;
            string patronId = "123";
            var readingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);
            string bookId = "9781234567890";

            // Act
            await _readingListRepository.RemoveBookFromReadingListAsync(readingList, bookId);
            var updatedReadingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);

            // Assert
            Assert.NotNull(updatedReadingList);
            Assert.DoesNotContain(updatedReadingList.BookLists, bl => bl.BookId == bookId);
        }

        // Clean up the in-memory database after each test
        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
