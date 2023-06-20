using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Enums;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IBookService
    {
        Task<Book> GetBookByIdAsync(string ISBN);
        Task<PaginatedResult<BookDto>> SearchBooksAsync(string title, string author,
            string genre, int pageNumber, int pageSize);
        Task<PaginatedResult<BookDto>> GetAllBooksAsync(int pageNumber, int pageSize);
        Task AddBookAsync(RequestBookDto bookDto);
        Task UpdateBookAsync(string isbn, RequestBookDto bookDto);
        Task RemoveBookAsync(string bookId);
        Task<IEnumerable<Book>> GetBooksByGenres(IEnumerable<Genre> genres);
    }
}
