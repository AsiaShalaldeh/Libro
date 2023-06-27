using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Repositories
{
    public class LibrarianRepository : ILibrarianRepository
    {
        private readonly LibroDbContext _context;

        public LibrarianRepository(LibroDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Librarian>> GetAllLibrariansAsync(int pageNumber, int pageSize)
        {
            var query = _context.Librarians.AsQueryable();

            return await PaginatedResult<Librarian>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<Librarian> GetLibrarianByIdAsync(string librarianId)
        {
            return await _context.Librarians
                .FirstOrDefaultAsync(lib => lib.LibrarianId.Equals(librarianId));
        }


        public async Task<Librarian> AddLibrarianAsync(Librarian librarian)
        {
            _context.Librarians.Add(librarian);
            await _context.SaveChangesAsync();
            return librarian;
        }

        public async Task<Librarian> UpdateLibrarianAsync(Librarian librarian)
        {
            _context.Librarians.Update(librarian);
            await _context.SaveChangesAsync();
            return librarian;
        }

        public async Task DeleteLibrarianAsync(Librarian librarian)
        {
            _context.Librarians.Remove(librarian);
            await _context.SaveChangesAsync();
        }
    }

}
