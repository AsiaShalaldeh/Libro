using System.ComponentModel.DataAnnotations.Schema;

namespace Libro.Domain.Entities
{
    public class BookList // junction table between Book and ReadingList
    {
        public int ListId { get; set; }
        public ReadingList ReadingList { get; set; }

        [ForeignKey("Book")]
        public string BookId { get; set; }
        public Book Book { get; set; }
    }
}
