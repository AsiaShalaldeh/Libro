using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly LibroDbContext _context;

        public TransactionRepository(IBookRepository bookRepository, 
            IPatronRepository patronRepository, LibroDbContext context)
        {
            _context = context;
        }
        public async Task<Reservation> AddReservationAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }
        public async Task<Checkout> AddCheckoutAsync(Checkout checkout)
        {
            _context.Checkouts.Add(checkout);
            await _context.SaveChangesAsync();
            return checkout;
        }
        public async Task UpdateCheckoutAsync(Checkout checkout)
        {
            _context.Checkouts.Update(checkout);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Checkout>> GetCheckoutTransactionsByPatronAsync(string patronId)
        {
            var checkouts = await _context.Checkouts
                .Where(c => c.PatronId.Equals(patronId))
                .Include(t => t.Book)
                .ToListAsync();
            return checkouts;
        }
        public async Task<IEnumerable<string>> GetOverdueBookIdsAsync()
        {
            var currentDate = DateTime.Now.Date;
            var overdueBookIds = await _context.Checkouts
                .Where(t => t.DueDate < currentDate && !t.IsReturned)
                .Select(t => t.BookId)
                .ToListAsync();

            return overdueBookIds;
        }
        public async Task<IEnumerable<Checkout>> GetOverdueTransactionsAsync()
        {
            var currentDate = DateTime.Now.Date;
            var overdueTransactions = await _context.Checkouts
                .Where(t => t.DueDate < currentDate && !t.IsReturned)
                .Include(t => t.Book)
                .ToListAsync();

            return overdueTransactions;
        }
        public async Task<IEnumerable<string>> GetBorrowedBookIdsAsync()
        {
            var borrowedBookIds = await _context.Checkouts
                .Where(t => t.IsReturned == false)
                .Select(t => t.BookId)
                .ToListAsync();

            return borrowedBookIds;
        }
        public async Task<string> GetBorrowedBookByIdAsync(string ISBN)
        {
            var borrowedBookId = await _context.Checkouts
                .Where(t => !t.IsReturned && t.BookId.Equals(ISBN))
                .Select(t => t.BookId)
                .FirstOrDefaultAsync();

            return borrowedBookId;
        }
    }
}
