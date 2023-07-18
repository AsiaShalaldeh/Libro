using AutoMapper;
using FluentValidation;
using Libro.Application.Validators;
using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Libro.Domain.Models;
using Libro.WebAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/patrons")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PatronController : ControllerBase
    {
        private readonly IPatronService _patronService;
        private readonly ILogger<PatronController> _logger;

        public PatronController(IPatronService patronService, IMapper mapper, ILogger<PatronController> logger)
        {
            _patronService = patronService ?? throw new ArgumentNullException(nameof(patronService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a patron's profile by ID.
        /// </summary>
        /// <param name="patronId">The ID of the patron.</param>
        /// <returns>The patron's profile.</returns>
        [HttpGet("{patronId}")]
        [ProducesResponseType(typeof(PatronDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPatronProfile(string patronId)
        {
            try
            {
                var patronProfile = await _patronService.GetPatronAsync(patronId);
                if (patronProfile == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
                }

                _logger.LogInformation("Retrieved patron profile for ID: {PatronId}", patronId);

                return Ok(patronProfile);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {ErrorMessage}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving patron profile");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get all patrons.
        /// </summary>
        /// <returns>A list of all patrons.</returns>
        [HttpGet]
        [Authorize(Roles = "Librarian, Administrator")]
        [ProducesResponseType(typeof(List<PatronDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<PatronDto>>> GetAllPatrons([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var patrons = await _patronService.GetAllPatrons(pageNumber, pageSize);
                _logger.LogInformation("Retrieved all patrons successfully.");
                PaginationHelper.SetPaginationHeaders(Response, patrons.TotalCount, pageNumber, pageSize);
                return Ok(patrons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving patrons.");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update a patron's profile.
        /// </summary>
        /// <param name="patronId">The ID of the patron.</param>
        /// <param name="patronDto">The updated patron profile data.</param>
        /// <returns>The updated patron profile.</returns>
        [HttpPut("{patronId}")]
        [Authorize(Roles = "Librarian, Administrator")]
        [ProducesResponseType(typeof(PatronDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdatePatronProfile(string patronId, [FromBody] PatronDto patronDto)
        {
            try
            {
                PatronDtoValidator validator = new PatronDtoValidator();
                validator.ValidateAndThrow(patronDto);

                var updatedPatron = await _patronService.UpdatePatronAsync(patronId, patronDto);

                _logger.LogInformation("Updated patron profile for ID: {PatronId}", patronId);

                return Ok(updatedPatron);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error: {ErrorMessage}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {ErrorMessage}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating patron profile");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get a patron's borrowing history.
        /// </summary>
        /// <param name="patronId">The ID of the patron.</param>
        /// <returns>The patron's borrowing history.</returns>
        [HttpGet("{patronId}/borrowing-history")]
        [ProducesResponseType(typeof(TransactionResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBorrowingHistory(string patronId)
        {
            try
            {
                var borrowingHistory = await _patronService.GetBorrowingHistoryAsync(patronId);

                _logger.LogInformation("Retrieved borrowing history for patron ID: {PatronId}", patronId);

                return Ok(borrowingHistory);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {ErrorMessage}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving borrowing history");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}