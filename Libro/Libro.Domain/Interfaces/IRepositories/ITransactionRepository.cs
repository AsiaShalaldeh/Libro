using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface ITransactionRepository
    {
        Checkout GetActiveTransaction(string ISBN, string patronId);
        //void UpdateTransaction(Transaction transaction);
        //void AddTransaction(Transaction transaction);
        IEnumerable<string> GetOverdueBooksAsync(); // Will be replaced with Task<>
        IEnumerable<string> GetBorrowedBooksAsync();
        string GetBorrowedBookByIdAsync(string ISBN);
        Task<IEnumerable<Checkout>> GetTransactionsByPatron(string patronId);
        public IEnumerable<Checkout> GetOverdueTransactionsAsync();
    }
}
