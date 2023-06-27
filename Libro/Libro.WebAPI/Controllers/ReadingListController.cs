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

        public ReadingListController(IReadingListService readingListService, IMapper mapper)
        {
            _readingListService = readingListService;
            _mapper = mapper;
        }

        [HttpPost("{patronId}/reading-lists")]
        public async Task<IActionResult> CreateReadingList(string patronId, [FromBody] ReadingListDto readingListDto)
        {
            try
            {
                if (!patronId.Equals(readingListDto.PatronId))
                {
                    return BadRequest("Patron ID Mismatch");
                }
                var createdListDto = await _readingListService.CreateReadingListAsync(readingListDto, patronId);
                return CreatedAtRoute("GetReadingList", new { patronId = patronId, listId = createdListDto.ReadingListId }, createdListDto);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
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
                return Ok(readingListDtos);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
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
                return Ok(readingListDto);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
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
                    return BadRequest("The Book Already Added To That List !!");
                }
                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("{patronId}/reading-lists/{listId}/books")]
        public async Task<IActionResult> GetBooksOfReadingList(string patronId, int listId)
        {
            try
            {
                var books = await _readingListService.GetBooksByReadingListAsync(listId, patronId);
                return Ok(books);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{patronId}/reading-lists/{listId}/books/{ISBN}/remove")]
        public async Task<IActionResult> RemoveBookFromReadingList(string patronId, int listId, string ISBN)
        {
            try
            {
                await _readingListService.RemoveBookFromReadingListAsync(listId, patronId, ISBN);
                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{patronId}/reading-lists/{listId}")]
        public async Task<IActionResult> RemoveReadingList(string patronId, int listId)
        {
            try
            {
                await _readingListService.RemoveReadingListAsync(listId, patronId);
                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }

}
