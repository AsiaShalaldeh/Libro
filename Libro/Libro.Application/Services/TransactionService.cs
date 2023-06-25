using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Libro.Domain.Models;

namespace Libro.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IBookService _bookService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILoanPolicyService _loanPolicyService;

        public TransactionService(IBookService bookService, ITransactionRepository transactionRepository,
            ILoanPolicyService loanPolicyService)
        {
            _bookService = bookService;
            _transactionRepository = transactionRepository;
            _loanPolicyService = loanPolicyService;
        }
        public async Task<IEnumerable<Checkout>> GetTransactionsByPatron(string patronId)
        {
            var transactions = await _transactionRepository.GetTransactionsByPatron(patronId);
            return transactions;
        }

        public Checkout GetActiveTransaction(string ISBN, string patronId)
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
                    var book = await _bookService.GetBookByISBNAsync(bookId);
                    if (book != null)
                    {
                        overdueBooks.Add(book);
                    }
                }
            }
            return overdueBooks;
        }
        public IEnumerable<Checkout> GetOverdueTransactionsAsync()
        {
            return _transactionRepository.GetOverdueTransactionsAsync();
        }

        public async Task<IEnumerable<Book>> GetBorrowedBooksAsync()
        {
            var borrowedBookIds = _transactionRepository.GetBorrowedBooksAsync();
            var borrowedBooks = new List<Book>();

            if (borrowedBookIds.Any())
            {
                foreach (var bookId in borrowedBookIds)
                {
                    var book = await _bookService.GetBookByISBNAsync(bookId);
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
            var borrowedBook = await _bookService.GetBookByISBNAsync(borrowedBookId);
            return borrowedBook;
        }
        public async Task<Reservation> ReserveBookAsync(Book book, Patron patron)
        {
            // Create a reservation record and add it to the user's reserved books
            var reservation = new Reservation
            {
                PatronId = patron.PatronId,
                BookId = book.ISBN,
                ReservationDate = DateTime.UtcNow
            };
            book.Reservations.Add(reservation);
            patron.ReservedBooks.Add(reservation);
            //_transactionRepository.AddTransaction(reservation);
            return reservation;
        }

        public async Task<Checkout> CheckoutBookAsync(Book book, Patron patron)
        {
            // Create a checkout record and associate it with the user and the book
            var checkout = new Checkout
            {
                BookId = book.ISBN, // book = book 
                CheckoutDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(_loanPolicyService.GetLoanDuration()) 
            };

            //book.Transactions.Add(checkout);
            patron.CheckedoutBooks.Add(checkout);
            
            // remove the Reservation from the Transaction List
            var reservation = patron.ReservedBooks.Where(t => t.BookId.Equals(book.ISBN)).FirstOrDefault();
            patron.ReservedBooks.Remove(reservation);

            //_transactionRepository.AddTransaction(checkout);

            // Update the book status to CheckedOut
            book.IsAvailable = false;
            //await _bookService.UpdateBookAsync(book);
            return checkout;
        }

        public async Task<ReturnResponseModel> ReturnBookAsync(Book book, Patron patron)
        {
            // remove the Checkout from the Transaction List
            Checkout checkout = patron.CheckedoutBooks.Where(t => t.BookId.Equals(book.ISBN)).FirstOrDefault();
            if (checkout == null)
            {
                throw new ResourceNotFoundException("Checkout Book", "Value", null);
            }
            //patron.CheckedoutBooks.Remove(checkout);
            DateTime returnDate = DateTime.UtcNow;
            checkout.ReturnDate = returnDate;
            checkout.IsReturned = true;
            int days = (DateTime.Now - checkout.DueDate).Days;
            decimal totalFees = CalculateTotalFees(days);
            checkout.TotalFee = totalFees;

            // _transactionRepository.update(checkout);

            ReturnResponseModel response = new()
            {
                BookId = book.ISBN,
                PatronId = patron.PatronId,
                CheckoutDate = checkout.CheckoutDate,
                DueDate = checkout.DueDate,
                ReturnDate = returnDate,
                TotalFee = totalFees
            };

            return response;
        }
        private decimal CalculateTotalFees(int days)
        {
            decimal total = Math.Round(_loanPolicyService.GetLoanDuration() * _loanPolicyService.GetBorrowingFeePerDay(), 2);
            if (days == 0)
            {
                return total;
            }
            else if (days > 0)
            {
                int lateDays = days - _loanPolicyService.GetLoanDuration();
                return Math.Round(total + (_loanPolicyService.GetLateFeePerDay() * lateDays), 2);
            }
            else if (days < 0)
            {
                return Math.Round(days * _loanPolicyService.GetBorrowingFeePerDay(), 2);
            }
            return 0;
        }
    }
}
