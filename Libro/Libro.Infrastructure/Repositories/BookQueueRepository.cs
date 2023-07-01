using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libro.Infrastructure.Repositories
{
    public class BookQueueRepository : IBookQueueRepository
    {
        private readonly LibroDbContext _context;
        private readonly ILogger<BookQueueRepository> _logger;

        public BookQueueRepository(LibroDbContext context, ILogger<BookQueueRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<BookQueue>> GetAllBookQueuesAsync()
        {
            try
            {
                return await _context.BookQueues.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all book queues.");
                throw;
            }
        }

        public async Task<List<BookQueue>> GetBookQueuesByBookIdAsync(string bookId)
        {
            try
            {
                return await _context.BookQueues
                    .Where(q => q.BookId == bookId)
                    .OrderBy(q => q.QueuePosition)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting book queues for book with ID: {bookId}.");
                throw;
            }
        }

        public async Task<BookQueue> EnqueuePatronAsync(string bookId, string patronId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while enqueueing a patron for book with ID: {bookId}.");
                throw;
            }
        }

        public async Task<BookQueue> DequeuePatronAsync(string bookId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while dequeueing a patron for book with ID: {bookId}.");
                throw;
            }
        }

        public async Task<BookQueue> PeekPatronAsync(string bookId)
        {
            try
            {
                var queue = await GetBookQueuesByBookIdAsync(bookId);
                var firstInQueue = queue.FirstOrDefault();

                return firstInQueue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while peeking at the patron for book with ID: {bookId}.");
                throw;
            }
        }

        public async Task<int> GetQueueLengthAsync(string bookId)
        {
            try
            {
                var queue = await GetBookQueuesByBookIdAsync(bookId);
                return queue.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the queue length for book with ID: {bookId}.");
                throw;
            }
        }
    }
}
