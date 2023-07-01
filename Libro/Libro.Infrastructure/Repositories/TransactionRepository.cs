using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libro.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly LibroDbContext _context;
        private readonly ILogger<TransactionRepository> _logger;

        public TransactionRepository(LibroDbContext context, ILogger<TransactionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Reservation> AddReservationAsync(Reservation reservation)
        {
            try
            {
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
                return reservation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a reservation.");
                throw;
            }
        }

        public async Task<Checkout> AddCheckoutAsync(Checkout checkout)
        {
            try
            {
                _context.Checkouts.Add(checkout);
                await _context.SaveChangesAsync();
                return checkout;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a checkout transaction.");
                throw;
            }
        }

        public async Task UpdateCheckoutAsync(Checkout checkout)
        {
            try
            {
                _context.Checkouts.Update(checkout);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the checkout transaction with ID: {checkout.CheckoutId}.");
                throw;
            }
        }

        public async Task<IEnumerable<Checkout>> GetCheckoutTransactionsByPatronAsync(string patronId)
        {
            try
            {
                var checkouts = await _context.Checkouts
                    .Where(c => c.PatronId.Equals(patronId))
                    .Include(t => t.Book)
                    .ToListAsync();
                return checkouts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting checkout transactions for patron with ID: {patronId}.");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetOverdueBookIdsAsync()
        {
            try
            {
                var currentDate = DateTime.Now.Date;
                var overdueBookIds = await _context.Checkouts
                    .Where(t => t.DueDate < currentDate && !t.IsReturned)
                    .Select(t => t.BookId)
                    .ToListAsync();

                return overdueBookIds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting overdue book IDs.");
                throw;
            }
        }

        public async Task<IEnumerable<Checkout>> GetOverdueTransactionsAsync()
        {
            try
            {
                var currentDate = DateTime.Now.Date;
                var overdueTransactions = await _context.Checkouts
                    .Where(t => t.DueDate < currentDate && !t.IsReturned)
                    .Include(t => t.Book)
                    .ToListAsync();

                return overdueTransactions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting overdue transactions.");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetBorrowedBookIdsAsync()
        {
            try
            {
                var borrowedBookIds = await _context.Checkouts
                    .Where(t => t.IsReturned == false)
                    .Select(t => t.BookId)
                    .ToListAsync();

                return borrowedBookIds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting borrowed book IDs.");
                throw;
            }
        }

        // This methos can be canceled since it exists in Service: returns Borrowed Book with that ISBN
        public async Task<string> GetBorrowedBookByIdAsync(string ISBN)
        {
            try
            {
                var borrowedBookId = await _context.Checkouts
                    .Where(t => !t.IsReturned && t.BookId.Equals(ISBN))
                    .Select(t => t.BookId)
                    .FirstOrDefaultAsync();

                return borrowedBookId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the borrowed book ID for ISBN: {ISBN}.");
                throw;
            }
        }
    }
}
