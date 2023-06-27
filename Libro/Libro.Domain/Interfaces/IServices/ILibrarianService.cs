using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface ILibrarianService
    {
        Task<PaginatedResult<Librarian>> GetAllLibrariansAsync(int pageNumber, int pageSize);
        Task<Librarian> GetLibrarianByIdAsync(string librarianId);
        Task<Librarian> AddLibrarianAsync(string librarianId, string name);
        Task<Librarian> UpdateLibrarianAsync(string librarianId, LibrarianDto librarian);
        Task DeleteLibrarianAsync(string librarianId);
    }

}
