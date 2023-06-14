using Libro.Domain.Dtos;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync();
        Task<AuthorDto> GetAuthorByIdAsync(int authorId);
        Task<int> AddAuthorAsync(AuthorDto authorDto);
        Task<bool> UpdateAuthorAsync(AuthorDto authorDto);
        Task<bool> DeleteAuthorAsync(int authorId);
    }
}
