using Libro.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Libro.Domain.Entities
{
    public class Reservation
    {
        public string ReservationId { get; set; }

        [ForeignKey("Book")]
        public string BookId { get; set; }
        public Book Book { get; set; }
        public string PatronId { get; set; }
        public Patron Patron { get; set; }
        public DateTime ReservationDate { get; set; }
    }
}
