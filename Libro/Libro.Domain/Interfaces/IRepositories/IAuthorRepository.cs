using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IAuthorRepository
    {
        IEnumerable<Author> GetAllAuthorsAsync(); // Task
        Author GetAuthorByIdAsync(int authorId);
        int AddAuthorAsync(Author author);
        bool UpdateAuthorAsync(Author author);
        bool DeleteAuthorAsync(int authorId);
    }
}
