using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/books")]
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
        public async Task<IActionResult> SearchBooks([FromQuery] string searchTerm, 
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedBooks = await _bookService.SearchBooksAsync(searchTerm, pageNumber, pageSize);

                if (paginatedBooks == null)
                {
                    throw new ResourceNotFoundException("Books", "SearchTerm", searchTerm);
                }

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

    }

}
