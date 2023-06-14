using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        public Task AddAsync(Author author)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Author>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Author> GetByIdAsync(int authorId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(int authorId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Author author)
        {
            throw new NotImplementedException();
        }
    }
}
