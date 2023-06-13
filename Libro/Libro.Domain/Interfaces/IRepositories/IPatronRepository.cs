using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IPatronRepository
    {
        Patron GetPatronByIdAsync(int patronId); // Will be replaced by Task<>
        Patron UpdatePatronAsync(Patron patron);
    }
}
