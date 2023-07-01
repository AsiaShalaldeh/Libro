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
        private readonly IBookService _bookService;
        private readonly IPatronService _patronService;
        private readonly ITransactionService _transactionService;
        private readonly IBookQueueRepository _bookQueueRepository;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(IEmailSender emailSender, IBookService bookService,
            IPatronService patronService, ITransactionService transactionService,
            IBookQueueRepository bookQueueRepository, ILogger<EmailNotificationService> logger)
        {
            _emailSender = emailSender;
            _bookService = bookService;
            _patronService = patronService;
            _transactionService = transactionService;
            _bookQueueRepository = bookQueueRepository;
            _logger = logger;
        }

        public async Task<bool> SendOverdueNotification()
        {
            try
            {
                var overdueTransactions = await _transactionService.GetOverdueTransactionsAsync();

                if (overdueTransactions.Any())
                {
                    var subject = "Overdue Book Notification";
                    foreach (var overdueTransaction in overdueTransactions)
                    {
                        var patron = overdueTransaction.Patron;
                        var content = $"Dear {patron.Name},\n\nThis is a friendly reminder that " +
                            $"the book \"{overdueTransaction.Book.Title}\" is due on " +
                            $"{overdueTransaction.DueDate.ToShortDateString()}.\nPlease return it to the " +
                            $"library on time.\n\nBest regards,\nThe Libro";
                        var message = new Message(new List<string> { patron.Email }, subject, content);
                        await _emailSender.SendEmailAsync(message);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
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
                // check for patron with that Email
                Patron patron = await _patronService.GetPatronAsync(recipientId);
                if (patron == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", recipientId);
                }
                if (!patron.Email.Equals(recipientEmail))
                {
                    throw new ResourceNotFoundException("Patron", "Email", recipientEmail);
                }
                // Check if any reservation matches the book title
                bool hasReservation = patron.ReservedBooks != null &&
                                      patron.ReservedBooks.Any(p => p.Book != null && p.Book.Title.Equals(bookTitle));
                if (!hasReservation)
                {
                    return false;
                }
                var subject = "Book Reservation Notification";
                var content = $"Dear {patron.Name},\n\nYou have successfully reserved the book with " +
                    $"title \"{bookTitle}\". When the book be available we will notify you. " +
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

        public async Task ProcessNotificationQueue(string bookId)
        {
            try
            {
                var firstInQueue = await _bookQueueRepository.PeekPatronAsync(bookId);

                if (firstInQueue != null)
                {
                    var patron = await _patronService.GetPatronAsync(firstInQueue.PatronId);
                    var book = await _bookService.GetBookByISBNAsync(bookId);

                    if (patron != null && book != null)
                    {
                        string subject = "Book Available Notification";
                        string content = $"The book '{book.Title}' is now available for borrowing." +
                            $"\n\nBest regards,\nThe Libro";

                        var message = new Message(new List<string> { patron.Email }, subject, content);
                        await _emailSender.SendEmailAsync(message);

                        await _bookQueueRepository.DequeuePatronAsync(bookId);
                    }
                    else
                    {
                        // Log or handle the case when patron or book is not found
                        await _bookQueueRepository.DequeuePatronAsync(bookId);
                    }
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
    }
}
