using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libro.Infrastructure.Repositories
{
    public class LibrarianRepository : ILibrarianRepository
    {
        private readonly LibroDbContext _context;
        private readonly ILogger<LibrarianRepository> _logger;

        public LibrarianRepository(LibroDbContext context, ILogger<LibrarianRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedResult<Librarian>> GetAllLibrariansAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Librarians.AsQueryable();

                return await PaginatedResult<Librarian>.CreateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in LibrarianRepository while getting all librarians.");
                throw;
            }
        }

        public async Task<Librarian> GetLibrarianByIdAsync(string librarianId)
        {
            try
            {
                return await _context.Librarians
                    .FirstOrDefaultAsync(lib => lib.LibrarianId.Equals(librarianId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in LibrarianRepository while getting the librarian with ID: {librarianId}.");
                throw;
            }
        }

        public async Task<Librarian> AddLibrarianAsync(Librarian librarian)
        {
            try
            {
                _context.Librarians.Add(librarian);
                await _context.SaveChangesAsync();
                return librarian;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in LibrarianRepository while adding a librarian.");
                throw;
            }
        }

        public async Task<Librarian> UpdateLibrarianAsync(Librarian librarian)
        {
            try
            {
                _context.Librarians.Update(librarian);
                await _context.SaveChangesAsync();
                return librarian;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in LibrarianRepository while updating the librarian with ID: {librarian.LibrarianId}.");
                throw;
            }
        }

        public async Task DeleteLibrarianAsync(Librarian librarian)
        {
            try
            {
                _context.Librarians.Remove(librarian);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in LibrarianRepository while deleting the librarian with ID: {librarian.LibrarianId}.");
                throw;
            }
        }
    }
}