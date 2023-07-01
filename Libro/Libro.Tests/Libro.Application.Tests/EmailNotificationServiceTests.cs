using Infrastructure.EmailService.Interface;
using Infrastructure.EmailService.Model;
using Libro.Application.Services;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Tests.Libro.Application.Tests
{
    public class EmailNotificationServiceTests
    {
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IPatronService> _patronServiceMock;
        private readonly EmailNotificationService _notificationService;
        private readonly Mock<ILogger<EmailNotificationService>> loggerMock;

        public EmailNotificationServiceTests()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _patronServiceMock = new Mock<IPatronService>();
            loggerMock = new Mock<ILogger<EmailNotificationService>>();
            _notificationService = new EmailNotificationService(
                _emailSenderMock.Object,
                Mock.Of<IBookService>(),
                _patronServiceMock.Object,
                Mock.Of<ITransactionService>(),
                Mock.Of<IBookQueueRepository>(),
                Mock.Of<ILogger<EmailNotificationService>>());
        }

        [Fact]
        public async Task SendOverdueNotification_WithOverdueTransactions_SendsEmails()
        {
            // Arrange
            var transactionServiceMock = new Mock<ITransactionService>();
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

            transactionServiceMock.Setup(mock => mock.GetOverdueTransactionsAsync()).ReturnsAsync(overdueTransactions);

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
            var transactionServiceMock = new Mock<ITransactionService>();

            transactionServiceMock.Setup(mock => mock.GetOverdueTransactionsAsync()).ReturnsAsync(new List<Checkout>());

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
            var transactionServiceMock = new Mock<ITransactionService>();

            transactionServiceMock.Setup(mock => mock.GetOverdueTransactionsAsync()).ThrowsAsync(new Exception("Some error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _notificationService.SendOverdueNotification());
        }

        [Fact]
        public async Task SendReservationNotification_ValidRecipientEmailAndBookTitle_SendsEmail()
        {
            // Arrange
            var emailSenderMock = new Mock<IEmailSender>();
            var patronServiceMock = new Mock<IPatronService>();

            // Create a test patron with a reservation for the book title
            var testPatronId = "test-patron-id";
            var testRecipientEmail = "test@example.com";
            var testBookTitle = "Test Book";

            // Configure the mock patron service to return the test patron
            var testPatron = new Patron { PatronId = testPatronId, Email = testRecipientEmail };
            testPatron.ReservedBooks = new List<Reservation> { new Reservation { Book = new Book { Title = testBookTitle } } };
            patronServiceMock.Setup(mock => mock.GetPatronAsync(testPatronId))
                .ReturnsAsync(testPatron);

            // Act
            var result = await _notificationService.SendReservationNotification(testRecipientEmail, testBookTitle, testPatronId);

            // Assert
            Assert.True(result);
            emailSenderMock.Verify(mock =>
                mock.SendEmailAsync(It.IsAny<Message>()), Times.Once);
        }

        [Fact]
        public async Task SendReservationNotification_InvalidRecipientEmail_ThrowsResourceNotFoundException()
        {
            // Arrange
            var emailSenderMock = new Mock<IEmailSender>();
            var patronServiceMock = new Mock<IPatronService>();
            var loggerMock = new Mock<ILogger<EmailNotificationService>>();

            var patronId = "patron1";
            var recipientEmail = "john@example.com";
            var bookTitle = "Book 1";

            var patron = new Patron
            {
                PatronId = patronId,
                Email = "alice@example.com"
            };

            patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync(patron);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _notificationService.SendReservationNotification(recipientEmail, bookTitle, patronId));
        }

        [Fact]
        public async Task SendReservationNotification_InvalidPatronId_ThrowsResourceNotFoundException()
        {
            // Arrange
            var emailSenderMock = new Mock<IEmailSender>();
            var patronServiceMock = new Mock<IPatronService>();
            var loggerMock = new Mock<ILogger<EmailNotificationService>>();

            var patronId = "patron1";
            var recipientEmail = "john@example.com";
            var bookTitle = "Book 1";

            patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync((Patron)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _notificationService.SendReservationNotification(recipientEmail, bookTitle, patronId));
        }

        [Fact]
        public async Task SendReservationNotification_NoReservationForBookTitle_ReturnsFalse()
        {
            // Arrange
            var emailSenderMock = new Mock<IEmailSender>();
            var patronServiceMock = new Mock<IPatronService>();
            var loggerMock = new Mock<ILogger<EmailNotificationService>>();

            var patronId = "patron1";
            var recipientEmail = "john@example.com";
            var bookTitle = "Book 1";

            var patron = new Patron
            {
                PatronId = patronId,
                Email = recipientEmail,
                ReservedBooks = new List<Reservation>()
            };

            patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync(patron);

            // Act
            var result = await _notificationService.SendReservationNotification(recipientEmail, bookTitle, patronId);

            // Assert
            Assert.False(result);
            emailSenderMock.Verify(mock =>
                mock.SendEmailAsync(It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public async Task SendReservationNotification_ThrowsException()
        {
            // Arrange
            var emailSenderMock = new Mock<IEmailSender>();
            var patronServiceMock = new Mock<IPatronService>();
            var loggerMock = new Mock<ILogger<EmailNotificationService>>();

            var patronId = "patron1";
            var recipientEmail = "john@example.com";
            var bookTitle = "Book 1";

            patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ThrowsAsync(new Exception("Some error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _notificationService.SendReservationNotification(recipientEmail, bookTitle, patronId));
        }

        [Fact]
        public async Task AddPatronToNotificationQueue_ValidPatronIdAndBookId_EnqueuesPatron()
        {
            // Arrange
            var bookQueueRepositoryMock = new Mock<IBookQueueRepository>();
            var loggerMock = new Mock<ILogger<EmailNotificationService>>();

            var patronId = "patron1";
            var bookId = "book1";

            // Act
            await _notificationService.AddPatronToNotificationQueue(patronId, bookId);

            // Assert
            bookQueueRepositoryMock.Verify(mock =>
                mock.EnqueuePatronAsync(bookId, patronId), Times.Once);
        }

        [Fact]
        public async Task AddPatronToNotificationQueue_ThrowsException()
        {
            // Arrange
            var bookQueueRepositoryMock = new Mock<IBookQueueRepository>();
            var loggerMock = new Mock<ILogger<EmailNotificationService>>();

            var patronId = "patron1";
            var bookId = "book1";

            bookQueueRepositoryMock.Setup(mock => mock.EnqueuePatronAsync(bookId, patronId)).ThrowsAsync(new Exception("Some error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _notificationService.AddPatronToNotificationQueue(patronId, bookId));
        }
    }
}
