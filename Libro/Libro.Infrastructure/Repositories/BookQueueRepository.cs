using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Repositories
{
    public class BookQueueRepository : IBookQueueRepository
    {
        private readonly LibroDbContext _context;

        public BookQueueRepository(LibroDbContext context)
        {
            _context = context;
        }
        public async Task<List<BookQueue>> GetAllBookQueuesAsync()
        {
            return await _context.BookQueues.ToListAsync();
        }
        public async Task<List<BookQueue>> GetBookQueuesByBookIdAsync(string bookId)
        {
            return await _context.BookQueues
                .Where(q => q.BookId == bookId)
                .OrderBy(q => q.QueuePosition)
                .ToListAsync();
        }

        public async Task<BookQueue> EnqueuePatronAsync(string bookId, string patronId)
        {
            var queue = await GetBookQueuesByBookIdAsync(bookId);
            int nextPosition = queue.Count + 1;

            var bookQueue = new BookQueue
            {
                BookId = bookId,
                PatronId = patronId,
                QueuePosition = nextPosition
            };

            _context.BookQueues.Add(bookQueue);
            await _context.SaveChangesAsync();

            return bookQueue;
        }

        public async Task<BookQueue> DequeuePatronAsync(string bookId)
        {
            var queue = await GetBookQueuesByBookIdAsync(bookId);
            var firstInQueue = queue.FirstOrDefault();

            if (firstInQueue != null)
            {
                _context.BookQueues.Remove(firstInQueue);
                await _context.SaveChangesAsync();
            }

            return firstInQueue;
        }

        public async Task<BookQueue> PeekPatronAsync(string bookId)
        {
            var queue = await GetBookQueuesByBookIdAsync(bookId);
            var firstInQueue = queue.FirstOrDefault();

            return firstInQueue;
        }

        public async Task<int> GetQueueLengthAsync(string bookId)
        {
            var queue = await GetBookQueuesByBookIdAsync(bookId);
            return queue.Count;
        }
    }
}
