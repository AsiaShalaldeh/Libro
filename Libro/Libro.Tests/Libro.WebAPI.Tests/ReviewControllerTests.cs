using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Libro.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Libro.Tests.Libro.WebAPI.Tests
{
    public class ReviewControllerTests
    {
        private readonly Mock<IReviewService> _reviewServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ReviewController>> _loggerMock;
        private readonly ReviewController _reviewController;

        public ReviewControllerTests()
        {
            _reviewServiceMock = new Mock<IReviewService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ReviewController>>();

            _reviewController = new ReviewController(_reviewServiceMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetReview_ReviewFound_ReturnsOkResult()
        {
            // Arrange
            string isbn = "1234567890";
            int reviewId = 1;
            var review = new Review();
            var reviewDto = new ReviewDto();

            string logMessage = $"Retrieved review with ID {reviewId} for book with ISBN {isbn}.";
            string capturedLogMessage = null;

            _reviewServiceMock.Setup(x => x.GetReviewByIdAsync(isbn, reviewId)).ReturnsAsync(review);
            _mapperMock.Setup(x => x.Map<ReviewDto>(review)).Returns(reviewDto);

            // Act
            var result = await _reviewController.GetReview(isbn, reviewId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(reviewDto, okResult.Value);
        }

        [Fact]
        public async Task GetReview_ReviewNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            string isbn = "1234567890";
            int reviewId = 1;
            string message = "No Review found with ID = " + reviewId;

            _reviewServiceMock.Setup(x => x.GetReviewByIdAsync(isbn, reviewId)).ReturnsAsync((Review)null);

            // Act
            var result = await _reviewController.GetReview(isbn, reviewId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal(message, notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateReview_ReviewUpdated_ReturnsOkResult()
        {
            // Arrange
            string isbn = "1234567890";
            int reviewId = 1;
            var reviewDto = new ReviewDto() { Comment = "Bad Book !!"};
            var updatedReview = new Review();

            _reviewServiceMock.Setup(x => x.UpdateReviewAsync(isbn, reviewId, reviewDto)).ReturnsAsync(updatedReview);
            _mapperMock.Setup(x => x.Map<ReviewDto>(updatedReview)).Returns(reviewDto);

            // Act
            var result = await _reviewController.UpdateReview(isbn, reviewId, reviewDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(reviewDto, okResult.Value);
        }

        [Fact]
        public async Task UpdateReview_ReviewNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            string isbn = "1234567890";
            int reviewId = 1;
            var reviewDto = new ReviewDto() { Comment = "Very Bad Book !!"};
            string message = "No Review found with ID = " + reviewId;

            _reviewServiceMock.Setup(x => x.UpdateReviewAsync(isbn, reviewId, reviewDto)).ThrowsAsync(new ResourceNotFoundException("Review", "ID", reviewId.ToString()));

            // Act
            var result = await _reviewController.UpdateReview(isbn, reviewId, reviewDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal(message, notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteReview_ReviewDeleted_ReturnsNoContentResult()
        {
            // Arrange
            string isbn = "1234567890";
            int reviewId = 1;

            // Act
            var result = await _reviewController.DeleteReview(isbn, reviewId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteReview_ReviewNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            string isbn = "1234567890";
            int reviewId = 1;
            string message = "No Review found with ID = " + reviewId;

            _reviewServiceMock.Setup(x => x.DeleteReviewAsync(isbn, reviewId)).ThrowsAsync(new ResourceNotFoundException("Review", "ID", reviewId.ToString()));

            // Act
            var result = await _reviewController.DeleteReview(isbn, reviewId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal(message, notFoundResult.Value);
        }

        [Fact]
        public async Task AddReview_ReviewAdded_ReturnsOkResult()
        {
            // Arrange
            string isbn = "1234567890";
            var reviewDto = new ReviewDto() { BookId = isbn, Rating = 3, Comment = "Good Book" };
            var addedReview = new ReviewDto();

            _reviewServiceMock.Setup(x => x.AddReviewAsync(isbn, reviewDto)).Returns(Task.FromResult<ReviewDto>(addedReview));

            // Act
            var result = await _reviewController.AddReview(isbn, reviewDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(addedReview, okResult.Value);
        }

        [Fact]
        public async Task AddReview_BookNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            string isbn = "1234567890";
            var reviewDto = new ReviewDto() { BookId = isbn, Rating = 3, Comment = "Good Book" };
            string message = "No Book found with ISBN = " + isbn;

            _reviewServiceMock.Setup(x => x.AddReviewAsync(isbn, reviewDto)).ThrowsAsync(new ResourceNotFoundException("Book", "ISBN", isbn));

            // Act
            var result = await _reviewController.AddReview(isbn, reviewDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal(message, notFoundResult.Value);
        }
        [Fact]
        public async Task GetReviewsByBookISBN_ReviewsFound_ReturnsOkResult()
        {
            // Arrange
            string isbn = "1234567890";
            var reviews = new List<Review>
            { 
                new Review() { ReviewId = 1, BookId = isbn, PatronId = "12", Comment = "Good one", Rating = 3 },
                new Review() { ReviewId = 2, BookId = isbn, PatronId = "13", Comment = "Boring !!", Rating = 1 }
            };
            var reviewsDto = new List<ReviewDto> { new ReviewDto(), new ReviewDto() };

            _reviewServiceMock.Setup(x => x.GetReviewsByBookIdAsync(isbn)).ReturnsAsync(reviews);
            _mapperMock.Setup(x => x.Map<IEnumerable<ReviewDto>>(reviews)).Returns(reviewsDto);

            // Act
            var result = await _reviewController.GetReviewsByBookISBN(isbn);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(reviewsDto, okResult.Value);
            Assert.Equal(reviews.Count, reviewsDto.Count);
        }

        [Fact]
        public async Task GetReviewsByBookISBN_NoReviewsFound_ReturnsNotFoundResult()
        {
            // Arrange
            string isbn = "1234567890";
            var reviews = Enumerable.Empty<Review>();

            _reviewServiceMock.Setup(x => x.GetReviewsByBookIdAsync(isbn)).ReturnsAsync(reviews);

            // Act
            var result = await _reviewController.GetReviewsByBookISBN(isbn);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal("No reviews found.", notFoundResult.Value);
        }
        [Fact]
        public async Task GetAverageRatingByBookId_BookFound_ReturnsOkResult()
        {
            // Arrange
            string isbn = "1234567890";
            double averageRating = 4.5;

            _reviewServiceMock.Setup(x => x.GetAverageRatingByBookIdAsync(isbn)).ReturnsAsync(averageRating);

            // Act
            var result = await _reviewController.GetAverageRatingByBookId(isbn);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal("Average = " + averageRating, okResult.Value);
        }

        [Fact]
        public async Task GetAverageRatingByBookId_BookNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            string isbn = "1234567890";
            string message = "No Book found with ISBN = " + isbn;

            _reviewServiceMock.Setup(x => x.GetAverageRatingByBookIdAsync(isbn)).ThrowsAsync(new ResourceNotFoundException("Book", "ISBN", isbn));

            // Act
            var result = await _reviewController.GetAverageRatingByBookId(isbn);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal(message, notFoundResult.Value);
        }
    }
}
