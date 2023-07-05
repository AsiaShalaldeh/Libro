namespace Libro.Domain.Models
{
    public class TransactionResponseModel
    {
        public string CheckoutId { get; set; }
        public string BookId { get; set; }
        public string PatronId { get; set; }
        public DateTime CheckoutDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalFee { get; set; }
    }
}
