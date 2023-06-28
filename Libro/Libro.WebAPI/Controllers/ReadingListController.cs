using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/patrons")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Patron")]
    public class ReadingListController : ControllerBase
    {
        private readonly IReadingListService _readingListService;
        private readonly IMapper _mapper;
        private readonly ILogger<ReadingListController> _logger;

        public ReadingListController(IReadingListService readingListService, IMapper mapper,
            ILogger<ReadingListController> logger)
        {
            _readingListService = readingListService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("{patronId}/reading-lists")]
        public async Task<IActionResult> CreateReadingList(string patronId, [FromBody] ReadingListDto readingListDto)
        {
            try
            {
                if (!patronId.Equals(readingListDto.PatronId))
                {
                    _logger.LogWarning("Patron ID Mismatch: {patronId}", patronId);
                    return BadRequest("Patron ID Mismatch");
                }

                var createdListDto = await _readingListService.CreateReadingListAsync(readingListDto, patronId);
                _logger.LogInformation("Reading list created: {listId}", createdListDto.ReadingListId);
                return CreatedAtRoute("GetReadingList", new { patronId = patronId, listId = createdListDto.ReadingListId }, createdListDto);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, "Resource not found: {message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error: {message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("{patronId}/reading-lists")]
        public async Task<IActionResult> GetReadingListsByPatronId(string patronId)
        {
            try
            {
                var readingLists = await _readingListService.GetReadingListsByPatronIdAsync(patronId);
                var readingListDtos = _mapper.Map<IEnumerable<ReadingListDto>>(readingLists);
                _logger.LogInformation("Retrieved reading lists for patron: {patronId}", patronId);
                return Ok(readingListDtos);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, "Resource not found: {message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error: {message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("{patronId}/reading-lists/{listId}", Name = "GetReadingList")]
        public async Task<IActionResult> GetReadingListById(string patronId, int listId)
        {
            try
            {
                var readingList = await _readingListService.GetReadingListByIdAsync(listId, patronId);
                var readingListDto = _mapper.Map<ReadingListDto>(readingList);
                _logger.LogInformation("Retrieved reading list: {listId}", listId);
                return Ok(readingListDto);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, "Resource not found: {message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error: {message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost("{patronId}/reading-lists/{listId}/books/{ISBN}/add")]
        public async Task<IActionResult> AddBookToReadingList(string patronId, int listId, string ISBN)
        {
            try
            {
                var response = await _readingListService.AddBookToReadingListAsync(listId, patronId, ISBN);
                if (!response)
                {
                    _logger.LogWarning("The book already added to the list: {listId}, ISBN: {ISBN}", listId, ISBN);
                    return BadRequest("The Book Already Added To That List !!");
                }
                _logger.LogInformation("Book added to reading list: {listId}, ISBN: {ISBN}", listId, ISBN);
                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, "Resource not found: {message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error: {message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("{patronId}/reading-lists/{listId}/books")]
        public async Task<IActionResult> GetBooksOfReadingList(string patronId, int listId)
        {
            try
            {
                var books = await _readingListService.GetBooksByReadingListAsync(listId, patronId);
                _logger.LogInformation("Retrieved books of reading list: {listId}", listId);
                return Ok(books);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, "Resource not found: {message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error: {message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{patronId}/reading-lists/{listId}/books/{ISBN}/remove")]
        public async Task<IActionResult> RemoveBookFromReadingList(string patronId, int listId, string ISBN)
        {
            try
            {
                await _readingListService.RemoveBookFromReadingListAsync(listId, patronId, ISBN);
                _logger.LogInformation("Book removed from reading list: {listId}, ISBN: {ISBN}", listId, ISBN);
                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, "Resource not found: {message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error: {message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{patronId}/reading-lists/{listId}")]
        public async Task<IActionResult> RemoveReadingList(string patronId, int listId)
        {
            try
            {
                await _readingListService.RemoveReadingListAsync(listId, patronId);
                _logger.LogInformation("Reading list removed: {listId}", listId);
                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, "Resource not found: {message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error: {message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
