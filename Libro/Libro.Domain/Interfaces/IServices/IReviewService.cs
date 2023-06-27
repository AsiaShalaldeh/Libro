using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IReviewService
    {
        Task<Review> GetReviewByIdAsync(string ISBN, int reviewId);
        Task<Review> UpdateReviewAsync(string ISBN, int reviewId, ReviewDto reviewDto);
        Task DeleteReviewAsync(string ISBN, int reviewId);
        Task<ReviewDto> AddReviewAsync(string ISBN, ReviewDto reviewDto);
        Task<IEnumerable<Review>> GetReviewsByBookIdAsync(string bookId);
        Task<double> GetAverageRatingByBookIdAsync(string bookId);
    }

}
