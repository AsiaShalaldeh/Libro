﻿using AutoMapper;
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
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _patronService = patronService ?? throw new ArgumentNullException(nameof(patronService));
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _loanPolicyService = loanPolicyService ?? throw new ArgumentNullException(nameof(loanPolicyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Reserve a book for the current patron.
        /// </summary>
        /// <param name="ISBN">The ISBN of the book to reserve.</param>
        /// <returns>The reserved book transaction details.</returns>
        [HttpPost("{ISBN}/reserve")]
        [Authorize(Roles = "Patron")]
        [ProducesResponseType(typeof(BookTransactionDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
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

        /// <summary>
        /// Checkout a book for a patron by a librarian.
        /// </summary>
        /// <param name="ISBN">The ISBN of the book to checkout.</param>
        /// <param name="bookCheckout">The book checkout data.</param>
        /// <returns>The checked out book transaction details.</returns>
        [HttpPost("{ISBN}/checkout")]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(typeof(BookTransactionDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
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

                // remove the patron from the queue
                await _notificationService.RemovePatronFromNotificationQueue(ISBN);
                await _notificationService.SendCheckoutNotification(patron.Email, book.Title, patron.PatronId);
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

        /// <summary>
        /// Return a book by a patron.
        /// </summary>
        /// <param name="ISBN">The ISBN of the book to return.</param>
        /// <param name="bookReturn">The book return data.</param>
        /// <returns>The returned book transaction details.</returns>
        [HttpPost("{ISBN}/accept-return")]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(typeof(BookTransactionDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
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

                await _notificationService.SendReturnNotification(patron.Email, book.Title, patron.PatronId);
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
                return NotFound(ex.Message);
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

        /// <summary>
        /// Get a list of overdue books.
        /// </summary>
        /// <returns>List of overdue books.</returns>
        [HttpGet("borrowed-books/overdue")]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(typeof(IEnumerable<BookDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
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

        /// <summary>
        /// Get a list of borrowed books.
        /// </summary>
        /// <returns>List of borrowed books.</returns>
        [HttpGet("borrowed-books")]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(typeof(IEnumerable<BookDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
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

        /// <summary>
        /// Get a borrowed book by its ISBN.
        /// </summary>
        /// <param name="ISBN">The ISBN of the book.</param>
        /// <returns>The borrowed book details.</returns>
        [HttpGet("borrowed-books/{ISBN}")]
        [Authorize(Roles = "Librarian")]
        [ProducesResponseType(typeof(BookDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
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