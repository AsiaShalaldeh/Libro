namespace Libro.Domain.Interfaces.IServices
{
    public interface INotificationService
    {
        Task<bool> SendOverdueNotification();
        Task<bool> SendReservationNotification(string recipientEmail, string bookTitle, string recipientId);
        Task AddPatronToNotificationQueue(string patronId, string bookId);
        Task ProcessNotificationQueue(string bookId);
        Task<Dictionary<string, Queue<string>>> GetNotificationQueue();
    }
}
