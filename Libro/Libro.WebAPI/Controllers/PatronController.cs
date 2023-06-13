using AutoMapper;
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
        public async Task<IActionResult> GetPatronProfile(int patronId)
        {
            try
            {
                var patronProfile = await _patronService.GetPatronProfileAsync(patronId);
                return Ok(patronProfile);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }

        [HttpPut("{patronId}")]
        [Authorize(Roles = "Librarian,Administrator")]
        public async Task<IActionResult> UpdatePatronProfile(int patronId, [FromBody] PatronDto patronDto)
        {
            try
            {
                if (patronId != patronDto.PatronId)
                {
                    return BadRequest("Patron ID mismatch.");
                }
                Patron patron = _mapper.Map<Patron>(patronDto);
                var updatedPatron = await _patronService.UpdatePatronProfileAsync(patron);
                return Ok(updatedPatron);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }

        [HttpGet("{patronId}/borrowing-history")]
        public async Task<IActionResult> GetBorrowingHistory(int patronId)
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
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }

    }

}
