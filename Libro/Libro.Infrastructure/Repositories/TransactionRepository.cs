using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly List<Transaction> _transactions;
        private readonly IBookRepository _bookRepository;
        private readonly IPatronRepository _patronRepository;
        private readonly ILibrarianRepository _librarianRepository;

        public TransactionRepository(IBookRepository bookRepository, IPatronRepository patronRepository)
        {
            _transactions = new List<Transaction>();
            _bookRepository = bookRepository;
            _patronRepository = patronRepository;
        }
        public Transaction GetActiveTransaction(string ISBN, int patronId)
        {
            return _transactions.FirstOrDefault(t => t.BookId == ISBN && t.PatronId == patronId && !t.IsReturned);
        }
        public void AddTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
        }
        public void UpdateTransaction(Transaction transaction)
        {
            var existingTransaction = _transactions.FirstOrDefault(
                    t => t.TransactionId == transaction.TransactionId);

            if (existingTransaction != null)
            {
                existingTransaction.BookId = transaction.BookId;
                existingTransaction.PatronId = transaction.PatronId;
                existingTransaction.LibrarianId = transaction.LibrarianId;
                existingTransaction.Date = transaction.Date;
                existingTransaction.DueDate = transaction.DueDate;
                existingTransaction.IsReturned = transaction.IsReturned;
                existingTransaction.ReturnDate = transaction.ReturnDate;
            }
            else
            {
                throw new InvalidOperationException("Transaction Not Found");
            }
        }
        public async Task<Transaction> ReserveAsync(string ISBN, int patronId)
        {
            Book book = await _bookRepository.GetByIdAsync(ISBN); // GetBookByISBN
            Patron patron = _patronRepository.GetPatronByIdAsync(patronId); // GetPatronById

            if (book != null && patron != null)
            {
                if (book.IsAvailable)
                {
                    var transaction = new Transaction
                    {
                        TransactionId = IdGenerator.GenerateTransactionId(),
                        BookId = ISBN,
                        PatronId = patronId,
                        Patron = patron,
                        LibrarianId = 0, // Not borrowed yet
                        Date = DateTime.Now,
                        //DueDate = DateTime.Now.AddDays(7),
                        IsReturned = false
                    };

                    book.IsAvailable = false;
                    book.Transactions.Add(transaction);
                    patron.Transactions.Add(transaction);
                    AddTransaction(transaction);

                    return transaction;
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
            Book book = await _bookRepository.GetByIdAsync(ISBN);
            Patron patron = _patronRepository.GetPatronByIdAsync(patronId);
            Librarian librarian = _librarianRepository.GetLibrarianByIdAsync(librarianId);

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
                    AddTransaction(transaction);

                    return transaction;
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
            Book book = await _bookRepository.GetByIdAsync(ISBN);
            Patron patron = _patronRepository.GetPatronByIdAsync(patronId);

            if (book != null && patron != null)
            {
                var transaction = GetActiveTransaction(ISBN, patronId);

                if (transaction != null)
                {
                    transaction.IsReturned = true;
                    book.IsAvailable = true;
                    transaction.ReturnDate = DateTime.Now;

                    UpdateTransaction(transaction);

                    return transaction;
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
        public IEnumerable<string> GetOverdueBooksAsync()
        {
            var currentDate = DateTime.Now.Date;
            var overdueBookIds = _transactions
                .Where(t => t.DueDate < currentDate && !t.IsReturned)
                .Select(t => t.BookId)
                .ToList();

            return overdueBookIds;
        }
        public IEnumerable<string> GetBorrowedBooksAsync()
        {
            var borrowedBookIds = _transactions
                .Where(t => t.IsReturned == false)
                .Select(t => t.BookId)
                .ToList();

            return borrowedBookIds;
        }
        public string GetBorrowedBookByIdAsync(string ISBN)
        {
            var borrowedBookId = _transactions
                .Where(t => !t.IsReturned && t.BookId.Equals(ISBN))
                .Select(t => t.BookId)
                .FirstOrDefault();

            return borrowedBookId;
        }
    }
}
