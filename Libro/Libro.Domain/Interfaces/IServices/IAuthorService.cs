using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IAuthorService
    {
        Task<PaginatedResult<AuthorDto>> GetAllAuthorsAsync(int pageNumber, int pageSize);
        Task<Author> GetAuthorByIdAsync(int authorId);
        Task<Author> AddAuthorAsync(AuthorDto authorDto);
        Task UpdateAuthorAsync(int authorId, AuthorDto authorDto);
        Task DeleteAuthorAsync(int authorId);
    }
}
