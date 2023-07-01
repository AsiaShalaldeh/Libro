using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IReadingListRepository
    {
        Task<ReadingList> GetReadingListByIdAsync(int listId, string patronId);
        Task<IEnumerable<ReadingList>> GetReadingListsByPatronIdAsync(string patronId);
        Task<ReadingList> CreateReadingListAsync(ReadingList readingList);
        Task RemoveReadingListAsync(ReadingList readingList);
        Task<IEnumerable<Book>> GetBooksByReadingListAsync(ReadingList list);
        Task AddBookToReadingListAsync(ReadingList readingList, BookList bookList);
        Task RemoveBookFromReadingListAsync(ReadingList readingList, string bookId);
    }
}
