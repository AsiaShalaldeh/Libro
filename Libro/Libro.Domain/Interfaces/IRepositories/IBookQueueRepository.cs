using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IBookQueueRepository
    {
        Task<List<BookQueue>> GetAllBookQueuesAsync();
        Task<List<BookQueue>> GetBookQueuesByBookIdAsync(string bookId);
        Task<BookQueue> EnqueuePatronAsync(string bookId, string patronId);
        Task<BookQueue> DequeuePatronAsync(string bookId);
        Task<BookQueue> PeekPatronAsync(string bookId);
        Task<int> GetQueueLengthAsync(string bookId);
    }
}
