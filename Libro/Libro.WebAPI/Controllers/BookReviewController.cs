using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/books/{ISBN}/reviews")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Patron")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewService reviewService, IMapper mapper, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get a review by its ID for a specific book.
        /// </summary>
        /// <param name="ISBN">The ISBN of the book.</param>
        /// <param name="reviewId">The ID of the review.</param>
        /// <returns>The review details.</returns>
        [HttpGet("{reviewId}")]
        [ProducesResponseType(typeof(ReviewDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetReview(string ISBN, int reviewId)
        {
            try
            {
                Review review = await _reviewService.GetReviewByIdAsync(ISBN, reviewId);
                if (review == null)
                {
                    throw new ResourceNotFoundException("Review", "ID", reviewId.ToString());
                }
                ReviewDto reviewDto = _mapper.Map<ReviewDto>(review);
                _logger.LogInformation("Retrieved review with ID {ReviewId} for book with ISBN {ISBN}.", reviewId, ISBN);
                return Ok(reviewDto);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Review not found. {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the review. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update a review for a specific book.
        /// </summary>
        /// <param name="ISBN">The ISBN of the book.</param>
        /// <param name="reviewId">The ID of the review.</param>
        /// <param name="reviewDto">The updated review data.</param>
        /// <returns>The updated review details.</returns>
        [HttpPut("{reviewId}")]
        [ProducesResponseType(typeof(ReviewDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateReview(string ISBN, int reviewId, ReviewDto reviewDto)
        {
            try
            {
                var updatedReview = await _reviewService.UpdateReviewAsync(ISBN, reviewId, reviewDto);
                var review = _mapper.Map<ReviewDto>(updatedReview);
                _logger.LogInformation("Updated review with ID {ReviewId} for book with ISBN {ISBN}.", reviewId, ISBN);
                return Ok(review);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Review not found. {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the review. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete a review for a specific book.
        /// </summary>
        /// <param name="ISBN">The ISBN of the book.</param>
        /// <param name="reviewId">The ID of the review.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{reviewId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteReview(string ISBN, int reviewId)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(ISBN, reviewId);
                _logger.LogInformation("Deleted review with ID {ReviewId} for book with ISBN {ISBN}.", reviewId, ISBN);
                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Review not found. {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the review. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Add a new review for a specific book.
        /// </summary>
        /// <param name="ISBN">The ISBN of the book.</param>
        /// <param name="reviewDto">The review data to be added.</param>
        /// <returns>The added review details.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ReviewDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddReview(string ISBN, [FromBody] ReviewDto reviewDto)
        {
            try
            {
                // Book ISBN, Rating (1-5), and Comment are Required 
                var addedReview = await _reviewService.AddReviewAsync(ISBN, reviewDto);
                _logger.LogInformation("Added new review for book with ISBN {ISBN}.", ISBN);
                return Ok(addedReview);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Book not found. {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the review. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get all reviews for a specific book.
        /// </summary>
        /// <param name="ISBN">The ISBN of the book.</param>
        /// <returns>List of reviews.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReviewDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetReviewsByBookISBN(string ISBN)
        {
            try
            {
                IEnumerable<Review> reviews = await _reviewService.GetReviewsByBookIdAsync(ISBN);
                if (reviews == null || !reviews.Any())
                {
                    _logger.LogInformation("No reviews found for book with ISBN {ISBN}.", ISBN);
                    return NotFound("No reviews found.");
                }
                var reviewsDto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
                _logger.LogInformation("Retrieved reviews for book with ISBN {ISBN}.", ISBN);
                return Ok(reviewsDto);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Book not found. {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the reviews. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get the average rating for a specific book.
        /// </summary>
        /// <param name="ISBN">The ISBN of the book.</param>
        /// <returns>The average rating.</returns>
        [HttpGet("average-rating")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAverageRatingByBookId(string ISBN)
        {
            try
            {
                var averageRating = await _reviewService.GetAverageRatingByBookIdAsync(ISBN);
                _logger.LogInformation("Retrieved average rating for book with ISBN {ISBN}. Average: {AverageRating}", ISBN, averageRating);
                return Ok("Average = " + averageRating);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Book not found. {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the average rating. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}