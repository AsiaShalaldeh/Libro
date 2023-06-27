using System.ComponentModel.DataAnnotations.Schema;

namespace Libro.Domain.Entities
{
    public class BookQueue
    {
        [ForeignKey("Book")]
        public string BookId { get; set; }

        [ForeignKey("Patron")]
        public string PatronId { get; set; }
        public int QueuePosition { get; set; }
    }

}
