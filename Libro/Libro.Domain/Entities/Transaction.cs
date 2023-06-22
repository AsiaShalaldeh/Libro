using Libro.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Libro.Domain.Entities
{
    public class Transaction // junction table between Book and Patron
    {
        public string TransactionId { get; set; }

        [ForeignKey("Book")]
        public string BookId { get; set; }
        public Book Book { get; set; }
        public int PatronId { get; set; }
        public Patron Patron { get; set; }
        public int LibrarianId { get; set; }
        public Librarian Librarian { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; }
        public DateTime ReturnDate { get; set; }
        public TransactionType Type { get; set; }
    }
}
