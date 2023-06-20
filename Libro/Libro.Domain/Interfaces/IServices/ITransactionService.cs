using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface ITransactionService
    {
        Task<Transaction> ReserveBookAsync(string ISBN, int patronId);
        Task<Transaction> CheckoutBookAsync(string ISBN, int patronId, int librarianId);
        Task<Transaction> ReturnBookAsync(string ISBN, int patronId);
        Transaction GetActiveTransaction(string ISBN, int patronId);
        Task<IEnumerable<Book>> GetOverdueBooksAsync();
        Task<IEnumerable<Book>> GetBorrowedBooksAsync();
        Task<Book> GetBorrowedBookByIdAsync(string ISBN);
    }
}
