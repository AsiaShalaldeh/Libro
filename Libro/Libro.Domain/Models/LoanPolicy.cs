namespace Libro.Domain.Models
{
    public class LoanPolicy
    {
        public int LoanDurationInDays { get; set; }
        public int MaxBooksPerPatron { get; set; }
        public decimal BorrowingFeePerDay { get; set; }
        public decimal LateFeePerDay { get; set; }

    }
}
