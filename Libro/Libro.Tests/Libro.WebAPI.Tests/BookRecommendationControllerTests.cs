using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Libro.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Libro.Tests.Libro.WebAPI.Tests
{
    public class BookRecommendationControllerTests
    {
        private readonly BookRecommendationController _controller;
        private readonly Mock<IBookRecommendationService> _recommendationServiceMock;
        private readonly Mock<ILogger<BookRecommendationController>> _loggerMock;

        public BookRecommendationControllerTests()
        {
            _recommendationServiceMock = new Mock<IBookRecommendationService>();
            _loggerMock = new Mock<ILogger<BookRecommendationController>>();
            _controller = new BookRecommendationController(
                _recommendationServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetRecommendedBooks_ValidPatronId_ShouldReturnOkResult()
        {
            // Arrange
            var patronId = "123";
            var recommendedBooks = new List<BookDto>
            {
                new BookDto { ISBN = "1234567890", Title = "Book 1", Genre = "Romance"},
                new BookDto { ISBN = "4567890123", Title = "Book 2", Genre = "Drama" },
                new BookDto { ISBN = "7890123456", Title = "Book 3", Genre = "Poetry"}
            };
            _recommendationServiceMock.Setup(service => service.GetRecommendedBooks(patronId))
                .ReturnsAsync(recommendedBooks);

            // Act
            var result = await _controller.GetRecommendedBooks(patronId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(recommendedBooks);
        }

        [Fact]
        public async Task GetRecommendedBooks_NonExistingPatronId_ShouldReturnBadRequestResult()
        {
            // Arrange
            var patronId = "123";
            var resourceNotFoundException = new ResourceNotFoundException("Patron", "ID", patronId);
            _recommendationServiceMock.Setup(service => service.GetRecommendedBooks(patronId))
                .Throws(resourceNotFoundException);

            // Act
            var result = await _controller.GetRecommendedBooks(patronId);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be(resourceNotFoundException.Message);
        }
    }
}
