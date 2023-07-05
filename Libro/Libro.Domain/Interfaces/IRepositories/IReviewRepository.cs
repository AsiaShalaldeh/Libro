using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IReviewRepository
    {
        Task<Review> GetBookReviewByIdAsync(string ISBN, int reviewId);
        Task<Review> GetBookReviewByPatronIdAsync(string ISBN, string patronId);
        Task<Review> UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(Review review);
        Task<Review> AddReviewAsync(Review review);
        Task<IEnumerable<Review>> GetBookReviewsByISBNAsync(string ISBN);
        Task<double> GetAverageRatingByBookISBNAsync(string ISBN);
    }

}
