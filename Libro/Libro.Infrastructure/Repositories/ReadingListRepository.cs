using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Infrastructure.Repositories
{
    public class ReadingListRepository : IReadingListRepository
    {
        private readonly List<ReadingList> _readingLists;
        private readonly List<Book> _books;
        private readonly IPatronRepository _patronRepository;

        public ReadingListRepository(IPatronRepository patronRepository)
        {
            _readingLists = new List<ReadingList>
        {
            new ReadingList
            {
                ListId = 1,
                Name = "My First Reading List",
                PatronId = 1,
                BookLists = new List<BookList>
                {
                    new BookList { ListId = 1, BookId = "ISBN1" },
                    new BookList { ListId = 1, BookId = "ISBN2" }
                }
            },
            new ReadingList
            {
                ListId = 2,
                Name = "My Second Reading List",
                PatronId = 1,
                BookLists = new List<BookList>
                {
                    new BookList { ListId = 2, BookId = "ISBN3" },
                    new BookList { ListId = 2, BookId = "ISBN4" }
                }
            }
        };
            _books = new List<Book>
        {
            new Book
            {
                ISBN = "ISBN1",
                Title = "Harry Potter and the Sorcerer's Stone",
                PublicationDate = new DateTime(1997, 6, 26),
                Genre = Genre.Fantasy,
                IsAvailable = true,
                AuthorId = 1
            },
            new Book
            {
                ISBN = "ISBN2",
                Title = "Harry Potter and the Chamber of Secrets",
                PublicationDate = new DateTime(1998, 7, 2),
                Genre = Genre.Fantasy,
                IsAvailable = true,
                AuthorId = 1
            },
            new Book
            {
                ISBN = "ISBN3",
                Title = "Harry Potter and the Prisoner of Azkaban",
                PublicationDate = new DateTime(1999, 7, 8),
                Genre = Genre.Fantasy,
                IsAvailable = true,
                AuthorId = 1
            },
        };
            _patronRepository = patronRepository;
        }
        public ReadingList GetReadingListByIdAsync(int listId, int patronId)
        {
            var patron = _patronRepository.GetPatronByIdAsync(patronId);
            if (patron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
            }
            var list = _readingLists.FirstOrDefault(r => r.ListId == listId && r.PatronId == patronId);
            if (list == null)
            {
                throw new ResourceNotFoundException("Reading List", "ID", listId.ToString());
            }
            return list;
        }

        public async Task<IEnumerable<ReadingList>> GetReadingListsByPatronIdAsync(int patronId)
        {
            var patron = _patronRepository.GetPatronByIdAsync(patronId);
            if (patron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
            }
            return await Task.FromResult(_readingLists.Where(r => r.PatronId == patronId));
        }

        public async Task<ReadingList> CreateReadingListAsync(ReadingList readingList)
        {
            _readingLists.Add(readingList);
            return await Task.FromResult(readingList);
        }

        public async Task<bool> RemoveReadingListAsync(int listId, int patronId)
        {
            var readingList = _readingLists.FirstOrDefault(r => r.ListId == listId 
                                && r.PatronId == patronId);
            if (readingList == null)
                return await Task.FromResult(false);

            _readingLists.Remove(readingList);
            return await Task.FromResult(true);
        }
        public async Task<IEnumerable<Book>> GetBooksByReadingListAsync(int listId, int patronId)
        {
            var readingList = GetReadingListByIdAsync(listId, patronId);
            var bookIds = readingList.BookLists.Select(bl => bl.BookId);
            var books = _books.Where(b => bookIds.Contains(b.ISBN)).ToList();
            return books;
        }

        public async Task<bool> AddBookToReadingListAsync(int listId, int patronId, string bookId)
        {
            var readingList = GetReadingListByIdAsync(listId, patronId);
            if (readingList != null)
            {
                readingList.BookLists.Add(new BookList { ListId = listId, BookId = bookId });
                return true;
            }
            else
                return false;
        }

        public async Task<bool> RemoveBookFromReadingListAsync(int listId, int patronId, string bookId)
        {
            var readingList = _readingLists.FirstOrDefault(r => r.ListId == listId && r.PatronId == patronId);
            if (readingList == null)
                return false;

            var bookList = readingList.BookLists.FirstOrDefault(bl => bl.BookId == bookId);
            if (bookList != null)
            {
                readingList.BookLists.Remove(bookList);
                return true;
            }

            return false;
        }

    }

}
