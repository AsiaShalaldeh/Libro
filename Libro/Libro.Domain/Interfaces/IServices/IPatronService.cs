using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IPatronService
    {
        Task<Patron> GetPatronProfileAsync(int patronId);
        Task<Patron> UpdatePatronProfileAsync(Patron patron);
    }
}
