using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Repositories
{
    public class ReadingListRepository : IReadingListRepository
    {
        private readonly LibroDbContext _context;
        private readonly IPatronRepository _patronRepository;

        public ReadingListRepository(IPatronRepository patronRepository, LibroDbContext context)
        {
            _context = context;
            _patronRepository = patronRepository;
        }

        public async Task<ReadingList> GetReadingListByIdAsync(int listId, string patronId)
        {
            var list = await _context.ReadingLists.Include(r => r.BookLists)
                .FirstOrDefaultAsync(r => r.ReadingListId == listId && r.PatronId.Equals(patronId));
            return list;
        }

        public async Task<IEnumerable<ReadingList>> GetReadingListsByPatronIdAsync(string patronId)
        {
            return await _context.ReadingLists
                .Where(r => r.PatronId.Equals(patronId))
                .ToListAsync();
        }

        public async Task<ReadingList> CreateReadingListAsync(ReadingList readingList)
        {
            _context.ReadingLists.Add(readingList);
            await _context.SaveChangesAsync();
            return readingList;
        }

        public async Task RemoveReadingListAsync(ReadingList readingList)
        {
            _context.ReadingLists.Remove(readingList);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByReadingListAsync(ReadingList readingList, string patronId)
        {
            if (readingList.BookLists.Any())
            {
                var bookIds = readingList.BookLists.Select(bl => bl.BookId);
                var books = await _context.Books.Where(b => bookIds.Contains(b.ISBN)).ToListAsync();
                return books;
            }
            return null;
        }
        public async Task AddBookToReadingListAsync(ReadingList readingList, BookList bookList)
        {
            readingList.BookLists.Add(bookList);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveBookFromReadingListAsync(ReadingList readingList, string bookId)
        { 
            var bookList = readingList.BookLists.FirstOrDefault(bl => bl.BookId.Equals(bookId));
            if (bookList != null)
            {
                readingList.BookLists.Remove(bookList);
                await _context.SaveChangesAsync();
            }
        }
    }
}
