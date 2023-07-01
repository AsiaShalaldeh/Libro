using Libro.Domain.Entities;
using Libro.Infrastructure.Data;

namespace Libro.Tests.MockData
{
    public static class LibrarianMockData
    {
        public static IQueryable<Librarian> GetTestLibrarians()
        {
            return new[]
            {
                new Librarian { LibrarianId = "1" },
                new Librarian { LibrarianId = "2" },
                new Librarian { LibrarianId = "3" },
            }.AsQueryable();
        }
        public static void InitializeTestData(LibroDbContext dbContext)
        {
            if (!dbContext.Librarians.Any())
            {
                var authors = new[]
                {
                new Librarian { LibrarianId = "1", Name = "Author 1" },
                new Librarian { LibrarianId = "2", Name = "Author 2" },
                new Librarian { LibrarianId = "3", Name = "Author 3" }
                };

                dbContext.Librarians.AddRange(authors);
                dbContext.SaveChanges();
            }
        }
    }
}
