using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Libro.Infrastructure.Repositories;
using Libro.Tests.MockData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Libro.Tests.Libro.Infrastructure.Tests
{
    public class BookQueueRepositoryTests
    {
        private IBookQueueRepository _bookQueueRepository;
        private readonly Mock<IConfiguration> _configuration;
        private LibroDbContext _dbContext;
        private ILogger<BookQueueRepository> _logger;

        public BookQueueRepositoryTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibroDB")
                .Options;
            _configuration = new Mock<IConfiguration>();
            _dbContext = new LibroDbContext(optionsBuilder, _configuration.Object);
            var loggerMock = new Mock<ILogger<BookQueueRepository>>();
            _logger = loggerMock.Object;

            _bookQueueRepository = new BookQueueRepository(_dbContext, _logger);
            BookQueueMockData.InitializeTestData(_dbContext);
        }

        [Fact]
        public async Task GetAllBookQueuesAsync_ReturnsAllBookQueues()
        {
            // Act
            var bookQueues = await _bookQueueRepository.GetAllBookQueuesAsync();

            // Assert
            Assert.Equal(2, bookQueues.Count);
        }

        [Fact]
        public async Task GetBookQueuesByBookIdAsync_ReturnsBookQueuesForBookId()
        {
            // Arrange
            string bookId = "1";

            // Act
            var bookQueues = await _bookQueueRepository.GetBookQueuesByBookIdAsync(bookId);

            // Assert
            Assert.Equal(2, bookQueues.Count);
            Assert.True(bookQueues.All(q => q.BookId == bookId));
        }

        [Fact]
        public async Task EnqueuePatronAsync_EnqueuesPatronInBookQueue()
        {
            // Arrange
            string bookId = "1";
            string patronId = "3";

            // Act
            var bookQueue = await _bookQueueRepository.EnqueuePatronAsync(bookId, patronId);

            // Assert
            Assert.NotNull(bookQueue);
            Assert.Equal(bookId, bookQueue.BookId);
            Assert.Equal(patronId, bookQueue.PatronId);
            Assert.Equal(3, bookQueue.QueuePosition);
        }

        [Fact]
        public async Task DequeuePatronAsync_DequeuesPatronFromBookQueue()
        {
            // Arrange
            string bookId = "1";

            // Act
            var dequeuedBookQueue = await _bookQueueRepository.DequeuePatronAsync(bookId);
            var updatedBookQueues = await _bookQueueRepository.GetBookQueuesByBookIdAsync(bookId);

            // Assert
            Assert.NotNull(dequeuedBookQueue);
            Assert.Equal(bookId, dequeuedBookQueue.BookId);
            Assert.Equal(1, dequeuedBookQueue.QueuePosition);
            Assert.Single(updatedBookQueues);
        }

        [Fact]
        public async Task PeekPatronAsync_ReturnsFirstPatronInBookQueue()
        {
            // Arrange
            string bookId = "1"; // if bookId not exist returns message

            // Act
            var firstInQueue = await _bookQueueRepository.PeekPatronAsync(bookId);

            // Assert
            Assert.NotNull(firstInQueue);
            Assert.Equal(bookId, firstInQueue.BookId);
            Assert.Equal(1, firstInQueue.QueuePosition);
        }

        [Fact]
        public async Task GetQueueLengthAsync_ReturnsQueueLengthForBook()
        {
            // Arrange
            string bookId = "1";

            // Act
            var queueLength = await _bookQueueRepository.GetQueueLengthAsync(bookId);

            // Assert
            Assert.Equal(2, queueLength);
        }
    }
}
