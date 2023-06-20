using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface ITransactionRepository
    {
        Transaction GetActiveTransaction(string ISBN, int patronId);
        void UpdateTransaction(Transaction transaction);
        void AddTransaction(Transaction transaction);
        Task<Transaction> ReserveAsync(string ISBN, int patronId);
        Task<Transaction> CheckoutAsync(string ISBN, int patronId, int librarianId);
        Task<Transaction> ReturnAsync(string ISBN, int patronId);
        IEnumerable<string> GetOverdueBooksAsync(); // Will be replaced with Task<>
        IEnumerable<string> GetBorrowedBooksAsync();
        string GetBorrowedBookByIdAsync(string ISBN);
    }
}
