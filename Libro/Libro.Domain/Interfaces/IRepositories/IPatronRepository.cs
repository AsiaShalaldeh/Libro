using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IPatronRepository
    {
        Task<List<Patron>> GetAllPatrons();
        Task<Patron> GetPatronByIdAsync(string patronId);
        Task AddPatronAsync(Patron patron);
        Task<Patron> UpdatePatronAsync(Patron patron);
    }
}
