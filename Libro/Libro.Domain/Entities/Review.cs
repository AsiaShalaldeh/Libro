using System.ComponentModel.DataAnnotations.Schema;

namespace Libro.Domain.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string PatronId { get; set; }
        public Patron Patron { get; set; }
        [ForeignKey("Book")]
        public string BookId { get; set; }
        public Book Book { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
