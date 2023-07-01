using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Infrastructure.Data;

namespace Libro.Tests.MockData
{
    public static class BookMockData
    {
        public static void InitializeTestData(LibroDbContext dbContext)
        {
            var authors = new List<Author>
            {
                new Author { AuthorId = 1, Name = "John Smith" },
                new Author { AuthorId = 2, Name = "Jane Johnson" },
            };
            dbContext.Authors.AddRange(authors);

            var books = new List<Book>
            {
                new Book { ISBN = "1234567890", Title = "Book 1", Genre = Genre.ScienceFiction, AuthorId = 1 },
                new Book { ISBN = "1234027890", Title = "Book 2", Genre = Genre.ScienceFiction, AuthorId = 2 },
            };
            dbContext.Books.AddRange(books);

            dbContext.SaveChanges();
        }
    }
}
