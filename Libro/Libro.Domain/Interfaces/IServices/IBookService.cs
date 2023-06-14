using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IBookService
    {
        Task<Book> GetBookByIdAsync(string ISBN);
        Task<PaginatedResult<BookDto>> SearchBooksAsync(string title, string author,
            string genre, int pageNumber, int pageSize);
        Task<PaginatedResult<BookDto>> GetAllBooksAsync(int pageNumber, int pageSize);
        Task<Transaction> ReserveBookAsync(string isbn, int patronId);
        Task<Transaction> CheckoutBookAsync(string isbn, int patronId, int librarianId);
        Task<Transaction> ReturnBookAsync(string isbn, int patronId);
        Task<IEnumerable<Book>> GetOverdueBooksAsync();
        Task<IEnumerable<Book>> GetBorrowedBooksAsync();
        Task<Book> GetBorrowedBookByIdAsync(string ISBN);
        Task AddBookAsync(RequestBookDto bookDto);
        Task UpdateBookAsync(string isbn, RequestBookDto bookDto);
        Task RemoveBookAsync(string bookId);
    }
}
