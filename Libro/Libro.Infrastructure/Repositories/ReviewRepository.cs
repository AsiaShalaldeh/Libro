using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libro.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly LibroDbContext _context;
        private readonly ILogger<ReviewRepository> _logger;

        public ReviewRepository(LibroDbContext context, ILogger<ReviewRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Review> GetBookReviewByIdAsync(string ISBN, int reviewId)
        {
            try
            {
                // PatronId and BookId should be provided and not given null
                return await _context.Reviews.FirstOrDefaultAsync(r => r.BookId.Equals(ISBN)
                                            && r.ReviewId == reviewId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReviewRepository while getting the review with ID: {reviewId} for book with ISBN: {ISBN}.");
                throw;
            }
        }

        public async Task<Review> UpdateReviewAsync(Review review)
        {
            try
            {
                _context.Reviews.Update(review);
                await _context.SaveChangesAsync();
                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReviewRepository while updating the review with ID: {review.ReviewId}.");
                throw;
            }
        }

        public async Task DeleteReviewAsync(Review review)
        {
            try
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReviewRepository while deleting the review with ID: {review.ReviewId}.");
                throw;
            }
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            try
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ReviewRepository while adding a review.");
                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetBookReviewsByISBNAsync(string ISBN)
        {
            try
            {
                return await _context.Reviews.Where(r => r.BookId.Equals(ISBN)).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReviewRepository while getting reviews for the book with ISBN: {ISBN}.");
                throw;
            }
        }

        public async Task<double> GetAverageRatingByBookISBNAsync(string ISBN)
        {
            try
            {
                var bookReviews = await GetBookReviewsByISBNAsync(ISBN);
                if (bookReviews.Any())
                {
                    return bookReviews.Average(r => r.Rating);
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReviewRepository while getting the average rating for the book with ISBN: {ISBN}.");
                throw;
            }
        }
    }
}
