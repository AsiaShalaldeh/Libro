using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Enums;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IBookRepository
    {
        Task<Book> GetBookByISBNAsync(string ISBN);
        Task<PaginatedResult<Book>> SearchBooksAsync(string title, string author, string genre, 
            int pageNumber, int pageSize);
        Task<PaginatedResult<Book>> GetAllBooksAsync(int pageNumber, int pageSize);
        Task AddBookAsync(Book book, Author author);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(Book book);
        Task<IEnumerable<Book>> GetBooksByGenres(IEnumerable<Genre> genres);
    }
}
