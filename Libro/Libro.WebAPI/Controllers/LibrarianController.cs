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
    [Route("api/librarians")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
    public class LibrarianController : ControllerBase
    {
        private readonly ILibrarianService _librarianService;

        public LibrarianController(ILibrarianService librarianService)
        {
            _librarianService = librarianService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLibrarians([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _librarianService.GetAllLibrariansAsync(pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("{librarianId}")]
        public async Task<IActionResult> GetLibrarianById(string librarianId)
        {
            try
            {
                Librarian librarian = await _librarianService.GetLibrarianByIdAsync(librarianId);
                if (librarian == null)
                {
                    throw new ResourceNotFoundException("Librarian", "ID", librarianId.ToString());
                }
                return Ok(librarian);
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

        [HttpPut("{librarianId}")]
        public async Task<IActionResult> UpdateLibrarian(string librarianId, [FromBody] LibrarianDto librarianDto)
        {
            try
            {
                LibrarianDtoValidator validator = new LibrarianDtoValidator();
                validator.ValidateAndThrow(librarianDto);
                var librarian = await _librarianService.UpdateLibrarianAsync(librarianId, librarianDto);
                return Ok(librarian);
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

        [HttpDelete("{librarianId}")]
        public async Task<IActionResult> DeleteLibrarian(string librarianId)
        {
            try
            {
                await _librarianService.DeleteLibrarianAsync(librarianId);
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
