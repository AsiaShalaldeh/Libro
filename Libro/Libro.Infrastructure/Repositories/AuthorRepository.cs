using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libro.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibroDbContext _context;
        private readonly ILogger<AuthorRepository> _logger;

        public AuthorRepository(LibroDbContext context, ILogger<AuthorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<PaginatedResult<Author>> GetAllAuthorsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Authors.AsQueryable();

                return await PaginatedResult<Author>.CreateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AuthorRepository while getting all authors.");
                throw;
            }
        }
        public async Task<Author> GetAuthorByIdAsync(int authorId)
        {
            try
            {
                return await _context.Authors.Include(b => b.Books)
                    .FirstOrDefaultAsync(a => a.AuthorId == authorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in AuthorRepository while getting author with ID: {authorId}.");
                throw;
            }
        }

        public async Task<Author> AddAuthorAsync(Author author)
        {
            try
            {
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
                return author;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AuthorRepository while adding an author.");
                throw;
            }
        }

        public async Task UpdateAuthorAsync(Author author)
        {
            try
            {
                _context.Authors.Update(author);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in AuthorRepository while updating author with ID: {author.AuthorId}.");
                throw;
            }
        }

        public async Task DeleteAuthorAsync(Author author)
        {
            try
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in AuthorRepository while deleting author with ID: {author.AuthorId}.");
                throw;
            }
        }
    }
}
