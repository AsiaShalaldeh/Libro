namespace Libro.Domain.Dtos
{
    public class ReservationNotificationRequest
    {
        public string RecipientId { get; set; }
        public string RecipientEmail { get; set; }
        public string BookTitle { get; set; }
    }
}
