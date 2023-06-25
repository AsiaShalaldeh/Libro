using AutoMapper;
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
    [Route("api/patrons")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PatronController : Controller
    {
        private readonly IPatronService _patronService;
        private readonly IMapper _mapper;

        public PatronController(IPatronService patronService, IMapper mapper)
        {
            _patronService = patronService;
            _mapper = mapper;
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
                return Ok(patronProfile);
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

        [HttpPut("{patronId}")]
        [Authorize(Roles = "Librarian, Administrator")]
        public async Task<IActionResult> UpdatePatronProfile(string patronId, [FromBody] PatronDto patronDto)
        {
            try
            {
                PatronDtoValidator validator = new PatronDtoValidator();
                validator.ValidateAndThrow(patronDto);
                var updatedPatron = await _patronService.UpdatePatronAsync(patronId, patronDto);
                return Ok(updatedPatron);
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

        [HttpGet("{patronId}/borrowing-history")]
        public async Task<IActionResult> GetBorrowingHistory(string patronId)
        {
            try
            {
                var borrowingHistory = await _patronService.GetBorrowingHistoryAsync(patronId);
                return Ok(borrowingHistory);
            }
            catch(ResourceNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }

}
