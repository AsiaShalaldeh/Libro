using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libro.Infrastructure.Repositories
{
    public class ReadingListRepository : IReadingListRepository
    {
        private readonly LibroDbContext _context;
        private readonly ILogger<ReadingListRepository> _logger;

        public ReadingListRepository(LibroDbContext context, ILogger<ReadingListRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ReadingList> GetReadingListByIdAsync(int listId, string patronId)
        {
            try
            {
                var list = await _context.ReadingLists.Include(r => r.BookLists)
                    .FirstOrDefaultAsync(r => r.ReadingListId == listId && r.PatronId.Equals(patronId));
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListRepository while getting the reading list with ID: {listId} for patron with ID: {patronId}.");
                throw;
            }
        }
        public async Task<ReadingList> GetReadingListByNameAsync(string listName, string patronId)
        {
            try
            {
                var list = await _context.ReadingLists.Include(r => r.BookLists)
                    .FirstOrDefaultAsync(r => r.Name.Equals(listName) && r.PatronId.Equals(patronId));
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListRepository while getting the reading list with Name: {listName} for patron with ID: {patronId}.");
                throw;
            }
        }
        public async Task<IEnumerable<ReadingList>> GetReadingListsByPatronIdAsync(string patronId)
        {
            try
            {
                return await _context.ReadingLists
                    .Where(r => r.PatronId.Equals(patronId))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListRepository while getting reading lists for patron with ID: {patronId}.");
                throw;
            }
        }

        public async Task<ReadingList> CreateReadingListAsync(ReadingList readingList)
        {
            try
            {
                // PatronID and Name are Required 
                _context.ReadingLists.Add(readingList);
                await _context.SaveChangesAsync();
                return readingList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ReadingListRepository while creating a reading list.");
                throw;
            }
        }

        public async Task RemoveReadingListAsync(ReadingList readingList)
        {
            try
            {
                _context.ReadingLists.Remove(readingList);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListRepository while removing the reading list with ID: {readingList.ReadingListId}.");
                throw;
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByReadingListAsync(ReadingList readingList)
        {
            try
            {
                if (readingList.BookLists.Any())
                {
                    var bookIds = readingList.BookLists.Select(bl => bl.BookId);
                    var books = await _context.Books.Where(b => bookIds.Contains(b.ISBN)).ToListAsync();
                    return books;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListRepository while getting books for the reading list with ID: {readingList.ReadingListId}");
                throw;
            }
        }

        public async Task AddBookToReadingListAsync(ReadingList readingList, BookList bookList)
        {
            try
            {
                readingList.BookLists.Add(bookList);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListRepository while adding a book to the reading list with ID: {readingList.ReadingListId}.");
                throw;
            }
        }

        public async Task RemoveBookFromReadingListAsync(ReadingList readingList, string bookId)
        {
            try
            {
                var bookList = readingList.BookLists.FirstOrDefault(bl => bl.BookId.Equals(bookId));
                if (bookList != null)
                {
                    readingList.BookLists.Remove(bookList);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListRepository while removing a book from the reading list with ID: {readingList.ReadingListId}.");
                throw;
            }
        }
    }
}
