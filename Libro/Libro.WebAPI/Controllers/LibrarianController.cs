using FluentValidation;
using Libro.Application.Validators;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
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
    [Route("api/librarians")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
    public class LibrarianController : ControllerBase
    {
        private readonly ILibrarianService _librarianService;
        private readonly ILogger<LibrarianController> _logger;

        public LibrarianController(ILibrarianService librarianService, ILogger<LibrarianController> logger)
        {
            _librarianService = librarianService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLibrarians([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _librarianService.GetAllLibrariansAsync(pageNumber, pageSize);
                _logger.LogInformation("Retrieved all librarians successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all librarians");
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
                _logger.LogInformation("Retrieved librarian by ID successfully. Librarian ID: {LibrarianId}", librarian.LibrarianId);
                return Ok(librarian);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Requested librarian not found. Librarian ID: {LibrarianId}", librarianId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving librarian by ID. Librarian ID: {LibrarianId}", librarianId);
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
                _logger.LogInformation("Updated librarian successfully. Librarian ID: {LibrarianId}", librarian.LibrarianId);
                return Ok(librarian);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Invalid librarian data. Librarian ID: {LibrarianId}", librarianId);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Requested librarian not found. Librarian ID: {LibrarianId}", librarianId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating librarian. Librarian ID: {LibrarianId}", librarianId);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{librarianId}")]
        public async Task<IActionResult> DeleteLibrarian(string librarianId)
        {
            try
            {
                await _librarianService.DeleteLibrarianAsync(librarianId);
                _logger.LogInformation("Deleted librarian successfully. Librarian ID: {LibrarianId}", librarianId);
                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Requested librarian not found. Librarian ID: {LibrarianId}", librarianId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting librarian. Librarian ID: {LibrarianId}", librarianId);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
