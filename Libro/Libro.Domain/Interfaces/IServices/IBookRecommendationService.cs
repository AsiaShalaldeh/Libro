using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IBookRecommendationService
    {
        Task<IEnumerable<BookDto>> GetRecommendedBooks(string patronId);
    }
}
