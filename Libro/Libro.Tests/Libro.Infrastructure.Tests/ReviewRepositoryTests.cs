using Libro.Domain.Entities;
using Libro.Infrastructure.Data;
using Libro.Infrastructure.Repositories;
using Libro.Tests.MockData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Libro.Tests.Libro.Infrastructure.Tests
{
    public class ReviewRepositoryTests
    {
        private readonly ReviewRepository _reviewRepository;
        private readonly Mock<IConfiguration> _configuration;
        private readonly LibroDbContext _dbContext;

        public ReviewRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibroDB")
                .Options;
            _configuration = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<ReviewRepository>>();
            _dbContext = new LibroDbContext(options, _configuration.Object);

            // Initialize test data
            ReviewMockData.InitializeTestData(_dbContext);

            _reviewRepository = new ReviewRepository(_dbContext, loggerMock.Object);
        }

        [Fact]
        public async Task GetBookReviewByIdAsync_ValidISBNAndReviewId_ReturnsReview()
        {
            // Arrange
            string ISBN = "9781234567890";
            int reviewId = 1;

            // Act
            Review review = await _reviewRepository.GetBookReviewByIdAsync(ISBN, reviewId);

            // Assert
            Assert.NotNull(review);
            Assert.Equal(ISBN, review.BookId);
            Assert.Equal(reviewId, review.ReviewId);
        }

        [Fact]
        public async Task UpdateReviewAsync_UpdatesExistingReviewInDatabase()
        {
            // Arrange
            int reviewId = 1;
            string updatedComment = "Updated comment";
            var review = await _reviewRepository.GetBookReviewByIdAsync("9781234567890", reviewId);
            review.Comment = updatedComment;

            // Act
            await _reviewRepository.UpdateReviewAsync(review);
            var updatedReview = await _reviewRepository.GetBookReviewByIdAsync("9781234567890", reviewId);

            // Assert
            Assert.NotNull(updatedReview);
            Assert.Equal(updatedComment, updatedReview.Comment);
        }

        [Fact]
        public async Task DeleteReviewAsync_DeletesReviewFromDatabase()
        {
            // Arrange
            int reviewId = 1;
            var review = await _reviewRepository.GetBookReviewByIdAsync("9781234567890", reviewId);

            // Act
            await _reviewRepository.DeleteReviewAsync(review);
            var deletedReview = await _reviewRepository.GetBookReviewByIdAsync("9781234567890", reviewId);

            // Assert
            Assert.Null(deletedReview);
        }

        [Fact]
        public async Task AddReviewAsync_AddsNewReviewToDatabase()
        {
            // Arrange
            var review = new Review
            {
                BookId = "9780987654321",
                ReviewId = 4,
                Rating = 5,
                Comment = "Great book",
                PatronId = "3"
            };

            // Act
            var result = await _reviewRepository.AddReviewAsync(review);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(review.BookId, result.BookId);
            Assert.Equal(review.ReviewId, result.ReviewId);
            Assert.Equal(review.PatronId, result.PatronId);

            // Check if the review is actually added to the database
            var addedReview = await _reviewRepository.GetBookReviewByIdAsync(review.BookId, review.ReviewId);
            Assert.NotNull(addedReview);
            Assert.Equal(review.Comment, addedReview.Comment);
        }

        [Fact]
        public async Task GetBookReviewsByISBNAsync_ValidISBN_ReturnsReviews()
        {
            // Arrange
            string ISBN = "9780987654321";

            // Act
            var reviews = await _reviewRepository.GetBookReviewsByISBNAsync(ISBN);

            // Assert
            Assert.NotNull(reviews);
            Assert.NotEmpty(reviews);
            Assert.All(reviews, review => Assert.Equal(ISBN, review.BookId));
        }

        [Fact]
        public async Task GetAverageRatingByBookISBNAsync_ValidISBN_ReturnsAverageRating()
        {
            // Arrange
            string ISBN = "9781234567890";

            // Act
            var averageRating = await _reviewRepository.GetAverageRatingByBookISBNAsync(ISBN);

            // Assert
            Assert.Equal(3.5, averageRating);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
