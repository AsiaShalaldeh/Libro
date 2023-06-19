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

        public BookRecommendationService(IBookService bookService, IPatronService patronService)
        {
            _bookService = bookService;
            _patronService = patronService;
        }

        public async Task<IEnumerable<Book>> GetRecommendedBooks(int patronId)
        {
            Patron patron = await _patronService.GetPatronProfileAsync(patronId);

            if (patron != null)
            {
                // Get the top 3 genres of books previously borrowed by the patron
                var topGenres = patron.Transactions
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
                throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
            }
        }
    }
}
