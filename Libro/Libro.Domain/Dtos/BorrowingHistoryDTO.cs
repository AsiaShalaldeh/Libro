namespace Libro.Domain.Dtos
{
    public class BorrowingHistoryDTO
    {
        public string TransactionId { get; set; }
        public string BookTitle { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsOverdue { get; set; }
    }

}
