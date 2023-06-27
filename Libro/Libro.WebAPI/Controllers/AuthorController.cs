using AutoMapper;
using FluentValidation;
using Libro.Application.Validators;
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
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly IMapper _mapper;

        public AuthorController(IAuthorService authorService, IMapper mapper)
        {
            _authorService = authorService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> GetAllAuthors([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _authorService.GetAllAuthorsAsync(pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
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
                var authorDto = _mapper.Map<AuthorResponseDto>(author);
                return Ok(authorDto);
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

        [HttpPost]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorDto authorDto)
        {
            try
            {
                AuthorDtoValidator validator = new AuthorDtoValidator();
                validator.ValidateAndThrow(authorDto);

                var author = await _authorService.AddAuthorAsync(authorDto);
                return CreatedAtAction(nameof(GetAuthorById), new { author.AuthorId }, author);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut("{authorId}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> UpdateAuthor(int authorId, AuthorDto authorDto)
        {
            try
            {
                AuthorDtoValidator validator = new AuthorDtoValidator();
                validator.ValidateAndThrow(authorDto);
                await _authorService.UpdateAuthorAsync(authorId, authorDto);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
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

        [HttpDelete("{authorId}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> DeleteAuthor(int authorId)
        {
            try
            {
                await _authorService.DeleteAuthorAsync(authorId);
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
