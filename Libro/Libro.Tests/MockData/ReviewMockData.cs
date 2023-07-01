using Libro.Domain.Entities;
using Libro.Infrastructure.Data;

namespace Libro.Tests.MockData
{
    public static class ReviewMockData
    {
        public static void InitializeTestData(LibroDbContext dbContext)
        {
            var reviews = new List<Review>
            {
                new Review { BookId = "9781234567890", ReviewId = 1, Rating = 4, Comment = "Great book", PatronId = "1"},
                new Review { BookId = "9781234567890", ReviewId = 2, Rating = 3, Comment = "Good book", PatronId = "2"},
                new Review { BookId = "9780987654321", ReviewId = 3, Rating = 2, Comment = "Average book", PatronId = "3" },
            };

            dbContext.Reviews.AddRange(reviews);
            dbContext.SaveChanges();
        }
    }
}
