using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/reading-lists")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Patron")]
    public class ReadingListController : ControllerBase
    {
        private readonly IReadingListService _readingListService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReadingListController> _logger;

        public ReadingListController(IReadingListService readingListService, IMapper mapper,
            ILogger<ReadingListController> logger, IUserRepository userRepository)
        {
            _readingListService = readingListService;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReadingList([FromBody] ReadingListDto readingListDto)
        {
            try
            {
                var currentPatron = await _userRepository.GetCurrentUserIdAsync();
                var createdListDto = await _readingListService.CreateReadingListAsync(readingListDto, currentPatron);
                _logger.LogInformation("Reading list created: {listId}", createdListDto.ReadingListId);
                return CreatedAtRoute("GetReadingList", new { patronId = currentPatron, listId = createdListDto.ReadingListId }, createdListDto);
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

        [HttpGet]
        public async Task<IActionResult> GetReadingListsByPatronId()
        {
            try
            {
                var currentPatron = await _userRepository.GetCurrentUserIdAsync();
                var readingLists = await _readingListService.GetReadingListsByPatronIdAsync(currentPatron);
                var readingListDtos = _mapper.Map<IEnumerable<ReadingListDto>>(readingLists);
                _logger.LogInformation("Retrieved reading lists for patron: {patronId}", currentPatron);
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

        [HttpGet("{listId}", Name = "GetReadingList")]
        public async Task<IActionResult> GetReadingListById(int listId)
        {
            try
            {
                var currentPatron = await _userRepository.GetCurrentUserIdAsync();
                var readingList = await _readingListService.GetReadingListByIdAsync(listId, currentPatron);
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

        [HttpPost("{listId}/books/{ISBN}/add")]
        public async Task<IActionResult> AddBookToReadingList(int listId, string ISBN)
        {
            try
            {
                var currentPatron = await _userRepository.GetCurrentUserIdAsync();
                var response = await _readingListService.AddBookToReadingListAsync(listId, currentPatron, ISBN);
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

        [HttpGet("{listId}/books")]
        public async Task<IActionResult> GetBooksOfReadingList(int listId)
        {
            try
            {
                var currentPatron = await _userRepository.GetCurrentUserIdAsync();
                var books = await _readingListService.GetBooksByReadingListAsync(listId, currentPatron);
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

        [HttpDelete("{listId}/books/{ISBN}/remove")]
        public async Task<IActionResult> RemoveBookFromReadingList(int listId, string ISBN)
        {
            try
            {
                var currentPatron = await _userRepository.GetCurrentUserIdAsync();
                await _readingListService.RemoveBookFromReadingListAsync(listId, currentPatron, ISBN);
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

        [HttpDelete("{listId}")]
        public async Task<IActionResult> RemoveReadingList(int listId)
        {
            try
            {
                var currentPatron = await _userRepository.GetCurrentUserIdAsync();
                await _readingListService.RemoveReadingListAsync(listId, currentPatron);
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
