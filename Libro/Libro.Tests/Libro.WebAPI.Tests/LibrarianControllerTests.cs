using FluentValidation;
using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Libro.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.Tests.Libro.WebAPI.Tests
{
    public class LibrarianControllerTests
    {
        private readonly Mock<ILibrarianService> _librarianServiceMock;
        private readonly Mock<ILogger<LibrarianController>> _loggerMock;
        private readonly LibrarianController _controller;

        public LibrarianControllerTests()
        {
            _librarianServiceMock = new Mock<ILibrarianService>();
            _loggerMock = new Mock<ILogger<LibrarianController>>();
            _controller = new LibrarianController(_librarianServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllLibrarians_ShouldReturnOkResult()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            IList<Librarian> librarians = new List<Librarian> { new Librarian { LibrarianId = "123", Name = "John Doe" } };

            var paginatedResult = new PaginatedResult<Librarian>(librarians, librarians.Count, pageNumber, pageSize);
            _librarianServiceMock.Setup(service => service.GetAllLibrariansAsync(pageNumber, pageSize))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetAllLibrarians(pageNumber, pageSize);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okObjectResult = (OkObjectResult)result;
            okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okObjectResult.Value.Should().BeEquivalentTo(paginatedResult);
        }

        [Fact]
        public async Task GetLibrarianById_ExistingId_ShouldReturnOkResult()
        {
            // Arrange
            var librarianId = "123";
            var librarian = new Librarian(); 
            _librarianServiceMock.Setup(service => service.GetLibrarianByIdAsync(librarianId))
                .ReturnsAsync(librarian);

            // Act
            var result = await _controller.GetLibrarianById(librarianId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeAssignableTo<Librarian>().Subject;

            response.Should().BeEquivalentTo(librarian);
        }

        [Fact]
        public async Task GetLibrarianById_NonExistingId_ShouldReturnNotFoundResult()
        {
            // Arrange
            var librarianId = "123";
            _librarianServiceMock.Setup(service => service.GetLibrarianByIdAsync(librarianId))
                .ReturnsAsync((Librarian)null);

            // Act
            var result = await _controller.GetLibrarianById(librarianId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateLibrarian_ValidData_ShouldReturnOkResult()
        {
            // Arrange
            var librarianId = "123";
            var librarianDto = new LibrarianDto { Name = "Updated Name"}; 
            var updatedLibrarian = new Librarian(); 
            _librarianServiceMock.Setup(service => service.UpdateLibrarianAsync(librarianId, librarianDto))
                .ReturnsAsync(updatedLibrarian);

            // Act
            var result = await _controller.UpdateLibrarian(librarianId, librarianDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeAssignableTo<Librarian>().Subject;

            response.Should().BeEquivalentTo(updatedLibrarian);
        }
        [Fact]
        public async Task UpdateLibrarian_InvalidData_ShouldReturnBadRequestResult()
        {
            // Arrange
            var librarianId = "123";
            var librarianDto = new LibrarianDto { Name = ""};
            var validationException = new ValidationException("Validation error");
            _librarianServiceMock.Setup(service => service.UpdateLibrarianAsync(librarianId, librarianDto))
                .Throws(validationException);

            // Act
            var result = await _controller.UpdateLibrarian(librarianId, librarianDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        }

        [Fact]
        public async Task UpdateLibrarian_NonExistingId_ShouldReturnNotFoundResult()
        {
            // Arrange
            var librarianId = "123";
            var librarianDto = new LibrarianDto() { Name = "new name"}; 
            _librarianServiceMock.Setup(service => service.UpdateLibrarianAsync(librarianId, librarianDto))
                .Throws(new ResourceNotFoundException("Librarian", "ID", librarianId));

            // Act
            var result = await _controller.UpdateLibrarian(librarianId, librarianDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteLibrarian_ExistingId_ShouldReturnNoContentResult()
        {
            // Arrange
            var librarianId = "123";

            // Act
            var result = await _controller.DeleteLibrarian(librarianId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteLibrarian_NonExistingId_ShouldReturnNotFoundResult()
        {
            // Arrange
            var librarianId = "123";
            _librarianServiceMock.Setup(service => service.DeleteLibrarianAsync(librarianId))
                .Throws(new ResourceNotFoundException("Librarian", "ID", librarianId));

            // Act
            var result = await _controller.DeleteLibrarian(librarianId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

    }
}
