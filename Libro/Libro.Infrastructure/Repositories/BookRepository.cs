using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly List<Book> _books;

        public BookRepository()
        {
            _books = new List<Book>
        {
            new Book
            {
                ISBN = "1234567890",
                Title = "Book1",
                PublicatinDate = new DateTime(2022, 1, 1),
                Genre = Genre.ScienceFiction,
                IsAvailable = true,
                Author = new Author {AuthorId = 1 ,Name = "John Doe" }
            },
            new Book
            {
                ISBN = "0987654321",
                Title = "Book2",
                PublicatinDate = new DateTime(2021, 5, 15),
                Genre = Genre.Mystery,
                IsAvailable = true,
                Author = new Author {AuthorId = 2 ,Name = "Tom Cruise" }
            },
        };
        }

        public async Task<Book> GetByIdAsync(string ISBN)
        {
            return await Task.FromResult(_books.FirstOrDefault(b => b.ISBN.Equals(ISBN)));
        }

        public async Task<PaginatedResult<Book>> SearchAsync(string title, string author,
            string genre, int pageNumber, int pageSize)
        {
            var query = _books.AsQueryable();

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
                query = query.Where(b => b.Genre.ToString().Contains(genre));
            }
            return await PaginatedResult<Book>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Book>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _books.AsQueryable();

            return await PaginatedResult<Book>.CreateAsync(query, pageNumber, pageSize);
        }
    }

}
