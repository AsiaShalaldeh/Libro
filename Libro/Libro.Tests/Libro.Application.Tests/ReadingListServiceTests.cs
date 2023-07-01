using AutoMapper;
using Libro.Application.Services;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Tests.Libro.Application.Tests
{
    public class ReadingListServiceTests
    {
        private readonly Mock<IReadingListRepository> _readingListRepositoryMock;
        private readonly Mock<IPatronService> _patronServiceMock;
        private readonly Mock<IBookService> _bookServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ReadingListService>> _loggerMock;
        private readonly ReadingListService _readingListService;

        public ReadingListServiceTests()
        {
            _readingListRepositoryMock = new Mock<IReadingListRepository>();
            _patronServiceMock = new Mock<IPatronService>();
            _bookServiceMock = new Mock<IBookService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ReadingListService>>();

            _readingListService = new ReadingListService(
                _readingListRepositoryMock.Object,
                _patronServiceMock.Object,
                _mapperMock.Object,
                _bookServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetReadingListByIdAsync_ValidListIdAndPatronId_ReturnsReadingList()
        {
            // Arrange
            int listId = 1;
            string patronId = "patron1";
            var patron = new Patron();
            var readingList = new ReadingList();

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync(patron);
            _readingListRepositoryMock.Setup(mock => mock.GetReadingListByIdAsync(listId, patronId)).ReturnsAsync(readingList);

            // Act
            var result = await _readingListService.GetReadingListByIdAsync(listId, patronId);

            // Assert
            Assert.Equal(readingList, result);
            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(listId, patronId), Times.Once);
        }

        [Fact]
        public async Task GetReadingListByIdAsync_InvalidPatronId_ThrowsResourceNotFoundException()
        {
            // Arrange
            int listId = 1;
            string patronId = "patron1";

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync((Patron)null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _readingListService.GetReadingListByIdAsync(listId, patronId));
            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }
        [Fact]
        public async Task GetReadingListsByPatronIdAsync_ValidPatronId_ReturnsReadingLists()
        {
            // Arrange
            string patronId = "patron1";
            var patron = new Patron();
            var readingLists = new List<ReadingList> { new ReadingList(), new ReadingList() };

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync(patron);
            _readingListRepositoryMock.Setup(mock => mock.GetReadingListsByPatronIdAsync(patronId)).ReturnsAsync(readingLists);

            // Act
            var result = await _readingListService.GetReadingListsByPatronIdAsync(patronId);

            // Assert
            Assert.Equal(readingLists, result);
            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListsByPatronIdAsync(patronId), Times.Once);
        }

        [Fact]
        public async Task GetReadingListsByPatronIdAsync_InvalidPatronId_ThrowsResourceNotFoundException()
        {
            // Arrange
            string patronId = "patron1";

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync((Patron)null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _readingListService.GetReadingListsByPatronIdAsync(patronId));
            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListsByPatronIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CreateReadingListAsync_ValidInput_ReturnsCreatedReadingListDto()
        {
            // Arrange
            string patronId = "patron1";
            var patron = new Patron();
            var readingListDto = new ReadingListDto();
            var readingList = new ReadingList { ReadingListId = 1 };

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync(patron);
            _mapperMock.Setup(mock => mock.Map<ReadingList>(readingListDto)).Returns(readingList);
            _readingListRepositoryMock.Setup(mock => mock.CreateReadingListAsync(readingList)).ReturnsAsync(readingList);
            _mapperMock.Setup(mock => mock.Map<ReadingListDto>(readingList)).Returns(readingListDto);

            // Act
            var result = await _readingListService.CreateReadingListAsync(readingListDto, patronId);

            // Assert
            Assert.Equal(readingListDto, result);
            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _mapperMock.Verify(mock => mock.Map<ReadingList>(readingListDto), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.CreateReadingListAsync(readingList), Times.Once);
            _mapperMock.Verify(mock => mock.Map<ReadingListDto>(readingList), Times.Once);
        }

        [Fact]
        public async Task CreateReadingListAsync_InvalidPatronId_ThrowsResourceNotFoundException()
        {
            // Arrange
            string patronId = "patron1";
            var readingListDto = new ReadingListDto();

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync((Patron)null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _readingListService.CreateReadingListAsync(readingListDto, patronId));
            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.CreateReadingListAsync(It.IsAny<ReadingList>()), Times.Never);
        }

        [Fact]
        public async Task RemoveReadingListAsync_ValidListIdAndPatronId_RemovesReadingList()
        {
            // Arrange
            int listId = 1;
            string patronId = "patron1";
            var readingList = new ReadingList();

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync(new Patron());
            _readingListRepositoryMock.Setup(mock => mock.GetReadingListByIdAsync(listId, patronId)).ReturnsAsync(readingList);

            // Act
            await _readingListService.RemoveReadingListAsync(listId, patronId);

            // Assert
            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(listId, patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.RemoveReadingListAsync(readingList), Times.Once);
        }

        [Fact]
        public async Task RemoveReadingListAsync_InvalidListIdOrPatronId_ThrowsResourceNotFoundException()
        {
            // Arrange
            int listId = 1;
            string patronId = "patron1";

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync((Patron)null);
            _readingListRepositoryMock.Setup(mock => mock.GetReadingListByIdAsync(listId, patronId)).ReturnsAsync((ReadingList)null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _readingListService.RemoveReadingListAsync(listId, patronId));
            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(listId, patronId), Times.Never);
            _readingListRepositoryMock.Verify(mock => mock.RemoveReadingListAsync(It.IsAny<ReadingList>()), Times.Never);
        }

        [Fact]
        public async Task GetBooksByReadingListAsync_ValidListIdAndPatronId_ReturnsBooks()
        {
            // Arrange
            int listId = 1;
            string patronId = "patron1";
            List<Book> books = new List<Book>
            {
                new Book { ISBN = "isbn1" },
                new Book { ISBN = "isbn2" }
            };
            IEnumerable<BookDto> expectedBooks = new List<BookDto>
            {
                new BookDto { ISBN = "isbn1" },
                new BookDto { ISBN = "isbn2" }
            };
            var readingList = new ReadingList();
            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync(new Patron());
            _readingListRepositoryMock.Setup(mock => mock.GetReadingListByIdAsync(listId, patronId)).ReturnsAsync(readingList);
            _readingListRepositoryMock.Setup(mock => mock.GetBooksByReadingListAsync(readingList)).ReturnsAsync(books);
            _mapperMock.Setup(mock => mock.Map<IEnumerable<BookDto>>(books)).Returns(expectedBooks);

            // Act
            IEnumerable<BookDto> result = await _readingListService.GetBooksByReadingListAsync(listId, patronId);

            // Assert
            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(listId, patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetBooksByReadingListAsync(readingList), Times.Once);
            _mapperMock.Verify(mock => mock.Map<IEnumerable<BookDto>>(books), Times.Once);
            Assert.Equal(expectedBooks, result);
        }

        [Fact]
        public async Task GetBooksByReadingListAsync_InvalidListIdOrPatronId_ThrowsResourceNotFoundException()
        {
            // Arrange
            int listId = 1;
            string patronId = "patron1";

            _readingListRepositoryMock.Setup(mock => mock.GetReadingListByIdAsync(listId, patronId)).ReturnsAsync((ReadingList)null);
            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync((Patron)null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _readingListService.GetBooksByReadingListAsync(listId, patronId));
            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(listId, patronId), Times.Never);
            _readingListRepositoryMock.Verify(mock => mock.GetBooksByReadingListAsync(It.IsAny<ReadingList>()), Times.Never);
        }

        [Fact]
        public async Task AddBookToReadingListAsync_ValidInput_AddsBookToReadingList()
        {
            // Arrange
            int listId = 1;
            string patronId = "patron1";
            string bookId = "isbn1";
            var patron = new Patron();
            var readingList = new ReadingList();
            var book = new Book { ISBN = "isbn1" };

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync(patron);
            _readingListRepositoryMock.Setup(mock => mock.GetReadingListByIdAsync(listId, patronId)).ReturnsAsync(readingList);
            _bookServiceMock.Setup(mock => mock.GetBookByISBNAsync(bookId)).ReturnsAsync(book);
            _readingListRepositoryMock.Setup(mock => mock.AddBookToReadingListAsync(readingList, It.IsAny<BookList>())).Returns(Task.CompletedTask);

            // Act
            bool result = await _readingListService.AddBookToReadingListAsync(listId, patronId, bookId);

            // Assert
            Assert.True(result);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(listId, patronId), Times.Once);
            _bookServiceMock.Verify(mock => mock.GetBookByISBNAsync(bookId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.AddBookToReadingListAsync(readingList, It.IsAny<BookList>()), Times.Once);
        }

        [Fact]
        public async Task AddBookToReadingListAsync_InvalidInput_ThrowsResourceNotFoundException()
        {
            // Arrange
            int listId = 1;
            string patronId = "patron1";
            string bookId = "isbn1";
            var patron = new Patron();
            var readingList = new ReadingList();

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync(patron);
            _readingListRepositoryMock.Setup(mock => mock.GetReadingListByIdAsync(listId, patronId)).ReturnsAsync(readingList);
            _bookServiceMock.Setup(mock => mock.GetBookByISBNAsync(bookId)).ReturnsAsync((Book)null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _readingListService.AddBookToReadingListAsync(listId, patronId, bookId));
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(listId, patronId), Times.Once);
            _bookServiceMock.Verify(mock => mock.GetBookByISBNAsync(bookId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.AddBookToReadingListAsync(readingList, It.IsAny<BookList>()), Times.Never);
        }

        [Fact]
        public async Task AddBookToReadingListAsync_InvalidListIdOrPatronId_ThrowsResourceNotFoundException()
        {
            // Arrange
            int listId = 1;
            string patronId = "patron1";
            string bookId = "book1";

            _readingListRepositoryMock.Setup(mock => mock.GetReadingListByIdAsync(listId, patronId)).ReturnsAsync((ReadingList)null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _readingListService.AddBookToReadingListAsync(listId, patronId, bookId));
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(listId, patronId), Times.Once);
            _bookServiceMock.Verify(mock => mock.GetBookByISBNAsync(It.IsAny<string>()), Times.Never);
            _readingListRepositoryMock.Verify(mock => mock.AddBookToReadingListAsync(It.IsAny<ReadingList>(), It.IsAny<BookList>()), Times.Never);
        }

        [Fact]
        public async Task RemoveBookFromReadingListAsync_ValidInput_RemovesBookFromReadingList()
        {
            // Arrange
            int listId = 1;
            string patronId = "patron1";
            string bookId = "isbn1";
            var patron = new Patron();
            var readingList = new ReadingList
            {
                BookLists = new List<BookList>
                {
                    new BookList { BookId = bookId }
                }
            };
            var book = new Book();

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync(patron);
            _readingListRepositoryMock.Setup(mock => mock.GetReadingListByIdAsync(listId, patronId)).ReturnsAsync(readingList);
            _bookServiceMock.Setup(mock => mock.GetBookByISBNAsync(bookId)).ReturnsAsync(book);

            // Act
            await _readingListService.RemoveBookFromReadingListAsync(listId, patronId, bookId);

            // Assert
            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(listId, patronId), Times.Once);
            _bookServiceMock.Verify(mock => mock.GetBookByISBNAsync(bookId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.RemoveBookFromReadingListAsync(readingList, bookId), Times.Once);
        }

        [Fact]
        public async Task RemoveBookFromReadingListAsync_InvalidPatronId_ThrowsResourceNotFoundException()
        {
            // Arrange
            int listId = 1;
            string patronId = "invalidPatronId";
            string bookId = "isbn1";

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync((Patron)null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _readingListService.RemoveBookFromReadingListAsync(listId, patronId, bookId));

            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _bookServiceMock.Verify(mock => mock.GetBookByISBNAsync(It.IsAny<string>()), Times.Never);
            _readingListRepositoryMock.Verify(mock => mock.RemoveBookFromReadingListAsync(It.IsAny<ReadingList>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RemoveBookFromReadingListAsync_InvalidListId_ThrowsResourceNotFoundException()
        {
            // Arrange
            int listId = 1;
            string patronId = "patron1";
            string bookId = "isbn1";
            var patron = new Patron();

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync(patron);
            _readingListRepositoryMock.Setup(mock => mock.GetReadingListByIdAsync(listId, patronId)).ReturnsAsync((ReadingList)null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _readingListService.RemoveBookFromReadingListAsync(listId, patronId, bookId));

            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(listId, patronId), Times.Once);
            _bookServiceMock.Verify(mock => mock.GetBookByISBNAsync(It.IsAny<string>()), Times.Never);
            _readingListRepositoryMock.Verify(mock => mock.RemoveBookFromReadingListAsync(It.IsAny<ReadingList>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RemoveBookFromReadingListAsync_InvalidBookId_ThrowsResourceNotFoundException()
        {
            // Arrange
            int listId = 1;
            string patronId = "patron1";
            string bookId = "invalidBookId";
            var patron = new Patron();
            var readingList = new ReadingList { BookLists = new List<BookList>() };

            _patronServiceMock.Setup(mock => mock.GetPatronAsync(patronId)).ReturnsAsync(patron);
            _readingListRepositoryMock.Setup(mock => mock.GetReadingListByIdAsync(listId, patronId)).ReturnsAsync(readingList);
            _bookServiceMock.Setup(mock => mock.GetBookByISBNAsync(bookId)).ReturnsAsync((Book)null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _readingListService.RemoveBookFromReadingListAsync(listId, patronId, bookId));

            _patronServiceMock.Verify(mock => mock.GetPatronAsync(patronId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.GetReadingListByIdAsync(listId, patronId), Times.Once);
            _bookServiceMock.Verify(mock => mock.GetBookByISBNAsync(bookId), Times.Once);
            _readingListRepositoryMock.Verify(mock => mock.RemoveBookFromReadingListAsync(It.IsAny<ReadingList>(), It.IsAny<string>()), Times.Never);
        }

    }

}
