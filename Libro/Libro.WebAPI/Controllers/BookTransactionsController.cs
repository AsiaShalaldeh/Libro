using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookTransactionsController : ControllerBase
    {
        private readonly ITransactionService _trsansactionService;
        private readonly INotificationService _notificationService;
        private readonly IPatronService _patronService;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BookTransactionsController(ITransactionService transactionService,
            IMapper mapper, IPatronService patronService, IBookService bookService,
            INotificationService notificationService)
        {
            _trsansactionService = transactionService;
            _mapper = mapper;
            _patronService = patronService;
            _bookService = bookService;
            _notificationService = notificationService;
        }

        [HttpPost("{ISBN}/reserve")]
        //[Authorize(Roles = "Patron")]
        public async Task<IActionResult> ReserveBook([FromBody] BookTransactionDto bookReservation)
        {
            try
            {
                Book book = await _bookService.GetBookByIdAsync(bookReservation.ISBN); // GetBookByISBN
                Patron patron = await _patronService.GetPatronProfileAsync(bookReservation.PatronID); // GetPatronById

                // check book and patron
                if (book.IsAvailable)
                {
                    return BadRequest("The book is currently available for borrowing.");
                }
                //if (patron.Transactions.Any(transaction => transaction.BookId.Equals
                //(bookReservation.ISBN) && !(transaction is Checkout)))
                //{
                //    return BadRequest("You have already reserved this book.");
                //}
                if (patron.ReservedBooks.Any(transaction => transaction.BookId.Equals(bookReservation.ISBN)))
                {
                    return BadRequest("You have already reserved this book.");
                }
                var transaction = await _trsansactionService.ReserveBookAsync(book, patron);
                await _notificationService.AddPatronToNotificationQueue(patron.PatronId, book.ISBN);
                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpPost("{ISBN}/checkout")]
        //[Authorize(Roles = "Librarian")]
        public async Task<IActionResult> CheckoutBook([FromBody] BookTransactionDto bookCheckout)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(bookCheckout.ISBN);
                var patron = await _patronService.GetPatronProfileAsync(bookCheckout.PatronID);
                if (book == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", bookCheckout.ISBN);
                }
                // patron
                if (!book.IsAvailable)
                {
                    return BadRequest("The book is currently not available for checkout");
                }
                Dictionary<string, Queue<int>> queues = await _notificationService.GetNotificationQueue();
                if (queues.Count > 0 && queues.ContainsKey(book.ISBN))
                {
                    var queue = queues[book.ISBN];
                    if (queue.Peek() != bookCheckout.PatronID)
                    {
                        return BadRequest("Sorry, It is not your turn to borrow the book !!");
                    }
                }
                var transaction = await _trsansactionService.CheckoutBookAsync(book, patron);
                return Ok(transaction);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpPost("{ISBN}/return")]
        //[Authorize(Roles = "Librarian")]
        public async Task<IActionResult> ReturnBook([FromBody] BookTransactionDto bookReturn)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(bookReturn.ISBN);
                var patron = await _patronService.GetPatronProfileAsync(bookReturn.PatronID);
                if (book == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", bookReturn.ISBN);
                }
                // patron
                if (!book.IsAvailable)
                {
                    return BadRequest("The book is currently not available for checkout");
                }
                var transaction = await _trsansactionService.ReturnBookAsync(book, patron);

                // Notify the first patron in the queue if there is any
                await _notificationService.ProcessNotificationQueue(book.ISBN);

                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }
        [HttpGet("borrowed-books/overdue")]
        //[Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetOverdueBooks()
        {
            try
            {
                // should be updated to return OverdueBookDto that contains patron name/email
                var overdueBooks = await _trsansactionService.GetOverdueBooksAsync();

                if (overdueBooks == null || !overdueBooks.Any())
                {
                    return NotFound("No overdue books found.");
                }

                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(overdueBooks);
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }
        [HttpGet("borrowed-books")]
        //[Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            try
            {
                var borrowedBooks = await _trsansactionService.GetBorrowedBooksAsync();

                if (borrowedBooks == null || !borrowedBooks.Any())
                {
                    return NotFound("No borrowed books found.");
                }

                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(borrowedBooks);
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }
        [HttpGet("borrowed-books/{ISBN}")]
        //[Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetBorrowedBookById(string ISBN)
        {
            try
            {
                var borrowedBook = await _trsansactionService.GetBorrowedBookByIdAsync(ISBN);

                if (borrowedBook == null)
                {
                    return NotFound("Book not found or not borrowed.");
                }

                var bookDto = _mapper.Map<BookDto>(borrowedBook);
                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }
    }
}
