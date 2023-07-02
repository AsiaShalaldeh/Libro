using AutoMapper;
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
    public class BookControllerTests
    {
        private readonly BookController _controller;
        private readonly Mock<IBookService> _bookServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<BookController>> _loggerMock;

        public BookControllerTests()
        {
            _bookServiceMock = new Mock<IBookService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<BookController>>();

            _controller = new BookController(
                _bookServiceMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetBookByISBN_ValidISBN_ShouldReturnOkResult()
        {
            // Arrange
            string ISBN = "1234567890";
            var book = new Book { ISBN = ISBN };
            var bookDto = new BookDto { ISBN = ISBN };

            _bookServiceMock.Setup(service => service.GetBookByISBNAsync(ISBN))
                .ReturnsAsync(book);
            _mapperMock.Setup(mapper => mapper.Map<BookDto>(book))
                .Returns(bookDto);

            // Act
            var result = await _controller.GetBookByISBN(ISBN);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBookDto = okResult.Value.Should().BeAssignableTo<BookDto>().Subject;
            returnedBookDto.ISBN.Should().Be(ISBN);
        }

        [Fact]
        public async Task GetBookByISBN_InvalidISBN_ShouldReturnNotFoundResult()
        {
            // Arrange
            string ISBN = "1234567890";
            var exceptionMessage = "No Book found with ISBN = " + ISBN;

            _bookServiceMock.Setup(service => service.GetBookByISBNAsync(ISBN))
                .ReturnsAsync((Book)null);

            // Act
            var result = await _controller.GetBookByISBN(ISBN);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task GetBookByISBN_InternalServerError_ShouldReturnInternalServerErrorResult()
        {
            // Arrange
            string ISBN = "1234567890";
            var exceptionMessage = "An error occurred.";

            _bookServiceMock.Setup(service => service.GetBookByISBNAsync(ISBN))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetBookByISBN(ISBN);

            // Assert
            var internalServerErrorResult = result.Should().BeOfType<ObjectResult>().Subject;
            internalServerErrorResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            internalServerErrorResult.Value.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task GetAllBooks_ValidRequest_ShouldReturnOkResult()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var books = new List<BookDto> { new BookDto { ISBN = "1234567890", Title = "Book1" } };
            var response = new PaginatedResult<BookDto>(books, books.Count, pageNumber, pageSize);

            _bookServiceMock.Setup(service => service.GetAllBooksAsync(pageNumber, pageSize))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAllBooks(pageNumber, pageSize);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(response);
        }

        [Fact]
        public async Task GetAllBooks_InternalServerError_ShouldReturnInternalServerErrorResult()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var exceptionMessage = "An error occurred.";

            _bookServiceMock.Setup(service => service.GetAllBooksAsync(pageNumber, pageSize))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetAllBooks(pageNumber, pageSize);

            // Assert
            var internalServerErrorResult = result.Should().BeOfType<ObjectResult>().Subject;
            internalServerErrorResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            internalServerErrorResult.Value.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task SearchBooks_ValidRequest_ShouldReturnOkResult()
        {
            // Arrange
            string title = "Title";
            string author = "Author";
            string genre = "Genre";
            int pageNumber = 1;
            int pageSize = 10;
            var books = new List<BookDto> { new BookDto { ISBN = "1234567890", Title = "Book1" } };
            var response = new PaginatedResult<BookDto>(books, books.Count, pageNumber, pageSize);

            _bookServiceMock.Setup(service => service.SearchBooksAsync(title, author, genre, pageNumber, pageSize))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.SearchBooks(title, author, genre, pageNumber, pageSize);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(response);
        }

        [Fact]
        public async Task SearchBooks_InvalidRequest_ShouldReturnBadRequestResult()
        {
            // Arrange
            string title = null;
            string author = null;
            string genre = null;
            int pageNumber = 1;
            int pageSize = 10;
            var errorMessage = "You should provide any search term !!";

            // Act
            var result = await _controller.SearchBooks(title, author, genre, pageNumber, pageSize);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be(errorMessage);
        }

        [Fact]
        public async Task SearchBooks_NoBooksFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            string title = "Title";
            string author = "Author";
            string genre = "Genre";
            int pageNumber = 1;
            int pageSize = 10;
            var errorMessage = "No books found !!";

            _bookServiceMock.Setup(service => service.SearchBooksAsync(title, author, genre, pageNumber, pageSize))
                .ReturnsAsync((PaginatedResult<BookDto>)null);

            // Act
            var result = await _controller.SearchBooks(title, author, genre, pageNumber, pageSize);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be(errorMessage);
        }

        [Fact]
        public async Task SearchBooks_InternalServerError_ShouldReturnInternalServerErrorResult()
        {
            // Arrange
            string title = "Title";
            string author = "Author";
            string genre = "Genre";
            int pageNumber = 1;
            int pageSize = 10;
            var exceptionMessage = "An error occurred.";

            _bookServiceMock.Setup(service => service.SearchBooksAsync(title, author, genre, pageNumber, pageSize))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = await _controller.SearchBooks(title, author, genre, pageNumber, pageSize);

            // Assert
            var internalServerErrorResult = result.Should().BeOfType<ObjectResult>().Subject;
            internalServerErrorResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            internalServerErrorResult.Value.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task AddBook_ValidRequest_ShouldReturnCreatedResult()
        {
            // Arrange
            var bookDto = new BookRequest { ISBN = "1234567890", Title = "Title", AuthorId = 5, Genre = "Romance" };
            var createdBook = new BookDto { ISBN = bookDto.ISBN, Title = bookDto.Title };

            _bookServiceMock.Setup(service => service.GetBookByISBNAsync(bookDto.ISBN))
                .ReturnsAsync((Book)null);
            _bookServiceMock.Setup(service => service.AddBookAsync(bookDto))
                .ReturnsAsync(createdBook);

            // Act
            var result = await _controller.AddBook(bookDto);

            // Assert
            var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtActionResult.ActionName.Should().Be(nameof(BookController.GetBookByISBN));
            createdAtActionResult.RouteValues["ISBN"].Should().Be(createdBook.ISBN);
            createdAtActionResult.Value.Should().Be(createdBook);
        }

        [Fact]
        public async Task AddBook_BookWithSameISBNExists_ShouldReturnBadRequestResult()
        {
            // Arrange
            var bookDto = new BookRequest { ISBN = "1234567890", Title = "Title", AuthorId = 5, Genre = "Romance" };
            var existingBook = new Book { ISBN = bookDto.ISBN };
            var errorMessage = "The ISBN should be unique !!";

            _bookServiceMock.Setup(service => service.GetBookByISBNAsync(bookDto.ISBN))
                .ReturnsAsync(existingBook);

            // Act
            var result = await _controller.AddBook(bookDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be(errorMessage);
        }

        [Fact]
        public async Task AddBook_ValidationFailed_ShouldReturnBadRequestResult()
        {
            // Arrange
            var bookDto = new BookRequest { ISBN = "1234567890", Title = null, AuthorId = 5, Genre = "Romance" };

            _bookServiceMock.Setup(service => service.GetBookByISBNAsync(bookDto.ISBN))
                .ReturnsAsync((Book)null);
            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            // Act
            var result = await _controller.AddBook(bookDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        }

        [Fact]
        public async Task UpdateBook_ValidRequest_ShouldReturnNoContentResult()
        {
            // Arrange
            string ISBN = "1234567890";
            var bookDto = new BookRequest { ISBN = ISBN, Title = "Title", Genre = "Romance" };

            _bookServiceMock.Setup(service => service.UpdateBookAsync(ISBN, bookDto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateBook(ISBN, bookDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateBook_BookNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            string ISBN = "1234567890";
            var bookDto = new BookRequest { ISBN = ISBN, Title = "Title" };
            var exceptionMessage = "No Book found with ISBN = " + bookDto.ISBN;

            _bookServiceMock.Setup(service => service.UpdateBookAsync(ISBN, bookDto))
                .Throws(new ResourceNotFoundException("Book", "ISBN", bookDto.ISBN));

            // Act
            var result = await _controller.UpdateBook(ISBN, bookDto);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task UpdateBook_ValidationFailed_ShouldReturnBadRequestResult()
        {
            // Arrange
            string ISBN = "1234567890";
            var bookDto = new BookRequest { ISBN = null, Title = "Title" };

            _controller.ModelState.AddModelError("ISBN", "ISBN is required!");

            // Act
            var result = await _controller.UpdateBook(ISBN, bookDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        }

        [Fact]
        public async Task DeleteBook_ValidRequest_ShouldReturnNoContentResult()
        {
            // Arrange
            string ISBN = "1234567890";

            _bookServiceMock.Setup(service => service.RemoveBookAsync(ISBN))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RemoveBook(ISBN);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteBook_BookNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            string ISBN = "1234567890";
            var exceptionMessage = "No Book found with ISBN = " + ISBN;

            _bookServiceMock.Setup(service => service.RemoveBookAsync(ISBN))
                .Throws(new ResourceNotFoundException("Book", "ISBN", ISBN));

            // Act
            var result = await _controller.RemoveBook(ISBN);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task DeleteBook_InternalServerError_ShouldReturnInternalServerErrorResult()
        {
            // Arrange
            string ISBN = "1234567890";
            var exceptionMessage = "An error occurred.";

            _bookServiceMock.Setup(service => service.RemoveBookAsync(ISBN))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = await _controller.RemoveBook(ISBN);

            // Assert
            var internalServerErrorResult = result.Should().BeOfType<ObjectResult>().Subject;
            internalServerErrorResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            internalServerErrorResult.Value.Should().Be(exceptionMessage);
        }
    }
}
