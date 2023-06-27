using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly LibroDbContext _context;

        public ReviewRepository(LibroDbContext context)
        {
            _context = context;
        }
        public async Task<Review> GetBookReviewByIdAsync(string ISBN, int reviewId)
        {
            return await _context.Reviews.FirstOrDefaultAsync(r => r.BookId.Equals(ISBN) 
                                            && r.ReviewId == reviewId);
        }

        public async Task<Review> UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task DeleteReviewAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
        public async Task<Review> AddReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<IEnumerable<Review>> GetBookReviewsByISBNAsync(string ISBN)
        {
            return await _context.Reviews.Where(r => r.BookId.Equals(ISBN)).ToListAsync();
        }
        public async Task<double> GetAverageRatingByBookISBNAsync(string ISBN)
        {
            var bookReviews = await GetBookReviewsByISBNAsync(ISBN);
            if (bookReviews.Any())
            {
                return bookReviews.Average(r => r.Rating);
            }
            return 0;
        }
    }

}
