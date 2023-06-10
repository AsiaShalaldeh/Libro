using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IBookService
    {
        Task<Book> GetBookByIdAsync(string ISBN);
        Task<PaginatedResult<BookDto>> SearchBooksAsync(string searchTerm, int pageNumber, int pageSize);
        Task<PaginatedResult<BookDto>> GetAllBooksAsync(int pageNumber, int pageSize);
    }
}
