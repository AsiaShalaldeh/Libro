using Libro.Domain.Common;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibroDbContext _context;

        public AuthorRepository(LibroDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Author>> GetAllAuthorsAsync(int pageNumber, int pageSize)
        {
            var query = _context.Authors.AsQueryable();

            return await PaginatedResult<Author>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<Author> GetAuthorByIdAsync(int authorId)
        {
            return await _context.Authors.Include(b => b.Books).
                FirstOrDefaultAsync(a => a.AuthorId == authorId);
        }

        public async Task<Author> AddAuthorAsync(Author author)
        {
             _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task UpdateAuthorAsync(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAuthorAsync(Author author)
        {
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
        }
    }

}
