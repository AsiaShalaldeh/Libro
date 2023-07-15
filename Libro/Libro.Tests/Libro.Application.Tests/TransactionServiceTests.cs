using AutoMapper;
using Libro.Application.Services;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Libro.Domain.Models;

namespace Libro.Tests.Libro.Application.Tests
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<ILoanPolicyService> _loanPolicyServiceMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TransactionService>> _loggerMock;
        private readonly Mock<IPatronService> _patronServiceMock;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _loanPolicyServiceMock = new Mock<ILoanPolicyService>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TransactionService>>();
            _patronServiceMock = new Mock<IPatronService>();

            _transactionService = new TransactionService(
                _transactionRepositoryMock.Object,
                _loanPolicyServiceMock.Object,
                _bookRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _patronServiceMock.Object
            );
        }

        [Fact]
        public async Task GetCheckoutTransactionsByPatron_ValidPatronId_ReturnsTransactions()
        {
            // Arrange
            string patronId = "patron1";
            var expectedTransactions = new List<Checkout>
            {
                new Checkout { CheckoutId = "checkout1" },
                new Checkout { CheckoutId = "checkout2" }
            };

            _transactionRepositoryMock.Setup(repo => repo.GetCheckoutTransactionsByPatronAsync(patronId))
                .ReturnsAsync(expectedTransactions);

            // Act
            var transactions = await _transactionService.GetCheckoutTransactionsByPatron(patronId);

            // Assert
            Assert.Equal(expectedTransactions, transactions);
            Assert.Equal(2, transactions.Count());
        }

        [Fact]
        public async Task GetCheckoutTransactionsByPatron_InvalidPatronId_ThrowsArgumentException()
        {
            // Arrange
            string invalidPatronId = "1234";
            _transactionRepositoryMock.Setup(repo => repo.GetCheckoutTransactionsByPatronAsync(invalidPatronId))
                .ThrowsAsync(new ResourceNotFoundException("Patron", "ID", invalidPatronId));

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(async () =>
            {
                await _transactionService.GetCheckoutTransactionsByPatron(invalidPatronId);
            });
        }

        [Fact]
        public async Task GetOverdueBooksAsync_OverdueBookIdsExist_ReturnsOverdueBooks()
        {
            // Arrange
            var overdueBookIds = new List<string> { "book1", "book2" };
            var expectedOverdueBooks = new List<Book>
            {
                new Book { ISBN = "book1" },
                new Book { ISBN = "book2" }
            };

            _transactionRepositoryMock.Setup(repo => repo.GetOverdueBookIdsAsync())
                .ReturnsAsync(overdueBookIds);

            _bookRepositoryMock.Setup(service => service.GetBookByISBNAsync(It.IsAny<string>()))
                .ReturnsAsync((string isbn) => expectedOverdueBooks.FirstOrDefault(book => book.ISBN == isbn));

            // Act
            var overdueBooks = await _transactionService.GetOverdueBooksAsync();

            // Assert
            Assert.Equal(expectedOverdueBooks, overdueBooks);
        }

        [Fact]
        public async Task GetOverdueBooksAsync_NoOverdueBookIds_ReturnsEmptyList()
        {
            // Arrange
            var overdueBookIds = new List<string>();
            var expectedOverdueBooks = new List<Book>();

            _transactionRepositoryMock.Setup(repo => repo.GetOverdueBookIdsAsync())
                .ReturnsAsync(overdueBookIds);

            // Act
            var overdueBooks = await _transactionService.GetOverdueBooksAsync();

            // Assert
            Assert.Equal(expectedOverdueBooks, overdueBooks);
        }

        [Fact]
        public async Task GetOverdueTransactionsAsync_ValidTransactionsExist_ReturnsTransactions()
        {
            // Arrange
            var expectedTransactions = new List<Checkout>
            {
                new Checkout { CheckoutId = "checkout1" },
                new Checkout { CheckoutId = "checkout2" }
            };

            _transactionRepositoryMock.Setup(repo => repo.GetOverdueTransactionsAsync())
                .ReturnsAsync(expectedTransactions);

            // Act
            var transactions = await _transactionService.GetOverdueTransactionsAsync();

            // Assert
            Assert.Equal(expectedTransactions, transactions);
        }

        [Fact]
        public async Task GetOverdueTransactionsAsync_NoTransactionsExist_ReturnsEmptyList()
        {
            // Arrange
            var expectedTransactions = new List<Checkout>();

            _transactionRepositoryMock.Setup(repo => repo.GetOverdueTransactionsAsync())
                .ReturnsAsync(expectedTransactions);

            // Act
            var transactions = await _transactionService.GetOverdueTransactionsAsync();

            // Assert
            Assert.Equal(expectedTransactions, transactions);
        }

        [Fact]
        public async Task GetBorrowedBooksAsync_BorrowedBookIdsExist_ReturnsBorrowedBooks()
        {
            // Arrange
            var borrowedBookIds = new List<string> { "book1", "book2" };
            var expectedBorrowedBooks = new List<Book>
            {
                new Book { ISBN = "book1" },
                new Book { ISBN = "book2" }
            };

            _transactionRepositoryMock.Setup(repo => repo.GetBorrowedBookIdsAsync())
                .ReturnsAsync(borrowedBookIds);

            _bookRepositoryMock.Setup(service => service.GetBookByISBNAsync(It.IsAny<string>()))
                .ReturnsAsync((string isbn) => expectedBorrowedBooks.FirstOrDefault(book => book.ISBN == isbn));

            // Act
            var borrowedBooks = await _transactionService.GetBorrowedBooksAsync();

            // Assert
            Assert.Equal(expectedBorrowedBooks, borrowedBooks);
        }

        [Fact]
        public async Task GetBorrowedBooksAsync_NoBorrowedBookIds_ReturnsEmptyList()
        {
            // Arrange
            var borrowedBookIds = new List<string>();
            var expectedBorrowedBooks = new List<Book>();

            _transactionRepositoryMock.Setup(repo => repo.GetBorrowedBookIdsAsync())
                .ReturnsAsync(borrowedBookIds);

            // Act
            var borrowedBooks = await _transactionService.GetBorrowedBooksAsync();

            // Assert
            Assert.Equal(expectedBorrowedBooks, borrowedBooks);
        }

        [Fact]
        public async Task GetBorrowedBookByIdAsync_ValidISBN_ReturnsBorrowedBook()
        {
            // Arrange
            string isbn = "book1";
            var borrowedBookId = "book1";
            var expectedBorrowedBook = new Book { ISBN = "book1" };

            _transactionRepositoryMock.Setup(repo => repo.GetBorrowedBookByIdAsync(isbn))
                .ReturnsAsync(borrowedBookId);

            _bookRepositoryMock.Setup(service => service.GetBookByISBNAsync(borrowedBookId))
                .ReturnsAsync(expectedBorrowedBook);

            // Act
            var borrowedBook = await _transactionService.GetBorrowedBookByIdAsync(isbn);

            // Assert
            Assert.Equal(expectedBorrowedBook, borrowedBook);
        }

        [Fact]
        public async Task GetBorrowedBookByIdAsync_InvalidISBN_ReturnsNull()
        {
            // Arrange
            string isbn = "1234567890";

            // Act
            var borrowedBook = await _transactionService.GetBorrowedBookByIdAsync(isbn);

            // Assert
            Assert.Null(borrowedBook);
        }

        [Fact]
        public async Task ReserveBookAsync_ValidBookAndPatron_ReturnsReservationDto()
        {
            // Arrange
            var book = new Book { ISBN = "book1" };
            var patron = new Patron { PatronId = "patron1" };
            var expectedReservation = new Reservation { ReservationId = "reservation1" };
            var expectedReservationDto = new ReservationDto { ReservationId = "reservation1" };

            _transactionRepositoryMock.Setup(repo => repo.AddReservationAsync(It.IsAny<Reservation>()))
                .ReturnsAsync(expectedReservation);

            _mapperMock.Setup(mapper => mapper.Map<ReservationDto>(expectedReservation))
                .Returns(expectedReservationDto);

            // Act
            var reservationDto = await _transactionService.ReserveBookAsync(book, patron);

            // Assert
            Assert.Equal(expectedReservationDto, reservationDto);
        }

        [Fact]
        public async Task CheckoutBookAsync_ValidBookAndPatron_ReturnsTransactionResponseModel()
        {
            // Arrange
            var book = new Book { ISBN = "book1" };
            var patron = new Patron { PatronId = "patron1" };
            var expectedCheckout = new Checkout { CheckoutId = "checkout1" };
            var expectedTransactionResponseModel = new TransactionResponseModel { BookId = "book1" };

            _transactionRepositoryMock.Setup(repo => repo.AddCheckoutAsync(It.IsAny<Checkout>()))
                .ReturnsAsync(expectedCheckout);

            _loanPolicyServiceMock.Setup(service => service.GetLoanDuration())
                .Returns(14);

            _bookRepositoryMock.Setup(repo => repo.UpdateBookStatus(book.ISBN, false))
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(mapper => mapper.Map<TransactionResponseModel>(expectedCheckout))
                .Returns(expectedTransactionResponseModel);

            // Act
            var transactionResponseModel = await _transactionService.CheckoutBookAsync(book, patron);

            // Assert
            Assert.Equal(expectedTransactionResponseModel, transactionResponseModel);
        }

        [Fact]
        public async Task ReturnBookAsync_ValidBook_ReturnsTransactionResponseModel()
        {
            // Arrange
            var book = new Book { ISBN = "book1" };
            var patron = new Patron { PatronId = "patron1", Email = "patron1@gmail.com" };
            var checkout = new Checkout
            {
                BookId = book.ISBN,
                PatronId = patron.PatronId,
                CheckoutDate = DateTime.UtcNow.AddDays(-7),
                DueDate = DateTime.UtcNow.AddDays(-2),
                IsReturned = false,
                ReturnDate = DateTime.MinValue, 
                TotalFee = 0 
            };
            var expectedReturn = new Checkout { CheckoutId = "checkout1" };
            var expectedTransactionResponseModel = new TransactionResponseModel
            {
                BookId = book.ISBN,
                PatronId = patron.PatronId,
                CheckoutDate = checkout.CheckoutDate,
                DueDate = checkout.DueDate,
                ReturnDate = expectedReturn.ReturnDate,
                TotalFee = expectedReturn.TotalFee
            };

            _bookRepositoryMock.Setup(service => service.GetBookByISBNAsync(book.ISBN))
                .ReturnsAsync(book);

            _patronServiceMock.Setup(service => service.GetPatronAsync(patron.PatronId))
                .ReturnsAsync(patron);

            _transactionRepositoryMock.Setup(repo => repo.UpdateCheckoutAsync(It.IsAny<Checkout>()))
            .Returns(Task.FromResult(expectedReturn));

            _transactionRepositoryMock.Setup(repo => repo.GetBorrowedBookByIdAsync(book.ISBN))
            .ReturnsAsync(checkout.BookId);

            _mapperMock.Setup(mapper => mapper.Map<TransactionResponseModel>(expectedReturn))
                .Returns(expectedTransactionResponseModel);

            // Act
            var transactionResponseModel = await _transactionService.ReturnBookAsync(book, patron);

            // Assert
            Assert.Equal(expectedTransactionResponseModel, transactionResponseModel); // Failed
        }

    }
}
