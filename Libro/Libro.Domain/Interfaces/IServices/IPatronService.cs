using Libro.Domain.Dtos;
using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Models;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IPatronService
    {
        Task<PaginatedResult<PatronDto>> GetAllPatrons(int pageNumber, int pageSize);
        Task<Patron> GetPatronAsync(string patronId);
        Task AddPatronAsync(string patronId, string name, string email);
        Task<PatronDto> UpdatePatronAsync(string patronId, PatronDto patronDto);
        Task<IEnumerable<TransactionResponseModel>> GetBorrowingHistoryAsync(string patronId);
    }
}
