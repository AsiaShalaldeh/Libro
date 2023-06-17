using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IReviewService
    {
        Task<Review> GetReviewByIdAsync(string ISBN, int reviewId);
        Task<Review> UpdateReviewAsync(string ISBN, int reviewId, ReviewDto reviewDto);
        Task<bool> DeleteReviewAsync(string ISBN, int reviewId);
        Task<Review> AddReviewAsync(ReviewDto reviewDto);
        Task<IEnumerable<Review>> GetReviewsByBookIdAsync(string bookId);
        //Task<IEnumerable<Review>> GetReviewsByPatronIdAsync(int patronId);
        Task<double> GetAverageRatingByBookIdAsync(string bookId);
    }

}
