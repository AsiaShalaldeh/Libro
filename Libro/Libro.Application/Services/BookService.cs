using AutoMapper;
using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository bookRepository, IMapper mapper,
            IAuthorRepository authorRepository, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Book> GetBookByISBNAsync(string ISBN)
        {
            try
            {
                Book book = await _bookRepository.GetBookByISBNAsync(ISBN);
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in BookService while retrieving the book with ISBN: {ISBN}");
                throw;
            }
        }

        public async Task<PaginatedResult<BookDto>> GetAllBooksAsync(int pageNumber, int pageSize)
        {
            try
            {
                var paginatedResult = await _bookRepository.GetAllBooksAsync(pageNumber, pageSize);

                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(paginatedResult.Items);

                return new PaginatedResult<BookDto>(bookDtos, paginatedResult.TotalCount, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in BookService while retrieving all books.");
                throw;
            }
        }

        public async Task<PaginatedResult<BookDto>> SearchBooksAsync(string title, string author,
            string genre, int pageNumber, int pageSize)
        {
            try
            {
                var paginatedResult = await _bookRepository.SearchBooksAsync(title, author, genre, pageNumber, pageSize);

                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(paginatedResult.Items);

                return new PaginatedResult<BookDto>(bookDtos, paginatedResult.TotalCount, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in BookService while searching for books.");
                throw;
            }
        }

        public async Task<BookDto> AddBookAsync(BookRequest bookDto)
        {
            try
            {
                Author author = await _authorRepository.GetAuthorByIdAsync(bookDto.AuthorId ?? 0);
                if (author == null)
                {
                    throw new ResourceNotFoundException("Author", "ID", bookDto.AuthorId.ToString());
                }
                var book = _mapper.Map<Book>(bookDto);
                await _bookRepository.AddBookAsync(book, author);
                return _mapper.Map<BookDto>(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in BookService while adding a book.");
                throw;
            }
        }

        public async Task UpdateBookAsync(string ISBN, BookRequest bookDto)
        {
            try
            {
                var existingBook = await _bookRepository.GetBookByISBNAsync(ISBN);
                if (existingBook == null)
                {
                    throw new ResourceNotFoundException("Book", "ID", ISBN);
                }
                if (bookDto.AuthorId.HasValue)
                {
                    Author author = await _authorRepository.GetAuthorByIdAsync(bookDto.AuthorId ?? 0);
                    if (author == null)
                    {
                        throw new ResourceNotFoundException("Author", "ID", bookDto.AuthorId.ToString());
                    }
                    existingBook.Author = author;
                }
                existingBook.Title = bookDto.Title;
                existingBook.IsAvailable = bookDto.IsAvailable;
                if (!string.IsNullOrWhiteSpace(bookDto.Genre))
                    existingBook.Genre = Enum.Parse<Genre>(bookDto.Genre, ignoreCase: true);
                if (bookDto.PublicationDate.HasValue)
                {
                    var newPublicationDate = bookDto.PublicationDate.Value;
                    if (existingBook.PublicationDate.GetValueOrDefault() != newPublicationDate)
                    {
                        existingBook.PublicationDate = newPublicationDate;
                    }
                }
                await _bookRepository.UpdateBookAsync(existingBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in BookService while updating the book with ISBN: {ISBN}");
                throw;
            }
        }

        public async Task RemoveBookAsync(string ISBN)
        {
            try
            {
                var existingBook = await _bookRepository.GetBookByISBNAsync(ISBN);
                if (existingBook == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", ISBN);
                }

                await _bookRepository.DeleteBookAsync(existingBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in BookService while removing the book with ISBN: {ISBN}");
                throw;
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByGenres(IEnumerable<Genre> genres)
        {
            try
            {
                return await _bookRepository.GetBooksByGenres(genres);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in BookService while retrieving books by genres.");
                throw;
            }
        }
    }
}
