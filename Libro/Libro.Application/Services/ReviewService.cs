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
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IBookRepository bookRepository,
            IPatronRepository patronRepository,IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
            _patronRepository = patronRepository;
            _mapper = mapper;
        }
        public async Task<Review> GetReviewByIdAsync(string ISBN, int reviewId)
        {
            Book book = await _bookRepository.GetByIdAsync(ISBN);
            if (book == null)
            {
                throw new ResourceNotFoundException("Book", "ISBN", ISBN);
            }
            return _reviewRepository.GetReviewByIdAsync(ISBN, reviewId);
        }
        public async Task<Review> UpdateReviewAsync(string ISBN, int reviewId, ReviewDto reviewDto)
        {
            var patron = _patronRepository.GetPatronByIdAsync(reviewDto.PatronId);
            if (patron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", reviewDto.PatronId.ToString());
            }
            Review existingReview = await GetReviewByIdAsync(ISBN, reviewId);
            if (existingReview != null)
            {
                if (existingReview.Rating > 0)
                    existingReview.Rating = reviewDto.Rating;
                if (!string.IsNullOrEmpty(existingReview.Comment))
                    existingReview.Comment = reviewDto.Comment;

                return await _reviewRepository.UpdateReviewAsync(existingReview);
            }
            return null;
        }

        public async Task<bool> DeleteReviewAsync(string ISBN, int reviewId)
        {
            Book book = await _bookRepository.GetByIdAsync(ISBN);
            if (book == null)
            {
                throw new ResourceNotFoundException("Book", "ISBN", ISBN);
            }
            return await _reviewRepository.DeleteReviewAsync(ISBN, reviewId);
        }
        public async Task<Review> AddReviewAsync(ReviewDto reviewDto)
        {
            var book = await _bookRepository.GetByIdAsync(reviewDto.BookId);
            if (book == null)
            {
                throw new ResourceNotFoundException("Book", "ID", reviewDto.BookId);
            }
            var patron = _patronRepository.GetPatronByIdAsync(reviewDto.PatronId);
            if (patron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", reviewDto.PatronId.ToString());
            }

            var review = _mapper.Map<Review>(reviewDto);
            var addedReview = await _reviewRepository.AddReviewAsync(review);

            return addedReview;
        }

        public async Task<IEnumerable<Review>> GetReviewsByBookIdAsync(string bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
            {
                throw new ResourceNotFoundException("Book", "ID", bookId);
            }

            var reviews = await _reviewRepository.GetReviewsByBookIdAsync(bookId);

            return reviews;
        }
        public async Task<double> GetAverageRatingByBookIdAsync(string bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
            {
                throw new ResourceNotFoundException("Book", "ID", bookId);
            }

            // Get the average rating for the book
            var averageRating = await _reviewRepository.GetAverageRatingByBookIdAsync(bookId);

            return averageRating;
        }
    }

}
