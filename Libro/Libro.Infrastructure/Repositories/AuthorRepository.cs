using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly IList<Author> _authors;

        public AuthorRepository()
        {
            _authors = new List<Author>
            {
                new Author { AuthorId = 1, Name = "John Doe" },
                new Author { AuthorId = 2, Name = "Jane Smith" },
                new Author { AuthorId = 3, Name = "Michael Johnson" }
            };
        }

        public IEnumerable<Author> GetAllAuthorsAsync()
        {
            return _authors;
        }

        public Author GetAuthorByIdAsync(int authorId)
        {
            return _authors.FirstOrDefault(a => a.AuthorId == authorId);
        }

        public int AddAuthorAsync(Author author)
        {
            author.AuthorId = _authors.Any() ? _authors.Max(a => a.AuthorId) + 1 : 1;
            _authors.Add(author);
            return author.AuthorId;
        }

        public bool UpdateAuthorAsync(Author author)
        {
            var existingAuthor = _authors.FirstOrDefault(a => a.AuthorId == author.AuthorId);
            if (existingAuthor == null)
                return false;

            existingAuthor.Name = author.Name;
            return true;
        }

        public bool DeleteAuthorAsync(int authorId)
        {
            var author = _authors.FirstOrDefault(a => a.AuthorId == authorId);
            if (author == null)
                return false;

            _authors.Remove(author);
            return true;
        }
    }

}
