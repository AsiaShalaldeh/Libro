namespace Libro.Domain.Dtos
{
    public class ReservationDto
    {
        public string ReservationId { get; set; }
        public string patronId { get; set; }
        public string BookId { get; set; }
        public DateTime ReservationDate { get; set; }
    }
}
