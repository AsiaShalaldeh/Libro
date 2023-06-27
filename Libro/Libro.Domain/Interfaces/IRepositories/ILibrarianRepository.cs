using Libro.Domain.Common;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface ILibrarianRepository
    {
        Task<PaginatedResult<Librarian>> GetAllLibrariansAsync(int pageNumber, int pageSize);
        Task<Librarian> GetLibrarianByIdAsync(string librarianId);
        Task<Librarian> AddLibrarianAsync(Librarian librarian);
        Task<Librarian> UpdateLibrarianAsync(Librarian librarian);
        Task DeleteLibrarianAsync(Librarian librarian);
    }
}
