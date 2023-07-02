using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Libro.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Libro.Tests.Libro.WebAPI.Tests
{
    public class ReadingListControllerTests
    {
        private readonly Mock<IReadingListService> _readingListServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ReadingListController>> _loggerMock;
        private readonly ReadingListController _readingListController;

        public ReadingListControllerTests()
        {
            _readingListServiceMock = new Mock<IReadingListService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ReadingListController>>();
            _readingListController = new ReadingListController(_readingListServiceMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetReadingListsByPatronId_ValidPatronId_ReturnsOkResultWithReadingLists()
        {
            // Arrange
            string patronId = "1";
            var readingLists = new List<ReadingList>()
            {
                new ReadingList {ReadingListId = 1, PatronId = patronId},
                new ReadingList {ReadingListId = 2, PatronId = patronId},
            };
            var readingListDtos = new List<ReadingListDto>();

            _readingListServiceMock.Setup(x => x.GetReadingListsByPatronIdAsync(patronId)).ReturnsAsync(readingLists);
            _mapperMock.Setup(x => x.Map<IEnumerable<ReadingListDto>>(readingLists)).Returns(readingListDtos);

            // Act
            var result = await _readingListController.GetReadingListsByPatronId(patronId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(readingListDtos, okResult.Value);
        }

        [Fact]
        public async Task GetReadingListsByPatronId_ResourceNotFoundException_ReturnsNotFound()
        {
            // Arrange
            string patronId = "1";
            var exception = new ResourceNotFoundException("Patron", "ID", patronId);

            _readingListServiceMock.Setup(x => x.GetReadingListsByPatronIdAsync(patronId)).ThrowsAsync(exception);

            // Act
            var result = await _readingListController.GetReadingListsByPatronId(patronId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal(exception.Message, notFoundResult.Value);
        }

        [Fact]
        public async Task GetReadingListById_ValidIds_ReturnsOkResultWithReadingList()
        {
            // Arrange
            string patronId = "1";
            int listId = 1;
            var readingList = new ReadingList();
            var readingListDto = new ReadingListDto();

            _readingListServiceMock.Setup(x => x.GetReadingListByIdAsync(listId, patronId)).ReturnsAsync(readingList);
            _mapperMock.Setup(x => x.Map<ReadingListDto>(readingList)).Returns(readingListDto);

            // Act
            var result = await _readingListController.GetReadingListById(patronId, listId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(readingListDto, okResult.Value);
        }

        [Fact]
        public async Task GetReadingListById_ResourceNotFoundException_ReturnsNotFound()
        {
            // Arrange
            string patronId = "1";
            int listId = 1;
            var exception = new ResourceNotFoundException("Reading List", "ID", listId.ToString());

            _readingListServiceMock.Setup(x => x.GetReadingListByIdAsync(listId, patronId)).ThrowsAsync(exception);

            // Act
            var result = await _readingListController.GetReadingListById(patronId, listId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal(exception.Message, notFoundResult.Value);
        }

        [Fact]
        public async Task AddBookToReadingList_ValidData_ReturnsNoContent()
        {
            // Arrange
            string patronId = "1";
            int listId = 1;
            string isbn = "1234567890";

            _readingListServiceMock.Setup(x => x.AddBookToReadingListAsync(listId, patronId, isbn)).ReturnsAsync(true);

            // Act
            var result = await _readingListController.AddBookToReadingList(patronId, listId, isbn);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddBookToReadingList_BookAlreadyAdded_ReturnsBadRequest()
        {
            // Arrange
            string patronId = "1";
            int listId = 1;
            string isbn = "1234567890";

            _readingListServiceMock.Setup(x => x.AddBookToReadingListAsync(listId, patronId, isbn)).ReturnsAsync(false);

            // Act
            var result = await _readingListController.AddBookToReadingList(patronId, listId, isbn);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.Equal("The Book Already Added To That List !!", badRequestResult.Value);
        }

        [Fact]
        public async Task RemoveBookFromReadingList_ValidData_ReturnsNoContent()
        {
            // Arrange
            string patronId = "1";
            int listId = 1;
            string isbn = "1234567890";

            // Act
            var result = await _readingListController.RemoveBookFromReadingList(patronId, listId, isbn);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RemoveReadingList_ValidData_ReturnsNoContent()
        {
            // Arrange
            string patronId = "1";
            int listId = 1;

            // Act
            var result = await _readingListController.RemoveReadingList(patronId, listId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
        [Fact]
        public async Task CreateReadingList_ValidData_ReturnsCreatedAtRouteResultWithCreatedReadingList()
        {
            // Arrange
            string patronId = "1";
            var readingListDto = new ReadingListDto { PatronId = patronId };
            var createdListDto = new ReadingListDto { ReadingListId = 1, PatronId = patronId};

            _readingListServiceMock.Setup(x => x.CreateReadingListAsync(readingListDto, patronId)).ReturnsAsync(createdListDto);

            // Act
            var result = await _readingListController.CreateReadingList(patronId, readingListDto);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(result);
            var createdAtRouteResult = (CreatedAtRouteResult)result;
            Assert.Equal("GetReadingList", createdAtRouteResult.RouteName);
            Assert.Equal(createdListDto.PatronId, createdAtRouteResult.RouteValues["patronId"]);
            Assert.Equal(createdListDto.ReadingListId, createdAtRouteResult.RouteValues["listId"]);
            Assert.Equal(createdListDto, createdAtRouteResult.Value);
        }

        [Fact]
        public async Task CreateReadingList_PatronIdMismatch_ReturnsBadRequest()
        {
            // Arrange
            string patronId = "1";
            var readingListDto = new ReadingListDto { PatronId = "2" };

            // Act
            var result = await _readingListController.CreateReadingList(patronId, readingListDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.Equal("Patron ID Mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateReadingList_ResourceNotFoundException_ReturnsNotFound()
        {
            // Arrange
            string patronId = "1";
            var readingListDto = new ReadingListDto { PatronId = patronId };
            var exception = new ResourceNotFoundException("Patron", "ID", patronId);

            _readingListServiceMock.Setup(x => x.CreateReadingListAsync(readingListDto, patronId)).ThrowsAsync(exception);

            // Act
            var result = await _readingListController.CreateReadingList(patronId, readingListDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal(exception.Message, notFoundResult.Value);
        }

        [Fact]
        public async Task GetBooksOfReadingList_ValidData_ReturnsOkResultWithBooks()
        {
            // Arrange
            string patronId = "1";
            int listId = 1;
            var books = new List<BookDto>();

            _readingListServiceMock.Setup(x => x.GetBooksByReadingListAsync(listId, patronId)).ReturnsAsync(books);

            // Act
            var result = await _readingListController.GetBooksOfReadingList(patronId, listId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(books, okResult.Value);
        }

        [Fact]
        public async Task GetBooksOfReadingList_ResourceNotFoundException_ReturnsNotFound()
        {
            // Arrange
            string patronId = "1";
            int listId = 1;
            var exception = new ResourceNotFoundException("Patron", "ID", patronId);

            _readingListServiceMock.Setup(x => x.GetBooksByReadingListAsync(listId, patronId)).ThrowsAsync(exception);

            // Act
            var result = await _readingListController.GetBooksOfReadingList(patronId, listId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal(exception.Message, notFoundResult.Value);
        }

    }
}
