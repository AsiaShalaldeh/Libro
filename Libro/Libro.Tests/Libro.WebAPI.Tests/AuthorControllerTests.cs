using AutoMapper;
using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IServices;
using Libro.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.Tests.Libro.WebAPI.Tests
{
    public class AuthorControllerTests
    {
        private readonly Mock<IAuthorService> _authorServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<AuthorController>> _loggerMock;
        private readonly AuthorController _authorController;

        public AuthorControllerTests()
        {
            _authorServiceMock = new Mock<IAuthorService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<AuthorController>>();

            _authorController = new AuthorController(_authorServiceMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAuthors_WithValidParameters_ReturnsOkResult()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            IList<Author> authors = new List<Author> { new Author { AuthorId = 1, Name = "Author 1" },
                                               new Author { AuthorId = 2, Name = "Author 2" }
                                                    };

            var paginatedResult = new PaginatedResult<Author>(authors, authors.Count, pageNumber, pageSize);

            _authorServiceMock.Setup(service => service.GetAllAuthorsAsync(pageNumber, pageSize))
                .ReturnsAsync(paginatedResult);

            // Act
            var actionResult = await _authorController.GetAllAuthors(pageNumber, pageSize);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>();
            var okObjectResult = (OkObjectResult)actionResult;
            okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okObjectResult.Value.Should().BeEquivalentTo(paginatedResult);
        }

        [Fact]
        public async Task GetAuthorById_WithValidAuthorId_ReturnsOkResult()
        {
            // Arrange
            var authorId = 1;
            var author = new Author { AuthorId = authorId, Name = "Author 1" };
            var authorDto = new AuthorResponseDto { AuthorId = authorId, Name = "Author 1" };

            _authorServiceMock.Setup(service => service.GetAuthorByIdAsync(authorId))
                .ReturnsAsync(author);

            _mapperMock.Setup(mapper => mapper.Map<AuthorResponseDto>(author))
                .Returns(authorDto);

            // Act
            var actionResult = await _authorController.GetAuthorById(authorId);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>();
            var okObjectResult = (OkObjectResult)actionResult;
            okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okObjectResult.Value.Should().BeEquivalentTo(authorDto);
        }

        [Fact]
        public async Task GetAuthorById_WithInvalidAuthorId_ReturnsNotFoundResult()
        {
            // Arrange
            int authorId = 1;

            _authorServiceMock.Setup(service => service.GetAuthorByIdAsync(authorId))
                .Returns(Task.FromResult<Author>(null));

            // Act
            var actionResult = await _authorController.GetAuthorById(authorId);

            // Assert
            actionResult.Should().BeOfType<NotFoundObjectResult>();
            var notFoundObjectResult = (NotFoundObjectResult)actionResult;
            notFoundObjectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            notFoundObjectResult.Value.Should().Be($"No Author found with ID = {authorId}");
        }

        [Fact]
        public async Task AddAuthor_WithValidAuthorDto_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var authorDto = new AuthorDto { Name = "Author 1" };
            var author = new Author { AuthorId = 1, Name = "Author 1" };

            _authorServiceMock.Setup(service => service.AddAuthorAsync(authorDto))
            .ReturnsAsync(author);

            // Act
            var actionResult = await _authorController.AddAuthor(authorDto);

            // Assert
            actionResult.Should().BeOfType<CreatedAtActionResult>();
            var createdAtActionResult = (CreatedAtActionResult)actionResult;
            createdAtActionResult.StatusCode.Should().Be((int)HttpStatusCode.Created);
            createdAtActionResult.ActionName.Should().Be(nameof(AuthorController.GetAuthorById));
            createdAtActionResult.RouteValues.Should().ContainKey("authorId");
            createdAtActionResult.RouteValues["authorId"].Should().Be(author.AuthorId);
            createdAtActionResult.Value.Should().BeEquivalentTo(author);
        }

        [Fact]
        public async Task UpdateAuthor_WithValidAuthorIdAndAuthorDto_ReturnsNoContentResult()
        {
            // Arrange
            var authorId = 1;
            var authorDto = new AuthorDto { Name = "Author 1" };

            // Act
            var actionResult = await _authorController.UpdateAuthor(authorId, authorDto);

            // Assert
            actionResult.Should().BeOfType<NoContentResult>();
            var noContentResult = (NoContentResult)actionResult;
            noContentResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteAuthor_WithValidAuthorId_ReturnsNoContentResult()
        {
            // Arrange
            var authorId = 1;

            // Act
            var actionResult = await _authorController.DeleteAuthor(authorId);

            // Assert
            actionResult.Should().BeOfType<NoContentResult>();
            var noContentResult = (NoContentResult)actionResult;
            noContentResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }
}
