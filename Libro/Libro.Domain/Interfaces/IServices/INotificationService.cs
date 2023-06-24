namespace Libro.Domain.Interfaces.IServices
{
    public interface INotificationService
    {
        Task<bool> SendOverdueNotification();
        Task SendReservationNotification(string recipientEmail, string bookTitle, int recipientId);
        Task AddPatronToNotificationQueue(int patronId, string bookId);
        Task ProcessNotificationQueue(string bookId);
        Task<Dictionary<string, Queue<int>>> GetNotificationQueue();
    }
}
