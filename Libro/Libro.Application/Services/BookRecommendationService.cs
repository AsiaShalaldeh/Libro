using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Application.Services
{
    public class BookRecommendationService : IBookRecommendationService
    {
        private readonly IBookService _bookService;
        private readonly IPatronService _patronService;
        private readonly ITransactionService _transactionService;

        public BookRecommendationService(IBookService bookService,
            IPatronService patronService,ITransactionService transactionService)
        {
            _bookService = bookService;
            _patronService = patronService;
            _transactionService = transactionService;
        }

        public async Task<IEnumerable<Book>> GetRecommendedBooks(int patronId)
        {
            Patron patron = await _patronService.GetPatronProfileAsync(patronId);

            if (patron != null)
            {
                // Get the top 3 genres of books previously borrowed by the patron
                var patronTransactions = await _transactionService.GetTransactionsByPatron(patronId);
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
                    return recommendedBooks;
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
    }
}
