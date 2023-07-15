using System.ComponentModel.DataAnnotations.Schema;

namespace Libro.Domain.Entities
{
    public class Checkout 
    {
        public string CheckoutId { get; set; }

        [ForeignKey("Book")]
        public string BookId { get; set; }
        public Book Book { get; set; }
        public string PatronId { get; set; }
        public Patron Patron { get; set; }
        public DateTime CheckoutDate { get; set; }
        public DateTime DueDate { get; set; } 
        public bool IsReturned { get; set; }
        public DateTime ReturnDate { get; set; } = DateTime.MinValue;
        public decimal TotalFee { get; set; } = 0.0m;
    }
}
