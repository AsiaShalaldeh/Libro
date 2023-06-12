using Libro.Domain.Common;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IBookRepository
    {
        Task<Book> GetByIdAsync(string ISBN);
        Task<PaginatedResult<Book>> SearchAsync(string title, string author, string genre, 
            int pageNumber, int pageSize);
        Task<PaginatedResult<Book>> GetAllAsync(int pageNumber, int pageSize);
        Task<Transaction> ReserveAsync(string ISBN, int patronId);
        Task<Transaction> CheckoutAsync(string ISBN, int patronId, int librarianId);
        Task<Transaction> ReturnAsync(string ISBN, int patronId);
        IEnumerable<string> GetOverdueBooksAsync(); // Will be replaced with Task<>
        IEnumerable<string> GetBorrowedBooksAsync();
        string GetBorrowedBookByIdAsync(string ISBN);
    }
}
