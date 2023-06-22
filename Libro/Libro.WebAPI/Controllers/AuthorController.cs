using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> GetAllAuthors()
        {
            try
            {
                var authors = await _authorService.GetAllAuthorsAsync();
                return Ok(authors);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpGet("{authorId}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> GetAuthorById(int authorId)
        {
            try
            {
                var author = await _authorService.GetAuthorByIdAsync(authorId);
                if (author == null)
                    throw new ResourceNotFoundException("Author", "ID", authorId.ToString());

                return Ok(author);
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
        public async Task<IActionResult> AddAuthor(AuthorDto authorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var authorId = await _authorService.AddAuthorAsync(authorDto);
                return CreatedAtAction(nameof(GetAuthorById), new { authorId }, authorDto);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpPut("{authorId}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> UpdateAuthor(int authorId, AuthorDto authorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (authorId != authorDto.AuthorId)
                    return BadRequest("Author ID mismatch.");

                var updated = await _authorService.UpdateAuthorAsync(authorDto);
                if (!updated)
                    throw new ResourceNotFoundException("Author", "ID", authorId.ToString());

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

        [HttpDelete("{authorId}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> DeleteAuthor(int authorId)
        {
            try
            {
                var deleted = await _authorService.DeleteAuthorAsync(authorId);
                if (!deleted)
                    throw new ResourceNotFoundException("Author", "ID", authorId.ToString());

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
