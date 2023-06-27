using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IPatronRepository _patronRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IBookRepository bookRepository,
            IPatronRepository patronRepository,IMapper mapper, IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
            _patronRepository = patronRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }
        public async Task<Review> GetReviewByIdAsync(string ISBN, int reviewId)
        {
            Book book = await _bookRepository.GetBookByISBNAsync(ISBN);
            if (book == null)
            {
                throw new ResourceNotFoundException("Book", "ISBN", ISBN);
            }
            var review = await _reviewRepository.GetBookReviewByIdAsync(ISBN, reviewId);
            return review;
        }
        public async Task<Review> UpdateReviewAsync(string ISBN, int reviewId, ReviewDto reviewDto)
        {
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

        public async Task DeleteReviewAsync(string ISBN, int reviewId)
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
        public async Task<ReviewDto> AddReviewAsync(string ISBN, ReviewDto reviewDto)
        {
            var book = await _bookRepository.GetBookByISBNAsync(ISBN);
            if (book == null)
            {
                throw new ResourceNotFoundException("Book", "ISBN", ISBN);
            }
            string patronId = await _userRepository.GetCurrentUserIdAsync();
            if (patronId.Equals(""))
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

        public async Task<IEnumerable<Review>> GetReviewsByBookIdAsync(string ISBN)
        {
            var book = await _bookRepository.GetBookByISBNAsync(ISBN);
            if (book == null)
            {
                throw new ResourceNotFoundException("Book", "ISBN", ISBN);
            }

            var reviews = await _reviewRepository.GetBookReviewsByISBNAsync(ISBN);

            return reviews;
        }
        public async Task<double> GetAverageRatingByBookIdAsync(string ISBN)
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
    }

}
