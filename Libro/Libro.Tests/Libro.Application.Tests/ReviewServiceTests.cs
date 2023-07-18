using AutoMapper;
using Libro.Application.Services;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Tests.Libro.Application.Tests
{
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IPatronRepository> _patronRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ReviewService>> _loggerMock;
        private readonly IReviewService _reviewService;

        public ReviewServiceTests()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _patronRepositoryMock = new Mock<IPatronRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ReviewService>>();

            _reviewService = new ReviewService(
                _reviewRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _patronRepositoryMock.Object,
                _mapperMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetReviewByIdAsync_ValidISBNAndReviewId_ReturnsReview()
        {
            // Arrange
            string ISBN = "1234567890";
            int reviewId = 1;
            var book = new Book();
            var review = new Review();

            _bookRepositoryMock.Setup(mock => mock.GetBookByISBNAsync(ISBN)).ReturnsAsync(book);
            _reviewRepositoryMock.Setup(mock => mock.GetBookReviewByIdAsync(ISBN, reviewId)).ReturnsAsync(review);

            // Act
            var result = await _reviewService.GetReviewByIdAsync(ISBN, reviewId);

            // Assert
            _bookRepositoryMock.Verify(mock => mock.GetBookByISBNAsync(ISBN), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.GetBookReviewByIdAsync(ISBN, reviewId), Times.Once);
            Assert.Equal(review, result);
        }

        [Fact]
        public async Task GetReviewByIdAsync_InvalidISBN_ThrowsResourceNotFoundException()
        {
            // Arrange
            string ISBN = "3567890123";
            int reviewId = 1;
            Book book = null; 

            _bookRepositoryMock.Setup(mock => mock.GetBookByISBNAsync(ISBN)).ReturnsAsync(book);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _reviewService.GetReviewByIdAsync(ISBN, reviewId));
            _bookRepositoryMock.Verify(mock => mock.GetBookByISBNAsync(ISBN), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.GetBookReviewByIdAsync(ISBN, reviewId), Times.Never);
        }

        [Fact]
        public async Task UpdateReviewAsync_ExistingReview_ReturnsUpdatedReview()
        {
            // Arrange
            string ISBN = "1234567890";
            var book = new Book();
            int reviewId = 1;
            var existingReview = new Review() { Rating = 5, Comment = "Great book!" };
            var reviewDto = new ReviewDto { Rating = 4, Comment = "Good book!" };
            var updatedReview = new Review { Rating = reviewDto.Rating, Comment = reviewDto.Comment };

            _bookRepositoryMock.Setup(mock => mock.GetBookByISBNAsync(ISBN)).ReturnsAsync(book);
            _reviewRepositoryMock.Setup(mock => mock.GetBookReviewByIdAsync(ISBN, reviewId)).ReturnsAsync(existingReview);
            _reviewRepositoryMock.Setup(mock => mock.UpdateReviewAsync(existingReview)).ReturnsAsync(updatedReview);

            // Act
            var result = await _reviewService.UpdateReviewAsync(ISBN, reviewId, reviewDto);

            // Assert
            _bookRepositoryMock.Verify(mock => mock.GetBookByISBNAsync(ISBN), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.GetBookReviewByIdAsync(ISBN, reviewId), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.UpdateReviewAsync(existingReview), Times.Once);
            Assert.Equal(updatedReview, result);
        }

        [Fact]
        public async Task UpdateReviewAsync_NonExistingReview_ThrowsResourceNotFoundException()
        {
            // Arrange
            string ISBN = "1234567890";
            var book = new Book();
            int reviewId = 1;
            Review existingReview = null;
            var reviewDto = new ReviewDto { Rating = 5, Comment = "Great book!" };

            _bookRepositoryMock.Setup(mock => mock.GetBookByISBNAsync(ISBN)).ReturnsAsync(book);
            _reviewRepositoryMock.Setup(mock => mock.GetBookReviewByIdAsync(ISBN, reviewId)).ReturnsAsync(existingReview);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _reviewService.UpdateReviewAsync(ISBN, reviewId, reviewDto));
            _bookRepositoryMock.Verify(mock => mock.GetBookByISBNAsync(ISBN), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.GetBookReviewByIdAsync(ISBN, reviewId), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.UpdateReviewAsync(existingReview), Times.Never);
        }

        [Fact]
        public async Task DeleteReviewAsync_ExistingReview_DeletesReview()
        {
            // Arrange
            string ISBN = "1234567890";
            var book = new Book();
            int reviewId = 1;
            var review = new Review();

            _bookRepositoryMock.Setup(mock => mock.GetBookByISBNAsync(ISBN)).ReturnsAsync(book);
            _reviewRepositoryMock.Setup(mock => mock.GetBookReviewByIdAsync(ISBN, reviewId)).ReturnsAsync(review);

            // Act
            await _reviewService.DeleteReviewAsync(ISBN, reviewId);

            // Assert
            _bookRepositoryMock.Verify(mock => mock.GetBookByISBNAsync(ISBN), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.GetBookReviewByIdAsync(ISBN, reviewId), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.DeleteReviewAsync(review), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_NonExistingReview_ThrowsResourceNotFoundException()
        {
            // Arrange
            string ISBN = "1234567890";
            int reviewId = 1;
            var book = new Book();
            Review review = null; 

            _bookRepositoryMock.Setup(mock => mock.GetBookByISBNAsync(ISBN)).ReturnsAsync(book);
            _reviewRepositoryMock.Setup(mock => mock.GetBookReviewByIdAsync(ISBN, reviewId)).ReturnsAsync(review);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _reviewService.DeleteReviewAsync(ISBN, reviewId));
            _bookRepositoryMock.Verify(mock => mock.GetBookByISBNAsync(ISBN), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.GetBookReviewByIdAsync(ISBN, reviewId), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.DeleteReviewAsync(review), Times.Never);
        }

        [Fact]
        public async Task AddReviewAsync_ValidISBNAndReviewDto_AddsReview()
        {
            // Arrange
            string ISBN = "1234567890";
            var book = new Book();
            var reviewDto = new ReviewDto { Rating = 5, Comment = "Great book!" };
            var patronId = "12345";
            var patron = new Patron();
            var review = new Review();

            _bookRepositoryMock.Setup(mock => mock.GetBookByISBNAsync(ISBN)).ReturnsAsync(book);
            _userRepositoryMock.Setup(mock => mock.GetCurrentUserIdAsync()).ReturnsAsync(patronId);
            _patronRepositoryMock.Setup(mock => mock.GetPatronByIdAsync(patronId)).ReturnsAsync(patron);
            _reviewRepositoryMock.Setup(mock => mock.AddReviewAsync(It.IsAny<Review>())).ReturnsAsync(review);
            _mapperMock.Setup(mock => mock.Map<ReviewDto>(review)).Returns(reviewDto);

            // Act
            var result = await _reviewService.AddReviewAsync(ISBN, reviewDto);

            // Assert
            _bookRepositoryMock.Verify(mock => mock.GetBookByISBNAsync(ISBN), Times.Once);
            _userRepositoryMock.Verify(mock => mock.GetCurrentUserIdAsync(), Times.Once);
            _patronRepositoryMock.Verify(mock => mock.GetPatronByIdAsync(patronId), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.AddReviewAsync(It.IsAny<Review>()), Times.Once);
            _mapperMock.Verify(mock => mock.Map<ReviewDto>(review), Times.Once);
            Assert.Equal(reviewDto, result);
        }

        [Fact]
        public async Task AddReviewAsync_InvalidISBN_ThrowsResourceNotFoundException()
        {
            // Arrange
            string ISBN = "3456789012";
            Book book = null;
            var reviewDto = new ReviewDto { Rating = 5, Comment = "Great book!" };

            _bookRepositoryMock.Setup(mock => mock.GetBookByISBNAsync(ISBN)).ReturnsAsync(book);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _reviewService.AddReviewAsync(ISBN, reviewDto));
            _bookRepositoryMock.Verify(mock => mock.GetBookByISBNAsync(ISBN), Times.Once);
            _userRepositoryMock.Verify(mock => mock.GetCurrentUserIdAsync(), Times.Never);
            _patronRepositoryMock.Verify(mock => mock.GetPatronByIdAsync(It.IsAny<string>()), Times.Never);
            _reviewRepositoryMock.Verify(mock => mock.AddReviewAsync(It.IsAny<Review>()), Times.Never);
            _mapperMock.Verify(mock => mock.Map<ReviewDto>(It.IsAny<Review>()), Times.Never);
        }
        [Fact]
        public async Task GetReviewsByBookIdAsync_ExistingBook_ReturnsListOfReviews()
        {
            // Arrange
            string ISBN = "1234567890";
            var book = new Book();
            var reviews = new List<Review> { new Review(), new Review(), new Review() };

            _bookRepositoryMock.Setup(mock => mock.GetBookByISBNAsync(ISBN)).ReturnsAsync(book);
            _reviewRepositoryMock.Setup(mock => mock.GetBookReviewsByISBNAsync(ISBN)).ReturnsAsync(reviews);

            // Act
            var result = await _reviewService.GetReviewsByBookIdAsync(ISBN);

            // Assert
            _bookRepositoryMock.Verify(mock => mock.GetBookByISBNAsync(ISBN), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.GetBookReviewsByISBNAsync(ISBN), Times.Once);
            Assert.Equal(reviews, result);
        }

        [Fact]
        public async Task GetReviewsByBookIdAsync_NonExistingBook_ThrowsResourceNotFoundException()
        {
            // Arrange
            string ISBN = "3456789012";
            Book book = null; 

            _bookRepositoryMock.Setup(mock => mock.GetBookByISBNAsync(ISBN)).ReturnsAsync(book);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _reviewService.GetReviewsByBookIdAsync(ISBN));
            _bookRepositoryMock.Verify(mock => mock.GetBookByISBNAsync(ISBN), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.GetBookReviewsByISBNAsync(ISBN), Times.Never);
        }

        [Fact]
        public async Task GetAverageRatingByBookIdAsync_ExistingBook_ReturnsAverageRating()
        {
            // Arrange
            string ISBN = "1234567890";
            var book = new Book();
            double averageRating = 4.5;

            _bookRepositoryMock.Setup(mock => mock.GetBookByISBNAsync(ISBN)).ReturnsAsync(book);
            _reviewRepositoryMock.Setup(mock => mock.GetAverageRatingByBookISBNAsync(ISBN)).ReturnsAsync(averageRating);

            // Act
            var result = await _reviewService.GetAverageRatingByBookIdAsync(ISBN);

            // Assert
            _bookRepositoryMock.Verify(mock => mock.GetBookByISBNAsync(ISBN), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.GetAverageRatingByBookISBNAsync(ISBN), Times.Once);
            Assert.Equal(averageRating, result);
        }

        [Fact]
        public async Task GetAverageRatingByBookIdAsync_NonExistingBook_ThrowsResourceNotFoundException()
        {
            // Arrange
            string ISBN = "3456789012";
            Book book = null; 

            _bookRepositoryMock.Setup(mock => mock.GetBookByISBNAsync(ISBN)).ReturnsAsync(book);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _reviewService.GetAverageRatingByBookIdAsync(ISBN));
            _bookRepositoryMock.Verify(mock => mock.GetBookByISBNAsync(ISBN), Times.Once);
            _reviewRepositoryMock.Verify(mock => mock.GetAverageRatingByBookISBNAsync(ISBN), Times.Never);
        }
    }
}
