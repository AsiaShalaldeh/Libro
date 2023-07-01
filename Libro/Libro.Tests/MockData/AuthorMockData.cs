using Libro.Domain.Entities;
using Libro.Infrastructure.Data;
using System.Collections.Generic;

namespace Libro.Infrastructure.Tests.MockData
{
    public static class AuthorMockData
    {
        public static List<Author> CreateTestAuthors(int count)
        {
            var authors = new List<Author>();
            for (int i = 1; i <= count; i++)
            {
                authors.Add(new Author { AuthorId = i, Name = $"Author {i}" });
            }
            return authors;
        }
        public static void InitializeTestData(LibroDbContext dbContext)
        {
            if (!dbContext.Authors.Any())
            {
                // Add test authors to the in-memory database
                var authors = new[]
                {
                 new Author { AuthorId = 1, Name = "John Doe", Books = new List<Book> { new Book { ISBN = "1", Title = "Book 1" } } },
                 new Author { AuthorId = 2, Name = "Jane Smith", Books = new List<Book> { new Book { ISBN = "2", Title = "Book 2" } } },
                 new Author { AuthorId = 3, Name = "Tom Johnson", Books = new List<Book> { new Book { ISBN = "3", Title = "Book 3" } } }
                };

                dbContext.Authors.AddRange(authors);
                dbContext.SaveChanges();
            }
        }
    }
}
