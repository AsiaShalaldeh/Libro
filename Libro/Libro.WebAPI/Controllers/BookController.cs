using AutoMapper;
using FluentValidation;
using Libro.Application.Validators;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    [Authorize(AuthenticationSchemes = "Bearer")]
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
        public async Task<IActionResult> GetBookByISBN(string ISBN)
        {
            try
            {
                var book = await _bookService.GetBookByISBNAsync(ISBN);

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
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _bookService.GetAllBooksAsync(pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
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
                    return NotFound("No books found !!");
                }

                return Ok(paginatedBooks);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpPost]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> AddBook([FromBody] BookRequest bookDto)
        {
            try
            {
                BookRequestValidator validator = new BookRequestValidator();
                validator.ValidateAndThrow(bookDto);
                Book book = await _bookService.GetBookByISBNAsync(bookDto.ISBN);
                if (book != null)
                {
                    return BadRequest("The ISBN should be unique !!");
                }
                BookDto createdBook = await _bookService.AddBookAsync(bookDto);
                return CreatedAtAction(nameof(GetBookByISBN), new { createdBook.ISBN }, createdBook);
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
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpPut("{ISBN}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> UpdateBook(string ISBN, [FromBody] BookRequest bookDto)
        {
            try
            {
                BookRequestValidator validator = new BookRequestValidator(false, false, false);
                validator.ValidateAndThrow(bookDto);
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
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
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
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

    }

}
