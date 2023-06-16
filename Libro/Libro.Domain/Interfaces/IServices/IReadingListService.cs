using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IReadingListService
    {
        Task<ReadingList> GetReadingListByIdAsync(int listId, int patronId);
        Task<IEnumerable<ReadingList>> GetReadingListsByPatronIdAsync(int patronId);
        Task<ReadingListDto> CreateReadingListAsync(ReadingListDto readingListDto, int patronId);
        Task<bool> RemoveReadingListAsync(int listId, int patronId);
        Task<IEnumerable<BookDto>> GetBooksByReadingListAsync(int listId, int patronId);
        Task<bool> AddBookToReadingListAsync(int listId, int patronId, string bookId);
        Task<bool> RemoveBookFromReadingListAsync(int listId, int patronId, string bookId);
    }
}
