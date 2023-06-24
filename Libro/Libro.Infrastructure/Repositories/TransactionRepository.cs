using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly List<Checkout> _transactions;
        private readonly List<Reservation> _reservations;
        private readonly IBookRepository _bookRepository;
        private readonly IPatronRepository _patronRepository;
        private readonly ILibrarianRepository _librarianRepository;

        public TransactionRepository(IBookRepository bookRepository, IPatronRepository patronRepository)
        {
            _transactions = new List<Checkout>();
            _bookRepository = bookRepository;
            _patronRepository = patronRepository;
        }
        public Checkout GetActiveTransaction(string ISBN, int patronId)
        {
            return _transactions.
                FirstOrDefault(t => t.BookId == ISBN && t.PatronId == patronId && !t.IsReturned);
        }
        public void AddTransaction(Checkout transaction)
        {
            _transactions.Add(transaction);
        }
        public async Task<IEnumerable<Checkout>> GetTransactionsByPatron(int patronId)
        {
            return _transactions.Where(t => t.PatronId == patronId).ToList();
        }

        public void UpdateTransaction(Checkout transaction)
        {
            var existingTransaction = _transactions.FirstOrDefault(
                    t => t.CheckoutId == transaction.CheckoutId);        
            if (existingTransaction != null)
            {
                existingTransaction.BookId = transaction.BookId;
                existingTransaction.PatronId = transaction.PatronId;
                existingTransaction.CheckoutDate = transaction.CheckoutDate;
                if (existingTransaction is Checkout)
                {
                    //Checkout checkout = (Checkout)existingTransaction;
                    //checkout.DueDate = (Checkout)transaction.DueDate;
                    //checkout.IsReturned = transaction.IsReturned;
                    //checkout.ReturnDate = transaction.ReturnDate;
                }     
            }
            else
            {
                throw new InvalidOperationException("Transaction Not Found");
            }
        }
        public IEnumerable<string> GetOverdueBooksAsync()
        {
            var currentDate = DateTime.Now.Date;
            var overdueBookIds = _transactions
                .OfType<Checkout>()
                .Where(t => t.DueDate < currentDate && !t.IsReturned)
                .Select(t => t.BookId)
                .ToList();

            return overdueBookIds;
        }
        public IEnumerable<Checkout> GetOverdueTransactionsAsync()
        {
            var currentDate = DateTime.Now.Date;
            var overdueTransactions = _transactions
                .Where(t => t.DueDate < currentDate && !t.IsReturned)
                .ToList();

            return overdueTransactions;
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
