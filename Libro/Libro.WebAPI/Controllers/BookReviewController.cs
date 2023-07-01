using AutoMapper;
using Infrastructure.EmailService.Interface;
using Infrastructure.EmailService.Model;
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

        [HttpGet("{reviewId}")]
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

        [HttpPut("{reviewId}")]
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the review. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{reviewId}")]
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the review. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(string ISBN, [FromBody] ReviewDto reviewDto)
        {
            try
            {
                var addedReview = await _reviewService.AddReviewAsync(ISBN, reviewDto);
                _logger.LogInformation("Added new review for book with ISBN {ISBN}.", ISBN);
                return Ok(addedReview);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Book not found. {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the review. {Message}", ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
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

        [HttpGet("average-rating")]
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
