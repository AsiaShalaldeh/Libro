using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/books/{ISBN}/reviews")]
    //[Authorize(Roles = "Patron")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        public ReviewController(IReviewService reviewService, IMapper mapper)
        {
            _reviewService = reviewService;
            _mapper = mapper;
        }

        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetReview(string ISBN, int reviewId)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(ISBN, reviewId);
                if (review == null)
                {
                    throw new ResourceNotFoundException("Review", "ID", reviewId.ToString());
                }

                return Ok(review);
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

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(string ISBN, int reviewId, ReviewDto reviewDto)
        {
            try
            {
                if (!ISBN.Equals(reviewDto.BookId))
                {
                    return BadRequest("Book ISBNs Mismatch");
                }
                var updatedReview = await _reviewService.UpdateReviewAsync(ISBN, reviewId, reviewDto);
                if (updatedReview == null)
                {
                    throw new ResourceNotFoundException("Review", "ID", reviewId.ToString());
                }

                return Ok(updatedReview);
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

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(string ISBN, int reviewId)
        {
            try
            {
                var result = await _reviewService.DeleteReviewAsync(ISBN, reviewId);
                if (!result)
                {
                    throw new ResourceNotFoundException("Review", "ID", reviewId.ToString());
                }

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

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewDto reviewDto)
        {
            try
            {
                var addedReview = await _reviewService.AddReviewAsync(reviewDto);
                return Ok(addedReview);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewsByBookId(string ISBN)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByBookIdAsync(ISBN);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }

        [HttpGet("average-rating")]
        public async Task<IActionResult> GetAverageRatingByBookId(string ISBN)
        {
            try
            {
                var averageRating = await _reviewService.GetAverageRatingByBookIdAsync(ISBN);
                return Ok(averageRating);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
        }
    }

}
