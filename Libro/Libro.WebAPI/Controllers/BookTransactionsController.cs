using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BookTransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly INotificationService _notificationService;
        private readonly IPatronService _patronService;
        private readonly IBookService _bookService;
        private readonly IUserRepository _userRepository;
        private readonly ILoanPolicyService _loanPolicyService;
        private readonly IMapper _mapper;
        private readonly ILogger<BookTransactionsController> _logger;

        public BookTransactionsController(ITransactionService transactionService, IMapper mapper, IPatronService patronService,
            IBookService bookService, INotificationService notificationService, IUserRepository userRepository,
            ILogger<BookTransactionsController> logger, ILoanPolicyService loanPolicyService)
        {
            _transactionService = transactionService;
            _mapper = mapper;
            _patronService = patronService;
            _bookService = bookService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _loanPolicyService = loanPolicyService;
            _logger = logger;
        }

        [HttpPost("{ISBN}/reserve")]
        [Authorize(Roles = "Patron")]
        public async Task<IActionResult> ReserveBook(string ISBN)
        {
            try
            {
                Book book = await _bookService.GetBookByISBNAsync(ISBN);
                var currentPatron = await _userRepository.GetCurrentUserIdAsync();
                Patron patron = await _patronService.GetPatronAsync(currentPatron);

                if (book == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", ISBN);
                }
                if (book.IsAvailable)
                {
                    return BadRequest("The book is currently available for borrowing");
                }
                if (patron.ReservedBooks != null && patron.ReservedBooks.Any(transaction => transaction.BookId.Equals(ISBN)))
                {
                    return BadRequest("You have already reserved this book!!");
                }

                var transaction = await _transactionService.ReserveBookAsync(book, patron);
                await _notificationService.AddPatronToNotificationQueue(patron.PatronId, book.ISBN);
                await _notificationService.SendReservationNotification(patron.Email, book.Title, patron.PatronId);

                _logger.LogInformation("Book reserved successfully. Book ISBN: {ISBN}, Patron ID: {PatronID}", ISBN, patron.PatronId);

                return Ok(transaction);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "ReserveBook failed. {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ReserveBook failed. {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reserving the book. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost("{ISBN}/checkout")]
        public async Task<IActionResult> CheckoutBook(string ISBN, [FromBody] BookTransactionDto bookCheckout)
        {
            try
            {
                if (!ISBN.Equals(bookCheckout.ISBN))
                {
                    return BadRequest("Book ISBN Mismatch!");
                }
                var book = await _bookService.GetBookByISBNAsync(bookCheckout.ISBN);
                var patron = await _patronService.GetPatronAsync(bookCheckout.PatronID);
                if (book == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", bookCheckout.ISBN);
                }
                if (patron == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", bookCheckout.PatronID);
                }
                if (!book.IsAvailable)
                {
                    return BadRequest("The book is currently not available for checkout");
                }
                Dictionary<string, Queue<string>> queues = await _notificationService.GetNotificationQueue();
                if (queues.Count > 0 && queues.ContainsKey(book.ISBN))
                {
                    var queue = queues[book.ISBN];
                    if (queue.Peek() != bookCheckout.PatronID)
                    {
                        return BadRequest($"Sorry, It is not {patron.Name} turn to borrow the book !!");
                    }
                }
                if (!_loanPolicyService.CanPatronCheckoutBook(patron))
                {
                    return BadRequest($"Patron with ID: {patron.PatronId} Can not Checkout The Book Since he/she " +
                        $"is not allowed to checkout more thn five books at a time");
                }
                var transaction = await _transactionService.CheckoutBookAsync(book, patron);

                _logger.LogInformation("Book checked out successfully. Book ISBN: {ISBN}, Patron ID: {PatronID}", ISBN, patron.PatronId);

                return Ok(transaction);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "CheckoutBook failed. {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "CheckoutBook failed. {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking out the book. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost("{ISBN}/return")]
        public async Task<IActionResult> ReturnBook(string ISBN, [FromBody] BookTransactionDto bookReturn)
        {
            try
            {
                if (!ISBN.Equals(bookReturn.ISBN))
                {
                    return BadRequest("Book ISBN Mismatch!");
                }
                var book = await _bookService.GetBookByISBNAsync(bookReturn.ISBN);
                var patron = await _patronService.GetPatronAsync(bookReturn.PatronID);
                if (book == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", bookReturn.ISBN);
                }
                if (patron == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", bookReturn.PatronID);
                }
                if (book.IsAvailable)
                {
                    return BadRequest($"The {book.Title} book is not borrowed by {patron.Name} Patron to be returned!!");
                }
                var transaction = await _transactionService.ReturnBookAsync(book, patron);

                // Notify the first patron in the queue if there is any
                await _notificationService.ProcessNotificationQueue(book.ISBN);

                _logger.LogInformation("Book returned successfully. Book ISBN: {ISBN}, Patron ID: {PatronID}", ISBN, patron.PatronId);

                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ReturnBook failed. {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "ReturnBook failed. {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "ReturnBook failed. {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while returning the book. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("borrowed-books/overdue")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetOverdueBooks()
        {
            try
            {
                var overdueBooks = await _transactionService.GetOverdueBooksAsync();

                if (overdueBooks == null || !overdueBooks.Any())
                {
                    return NotFound("No overdue books found.");
                }

                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(overdueBooks);
                _logger.LogInformation("Retrieved {Count} overdue books.", bookDtos.Count());
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving overdue books. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("borrowed-books")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            try
            {
                var borrowedBooks = await _transactionService.GetBorrowedBooksAsync();

                if (borrowedBooks == null || !borrowedBooks.Any())
                {
                    return NotFound("No borrowed books found.");
                }

                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(borrowedBooks);
                _logger.LogInformation("Retrieved {Count} borrowed books.", bookDtos.Count());
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving borrowed books. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("borrowed-books/{ISBN}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetBorrowedBookById(string ISBN)
        {
            try
            {
                var borrowedBook = await _transactionService.GetBorrowedBookByIdAsync(ISBN);

                if (borrowedBook == null)
                {
                    return NotFound("Book not found or not borrowed.");
                }

                var bookDto = _mapper.Map<BookDto>(borrowedBook);
                _logger.LogInformation("Retrieved borrowed book. ISBN: {ISBN}", ISBN);
                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the borrowed book. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
