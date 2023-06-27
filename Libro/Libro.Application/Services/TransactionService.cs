using AutoMapper;
using Libro.Domain.Common;
using Libro.Domain.Dtos;
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
        private readonly IBookRepository _bookRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILoanPolicyService _loanPolicyService;
        private readonly IMapper _mapper;

        public TransactionService(IBookService bookService, ITransactionRepository transactionRepository,
            ILoanPolicyService loanPolicyService, IBookRepository bookRepository, IMapper mapper)
        {
            _bookService = bookService;
            _bookRepository = bookRepository;
            _transactionRepository = transactionRepository;
            _loanPolicyService = loanPolicyService;
            _mapper = mapper;   
        }
        public async Task<IEnumerable<Checkout>> GetCheckoutTransactionsByPatron(string patronId)
        {
            var transactions = await _transactionRepository.GetCheckoutTransactionsByPatronAsync(patronId);
            return transactions;
        }
        public async Task<IEnumerable<Book>> GetOverdueBooksAsync()
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
        public async Task<IEnumerable<Checkout>> GetOverdueTransactionsAsync()
        {
            var transactons = await _transactionRepository.GetOverdueTransactionsAsync();
            return transactons;
        }
        public async Task<IEnumerable<Book>> GetBorrowedBooksAsync()
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
        public async Task<Book> GetBorrowedBookByIdAsync(string ISBN)
        {
            if (string.IsNullOrEmpty(ISBN))
                throw new ArgumentException("ISBN is required", nameof(ISBN));
            var borrowedBookId = await _transactionRepository.GetBorrowedBookByIdAsync(ISBN);
            if (string.IsNullOrEmpty(borrowedBookId))
                return null;
            var borrowedBook = await _bookService.GetBookByISBNAsync(borrowedBookId);
            return borrowedBook;
        }
        public async Task<ReservationDto> ReserveBookAsync(Book book, Patron patron)
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

        // return CheckoutDto
        public async Task<TransactionResponseModel> CheckoutBookAsync(Book book, Patron patron)
        {
            var checkout = new Checkout
            {
                CheckoutId = Guid.NewGuid().ToString(),
                Book = book,
                Patron = patron,
                CheckoutDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(_loanPolicyService.GetLoanDuration())
            };
            var addedChekout = await _transactionRepository.AddCheckoutAsync(checkout);

            // remove the Reservation from the Reservation List
            var reservation = patron.ReservedBooks.Where(t => t.BookId.Equals(book.ISBN)).FirstOrDefault();
            patron.ReservedBooks.Remove(reservation);

            // Update the book status 
            await _bookRepository.UpdateBookStatus(book.ISBN, false);
            return _mapper.Map<TransactionResponseModel>(addedChekout);
        }

        public async Task<TransactionResponseModel> ReturnBookAsync(Book book, Patron patron)
        {
            Checkout checkout = patron.CheckedoutBooks.Where(t => t.BookId.Equals(book.ISBN)).FirstOrDefault();
            if (checkout == null)
            {
                throw new ResourceNotFoundException("Checkouted Book", "ISBN", book.ISBN);
            }
            //patron.CheckedoutBooks.Remove(checkout);
            DateTime returnDate = DateTime.UtcNow;
            checkout.ReturnDate = returnDate;
            checkout.IsReturned = true;
            int days = (DateTime.Now - checkout.DueDate).Days;
            decimal totalFees = CalculateTotalFees(days);
            checkout.TotalFee = totalFees;

            await _transactionRepository.UpdateCheckoutAsync(checkout);
            await _bookRepository.UpdateBookStatus(book.ISBN, true);

            TransactionResponseModel response = new()
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
