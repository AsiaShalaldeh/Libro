using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface ILibrarianService
    {
        Task<IEnumerable<Librarian>> GetAllLibrariansAsync();
        Task<Librarian> GetLibrarianByIdAsync(string librarianId);
        Task<Librarian> AddLibrarianAsync(Librarian librarian);
        void UpdateLibrarianAsync(Librarian librarian);
        void DeleteLibrarianAsync(string librarianId);
    }

}
