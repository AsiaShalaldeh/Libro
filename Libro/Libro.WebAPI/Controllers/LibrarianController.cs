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
    //[Authorize(Roles = "Administrator")]
    public class LibrarianController : ControllerBase
    {
        private readonly ILibrarianService _librarianService;

        public LibrarianController(ILibrarianService librarianService)
        {
            _librarianService = librarianService;
        }

        [HttpGet]
        public IActionResult GetAllLibrarians()
        {
            try
            {
                var librarians = _librarianService.GetAllLibrariansAsync();
                return Ok(librarians);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetLibrarianById(int id)
        {
            try
            {
                var librarian = _librarianService.GetLibrarianByIdAsync(id);

                return Ok(librarian);
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
        public IActionResult CreateLibrarian([FromBody] Librarian librarian)
        {
            try
            {
                _librarianService.AddLibrarianAsync(librarian);
                return CreatedAtAction(nameof(GetLibrarianById), new { id = librarian.LibrarianId }, librarian);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateLibrarian(int id, [FromBody] Librarian librarian)
        {
            try
            {
                if (id != librarian.LibrarianId)
                {
                    return BadRequest("Librarian ID Mismatch");
                }
                _librarianService.UpdateLibrarianAsync(librarian);
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

        [HttpDelete("{id}")]
        public IActionResult DeleteLibrarian(int id)
        {
            try
            {
                _librarianService.DeleteLibrarianAsync(id);
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
