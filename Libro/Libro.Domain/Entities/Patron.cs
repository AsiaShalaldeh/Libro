using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Libro.Domain.Entities
{
    public class Patron
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string PatronId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } 
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Reservation> ReservedBooks { get; set; } 
        public ICollection<Checkout> CheckedoutBooks { get; set; } 
        public ICollection<ReadingList> ReadingLists { get; set; }
    }
}
