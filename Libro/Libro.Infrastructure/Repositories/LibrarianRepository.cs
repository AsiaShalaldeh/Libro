using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Infrastructure.Repositories
{
    public class LibrarianRepository : ILibrarianRepository
    {
        private readonly List<Librarian> _librarians;

        public LibrarianRepository()
        {
            _librarians = new List<Librarian>
            {
                new Librarian { LibrarianId = "1", Name = "John Doe" },
                new Librarian { LibrarianId = "2", Name = "Jane Smith" }
            };
        }

        public IEnumerable<Librarian> GetAllLibrariansAsync()
        {
            return _librarians;
        }

        public Librarian GetLibrarianByIdAsync(string id)
        {
            var librarian = _librarians.Where(l => l.LibrarianId == id).FirstOrDefault();
            return librarian;
        }

        public Librarian AddLibrarianAsync(Librarian librarian)
        {
            _librarians.Add(librarian);
            return librarian;
        }

        public async Task UpdateLibrarianAsync(Librarian librarian)
        {
            var existingLibrarian = GetLibrarianByIdAsync(librarian.LibrarianId);
            existingLibrarian.Name = librarian.Name;
        }

        public async Task DeleteLibrarianAsync(Librarian librarian)
        {
            _librarians.Remove(librarian);
        }
    }

}
