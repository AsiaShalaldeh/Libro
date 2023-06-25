using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IPatronRepository
    {
        Task<Patron> GetPatronByIdAsync(string patronId);
        Task AddPatronAsync(Patron patron);
        Task<Patron> UpdatePatronAsync(Patron patron);
    }
}
