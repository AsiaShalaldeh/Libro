namespace Libro.Domain.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public int PatronId { get; set; }
        public int LibrarianId { get; set; }

    }
}
