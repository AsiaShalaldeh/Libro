using EmailService.Interface;
using EmailService.Model;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Application.Services
{
    public class EmailNotificationService : INotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly IBookService _bookService;
        private readonly IPatronService _patronService;
        private readonly ITransactionService _transactionService;
        Dictionary<string, Queue<int>> booksQueues;

        public EmailNotificationService(IEmailSender emailSender, IBookService bookService,
            IPatronService patronService, ITransactionService transactionService)
        {
            _emailSender = emailSender;
            _bookService = bookService;
            _patronService = patronService;
            _transactionService = transactionService;
            booksQueues = new Dictionary<string, Queue<int>>();
        }
        public async Task<bool> SendOverdueNotification()
        {
            IEnumerable<Checkout> overdueTransactions = _transactionService.GetOverdueTransactionsAsync();

            if (overdueTransactions.Any())
            {
                var subject = "Overdue Book Notification";
                foreach (var overdueTransaction in overdueTransactions)
                {
                    //foreach (var transaction in overdueBook.Transactions)
                    //{
                        var patron = overdueTransaction.Patron;
                        var content = $"Dear {patron.Name},\n\nThis is a friendly reminder that " +
                            $"the book \"{overdueTransaction.Book.Title}\" is due on " +
                            $"{overdueTransaction.DueDate.ToShortDateString()}.\nPlease return it to the " +
                            $"library on time.\n\nBest regards,\nThe Libro";
                        var message = new Message(new List<string> { patron.Email }, subject, content);
                        await _emailSender.SendEmailAsync(message);
                    //}
                }
                return true;
            }
            else
            {
                return false;
            }

        }
        public async Task SendReservationNotification(string recipientEmail, string bookTitle, int recipientId)
        {
            // check for patron with that Email
            // check for transaion done with this info
            Patron patron = await _patronService.GetPatronProfileAsync(recipientId);
            var subject = "Book Reservation Notification";
            var content = $"Dear {patron.Name},\n\nYou have successfully reserved the book with " +
                $"title \"{bookTitle}\". Please pick up the book from the library within " +
                $"the next 3 days.\n\nBest regards,\nThe Libro";

            var message = new Message(new List<string> { recipientEmail }, subject, content);

            await _emailSender.SendEmailAsync(message);
        }

        public async Task AddPatronToNotificationQueue(int patronId, string bookId)
        {
            if (!booksQueues.ContainsKey(bookId))
            {
                booksQueues[bookId] = new Queue<int>();
            }

            booksQueues[bookId].Enqueue(patronId);
        }
        public async Task ProcessNotificationQueue(string bookId)
        {
            if (booksQueues.ContainsKey(bookId))
            {
                var queue = booksQueues[bookId];

                if (queue.Count > 0)
                {
                    var patronId = queue.Peek();
                    var patron = await _patronService.GetPatronProfileAsync(patronId);

                    if (patron != null)
                    {
                        var book = await _bookService.GetBookByIdAsync(bookId);

                        if (book != null)
                        {
                            string subject = "Book Available Notification";
                            string content = $"The book '{book.Title}' is now available for borrowing.";

                            // Use the email service to send the notification
                            var message = new Message(new List<string> { patron.Email }, subject, content);

                            await _emailSender.SendEmailAsync(message);

                            queue.Dequeue();
                        }
                        else
                        {
                            // Log or handle the case when the book is not found
                            queue.Dequeue();
                        }
                    }
                    else
                    {
                        // Log or handle the case when the patron profile is not found
                        queue.Dequeue();
                    }
                }
            }
        }

        public async Task<Dictionary<string, Queue<int>>> GetNotificationQueue()
        {
            return booksQueues;
        }
    }
}
