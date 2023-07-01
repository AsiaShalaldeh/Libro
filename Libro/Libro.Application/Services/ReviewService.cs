using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IPatronRepository _patronRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IReviewRepository reviewRepository,
            IBookRepository bookRepository,
            IPatronRepository patronRepository,
            IMapper mapper,
            IUserRepository userRepository,
            ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
            _patronRepository = patronRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Review> GetReviewByIdAsync(string ISBN, int reviewId)
        {
            try
            { 
                var review = await _reviewRepository.GetBookReviewByIdAsync(ISBN, reviewId);
                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReviewService while retrieving the review with ID: {reviewId} for book with ISBN: {ISBN}.");
                throw;
            }
        }

        public async Task<Review> UpdateReviewAsync(string ISBN, int reviewId, ReviewDto reviewDto)
        {
            try
            {
                Book book = await _bookRepository.GetBookByISBNAsync(ISBN);
                if (book == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", ISBN);
                }
                var existingReview = await GetReviewByIdAsync(ISBN, reviewId);
                if (existingReview == null)
                {
                    throw new ResourceNotFoundException("Review", "ID", reviewId.ToString());
                }
                if (reviewDto.Rating != 0)
                    existingReview.Rating = reviewDto.Rating;

                if (!string.IsNullOrEmpty(reviewDto.Comment))
                    existingReview.Comment = reviewDto.Comment;

                return await _reviewRepository.UpdateReviewAsync(existingReview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReviewService while updating the review with ID: {reviewId} for book with ISBN: {ISBN}.");
                throw;
            }
        }

        public async Task DeleteReviewAsync(string ISBN, int reviewId)
        {
            try
            {
                Book book = await _bookRepository.GetBookByISBNAsync(ISBN);
                if (book == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", ISBN);
                }

                Review review = await _reviewRepository.GetBookReviewByIdAsync(ISBN, reviewId);
                if (review == null)
                {
                    throw new ResourceNotFoundException("Review", "ID", reviewId.ToString());
                }

                await _reviewRepository.DeleteReviewAsync(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReviewService while deleting the review with ID: {reviewId} for book with ISBN: {ISBN}.");
                throw;
            }
        }

        public async Task<ReviewDto> AddReviewAsync(string ISBN, ReviewDto reviewDto)
        {
            try
            {
                var book = await _bookRepository.GetBookByISBNAsync(ISBN);
                if (book == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", ISBN);
                }

                string patronId = await _userRepository.GetCurrentUserIdAsync();
                if (string.IsNullOrEmpty(patronId))
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
                }

                Patron patron = await _patronRepository.GetPatronByIdAsync(patronId);

                var review = new Review()
                {
                    Book = book,
                    Patron = patron,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment
                };

                Review addedReview = await _reviewRepository.AddReviewAsync(review);
                return _mapper.Map<ReviewDto>(addedReview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReviewService while adding a review for book with ISBN: {ISBN}.");
                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetReviewsByBookIdAsync(string ISBN)
        {
            try
            {
                var book = await _bookRepository.GetBookByISBNAsync(ISBN);
                if (book == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", ISBN);
                }

                var reviews = await _reviewRepository.GetBookReviewsByISBNAsync(ISBN);

                return reviews;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReviewService while retrieving reviews for book with ISBN: {ISBN}.");
                throw;
            }
        }

        public async Task<double> GetAverageRatingByBookIdAsync(string ISBN)
        {
            try
            {
                var book = await _bookRepository.GetBookByISBNAsync(ISBN);
                if (book == null)
                {
                    throw new ResourceNotFoundException("Book", "ISBN", ISBN);
                }

                // Get the average rating for the book
                var averageRating = await _reviewRepository.GetAverageRatingByBookISBNAsync(ISBN);

                return averageRating;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReviewService while retrieving the average rating for book with ISBN: {ISBN}.");
                throw;
            }
        }
    }
}
