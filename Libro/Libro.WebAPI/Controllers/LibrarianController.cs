using FluentValidation;
using Libro.Application.Validators;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Libro.WebAPI.Helpers;
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
        private readonly ILogger<LibrarianController> _logger;

        public LibrarianController(ILibrarianService librarianService, ILogger<LibrarianController> logger)
        {
            _librarianService = librarianService ?? throw new ArgumentNullException(nameof(librarianService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all librarians with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>List of librarians.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Librarian>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllLibrarians([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _librarianService.GetAllLibrariansAsync(pageNumber, pageSize);
                _logger.LogInformation("Retrieved all librarians successfully");
                PaginationHelper.SetPaginationHeaders(Response, response.TotalCount, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all librarians");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get a librarian by ID.
        /// </summary>
        /// <param name="librarianId">The ID of the librarian.</param>
        /// <returns>The librarian details.</returns>
        [HttpGet("{librarianId}")]
        [ProducesResponseType(typeof(Librarian), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
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

        /// <summary>
        /// Update a librarian.
        /// </summary>
        /// <param name="librarianId">The ID of the librarian to update.</param>
        /// <param name="librarianDto">The updated librarian data.</param>
        /// <returns>The updated librarian details.</returns>
        [HttpPut("{librarianId}")]
        [ProducesResponseType(typeof(Librarian), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
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
                _logger.LogWarning(ex, "Invalid librarian data. Librarian Name: {LibrarianName}", librarianId);
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

        /// <summary>
        /// Delete a librarian.
        /// </summary>
        /// <param name="librarianId">The ID of the librarian to delete.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{librarianId}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
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