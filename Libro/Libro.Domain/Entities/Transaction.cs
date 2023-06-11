using System.ComponentModel.DataAnnotations.Schema;

namespace Libro.Domain.Entities
{
    public class Transaction
    {
        public string TransactionId { get; set; }

        [ForeignKey("Book")]
        public string BookId { get; set; }
        public int PatronId { get; set; }
        public int LibrarianId { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; }
    }
}
