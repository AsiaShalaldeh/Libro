using AutoMapper;
using FluentValidation;
using Libro.Application.Validators;
using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/authors")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(IAuthorService authorService, IMapper mapper, ILogger<AuthorController> logger)
        {
            _authorService = authorService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> GetAllAuthors([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Retrieving all authors.");

                var response = await _authorService.GetAllAuthorsAsync(pageNumber, pageSize);

                _logger.LogInformation("Successfully retrieved all authors.");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all authors: {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("{authorId}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> GetAuthorById(int authorId)
        {
            try
            {
                _logger.LogInformation("Retrieving author by ID: {AuthorId}", authorId);

                var author = await _authorService.GetAuthorByIdAsync(authorId);

                if (author == null)
                {
                    _logger.LogInformation("Author not found for ID: {AuthorId}", authorId);
                    throw new ResourceNotFoundException("Author", "ID", authorId.ToString());
                }

                var authorDto = _mapper.Map<AuthorResponseDto>(author);

                _logger.LogInformation("Successfully retrieved author. AuthorId: {AuthorId}", authorId);

                return Ok(authorDto);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving author: {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorDto authorDto)
        {
            try
            {
                _logger.LogInformation("Adding author: {AuthorName}", authorDto.Name);

                AuthorDtoValidator validator = new AuthorDtoValidator();
                validator.ValidateAndThrow(authorDto);

                var author = await _authorService.AddAuthorAsync(authorDto);

                _logger.LogInformation("Author added successfully. AuthorId: {AuthorId}", author.AuthorId);

                return CreatedAtAction(nameof(GetAuthorById), new { author.AuthorId }, author);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding author: {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut("{authorId}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> UpdateAuthor(int authorId, AuthorDto authorDto)
        {
            try
            {
                _logger.LogInformation("Updating author. AuthorId: {AuthorId}", authorId);

                AuthorDtoValidator validator = new AuthorDtoValidator();
                validator.ValidateAndThrow(authorDto);

                await _authorService.UpdateAuthorAsync(authorId, authorDto);

                _logger.LogInformation("Author updated successfully. AuthorId: {AuthorId}", authorId);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating author: {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{authorId}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> DeleteAuthor(int authorId)
        {
            try
            {
                _logger.LogInformation("Deleting author. AuthorId: {AuthorId}", authorId);

                await _authorService.DeleteAuthorAsync(authorId);

                _logger.LogInformation("Author deleted successfully. AuthorId: {AuthorId}", authorId);

                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting author: {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
