﻿using AutoMapper;
using Libro.Application.Services;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Tests.Libro.Application.Tests
{
    public class BookRecommendationServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IPatronRepository> _patronRepositoryMock;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<BookRecommendationService>> _loggerMock;
        private readonly BookRecommendationService _bookRecommendationService;

        public BookRecommendationServiceTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _patronRepositoryMock = new Mock<IPatronRepository>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<BookRecommendationService>>();
            _bookRecommendationService = new BookRecommendationService(
                _bookRepositoryMock.Object,
                _patronRepositoryMock.Object,
                _transactionRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetRecommendedBooks_ValidPatronId_ReturnsRecommendedBooks()
        {
            // Arrange
            string patronId = "123";

            var patron = new Patron { PatronId = patronId };
            _patronRepositoryMock.Setup(mock => mock.GetPatronByIdAsync(patronId))
                .ReturnsAsync(patron);

            var patronCheckouts = new List<Checkout>
            {
                new Checkout { Book = new Book { Genre = Genre.Fantasy } },
                new Checkout { Book = new Book { Genre = Genre.Drama } },
                new Checkout { Book = new Book { Genre = Genre.Romance } },
                new Checkout { Book = new Book { Genre = Genre.Fantasy } },
                new Checkout { Book = new Book { Genre = Genre.Mystery } }
            };
            _transactionRepositoryMock.Setup(mock => mock.GetCheckoutTransactionsByPatronAsync(patronId))
                .ReturnsAsync(patronCheckouts);

            var recommendedBooks = new List<Book>
            {
                new Book { Title = "Book 1", Genre = Genre.Fantasy },
                new Book { Title = "Book 2", Genre = Genre.Drama },
                new Book { Title = "Book 3", Genre = Genre.Romance }
            };
            _bookRepositoryMock.Setup(mock => mock.GetBooksByGenres(It.IsAny<IEnumerable<Genre>>()))
                .ReturnsAsync(recommendedBooks);

            var expectedDto = new List<BookDto>
            {
                new BookDto { Title = "Book 1" },
                new BookDto { Title = "Book 2" },
                new BookDto { Title = "Book 3" }
            };
            _mapperMock.Setup(mock => mock.Map<IEnumerable<BookDto>>(recommendedBooks))
                .Returns(expectedDto);

            // Act
            var result = await _bookRecommendationService.GetRecommendedBooks(patronId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto, result);
        }

        [Fact]
        public async Task GetRecommendedBooks_InvalidPatronId_ThrowsResourceNotFoundException()
        {
            // Arrange
            string patronId = "123";
            _patronRepositoryMock.Setup(mock => mock.GetPatronByIdAsync(patronId))
                .ReturnsAsync((Patron)null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(
                () => _bookRecommendationService.GetRecommendedBooks(patronId)
            );
        }

        [Fact]
        public async Task GetRecommendedBooks_NoPreviousCheckouts_ReturnsNull()
        {
            // Arrange
            string patronId = "123";
            var patron = new Patron { PatronId = patronId };
            _patronRepositoryMock.Setup(mock => mock.GetPatronByIdAsync(patronId))
                .ReturnsAsync(patron);

            var patronCheckouts = new List<Checkout>();
            _transactionRepositoryMock.Setup(mock => mock.GetCheckoutTransactionsByPatronAsync(patronId))
                .ReturnsAsync(patronCheckouts);

            // Act
            var result = await _bookRecommendationService.GetRecommendedBooks(patronId);

            // Assert
            Assert.Null(result);
        }
    }
}
