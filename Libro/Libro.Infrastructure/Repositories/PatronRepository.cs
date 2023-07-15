using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libro.Infrastructure.Repositories
{
    public class PatronRepository : IPatronRepository
    {
        private readonly LibroDbContext _context;
        private readonly ILogger<PatronRepository> _logger;

        public PatronRepository(LibroDbContext context, ILogger<PatronRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<Patron>> GetAllPatrons()
        {
            try
            {
                var patrons = await _context.Patrons.ToListAsync();
                return patrons;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all patrons.");
                throw;
            }
        }

        public async Task<Patron> GetPatronByIdAsync(string patronId)
        {
            try
            {
                return await _context.Patrons.Include(p => p.Reviews)
                    .Include(p => p.ReservedBooks)
                    .Include(p => p.CheckedoutBooks)
                    .Include(p => p.ReadingLists)
                    .FirstOrDefaultAsync(p => p.PatronId.Equals(patronId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in PatronRepository while getting the patron with ID: {patronId}.");
                throw;
            }
        }

        public async Task AddPatronAsync(Patron patron)
        {
            try
            {
                // return message if Email is empty
                // Email should be uniqe in add and update
                _context.Patrons.AddAsync(patron);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in PatronRepository while adding a patron.");
                throw;
            }
        }

        public async Task<Patron> UpdatePatronAsync(Patron patron)
        {
            try
            {
                _context.Patrons.Update(patron);
                await _context.SaveChangesAsync();
                return patron;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in PatronRepository while updating the patron with ID: {patron.PatronId}.");
                throw;
            }
        }
    }
}
