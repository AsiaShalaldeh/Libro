using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IReadingListService
    {
        Task<ReadingList> GetReadingListByIdAsync(int listId, string patronId);
        Task<ReadingList> GetReadingListByNameAsync(string listName, string patronId);
        Task<IEnumerable<ReadingList>> GetReadingListsByPatronIdAsync(string patronId);
        Task<ReadingListDto> CreateReadingListAsync(ReadingListDto readingListDto, string patronId);
        Task RemoveReadingListAsync(int listId, string patronId);
        Task<IEnumerable<BookDto>> GetBooksByReadingListAsync(int listId, string patronId);
        Task<bool> AddBookToReadingListAsync(int listId, string patronId, string bookId);
        Task RemoveBookFromReadingListAsync(int listId, string patronId, string bookId);
    }
}
