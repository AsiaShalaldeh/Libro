using AutoMapper;
using FluentValidation;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Libro.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.Tests.Libro.WebAPI.Tests
{
    public class PatronControllerTests
    {
        private readonly PatronController _patronController;
        private readonly Mock<IPatronService> _patronServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PatronController>> _loggerMock;

        public PatronControllerTests()
        {
            _patronServiceMock = new Mock<IPatronService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PatronController>>();

            _patronController = new PatronController(
                _patronServiceMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetPatronProfile_WithExistingPatronId_ReturnsOkResult()
        {
            // Arrange
            var patronId = "1";
            var patronProfile = new Patron { Email = "john@gmail.com", Name = "John Doe" };

            _patronServiceMock.Setup(service => service.GetPatronAsync(patronId))
                .Returns(Task.FromResult<Patron>(patronProfile));

            // Act
            var result = await _patronController.GetPatronProfile(patronId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().BeEquivalentTo(patronProfile);
        }

        [Fact]
        public async Task GetPatronProfile_WithNonExistingPatronId_ReturnsNotFoundResult()
        {
            // Arrange
            var patronId = "1";

            _patronServiceMock.Setup(service => service.GetPatronAsync(patronId))
                .ReturnsAsync((Patron)null);

            // Act
            var result = await _patronController.GetPatronProfile(patronId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = (NotFoundObjectResult)result;
            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            notFoundResult.Value.Should().Be($"No Patron found with ID = {patronId}");
        }

        [Fact]
        public async Task UpdatePatronProfile_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var patronId = "1";
            var patronDto = new PatronDto { Email = "john@gmail.com", Name = "John Doe" };
            var updatedPatron = new Patron { PatronId = patronId, Email = "name@gmail.com", Name = "Updated Name" };

            _patronServiceMock.Setup(service => service.UpdatePatronAsync(patronId, patronDto))
                .Returns(Task.FromResult<Patron>(updatedPatron));

            // Act
            var result = await _patronController.UpdatePatronProfile(patronId, patronDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().BeEquivalentTo(updatedPatron);
        }

        [Fact]
        public async Task UpdatePatronProfile_WithInvalidData_ReturnsBadRequestResult()
        {
            // Arrange
            var patronId = "1";
            var patronDto = new PatronDto { Email = "doe@gmail.com", Name = "John Doe" };
            var validationErrorMessage = "Invalid data";
            var validationException = new ValidationException(validationErrorMessage);

            _patronServiceMock.Setup(service => service.UpdatePatronAsync(patronId, patronDto))
                .ThrowsAsync(validationException);

            // Act
            var result = await _patronController.UpdatePatronProfile(patronId, patronDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = (BadRequestObjectResult)result;
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequestResult.Value.Should().Be(validationErrorMessage);
        }

        [Fact]
        public async Task UpdatePatronProfile_WithNonExistingPatronId_ReturnsNotFoundResult()
        {
            // Arrange
            var patronId = "1";
            var patronDto = new PatronDto { Email = "john@gmail.com", Name = "John Doe" };
            var resourceNotFoundException = new ResourceNotFoundException("Patron", "ID", patronId);

            _patronServiceMock.Setup(service => service.UpdatePatronAsync(patronId, patronDto))
                .ThrowsAsync(resourceNotFoundException);

            // Act
            var result = await _patronController.UpdatePatronProfile(patronId, patronDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = (NotFoundObjectResult)result;
            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            notFoundResult.Value.Should().Be(resourceNotFoundException.Message);
        }

        [Fact]
        public async Task GetBorrowingHistory_WithExistingPatronId_ReturnsOkResult()
        {
            // Arrange
            var patronId = "1";
            var borrowingHistory = new List<Checkout>
            {
                new Checkout { CheckoutId ="123", BookId ="1234567890", PatronId="1",CheckoutDate = DateTime.Now.AddDays(-20),
                                DueDate = DateTime.Now.AddDays(-4), ReturnDate = DateTime.Now.AddDays(-5)}
            };

            _patronServiceMock.Setup(service => service.GetBorrowingHistoryAsync(patronId))
            .Returns(Task.FromResult<IEnumerable<Checkout>>(borrowingHistory));

            // Act
            var result = await _patronController.GetBorrowingHistory(patronId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().BeEquivalentTo(borrowingHistory);
        }

        [Fact]
        public async Task GetBorrowingHistory_WithNonExistingPatronId_ReturnsBadRequestResult()
        {
            // Arrange
            var patronId = "1";
            var resourceNotFoundException = new ResourceNotFoundException("Patron", "ID", patronId);

            _patronServiceMock.Setup(service => service.GetBorrowingHistoryAsync(patronId))
                .ThrowsAsync(resourceNotFoundException);

            // Act
            var result = await _patronController.GetBorrowingHistory(patronId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = (BadRequestObjectResult)result;
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequestResult.Value.Should().Be(resourceNotFoundException.Message);
        }

    }
}
