using System.ComponentModel.DataAnnotations.Schema;

namespace Libro.Domain.Entities
{
    public class BookList
    {
        public int ListId { get; set; }

        [ForeignKey("Book")]
        public string BookId { get; set; }
    }
}
