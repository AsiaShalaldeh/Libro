using AutoMapper;
using EmailService.Interface;
using EmailService.Model;
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
        private readonly IEmailSender _emailSender;

        public ReviewController(IReviewService reviewService, IMapper mapper,
                                IEmailSender emailSender)
        {
            _reviewService = reviewService;
            _mapper = mapper;
            _emailSender = emailSender;
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
                return Ok(reviewDto);
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

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(string ISBN, int reviewId, ReviewDto reviewDto)
        {
            try
            {
                var updatedReview = await _reviewService.UpdateReviewAsync(ISBN, reviewId, reviewDto);
                var review = _mapper.Map<ReviewDto>(updatedReview);
                return Ok(review);
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

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(string ISBN, int reviewId)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(ISBN, reviewId);

                return NoContent();
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

        [HttpPost]
        public async Task<IActionResult> AddReview(string ISBN, [FromBody] ReviewDto reviewDto)
        {
            try
            {
                var addedReview = await _reviewService.AddReviewAsync(ISBN, reviewDto);
                return Ok(addedReview);
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

        [HttpGet]
        public async Task<IActionResult> GetReviewsByBookISBN(string ISBN)
        {
            try
            {
                IEnumerable<Review> reviews = await _reviewService.GetReviewsByBookIdAsync(ISBN);
                // if null
                var reviewsDto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
                return Ok(reviewsDto);
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

        [HttpGet("average-rating")]
        public async Task<IActionResult> GetAverageRatingByBookId(string ISBN)
        {
            try
            {
                var averageRating = await _reviewService.GetAverageRatingByBookIdAsync(ISBN);
                return Ok("Average = " + averageRating);
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
    }

}
