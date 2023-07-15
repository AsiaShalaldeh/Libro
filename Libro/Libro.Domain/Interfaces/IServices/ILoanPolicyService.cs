using Libro.Domain.Entities;
using Libro.Domain.Models;

namespace Libro.Domain.Interfaces.IServices
{
    public interface ILoanPolicyService
    {
        LoanPolicy GetLoanPolicy();
        public int GetLoanDuration();
        decimal GetBorrowingFeePerDay();
        decimal GetLateFeePerDay();
        bool CanPatronCheckoutBook(Patron patron);
        decimal CalculateTotalFees(int checkoutDay, int dueDay, int returnDay);
    }
}
