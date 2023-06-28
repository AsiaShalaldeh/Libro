using AutoMapper;
using FluentValidation;
using Libro.Application.Validators;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/patrons")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PatronController : Controller
    {
        private readonly IPatronService _patronService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatronController> _logger;

        public PatronController(IPatronService patronService, IMapper mapper, ILogger<PatronController> logger)
        {
            _patronService = patronService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{patronId}")]
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

        [HttpPut("{patronId}")]
        [Authorize(Roles = "Librarian, Administrator")]
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

        [HttpGet("{patronId}/borrowing-history")]
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
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving borrowing history");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
