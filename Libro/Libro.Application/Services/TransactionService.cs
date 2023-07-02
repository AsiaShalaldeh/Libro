using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Libro.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IBookService _bookService;
        private readonly IBookRepository _bookRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILoanPolicyService _loanPolicyService;
        private readonly IPatronService _patronService;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            IBookService bookService,
            ITransactionRepository transactionRepository,
            ILoanPolicyService loanPolicyService,
            IBookRepository bookRepository,
            IMapper mapper,
            ILogger<TransactionService> logger,
            IPatronService patronService)
        {
            _bookService = bookService;
            _bookRepository = bookRepository;
            _transactionRepository = transactionRepository;
            _loanPolicyService = loanPolicyService;
            _patronService = patronService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Checkout>> GetCheckoutTransactionsByPatron(string patronId)
        {
            try
            {
                // NEW
                var patron = _patronService.GetPatronAsync(patronId);
                if (patron == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId);
                }
                var transactions = await _transactionRepository.GetCheckoutTransactionsByPatronAsync(patronId);
                return transactions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in TransactionService while retrieving checkout transactions for patron with ID: {patronId}.");
                throw;
            }
        }

        public async Task<IEnumerable<Book>> GetOverdueBooksAsync()
        {
            try
            {
                var overdueBookIds = await _transactionRepository.GetOverdueBookIdsAsync();
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in TransactionService while retrieving overdue books.");
                throw;
            }
        }

        public async Task<IEnumerable<Checkout>> GetOverdueTransactionsAsync()
        {
            try
            {
                var transactions = await _transactionRepository.GetOverdueTransactionsAsync();
                return transactions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in TransactionService while retrieving overdue transactions.");
                throw;
            }
        }

        public async Task<IEnumerable<Book>> GetBorrowedBooksAsync()
        {
            try
            {
                var borrowedBookIds = await _transactionRepository.GetBorrowedBookIdsAsync();
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in TransactionService while retrieving borrowed books.");
                throw;
            }
        }

        public async Task<Book> GetBorrowedBookByIdAsync(string ISBN)
        {
            try
            {
                if (string.IsNullOrEmpty(ISBN))
                    throw new ArgumentException("ISBN is required", nameof(ISBN));

                var borrowedBookId = await _transactionRepository.GetBorrowedBookByIdAsync(ISBN);

                if (string.IsNullOrEmpty(borrowedBookId))
                    throw new ArgumentNullException(); // Write a message and catch it in the top level

                var borrowedBook = await _bookService.GetBookByISBNAsync(borrowedBookId);
                return borrowedBook;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in TransactionService while retrieving borrowed book with ISBN: {ISBN}.");
                throw;
            }
        }

        public async Task<ReservationDto> ReserveBookAsync(Book book, Patron patron)
        {
            try
            {
                var reservation = new Reservation
                {
                    ReservationId = Guid.NewGuid().ToString(),
                    Book = book,
                    Patron = patron,
                    ReservationDate = DateTime.UtcNow
                };

                var reserve = await _transactionRepository.AddReservationAsync(reservation);
                return _mapper.Map<ReservationDto>(reserve);
            }
            catch (ResourceNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in TransactionService while reserving a book.");
                throw;
            }
        }

        public async Task<TransactionResponseModel> CheckoutBookAsync(Book book, Patron patron)
        {
            try
            {
                if (patron.CheckedoutBooks == null)
                    patron.CheckedoutBooks = new List<Checkout>();
                var checkout = new Checkout
                {
                    CheckoutId = Guid.NewGuid().ToString(),
                    Book = book,
                    Patron = patron,
                    CheckoutDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(_loanPolicyService.GetLoanDuration())
                };

                var addedCheckout = await _transactionRepository.AddCheckoutAsync(checkout);

                // Remove the Reservation from the Reservation List
                if (patron.ReservedBooks != null && patron.ReservedBooks.Any())
                {
                    var reservation = patron.ReservedBooks.Where(t => t.BookId.Equals(book.ISBN)).FirstOrDefault();
                    if (reservation != null)
                    {
                        patron.ReservedBooks.Remove(reservation);
                    }
                }
                // Update the book status 
                await _bookRepository.UpdateBookStatus(book.ISBN, false);

                return _mapper.Map<TransactionResponseModel>(addedCheckout);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in TransactionService while checking out a book.");
                throw;
            }
        }

        public async Task<TransactionResponseModel> ReturnBookAsync(Book book, Patron patron)
        {
            try
            {
                Checkout checkout = new Checkout();
                if (patron.CheckedoutBooks != null && patron.CheckedoutBooks.Any())
                {
                    checkout = patron.CheckedoutBooks.Where(t => t.BookId.Equals(book.ISBN)).FirstOrDefault();
                    if (checkout == null)
                    {
                        throw new ResourceNotFoundException("Checked Out Book", "ISBN", book.ISBN);
                    }
                }
                var returnDate = DateTime.UtcNow;
                checkout.ReturnDate = returnDate;
                checkout.IsReturned = true;

                var days = (DateTime.Now - checkout.DueDate).Days;
                var totalFees = CalculateTotalFees(days);
                checkout.TotalFee = totalFees;

                await _transactionRepository.UpdateCheckoutAsync(checkout);
                await _bookRepository.UpdateBookStatus(book.ISBN, true);

                var response = new TransactionResponseModel
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in TransactionService while returning a book.");
                throw;
            }
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
