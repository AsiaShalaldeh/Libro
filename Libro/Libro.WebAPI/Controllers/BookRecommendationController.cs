using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/patrons/{patronId}/recommendations")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Patron")]
    public class BookRecommendationController : ControllerBase
    {
        private readonly IBookRecommendationService _recommendationService;
        private readonly ILogger<BookRecommendationController> _logger;

        public BookRecommendationController(IBookRecommendationService recommendationService,
            ILogger<BookRecommendationController> logger)
        {
            _recommendationService = recommendationService ?? throw new ArgumentNullException(nameof(recommendationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get recommended books for a patron.
        /// </summary>
        /// <param name="patronId">The ID of the patron.</param>
        /// <returns>List of recommended books.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Book>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRecommendedBooks(string patronId)
        {
            try
            {
                _logger.LogInformation("Getting recommended books for patron: {PatronId}", patronId);

                var recommendedBooks = await _recommendationService.GetRecommendedBooks(patronId);

                _logger.LogInformation("Retrieved {Count} recommended books for patron: {PatronId}",
                    recommendedBooks.Count(), patronId);

                return Ok(recommendedBooks);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting recommended books: {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}