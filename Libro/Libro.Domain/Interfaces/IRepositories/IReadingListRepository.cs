using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IReadingListRepository
    {
        ReadingList GetReadingListByIdAsync(int listId, string patronId);
        Task<IEnumerable<ReadingList>> GetReadingListsByPatronIdAsync(string patronId);
        Task<ReadingList> CreateReadingListAsync(ReadingList readingList);
        Task<bool> RemoveReadingListAsync(int listId, string patronId);
        Task<IEnumerable<Book>> GetBooksByReadingListAsync(int listId, string patronId);
        Task<bool> AddBookToReadingListAsync(int listId, string patronId, string bookId);
        Task<bool> RemoveBookFromReadingListAsync(int listId, string patronId, string bookId);
    }
}
