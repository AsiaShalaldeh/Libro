﻿using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BookController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

        [HttpGet("{ISBN}")]
        public async Task<IActionResult> GetBookById(string ISBN)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(ISBN);

                if (book == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", ISBN);
                }

                var bookDto = _mapper.Map<BookDto>(book);
                return Ok(bookDto);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string? title = null, 
            [FromQuery] string? author = null, [FromQuery] string? genre = null, 
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (title == null && author == null && genre == null)
                    return BadRequest("You should provide any search term !!");

                var paginatedBooks = await _bookService.SearchBooksAsync(title, author, genre,
                    pageNumber, pageSize);

                if (paginatedBooks == null || paginatedBooks.TotalCount == 0)
                {
                    return NotFound("No books found.");
                }

                // Some repition here 
                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(paginatedBooks.Items);
                var response = new
                {
                    TotalCount = paginatedBooks.TotalCount,
                    PageNumber = paginatedBooks.PageNumber,
                    PageSize = paginatedBooks.PageSize,
                    Items = bookDtos
                };

                return Ok(response);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedBooks = await _bookService.GetAllBooksAsync(pageNumber, pageSize);
                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(paginatedBooks.Items);

                var response = new
                {
                    TotalCount = paginatedBooks.TotalCount,
                    PageNumber = paginatedBooks.PageNumber,
                    PageSize = paginatedBooks.PageSize,
                    Items = bookDtos
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }
        [HttpPost("reserve")]
        [Authorize(Roles = "Patron")]
        public async Task<IActionResult> ReserveBook([FromBody] BookTransactionDto bookTransactionDto)
        {
            try
            {
                var transaction = await _bookService.ReserveBookAsync(bookTransactionDto.ISBN, 
                    bookTransactionDto.PatronID);
                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }

        [HttpPost("checkout")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> CheckoutBook([FromBody] CheckoutBookDto checkoutBookDto)
        {
            try
            {
                var transaction = await _bookService.CheckoutBookAsync(checkoutBookDto.ISBN,
                    checkoutBookDto.PatronID, checkoutBookDto.LibrarianId);
                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }

        [HttpPost("return")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> ReturnBook([FromBody] BookTransactionDto bookTransactionDto)
        {
            try
            {
                var transaction = await _bookService.ReturnBookAsync(bookTransactionDto.ISBN,
                    bookTransactionDto.PatronID);
                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }
        [HttpGet("borrowed-books/overdue")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetOverdueBooks()
        {
            try
            {
                var overdueBooks = await _bookService.GetOverdueBooksAsync();

                if (overdueBooks == null || !overdueBooks.Any())
                {
                    return NotFound("No overdue books found.");
                }

                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(overdueBooks);
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }
        [HttpGet("borrowed-books")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            try
            {
                var borrowedBooks = await _bookService.GetBorrowedBooksAsync();

                if (borrowedBooks == null || !borrowedBooks.Any())
                {
                    return NotFound("No borrowed books found.");
                }

                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(borrowedBooks);
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }
        [HttpGet("borrowed-books/{ISBN}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetBorrowedBookById(string ISBN)
        {
            try
            {
                var borrowedBook = await _bookService.GetBorrowedBookByIdAsync(ISBN);

                if (borrowedBook == null)
                {
                    return NotFound("Book not found or not borrowed.");
                }

                var bookDto = _mapper.Map<BookDto>(borrowedBook);
                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }
        [HttpPost]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> AddBook(RequestBookDto bookDto)
        {
            try
            {
                await _bookService.AddBookAsync(bookDto);
                return CreatedAtAction(nameof(GetBookById), new { bookDto.ISBN }, bookDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }

        [HttpPut("{ISBN}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> UpdateBook(string ISBN, RequestBookDto bookDto)
        {
            try
            {
                if (!ISBN.Equals(bookDto.ISBN))
                {
                    return BadRequest("Book ISBN Mismatch");
                }
                await _bookService.UpdateBookAsync(ISBN, bookDto);
                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }

        [HttpDelete("{ISBN}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> RemoveBook(string ISBN)
        {
            try
            {
                await _bookService.RemoveBookAsync(ISBN);
                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }

    }

}
