using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface ILibrarianRepository
    {
        IEnumerable<Librarian> GetAllLibrariansAsync();
        Librarian GetLibrarianByIdAsync(string librarianId);
        Librarian AddLibrarianAsync(Librarian librarian);
        Task UpdateLibrarianAsync(Librarian librarian);
        Task DeleteLibrarianAsync(Librarian librarian);
    }
}
