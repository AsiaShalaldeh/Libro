using Libro.Domain.Entities;
using Libro.Infrastructure.Data;

namespace Libro.Tests.MockData
{
    public static class BookQueueMockData
    {
        public static void InitializeTestData(LibroDbContext _dbContext)
        {
            var bookQueue1 = new BookQueue
            {
                BookId = "1",
                PatronId = "1",
                QueuePosition = 1
            };

            var bookQueue2 = new BookQueue
            {
                BookId = "1",
                PatronId = "2",
                QueuePosition = 2
            };

            _dbContext.BookQueues.Add(bookQueue1);
            _dbContext.BookQueues.Add(bookQueue2);
            _dbContext.SaveChanges();
        }
    }
}
