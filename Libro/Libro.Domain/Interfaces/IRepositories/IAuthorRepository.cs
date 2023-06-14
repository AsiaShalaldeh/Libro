using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IAuthorRepository
    {
        Task<Author> GetByIdAsync(int authorId);
        Task<IEnumerable<Author>> GetAllAsync();
        Task AddAsync(Author author);
        Task UpdateAsync(Author author);
        Task RemoveAsync(int authorId);
    }
}
