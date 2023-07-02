using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Libro.Domain.Models;
using Libro.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Libro.Tests.Libro.WebAPI.Tests
{
    public class BookTransactionsControllerTests
    {
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IPatronService> _patronServiceMock;
        private readonly Mock<IBookService> _bookServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILoanPolicyService> _loanPolicyServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<BookTransactionsController>> _loggerMock;
        private readonly BookTransactionsController _controller;

        public BookTransactionsControllerTests()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _patronServiceMock = new Mock<IPatronService>();
            _bookServiceMock = new Mock<IBookService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _loanPolicyServiceMock = new Mock<ILoanPolicyService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<BookTransactionsController>>();

            _controller = new BookTransactionsController(
                _transactionServiceMock.Object,
                _mapperMock.Object,
                _patronServiceMock.Object,
                _bookServiceMock.Object,
                _notificationServiceMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object,
                _loanPolicyServiceMock.Object
            );
        }

        [Fact]
        public async Task ReserveBook_ValidISBN_ReturnsOkResult()
        {
            // Arrange
            string isbn = "1234567890";
            var patron = new Patron { PatronId = "patronId", ReservedBooks = new List<Reservation>()};
            var book = new Book { ISBN = isbn, IsAvailable = false };
            var transaction = new Reservation() { BookId = isbn, PatronId = patron.PatronId, ReservationDate = DateTime.Now};
            var response = new ReservationDto()
            { 
                ReservationId = transaction.ReservationId,
                BookId = isbn, patronId = patron.PatronId, ReservationDate = transaction.ReservationDate
            };

            _bookServiceMock.Setup(x => x.GetBookByISBNAsync(isbn)).ReturnsAsync(book);
            _userRepositoryMock.Setup(x => x.GetCurrentUserIdAsync()).ReturnsAsync(patron.PatronId);
            _patronServiceMock.Setup(x => x.GetPatronAsync(patron.PatronId)).ReturnsAsync(patron);
            _transactionServiceMock.Setup(x => x.ReserveBookAsync(book, patron)).ReturnsAsync(response);
            _notificationServiceMock.Setup(x => x.AddPatronToNotificationQueue(patron.PatronId, book.ISBN))
                .Returns(Task.CompletedTask);
            _notificationServiceMock.Setup(x => x.SendReservationNotification(patron.Email, book.Title, patron.PatronId))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _controller.ReserveBook(isbn);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task ReserveBook_BookNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            string isbn = "1234567890";
            string message = "No Book found with ISBN = " + isbn;

            _bookServiceMock.Setup(x => x.GetBookByISBNAsync(isbn)).ReturnsAsync((Book)null);

            // Act
            var result = await _controller.ReserveBook(isbn);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(message, notFoundResult.Value);
        }

        [Fact]
        public async Task ReserveBook_BookAvailable_ReturnsBadRequestResult()
        {
            // Arrange
            string isbn = "1234567890";
            var book = new Book { ISBN = isbn, IsAvailable = true };

            _bookServiceMock.Setup(x => x.GetBookByISBNAsync(isbn)).ReturnsAsync(book);

            // Act
            var result = await _controller.ReserveBook(isbn);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The book is currently available for borrowing", badRequestResult.Value);
        }

        [Fact]
        public async Task CheckoutBook_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            string isbn = "1234567890";
            var book = new Book { ISBN = isbn, IsAvailable = true };
            var patron = new Patron { PatronId = "patronId", Name = "John Doe" };
            var transaction = new BookTransactionDto() { ISBN = isbn, PatronID = patron.PatronId };
            var bookCheckout = new TransactionResponseModel();

            var queues = new Dictionary<string, Queue<string>>();
            queues.Add(isbn, new Queue<string>());
            queues[isbn].Enqueue(patron.PatronId);

            _bookServiceMock.Setup(x => x.GetBookByISBNAsync(transaction.ISBN)).ReturnsAsync(book);
            _patronServiceMock.Setup(x => x.GetPatronAsync(transaction.PatronID)).ReturnsAsync(patron);
            _notificationServiceMock.Setup(x => x.GetNotificationQueue()).ReturnsAsync(queues);
            _loanPolicyServiceMock.Setup(x => x.CanPatronCheckoutBook(patron)).Returns(true);
            _transactionServiceMock.Setup(x => x.CheckoutBookAsync(book, patron)).ReturnsAsync(bookCheckout);

            // Act
            var result = await _controller.CheckoutBook(isbn, transaction);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(bookCheckout, okResult.Value);

            _notificationServiceMock.Verify(x => x.GetNotificationQueue(), Times.Once);
            _notificationServiceMock.Verify(x => x.SendReservationNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _loanPolicyServiceMock.Verify(x => x.CanPatronCheckoutBook(patron), Times.Once);
            _transactionServiceMock.Verify(x => x.CheckoutBookAsync(book, patron), Times.Once);
        }

        [Fact]
        public async Task CheckoutBook_BookNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            string isbn = "1234567890";
            string message = "No Book found with ISBN = " + isbn;
            var bookCheckout = new BookTransactionDto { ISBN = isbn };

            _bookServiceMock.Setup(x => x.GetBookByISBNAsync(bookCheckout.ISBN)).ReturnsAsync((Book)null);

            // Act
            var result = await _controller.CheckoutBook(isbn, bookCheckout);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(message, notFoundResult.Value);
        }

        [Fact]
        public async Task CheckoutBook_PatronNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            string isbn = "1234567890";
            var book = new Book { ISBN = isbn };
            var bookCheckout = new BookTransactionDto { ISBN = isbn, PatronID = "patronId" };
            string message = "No Patron found with ID = " + bookCheckout.PatronID;

            _bookServiceMock.Setup(x => x.GetBookByISBNAsync(bookCheckout.ISBN)).ReturnsAsync(book);
            _patronServiceMock.Setup(x => x.GetPatronAsync(bookCheckout.PatronID)).ReturnsAsync((Patron)null);

            // Act
            var result = await _controller.CheckoutBook(isbn, bookCheckout);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(message, notFoundResult.Value);
        }

        [Fact]
        public async Task GetOverdueBooks_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var overdueBooks = new List<Book> { new Book(), new Book() };

            _transactionServiceMock.Setup(x => x.GetOverdueBooksAsync()).ReturnsAsync(overdueBooks);
            _mapperMock.Setup(x => x.Map<IEnumerable<BookDto>>(overdueBooks)).Returns(new List<BookDto>());

            // Act
            var result = await _controller.GetOverdueBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<BookDto>>(okResult.Value);
        }

        [Fact]
        public async Task GetOverdueBooks_NoOverdueBooks_ReturnsNotFoundResult()
        {
            // Arrange
            _transactionServiceMock.Setup(x => x.GetOverdueBooksAsync()).ReturnsAsync((List<Book>)null);

            // Act
            var result = await _controller.GetOverdueBooks();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No overdue books found.", notFoundResult.Value);
        }

        [Fact]
        public async Task ReturnBook_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            string isbn = "1234567890";
            var book = new Book { ISBN = isbn, IsAvailable = false };
            var patron = new Patron { PatronId = "patronId" };
            var transaction = new TransactionResponseModel();
            var bookReturn = new BookTransactionDto { ISBN = isbn, PatronID = patron.PatronId };

            _bookServiceMock.Setup(x => x.GetBookByISBNAsync(bookReturn.ISBN)).ReturnsAsync(book);
            _patronServiceMock.Setup(x => x.GetPatronAsync(bookReturn.PatronID)).ReturnsAsync(patron);
            _transactionServiceMock.Setup(x => x.ReturnBookAsync(book, patron)).ReturnsAsync(transaction);

            // Act
            var result = await _controller.ReturnBook(isbn, bookReturn);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(transaction, okResult.Value);
        }

        [Fact]
        public async Task ReturnBook_BookNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            string isbn = "1234567890";
            var book = new Book();
            var bookReturn = new BookTransactionDto { ISBN = isbn, PatronID = "123" };
            string message = "No Book found with ISBN = " + isbn;

            _bookServiceMock.Setup(x => x.GetBookByISBNAsync(bookReturn.ISBN)).ReturnsAsync((Book)null);

            // Act
            var result = await _controller.ReturnBook(isbn, bookReturn);

            // Assert
            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(message, notFoundResult.Value);
        }

        [Fact]
        public async Task ReturnBook_PatronNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            string isbn = "1234567890";
            var book = new Book { ISBN = isbn };
            var bookReturn = new BookTransactionDto { ISBN = isbn, PatronID = "patronId" };
            string message = "No Patron found with ID = " + bookReturn.PatronID;

            _bookServiceMock.Setup(x => x.GetBookByISBNAsync(bookReturn.ISBN)).ReturnsAsync(book);
            _patronServiceMock.Setup(x => x.GetPatronAsync(bookReturn.PatronID)).ReturnsAsync((Patron)null);

            // Act
            var result = await _controller.ReturnBook(isbn, bookReturn);

            // Assert
            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(message, notFoundResult.Value);
        }

        [Fact]
        public async Task GetBorrowedBookById_BookFound_ReturnsOkResult()
        {
            // Arrange
            string transactionId = "1";
            var book = new Book { ISBN = "1234567890" };
            var transaction = new Checkout { CheckoutId = transactionId, Book = book };

            _transactionServiceMock.Setup(x => x.GetBorrowedBookByIdAsync(transactionId)).ReturnsAsync(book);
            _mapperMock.Setup(x => x.Map<BookDto>(book)).Returns(new BookDto());

            // Act
            var result = await _controller.GetBorrowedBookById(transactionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BookDto>(okResult.Value);
        }

        [Fact]
        public async Task GetBorrowedBookById_BookNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            string bookId = "1234567890";

            _transactionServiceMock.Setup(x => x.GetBorrowedBookByIdAsync(bookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _controller.GetBorrowedBookById(bookId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Book not found or not borrowed.", notFoundResult.Value);
        }
        
        [Fact]
        public async Task GetBorrowedBooks_BooksFound_ReturnsOkResultWithBookDtos()
        {
            // Arrange
            var borrowedBooks = new List<Book>
            {
                new Book { ISBN = "1234567890" },
                new Book { ISBN = "0987654321" }
            };

            _transactionServiceMock.Setup(x => x.GetBorrowedBooksAsync()).ReturnsAsync(borrowedBooks);
            _mapperMock.Setup(x => x.Map<IEnumerable<BookDto>>(borrowedBooks)).Returns(new List<BookDto>());

            // Act
            var result = await _controller.GetBorrowedBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<BookDto>>(okResult.Value);
        }

        [Fact]
        public async Task GetBorrowedBooks_NoBooksFound_ReturnsNotFoundResult()
        {
            // Arrange
            List<Book> borrowedBooks = null;

            _transactionServiceMock.Setup(x => x.GetBorrowedBooksAsync()).ReturnsAsync(borrowedBooks);

            // Act
            var result = await _controller.GetBorrowedBooks();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No borrowed books found.", notFoundResult.Value);
        }

    }
}
