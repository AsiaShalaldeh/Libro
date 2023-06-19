namespace Libro.Domain.Interfaces.IServices
{
    public interface INotificationService
    {
        Task<bool> SendOverdueNotification();
        Task SendReservationNotification(string recipientEmail, string bookTitle, int recipientId);
    }
}
