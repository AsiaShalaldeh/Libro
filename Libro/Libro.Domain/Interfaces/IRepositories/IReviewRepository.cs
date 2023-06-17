using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IReviewRepository
    {
        Review GetReviewByIdAsync(string ISBN, int reviewId);
        Task<Review> UpdateReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(string ISBN, int reviewId);
        Task<Review> AddReviewAsync(Review review);
        Task<IEnumerable<Review>> GetReviewsByBookIdAsync(string bookId);
        //Task<IEnumerable<Review>> GetReviewsByPatronIdAsync(int patronId);
        Task<double> GetAverageRatingByBookIdAsync(string bookId);
    }

}
