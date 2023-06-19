using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/recommendations")]
    //[Authorize(Roles = "Patron")]
    public class BookRecommendationController : Controller
    {
        private readonly IBookRecommendationService _recommendationService;

        public BookRecommendationController(IBookRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet("{patronId}")]
        public IActionResult GetRecommendedBooks(int patronId)
        {
            try
            {
                var recommendedBooks = _recommendationService.GetRecommendedBooks(patronId);

                return Ok(recommendedBooks);
            }
            catch(ResourceNotFoundException ex)
            {
                return BadRequest(ex.Message);  
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }
    }

}
