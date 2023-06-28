using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Services
{
    public class BookRecommendationService : IBookRecommendationService
    {
        private readonly IBookService _bookService;
        private readonly IPatronService _patronService;
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;
        private readonly ILogger<BookRecommendationService> _logger;

        public BookRecommendationService(IBookService bookService, IPatronService patronService,
            ITransactionService transactionService, IMapper mapper, ILogger<BookRecommendationService> logger)
        {
            _bookService = bookService;
            _patronService = patronService;
            _transactionService = transactionService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BookDto>> GetRecommendedBooks(string patronId)
        {
            try
            {
                Patron patron = await _patronService.GetPatronAsync(patronId);

                if (patron != null)
                {
                    // Get the top 3 genres of books previously borrowed by the patron
                    var patronTransactions = await _transactionService.GetCheckoutTransactionsByPatron(patronId);
                    if (patronTransactions.Any())
                    {
                        var topGenres = patronTransactions
                            .GroupBy(t => t.Book.Genre)
                            .OrderByDescending(g => g.Count())
                            .Take(3)
                            .Select(g => g.Key);

                        if (!topGenres.Any())
                            topGenres = new List<Genre>() { Genre.Fantasy, Genre.Drama, Genre.Romance };

                        IEnumerable<Book> recommendedBooks = await _bookService.GetBooksByGenres(topGenres);
                        return _mapper.Map<IEnumerable<BookDto>>(recommendedBooks);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
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
