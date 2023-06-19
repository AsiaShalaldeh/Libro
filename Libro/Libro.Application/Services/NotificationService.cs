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

        public EmailNotificationService(IEmailSender emailSender, IBookService bookService,
            IPatronService patronService)
        {
            _emailSender = emailSender;
            _bookService = bookService;
            _patronService = patronService;
        }
        public async Task<bool> SendOverdueNotification()
        {
            IEnumerable<Book> overdueBooks = await _bookService.GetOverdueBooksAsync();

            if (overdueBooks.Any())
            {
                var subject = "Overdue Book Notification";
                foreach (var overdueBook in overdueBooks)
                {
                    foreach (var transaction in overdueBook.Transactions)
                    {
                        var patron = transaction.Patron;
                        var content = $"Dear {patron.Name},\n\nThis is a friendly reminder that " +
                            $"the book \"{overdueBook.Title}\" is due on " +
                            $"{transaction.DueDate.ToShortDateString()}.\nPlease return it to the " +
                            $"library on time.\n\nBest regards,\nThe Libro";
                        var message = new Message(new List<string> { patron.Email }, subject, content);
                        await _emailSender.SendEmailAsync(message);
                    }
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
    }
}
