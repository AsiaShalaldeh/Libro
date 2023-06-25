using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;

namespace Libro.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibroDbContext _context;
        public BookRepository(LibroDbContext context)
        {
            _context = context;
        }

        public async Task<Book> GetBookByISBNAsync(string ISBN)
        {
            var book = await _context.Books.FindAsync(ISBN);
            return book;
        }

        public async Task<PaginatedResult<Book>> SearchBooksAsync(string title, string author,
            string genre, int pageNumber, int pageSize)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.Name.Contains(author));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                var genreValue = Enum.Parse<Genre>(genre, ignoreCase: true);
                query = query.Where(b => b.Genre == genreValue);
            }
            return await PaginatedResult<Book>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Book>> GetAllBooksAsync(int pageNumber, int pageSize)
        {
            var query = _context.Books.AsQueryable();

            return await PaginatedResult<Book>.CreateAsync(query, pageNumber, pageSize);
        }
        public async Task AddBookAsync(Book book, Author author)
        {
            _context.Attach(author);
            author.Books.Add(book);
            book.Author = author;
            _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateBookAsync(Book book)
        {
            _context.Attach(book);
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(Book book)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Book>> GetBooksByGenres(IEnumerable<Genre> genres)
        {
            return _context.Books.Where(book => genres.Contains(book.Genre)).ToList();
        }
    }

}
