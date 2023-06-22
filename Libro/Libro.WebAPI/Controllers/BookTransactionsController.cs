using AutoMapper;
using Libro.Domain.Dtos;
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
        private readonly IMapper _mapper;

        public BookTransactionsController(ITransactionService transactionService, IMapper mapper)
        {
            _trsansactionService = transactionService;
            _mapper = mapper;
        }

        [HttpPost("{ISBN}/reserve")]
        //[Authorize(Roles = "Patron")]
        public async Task<IActionResult> ReserveBook([FromBody] BookTransactionDto bookTransactionDto)
        {
            try
            {
                var transaction = await _trsansactionService.ReserveBookAsync(bookTransactionDto.ISBN,
                    bookTransactionDto.PatronID);
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
        public async Task<IActionResult> CheckoutBook([FromBody] CheckoutBookDto checkoutBookDto)
        {
            try
            {
                var transaction = await _trsansactionService.CheckoutBookAsync(checkoutBookDto.ISBN,
                    checkoutBookDto.PatronID, checkoutBookDto.LibrarianId);
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

        [HttpPost("{ISBN}/return")]
        //[Authorize(Roles = "Librarian")]
        public async Task<IActionResult> ReturnBook([FromBody] BookTransactionDto bookTransactionDto)
        {
            try
            {
                var transaction = await _trsansactionService.ReturnBookAsync(bookTransactionDto.ISBN,
                    bookTransactionDto.PatronID);
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
