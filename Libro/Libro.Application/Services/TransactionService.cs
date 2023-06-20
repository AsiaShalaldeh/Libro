using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IBookService _bookService;
        private readonly IPatronService _patronService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILibrarianService _librarianService;

        public TransactionService(IBookService bookService, IPatronService patronService,
            ITransactionRepository transactionRepository, ILibrarianService librarianService)
        {
            _bookService = bookService;
            _patronService = patronService;
            _transactionRepository = transactionRepository;
            _librarianService = librarianService;
        }

        public Transaction GetActiveTransaction(string ISBN, int patronId)
        {
            return _transactionRepository.GetActiveTransaction(ISBN, patronId);
        }
        public async Task<IEnumerable<Book>> GetOverdueBooksAsync()
        {
            var overdueBookIds = _transactionRepository.GetOverdueBooksAsync();
            var overdueBooks = new List<Book>();

            if (overdueBookIds.Any())
            {
                foreach (var bookId in overdueBookIds)
                {
                    var book = await _bookService.GetBookByIdAsync(bookId);
                    if (book != null)
                    {
                        overdueBooks.Add(book);
                    }
                }
            }
            return overdueBooks;
        }
        public async Task<IEnumerable<Book>> GetBorrowedBooksAsync()
        {
            var borrowedBookIds = _transactionRepository.GetBorrowedBooksAsync();
            var borrowedBooks = new List<Book>();

            if (borrowedBookIds.Any())
            {
                foreach (var bookId in borrowedBookIds)
                {
                    var book = await _bookService.GetBookByIdAsync(bookId);
                    if (book != null)
                    {
                        borrowedBooks.Add(book);
                    }
                }
            }
            return borrowedBooks;
        }
        public async Task<Book> GetBorrowedBookByIdAsync(string ISBN)
        {
            if (string.IsNullOrEmpty(ISBN))
                throw new ArgumentException("ISBN is required.", nameof(ISBN));
            var borrowedBookId = _transactionRepository.GetBorrowedBookByIdAsync(ISBN);
            if (string.IsNullOrEmpty(borrowedBookId))
                return null;
            var borrowedBook = await _bookService.GetBookByIdAsync(borrowedBookId);
            return borrowedBook;
        }
        public async Task<Transaction> ReserveBookAsync(string isbn, int patronId)
        {
            if (string.IsNullOrEmpty(isbn))
                throw new ArgumentException("ISBN is required.", nameof(isbn));

            if (patronId <= 0)
                throw new ArgumentException("Invalid patron ID.", nameof(patronId));

            var transaction = await _transactionRepository.ReserveAsync(isbn, patronId);
            return transaction;
        }

        public async Task<Transaction> CheckoutBookAsync(string isbn, int patronId, int librarianId)
        {
            if (string.IsNullOrEmpty(isbn))
                throw new ArgumentException("ISBN is required.", nameof(isbn));

            if (patronId <= 0)
                throw new ArgumentException("Invalid patron ID.", nameof(patronId));

            if (librarianId <= 0)
                throw new ArgumentException("Invalid librarian ID.", nameof(librarianId));

            var transaction = await _transactionRepository.CheckoutAsync(isbn, patronId, librarianId);
            return transaction;
        }

        public async Task<Transaction> ReturnBookAsync(string isbn, int patronId)
        {
            if (string.IsNullOrEmpty(isbn))
                throw new ArgumentException("ISBN is required.", nameof(isbn));

            if (patronId <= 0)
                throw new ArgumentException("Invalid patron ID.", nameof(patronId));

            var transaction = await _transactionRepository.ReturnAsync(isbn, patronId);
            return transaction;
        }
    }
}
