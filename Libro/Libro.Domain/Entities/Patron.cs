using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Entities
{
    public class Patron
    {
        public int PatronId { get; set; }
        public string Name { get; set; }

        [Required]
        public string Email { get; set; } 
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Reservation> ReservedBooks { get; set; } 
        public ICollection<Checkout> CheckedoutBooks { get; set; } 
        public ICollection<ReadingList> ReadingLists { get; set; }
    }
}
