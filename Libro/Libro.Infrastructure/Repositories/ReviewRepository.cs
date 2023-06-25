using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly List<Review> _reviews;

        public ReviewRepository()
        {
            // Add some fake reviews
            _reviews = new List<Review>
            {
                new Review { ReviewId = 1, PatronId = "1", BookId = "1234567890", Rating = 4, Comment = "Great book!" },
                new Review { ReviewId = 2, PatronId = "2", BookId = "1234567890", Rating = 5, Comment = "Highly recommended!" },
                new Review { ReviewId = 3, PatronId = "3", BookId = "ISBN3", Rating = 3, Comment = "Average read." }
            };
        }
        public Review GetReviewByIdAsync(string ISBN, int reviewId)
        {
            return _reviews.FirstOrDefault(r => r.BookId == ISBN 
                                            && r.ReviewId == reviewId);
        }

        public async Task<Review> UpdateReviewAsync(Review review)
        {
            // Update the DbSet and Save Changes 
            return await Task.FromResult(review);
        }

        public async Task<bool> DeleteReviewAsync(string ISBN, int reviewId)
        {
            var review = GetReviewByIdAsync(ISBN, reviewId);
            if (review != null)
            {
                _reviews.Remove(review);
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }
        public async Task<Review> AddReviewAsync(Review review)
        {
            // will be removed after adding DbContext
            review.ReviewId = GetNextReviewId();
            _reviews.Add(review);
            return review;
        }

        public async Task<IEnumerable<Review>> GetReviewsByBookIdAsync(string bookId)
        {
            return _reviews.Where(r => r.BookId == bookId);
        }
        public async Task<double> GetAverageRatingByBookIdAsync(string bookId)
        {
            var bookReviews = _reviews.Where(r => r.BookId == bookId);
            if (bookReviews.Any())
            {
                return bookReviews.Average(r => r.Rating);
            }
            return 0;
        }
        private int GetNextReviewId()
        {
            if (_reviews.Any())
            {
                return _reviews.Max(r => r.ReviewId) + 1;
            }
            return 1;
        }
    }

}
