using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IBookRecommendationService
    {
        Task<IEnumerable<Book>> GetRecommendedBooks(string patronId);
    }
}
