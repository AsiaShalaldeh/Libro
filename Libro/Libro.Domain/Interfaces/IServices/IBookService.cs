using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Enums;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IBookService
    {
        Task<Book> GetBookByISBNAsync(string ISBN);
        Task<PaginatedResult<BookDto>> SearchBooksAsync(string title, string author,
            string genre, int pageNumber, int pageSize);
        Task<PaginatedResult<BookDto>> GetAllBooksAsync(int pageNumber, int pageSize);
        Task<BookDto> AddBookAsync(BookRequest bookDto);
        Task UpdateBookAsync(string ISBN, BookRequest bookDto);
        Task RemoveBookAsync(string ISBN);
        Task<IEnumerable<Book>> GetBooksByGenres(IEnumerable<Genre> genres);
    }
}
