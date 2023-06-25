using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IPatronService
    {
        Task<Patron> GetPatronAsync(string patronId);
        Task AddPatronAsync(string patronId, string name, string email);
        Task<Patron> UpdatePatronAsync(string patronId, PatronDto patronDto);
        Task<IEnumerable<Checkout>> GetBorrowingHistoryAsync(string patronId);
    }
}
