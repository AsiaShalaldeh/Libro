using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IReadingListRepository
    {
        ReadingList GetReadingListByIdAsync(int listId, int patronId);
        Task<IEnumerable<ReadingList>> GetReadingListsByPatronIdAsync(int patronId);
        Task<ReadingList> CreateReadingListAsync(ReadingList readingList);
        Task<bool> RemoveReadingListAsync(int listId, int patronId);
        Task<IEnumerable<Book>> GetBooksByReadingListAsync(int listId, int patronId);
        Task<bool> AddBookToReadingListAsync(int listId, int patronId, string bookId);
        Task<bool> RemoveBookFromReadingListAsync(int listId, int patronId, string bookId);
    }
}
