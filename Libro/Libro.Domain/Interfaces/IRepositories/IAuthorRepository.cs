using Libro.Domain.Common;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IAuthorRepository
    {
        Task<PaginatedResult<Author>> GetAllAuthorsAsync(int pageNumber, int pageSize);
        Task<Author> GetAuthorByIdAsync(int authorId);
        Task<Author> AddAuthorAsync(Author author);
        Task UpdateAuthorAsync(Author author);
        Task DeleteAuthorAsync(Author author);
    }
}
