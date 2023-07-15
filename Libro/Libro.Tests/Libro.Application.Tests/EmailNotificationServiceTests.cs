using Infrastructure.EmailService.Interface;
using Infrastructure.EmailService.Model;
using Libro.Application.Services;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Tests.Libro.Application.Tests
{
    public class EmailNotificationServiceTests
    {
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IPatronRepository> _patronRepositoryMock;
        private readonly EmailNotificationService _notificationService;
        private readonly Mock<IBookQueueRepository> _bookQueueRepositoryMock;
        private readonly Mock<ILogger<EmailNotificationService>> _loggerMock;

        public EmailNotificationServiceTests()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _patronRepositoryMock = new Mock<IPatronRepository>();
            _loggerMock = new Mock<ILogger<EmailNotificationService>>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _bookQueueRepositoryMock = new Mock<IBookQueueRepository>();
            _notificationService = new EmailNotificationService(
                _emailSenderMock.Object,
                Mock.Of<IBookRepository>(),
                _patronRepositoryMock.Object,
                _transactionRepositoryMock.Object,
                _bookQueueRepositoryMock.Object,
                Mock.Of<ILogger<EmailNotificationService>>());
        }

        [Fact]
        public async Task SendOverdueNotification_WithOverdueTransactions_SendsEmails()
        {
            // Arrange
            var overdueTransactions = new List<Checkout>
            {
                new Checkout
                {
                    Patron = new Patron { Name = "John", Email = "john@example.com" },
                    Book = new Book { Title = "Book 1" },
                    DueDate = DateTime.Now.AddDays(-1)
                },
                new Checkout
                {
                    Patron = new Patron { Name = "Alice", Email = "alice@example.com" },
                    Book = new Book { Title = "Book 2" },
                    DueDate = DateTime.Now.AddDays(-1)
                }
            };

            _transactionRepositoryMock.Setup(mock => mock.GetOverdueTransactionsAsync()).ReturnsAsync(overdueTransactions);

            // Act
            var result = await _notificationService.SendOverdueNotification();

            // Assert
            Assert.True(result);
            _emailSenderMock.Verify(mock =>
                mock.SendEmailAsync(It.IsAny<Message>()), Times.Exactly(overdueTransactions.Count));
        }

        [Fact]
        public async Task SendOverdueNotification_WithNoOverdueTransactions_ReturnsFalse()
        {
            // Arrange
            _transactionRepositoryMock.Setup(mock => mock.GetOverdueTransactionsAsync()).ReturnsAsync(new List<Checkout>());

            // Act
            var result = await _notificationService.SendOverdueNotification();

            // Assert
            Assert.False(result);
            _emailSenderMock.Verify(mock =>
                mock.SendEmailAsync(It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public async Task SendOverdueNotification_ThrowsException()
        {
            // Arrange
            _transactionRepositoryMock.Setup(mock => mock.GetOverdueTransactionsAsync()).ThrowsAsync(new Exception("Some error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _notificationService.SendOverdueNotification());
        }

        [Fact]
        public async Task SendReservationNotification_ValidRecipientEmailAndBookTitle_SendsEmail()
        {
            // Arrange
            // Create a test patron with a reservation for the book title
            var testPatronId = "test-patron-id";
            var testRecipientEmail = "test@example.com";
            var testBookTitle = "Test Book";

            // Configure the mock patron service to return the test patron
            var testPatron = new Patron { PatronId = testPatronId, Email = testRecipientEmail };
            testPatron.ReservedBooks = new List<Reservation> { new Reservation { Book = new Book { Title = testBookTitle } } };
            _patronRepositoryMock.Setup(mock => mock.GetPatronByIdAsync(testPatronId))
                .ReturnsAsync(testPatron);

            // Act
            var result = await _notificationService.SendReservationNotification(testRecipientEmail, testBookTitle, testPatronId);

            // Assert
            Assert.True(result);
            _emailSenderMock.Verify(mock =>
                mock.SendEmailAsync(It.IsAny<Message>()), Times.Once);
        }

        [Fact]
        public async Task SendReservationNotification_InvalidRecipientEmail_ThrowsResourceNotFoundException()
        {
            // Arrange
            var patronId = "patron1";
            var recipientEmail = "john@example.com";
            var bookTitle = "Book 1";

            var patron = new Patron
            {
                PatronId = patronId,
                Email = "alice@example.com"
            };

            _patronRepositoryMock.Setup(mock => mock.GetPatronByIdAsync(patronId)).ReturnsAsync(patron);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _notificationService.SendReservationNotification(recipientEmail, bookTitle, patronId));
        }

        [Fact]
        public async Task SendReservationNotification_InvalidPatronId_ThrowsResourceNotFoundException()
        {
            // Arrange
            var patronId = "patron1";
            var recipientEmail = "john@example.com";
            var bookTitle = "Book 1";

            _patronRepositoryMock.Setup(mock => mock.GetPatronByIdAsync(patronId)).ReturnsAsync((Patron)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _notificationService.SendReservationNotification(recipientEmail, bookTitle, patronId));
        }

        [Fact]
        public async Task SendReservationNotification_NoReservationForBookTitle_ReturnsFalse()
        {
            // Arrange
            var patronId = "patron1";
            var recipientEmail = "john@example.com";
            var bookTitle = "Book 1";

            var patron = new Patron
            {
                PatronId = patronId,
                Email = recipientEmail,
                ReservedBooks = new List<Reservation>()
            };

            _patronRepositoryMock.Setup(mock => mock.GetPatronByIdAsync(patronId)).ReturnsAsync(patron);

            // Act
            var result = await _notificationService.SendReservationNotification(recipientEmail, bookTitle, patronId);

            // Assert
            Assert.False(result);
            _emailSenderMock.Verify(mock =>
                mock.SendEmailAsync(It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public async Task SendReservationNotification_ThrowsException()
        {
            // Arrange
            var patronId = "patron1";
            var recipientEmail = "john@example.com";
            var bookTitle = "Book 1";

            _patronRepositoryMock.Setup(mock => mock.GetPatronByIdAsync(patronId)).ThrowsAsync(new Exception("Some error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _notificationService.SendReservationNotification(recipientEmail, bookTitle, patronId));
        }

        [Fact]
        public async Task AddPatronToNotificationQueue_ValidPatronIdAndBookId_EnqueuesPatron()
        {
            // Arrange
            var patronId = "patron1";
            var bookId = "book1";

            // Act
            await _notificationService.AddPatronToNotificationQueue(patronId, bookId);

            // Assert
            _bookQueueRepositoryMock.Verify(mock =>
                mock.EnqueuePatronAsync(bookId, patronId), Times.Once);
        }

        [Fact]
        public async Task AddPatronToNotificationQueue_ThrowsException()
        {
            // Arrange
            var patronId = "patron1";
            var bookId = "book1";

            _bookQueueRepositoryMock.Setup(mock => mock.EnqueuePatronAsync(bookId, patronId)).ThrowsAsync(new Exception("Some error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _notificationService.AddPatronToNotificationQueue(patronId, bookId));
        }
    }
}
