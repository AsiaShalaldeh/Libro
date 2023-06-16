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
    //[Authorize(Roles = "Patron")]
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
        public async Task<IActionResult> CreateReadingList(int patronId, [FromBody] ReadingListDto readingListDto)
        {
            try
            {
                if (patronId != readingListDto.PatronId)
                {
                    return BadRequest("Patron IDs Mismatch");
                }
                var createdListDto = await _readingListService.CreateReadingListAsync(readingListDto, patronId);
                return CreatedAtRoute("GetReadingList", new { patronId = patronId, listId = createdListDto.ListId }, createdListDto);
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

        [HttpGet("{patronId}/reading-lists")]
        public async Task<IActionResult> GetReadingListsByPatronId(int patronId)
        {
            try
            {
                var readingLists = await _readingListService.GetReadingListsByPatronIdAsync(patronId);
                var readingListDtos = _mapper.Map<IEnumerable<ReadingListDto>>(readingLists);
                return Ok(readingListDtos);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpGet("{patronId}/reading-lists/{listId}", Name = "GetReadingList")]
        public async Task<IActionResult> GetReadingListById(int patronId, int listId)
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
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpPost("{patronId}/reading-lists/{listId}/books/{ISBN}/add")]
        public async Task<IActionResult> AddBookToReadingList(int patronId, int listId, string ISBN)
        {
            try
            {
                var result = await _readingListService.AddBookToReadingListAsync(listId, patronId, ISBN);
                if (result)
                    return NoContent();
                else
                    return NotFound("Reading List not found.");
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

        [HttpGet("{patronId}/reading-lists/{listId}/books")]
        public async Task<IActionResult> GetBooksOfReadingList(int patronId, int listId)
        {
            try
            {
                var books = await _readingListService.GetBooksByReadingListAsync(listId, patronId);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpDelete("{patronId}/reading-lists/{listId}/books/{ISBN}/remove")]
        public async Task<IActionResult> RemoveBookFromReadingList(int patronId, int listId, string ISBN)
        {
            try
            {
                var result = await _readingListService.RemoveBookFromReadingListAsync(listId, patronId, ISBN);
                if (result)
                    return NoContent();
                else
                    return NotFound("Reading List or Book not found.");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpDelete("{patronId}/reading-lists/{listId}")]
        public async Task<IActionResult> RemoveReadingList(int patronId, int listId)
        {
            try
            {
                var result = await _readingListService.RemoveReadingListAsync(listId, patronId);
                if (result)
                    return NoContent();
                else
                    return NotFound("Reading List not found.");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }
    }

}
