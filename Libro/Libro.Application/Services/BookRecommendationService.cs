using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Services
{
    public class BookRecommendationService : IBookRecommendationService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IPatronRepository _patronRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookRecommendationService> _logger;

        public BookRecommendationService(IBookRepository bookRepository, IPatronRepository patronRepository,
            ITransactionRepository transactionRepository, IMapper mapper, ILogger<BookRecommendationService> logger)
        {
            _bookRepository = bookRepository;
            _patronRepository = patronRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BookDto>> GetRecommendedBooks(string patronId)
        {
            try
            {
                Patron patron = await _patronRepository.GetPatronByIdAsync(patronId);

                if (patron != null)
                {
                    // Get the top 3 genres of books previously borrowed by the patron
                    var patronTransactions = await _transactionRepository.GetCheckoutTransactionsByPatronAsync(patronId);
                    if (patronTransactions != null && patronTransactions.Any())
                    {
                        var topGenres = patronTransactions
                            .GroupBy(t => t.Book.Genre)
                            .OrderByDescending(g => g.Count())
                            .Take(3)
                            .Select(g => g.Key);

                        if (!topGenres.Any())
                            topGenres = new List<Genre>() { Genre.Fantasy, Genre.Drama, Genre.Romance };

                        IEnumerable<Book> recommendedBooks = await _bookRepository.GetBooksByGenres(topGenres);
                        return _mapper.Map<IEnumerable<BookDto>>(recommendedBooks);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting recommended books.");
                throw;
            }
        }
    }
}
