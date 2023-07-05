using Libro.Domain.Entities;
using Libro.Infrastructure.Data;
using Libro.Infrastructure.Repositories;
using Libro.Tests.MockData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Libro.Tests.Libro.Infrastructure.Tests
{
    public class TransactionRepositoryTests
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly Mock<IConfiguration> _configuration;
        private readonly LibroDbContext _dbContext;

        public TransactionRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibroDB")
                .Options;
            _configuration = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<TransactionRepository>>();
            _dbContext = new LibroDbContext(options, _configuration.Object);
            TransactionMockData.InitializeTestData(_dbContext);
            _transactionRepository = new TransactionRepository(_dbContext, loggerMock.Object);
        }

        [Fact]
        public async Task AddReservationAsync_ShouldAddReservationToDatabase()
        {
            // Arrange
            var reservation = new Reservation
            {
                ReservationId = "3",
                BookId = "1",
                PatronId = "2",
                ReservationDate = DateTime.Now
            };

            // Act
            var addedReservation = await _transactionRepository.AddReservationAsync(reservation);

            // Assert
            Assert.NotNull(addedReservation);
            Assert.Equal(reservation.ReservationId, addedReservation.ReservationId);
        }

        [Fact]
        public async Task AddCheckoutAsync_ShouldAddCheckoutToDatabase()
        {
            // Arrange
            var checkout = new Checkout
            {
                CheckoutId = "3",
                BookId = "1",
                PatronId = "2",
                CheckoutDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                IsReturned = false,
                ReturnDate = DateTime.MinValue,
                TotalFee = 0
            };

            // Act
            var addedCheckout = await _transactionRepository.AddCheckoutAsync(checkout);

            // Assert
            Assert.NotNull(addedCheckout);
            Assert.Equal(checkout.CheckoutId, addedCheckout.CheckoutId);
        }

        [Fact]
        public async Task UpdateCheckoutAsync_ShouldUpdateCheckoutInDatabase()
        {
            // Arrange
            Checkout checkout = await _dbContext.Checkouts.FindAsync("1");
            checkout.IsReturned = true;

            // Act
            await _transactionRepository.UpdateCheckoutAsync(checkout);

            // Assert
            // Retrieve the updated checkout from the database
            var updatedCheckout = await _dbContext.Checkouts.FindAsync(checkout.CheckoutId);

            Assert.NotNull(updatedCheckout);
            Assert.Equal(checkout.CheckoutId, updatedCheckout.CheckoutId);
            Assert.Equal(checkout.BookId, updatedCheckout.BookId);
            Assert.Equal(checkout.PatronId, updatedCheckout.PatronId);
            Assert.Equal(checkout.CheckoutDate, updatedCheckout.CheckoutDate);
            Assert.Equal(checkout.DueDate, updatedCheckout.DueDate);
            Assert.Equal(checkout.IsReturned, updatedCheckout.IsReturned);
            Assert.Equal(checkout.ReturnDate, updatedCheckout.ReturnDate);
            Assert.Equal(checkout.TotalFee, updatedCheckout.TotalFee);
        }

        [Fact]
        public async Task GetCheckoutTransactionsByPatronAsync_ShouldReturnCheckoutTransactionsForPatron()
        {
            // Arrange
            var patronId = "1";

            // Act
            var checkouts = await _transactionRepository.GetCheckoutTransactionsByPatronAsync(patronId);

            // Assert
            Assert.NotNull(checkouts);
            Assert.True(checkouts.All(c => c.PatronId == patronId));
            Assert.Equal(1, checkouts.Count());
        }

        [Fact]
        public async Task GetOverdueBookIdsAsync_ShouldReturnOverdueBookIds()
        {
            // Act
            var overdueBookIds = await _transactionRepository.GetOverdueBookIdsAsync();

            // Assert
            Assert.NotNull(overdueBookIds);
            Assert.Equal(1, overdueBookIds.Count());
        }

        [Fact]
        public async Task GetOverdueTransactionsAsync_ShouldReturnOverdueTransactions()
        {
            // Act
            var overdueTransactions = await _transactionRepository.GetOverdueTransactionsAsync();

            // Assert
            Assert.NotNull(overdueTransactions);
            Assert.Equal(1, overdueTransactions.Count());
        }

        [Fact]
        public async Task GetBorrowedBookIdsAsync_ShouldReturnBorrowedBookIds()
        {
            // Act
            var borrowedBookIds = await _transactionRepository.GetBorrowedBookIdsAsync();

            // Assert
            Assert.NotNull(borrowedBookIds);
            Assert.Equal(2, borrowedBookIds.Count());
        }

        [Fact]
        public async Task GetBorrowedBookByIdAsync_ShouldReturnBorrowedBookId()
        {
            // Arrange
            var isbn = "1";

            // Act
            var borrowedBookId = await _transactionRepository.GetBorrowedBookByIdAsync(isbn);

            // Assert
            Assert.NotNull(borrowedBookId);
            Assert.Equal(isbn, borrowedBookId);
        }

        // Clean up the in-memory database after each test
        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
