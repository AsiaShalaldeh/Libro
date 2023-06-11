using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly List<Book> _books;
        private readonly List<Transaction> _transactions;
        private readonly List<Patron> _patrons;
        private readonly List<Librarian> _librarians;


        public BookRepository()
        {
            _books = new List<Book>
        {
            new Book
            {
                ISBN = "1234567890",
                Title = "Book1",
                PublicatinDate = new DateTime(2022, 1, 1),
                Genre = Genre.ScienceFiction,
                IsAvailable = true,
                Author = new Author {AuthorId = 1 ,Name = "John Doe" }
            },
            new Book
            {
                ISBN = "0987654321",
                Title = "Book2",
                PublicatinDate = new DateTime(2021, 5, 15),
                Genre = Genre.Mystery,
                IsAvailable = true,
                Author = new Author {AuthorId = 2 ,Name = "Tom Cruise" }
            },
        };
            _transactions = new List<Transaction>();
            _patrons = new List<Patron>();
            _librarians = new List<Librarian>();
        }

        public async Task<Book> GetByIdAsync(string ISBN)
        {
            return await Task.FromResult(_books.FirstOrDefault(b => b.ISBN.Equals(ISBN)));
        }

        public async Task<PaginatedResult<Book>> SearchAsync(string title, string author,
            string genre, int pageNumber, int pageSize)
        {
            var query = _books.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.Name.Contains(author));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(b => b.Genre.ToString().Contains(genre));
            }
            return await PaginatedResult<Book>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Book>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _books.AsQueryable();

            return await PaginatedResult<Book>.CreateAsync(query, pageNumber, pageSize);
        }
        public async Task<Transaction> ReserveAsync(string ISBN, int patronId)
        {
            var book = _books.FirstOrDefault(b => b.ISBN.Equals(ISBN));
            var patron = _patrons.FirstOrDefault(p => p.PatronId == patronId);

            if (book != null && patron != null)
            {
                if (book.IsAvailable)
                {
                    var transaction = new Transaction
                    {
                        TransactionId = IdGenerator.GenerateTransactionId(),
                        BookId = ISBN,
                        PatronId = patronId,
                        LibrarianId = 0, // Not borrowed yet
                        Date = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(7),
                        IsReturned = false
                    };

                    book.IsAvailable = false;
                    book.Transactions.Add(transaction);
                    patron.Transactions.Add(transaction);
                    _transactions.Add(transaction);

                    return await Task.FromResult(transaction);
                }
                else
                {
                    throw new InvalidOperationException("The book is not available for reservation.");
                }
            }
            else
            {
                throw new ArgumentException("Invalid ISBN or patron ID.");
            }
        }
        public async Task<Transaction> CheckoutAsync(string ISBN, int patronId, int librarianId)
        {
            var book = _books.FirstOrDefault(b => b.ISBN.Equals(ISBN));
            var patron = _patrons.FirstOrDefault(p => p.PatronId == patronId);
            var librarian = _librarians.FirstOrDefault(l => l.LibrarianId == librarianId);

            if (book != null && patron != null && librarian != null)
            {
                if (book.IsAvailable)
                {
                    var transaction = new Transaction
                    {
                        TransactionId = IdGenerator.GenerateTransactionId(),
                        BookId = ISBN,
                        PatronId = patronId,
                        LibrarianId = librarianId,
                        Date = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(14),
                        IsReturned = false
                    };

                    book.IsAvailable = false;
                    book.Transactions.Add(transaction);
                    patron.Transactions.Add(transaction);
                    _transactions.Add(transaction);

                    return await Task.FromResult(transaction);
                }
                else
                {
                    throw new InvalidOperationException("The book is not available for checkout.");
                }
            }
            else
            {
                throw new ArgumentException("Invalid ISBN, patron ID, or librarian ID.");
            }
        }
        public async Task<Transaction> ReturnAsync(string ISBN, int patronId)
        {
            var book = _books.FirstOrDefault(b => b.ISBN.Equals(ISBN));
            var patron = _patrons.FirstOrDefault(p => p.PatronId == patronId);

            if (book != null && patron != null)
            {
                var transaction = book.Transactions.FirstOrDefault(t =>
                    t.BookId.Equals(ISBN) && t.PatronId == patronId && !t.IsReturned);

                if (transaction != null)
                {
                    transaction.IsReturned = true;
                    book.IsAvailable = true;

                    return await Task.FromResult(transaction);
                }
                else
                {
                    throw new InvalidOperationException("The book is not currently borrowed by the patron.");
                }
            }
            else
            {
                throw new ArgumentException("Invalid ISBN or patron ID.");
            }
        }
        public async Task<IEnumerable<string>> GetOverdueBooksAsync()
        {
            var currentDate = DateTime.Now.Date;
            var overdueBookIds = await _transactions
                .Where(t => t.DueDate < currentDate && !t.IsReturned)
                .Select(t => t.BookId)
                .ToListAsync(); 

            return overdueBookIds;
        }
        public async Task<IEnumerable<string>> GetBorrowedBooksAsync()
        {
            var borrowedBookIds = await _transactions
                .Where(t => t.IsReturned == false)
                .Select(t => t.BookId)
                .ToListAsync();

            return borrowedBookIds;
        }
        public async Task<string> GetBorrowedBookByIdAsync(string ISBN)
        {
            var borrowedBookId = await _transactions
                .Where(t => !t.IsReturned && t.BookId.Equals(ISBN))
                .Select(t => t.BookId)
                .FirstOrDefaultAsync();

            return borrowedBookId;
        }

    }

}
