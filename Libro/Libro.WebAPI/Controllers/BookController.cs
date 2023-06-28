using AutoMapper;
using FluentValidation;
using Libro.Application.Validators;
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
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;
        private readonly IMapper _mapper;

        public BookController(IBookService bookService, IMapper mapper,
            ILogger<BookController> logger)
        {
            _bookService = bookService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{ISBN}")]
        public async Task<IActionResult> GetBookByISBN(string ISBN)
        {
            try
            {
                _logger.LogInformation("Getting book by ISBN: {ISBN}", ISBN);

                var book = await _bookService.GetBookByISBNAsync(ISBN);

                if (book == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", ISBN);
                }
                var bookDto = _mapper.Map<BookDto>(book);

                _logger.LogInformation("Retrieved book by ISBN: {ISBN}", ISBN);

                return Ok(bookDto);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting book by ISBN: {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting all books. Page number: {PageNumber}, Page size: {PageSize}", pageNumber, pageSize);

                var response = await _bookService.GetAllBooksAsync(pageNumber, pageSize);

                _logger.LogInformation("Retrieved all books. Page number: {PageNumber}, Page size: {PageSize}", pageNumber, pageSize);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all books: {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string? title = null, [FromQuery] string? author = null, [FromQuery] string? genre = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Searching books. Title: {Title}, Author: {Author}, Genre: {Genre}, Page number: {PageNumber}, Page size: {PageSize}", title, author, genre, pageNumber, pageSize);

                if (title == null && author == null && genre == null)
                    return BadRequest("You should provide any search term !!");

                var paginatedBooks = await _bookService.SearchBooksAsync(title, author, genre, pageNumber, pageSize);

                if (paginatedBooks == null || paginatedBooks.TotalCount == 0)
                {
                    _logger.LogInformation("No books found with search parameters. Title: {Title}, Author: {Author}, Genre: {Genre}", title, author, genre);
                    return NotFound("No books found !!");
                }

                _logger.LogInformation("Retrieved books with search parameters. Title: {Title}, Author: {Author}, Genre: {Genre}", title, author, genre);

                return Ok(paginatedBooks);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching books: {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> AddBook([FromBody] BookRequest bookDto)
        {
            try
            {
                _logger.LogInformation("Adding book: {ISBN}, Title: {Title}", bookDto.ISBN, bookDto.Title);

                BookRequestValidator validator = new BookRequestValidator();
                validator.ValidateAndThrow(bookDto);

                Book book = await _bookService.GetBookByISBNAsync(bookDto.ISBN);
                if (book != null)
                {
                    _logger.LogInformation("Book with ISBN {ISBN} already exists", bookDto.ISBN);
                    return BadRequest("The ISBN should be unique !!");
                }

                BookDto createdBook = await _bookService.AddBookAsync(bookDto);

                _logger.LogInformation("Book added successfully. ISBN: {ISBN}", bookDto.ISBN);

                return CreatedAtAction(nameof(GetBookByISBN), new { createdBook.ISBN }, createdBook);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding book: {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut("{ISBN}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> UpdateBook(string ISBN, [FromBody] BookRequest bookDto)
        {
            try
            {
                _logger.LogInformation("Updating book: {ISBN}, Title: {Title}", ISBN, bookDto.Title);

                BookRequestValidator validator = new BookRequestValidator(false, false, false);
                validator.ValidateAndThrow(bookDto);

                if (!ISBN.Equals(bookDto.ISBN))
                {
                    _logger.LogInformation("Book ISBN mismatch. Expected: {ExpectedISBN}, Received: {ReceivedISBN}", ISBN, bookDto.ISBN);
                    return BadRequest("Book ISBN Mismatch");
                }

                await _bookService.UpdateBookAsync(ISBN, bookDto);

                _logger.LogInformation("Book updated successfully. ISBN: {ISBN}", ISBN);

                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating book: {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{ISBN}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> RemoveBook(string ISBN)
        {
            try
            {
                _logger.LogInformation("Removing book: {ISBN}", ISBN);

                await _bookService.RemoveBookAsync(ISBN);

                _logger.LogInformation("Book removed successfully. ISBN: {ISBN}", ISBN);

                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing book: {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }

}
