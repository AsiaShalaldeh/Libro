using Infrastructure.EmailService.Interface;
using Infrastructure.EmailService.Model;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Services
{
    public class EmailNotificationService : INotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly IBookRepository _bookRepository;
        private readonly IPatronRepository _patronRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBookQueueRepository _bookQueueRepository;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(IEmailSender emailSender, IBookRepository bookRepository,
            IPatronRepository patronRepository, ITransactionRepository transactionRepository,
            IBookQueueRepository bookQueueRepository, ILogger<EmailNotificationService> logger)
        {
            _emailSender = emailSender;
            _bookRepository = bookRepository;
            _patronRepository = patronRepository;
            _transactionRepository = transactionRepository;
            _bookQueueRepository = bookQueueRepository;
            _logger = logger;
        }

        public async Task<bool> SendOverdueNotification()
        {
            try
            {
                var overdueTransactions = await _transactionRepository.GetOverdueTransactionsAsync();

                if (!overdueTransactions.Any())
                    return false;

                var subject = "Overdue Book Notification";

                foreach (var overdueTransaction in overdueTransactions)
                {
                    var patron = overdueTransaction.Patron;
                    var content = $"Dear {patron.Name},\n\nThis is a friendly reminder that " +
                        $"the book \"{overdueTransaction.Book.Title}\" has been due on " +
                        $"{overdueTransaction.DueDate.ToShortDateString()}.\nPlease return it to the " +
                        $"library as soon as possible.\n\nBest regards,\nThe Libro";

                    var message = new Message(new List<string> { patron.Email }, subject, content);
                    await _emailSender.SendEmailAsync(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending overdue notifications.");
                throw;
            }
        }

        public async Task<bool> SendReservationNotification(string recipientEmail, string bookTitle, string recipientId)
        {
            try
            {
                var patron = await ValidatePatronByEmail(recipientEmail, recipientId);

                var hasReservation = patron.ReservedBooks != null &&
                                     patron.ReservedBooks.Any(p => p.Book != null && p.Book.Title.Equals(bookTitle));

                if (!hasReservation)
                    return false;

                var subject = "Book Reservation Notification";
                var content = $"Dear {patron.Name},\n\nYou have successfully reserved the book with " +
                    $"title \"{bookTitle}\". When the book is available, we will notify you. " +
                    $"\n\nBest regards,\nThe Libro";

                var message = new Message(new List<string> { recipientEmail }, subject, content);
                await _emailSender.SendEmailAsync(message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending reservation notifications.");
                throw;
            }
        }

        public async Task AddPatronToNotificationQueue(string patronId, string bookId)
        {
            try
            {
                await _bookQueueRepository.EnqueuePatronAsync(bookId, patronId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a patron to the notification queue.");
                throw;
            }
        }

        public async Task RemovePatronFromNotificationQueue(string bookId)
        {
            try
            {
                await _bookQueueRepository.DequeuePatronAsync(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing a patron from the notification queue.");
                throw;
            }
        }

        public async Task ProcessNotificationQueue(string bookId)
        {
            try
            {
                var firstInQueue = await _bookQueueRepository.PeekPatronAsync(bookId);

                if (firstInQueue != null)
                {
                    var patron = await _patronRepository.GetPatronByIdAsync(firstInQueue.PatronId);
                    var book = await _bookRepository.GetBookByISBNAsync(bookId);

                    if (patron == null)
                        throw new ResourceNotFoundException("Patron", "ID", firstInQueue.PatronId);

                    if (book == null)
                        throw new ResourceNotFoundException("Book", "ISBN", bookId);

                    var subject = "Book Available Notification";
                    var content = $"The book '{book.Title}' is now available for borrowing." +
                        $"\n\nBest regards,\nThe Libro";

                    var message = new Message(new List<string> { patron.Email }, subject, content);
                    await _emailSender.SendEmailAsync(message);

                    //await _bookQueueRepository.DequeuePatronAsync(bookId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the notification queue.");
                throw;
            }
        }

        public async Task<Dictionary<string, Queue<string>>> GetNotificationQueue()
        {
            try
            {
                var bookQueues = new Dictionary<string, Queue<string>>();

                var allBookQueues = await _bookQueueRepository.GetAllBookQueuesAsync();

                foreach (var bookQueue in allBookQueues)
                {
                    if (!bookQueues.ContainsKey(bookQueue.BookId))
                    {
                        bookQueues[bookQueue.BookId] = new Queue<string>();
                    }

                    bookQueues[bookQueue.BookId].Enqueue(bookQueue.PatronId);
                }

                return bookQueues;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the notification queue.");
                throw;
            }
        }

        public async Task<bool> SendReminderNotification(string recipientEmail, string bookISBN, string recipientId)
        {
            try
            {
                var recipient = await ValidatePatronById(recipientId);
                var book = await _bookRepository.GetBookByISBNAsync(bookISBN);

                IEnumerable<Checkout> checkouts = await _transactionRepository.GetCheckoutTransactionsByPatronAsync(recipientId);

                if (checkouts == null || !checkouts.Any(ch => ch.BookId.Equals(bookISBN) && !ch.IsReturned))
                {
                    throw new ArgumentException($"No checked out book with ISBN {bookISBN} found for that patron.");
                }

                var checkout = checkouts.FirstOrDefault(ch => ch.BookId.Equals(bookISBN) && !ch.IsReturned);
                var subject = "Book Return Reminder";
                var content = $"Dear {recipient.Name},\n\nThis is a friendly reminder that " +
                              $"the book \"{book.Title}\" is due for return on {checkout.DueDate}.\nPlease return it to the " +
                              $"library on time.\n\nBest regards,\nThe Libro";

                var message = new Message(new List<string> { recipientEmail }, subject, content);
                await _emailSender.SendEmailAsync(message);

                _logger.LogInformation($"Reminder notification sent to {recipientEmail} for book {book.Title}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the reminder notification.");
                throw;
            }
        }

        public async Task<bool> SendCheckoutNotification(string recipientEmail, string bookTitle, string recipientId)
        {
            try
            {
                var patron = await ValidatePatronByEmail(recipientEmail, recipientId);

                IEnumerable<Checkout> checkouts = await _transactionRepository.GetCheckoutTransactionsByPatronAsync(recipientId);

                if (checkouts == null || !checkouts.Any(ch => ch.Book.Title.Equals(bookTitle) && !ch.IsReturned))
                {
                    throw new ArgumentException($"No checked out book with title {bookTitle} found for that patron.");
                }

                var subject = "Book Checkout Notification";
                var content = $"Dear {patron.Name},\n\nThis is to inform you that you " +
                    $"have successfully checked out the book \"{bookTitle}\" from our " +
                    $"library.\nPlease ensure to return it within the specified due date " +
                    $"to avoid any late fees.\n\nBest regards,\nThe Libro";

                var message = new Message(new List<string> { recipientEmail }, subject, content);
                await _emailSender.SendEmailAsync(message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the checkout notification.");
                throw;
            }
        }

        public async Task<bool> SendReturnNotification(string recipientEmail, string bookTitle, string recipientId)
        {
            try
            {
                var patron = await ValidatePatronByEmail(recipientEmail, recipientId);

                IEnumerable<Checkout> checkouts = await _transactionRepository.GetCheckoutTransactionsByPatronAsync(recipientId);

                if (checkouts == null || !checkouts.Any(ch => ch.Book.Title.Equals(bookTitle) && ch.IsReturned))
                {
                    throw new ArgumentException($"No returned book with title {bookTitle} found for that patron.");
                }

                var subject = "Book Return Notification";
                var content = $"Dear {patron.Name},\n\nThis is to confirm that you " +
                    $"have returned the book \"{bookTitle}\" to our library. " +
                    $"Thank you for returning it promptly. We hope you enjoyed " +
                    $"reading it.\n\nBest regards,\nThe Libro";

                var message = new Message(new List<string> { recipientEmail }, subject, content);
                await _emailSender.SendEmailAsync(message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the return notification.");
                throw;
            }
        }

        private async Task<Patron> ValidatePatronByEmail(string email, string patronId)
        {
            try
            {
                var patron = await _patronRepository.GetPatronByIdAsync(patronId);

                if (patron == null)
                    throw new ResourceNotFoundException("Patron", "ID", patronId);

                if (!patron.Email.Equals(email))
                    throw new ResourceNotFoundException("Patron", "Email", email);

                return patron;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating patron by email.");
                throw;
            }
        }

        private async Task<Patron> ValidatePatronById(string patronId)
        {
            try
            {
                var patron = await _patronRepository.GetPatronByIdAsync(patronId);

                if (patron == null)
                    throw new ResourceNotFoundException("Patron", "ID", patronId);

                return patron;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating patron by ID.");
                throw;
            }
        }
    }
}
