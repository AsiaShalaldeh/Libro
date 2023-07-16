using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libro.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibroDbContext _context;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(LibroDbContext context, ILogger<BookRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Book> GetBookByISBNAsync(string ISBN)
        {
            try
            {
                var book = await _context.Books.Include(book => book.Author)
                    .Include(b => b.Reviews)
                    .FirstOrDefaultAsync(b => b.ISBN.Equals(ISBN));
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in BookRepository while getting the book with ISBN: {ISBN}.");
                throw;
            }
        }

        public async Task<PaginatedResult<Book>> SearchBooksAsync(string title, string author,
            string genre, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Books.Include(book => book.Author)
                  .Include(b => b.Reviews)
                  .AsQueryable();

                if (!string.IsNullOrEmpty(title))
                {
                    title = title.Trim();
                    query = query.Where(b => b.Title.Contains(title));
                }

                if (!string.IsNullOrEmpty(author))
                {
                    author = author.Trim();
                    query = query.Where(b => b.Author.Name.Contains(author));
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    var genreValue = Enum.Parse<Genre>(genre, ignoreCase: true);
                    query = query.Where(b => b.Genre == genreValue);
                }

                return await PaginatedResult<Book>.CreateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in BookRepository while searching for books.");
                throw;
            }
        }

        public async Task<PaginatedResult<Book>> GetAllBooksAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Books.Include(book => book.Author)
                    .Include(b => b.Reviews)
                    .AsQueryable();

                return await PaginatedResult<Book>.CreateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in BookRepository while getting all books.");
                throw;
            }
        }

        public async Task AddBookAsync(Book book, Author author)
        {
            try
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in BookRepository while adding a book.");
                throw;
            }
        }

        public async Task UpdateBookAsync(Book book)
        {
            try
            {
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in BookRepository while updating the book with ID: {book.ISBN}.");
                throw;
            }
        }

        public async Task DeleteBookAsync(Book book)
        {
            try
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in BookRepository while deleting the book with ID: {book.ISBN}.");
                throw;
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByGenres(IEnumerable<Genre> genres)
        {
            try
            {
                return _context.Books.Where(book => genres.Contains(book.Genre)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in BookRepository while getting books by genres.");
                throw;
            }
        }

        public async Task UpdateBookStatus(string ISBN, bool availability)
        {
            try
            {
                var book = await GetBookByISBNAsync(ISBN);
                book.IsAvailable = availability;
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in BookRepository while updating the status of the book with ISBN: {ISBN}.");
                throw;
            }
        }
    }
}
