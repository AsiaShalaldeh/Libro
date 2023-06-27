using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Repositories
{
    public class PatronRepository : IPatronRepository
    {
        private readonly LibroDbContext _context;
        public PatronRepository(LibroDbContext context)
        {
            _context = context;
        }

        public async Task<Patron> GetPatronByIdAsync(string patronId)
        {
            return _context.Patrons.Include(p => p.Reviews)
                .Include(p => p.ReservedBooks)
                .Include(p => p.CheckedoutBooks)
                .FirstOrDefault(p => p.PatronId.Equals(patronId));
        }
        public async Task AddPatronAsync(Patron patron)
        {
            _context.Patrons.AddAsync(patron);
            await _context.SaveChangesAsync();
        }

        public async Task<Patron> UpdatePatronAsync(Patron patron)
        {
            _context.Patrons.Update(patron);
            await _context.SaveChangesAsync();
            return patron;
        }
    }
}
