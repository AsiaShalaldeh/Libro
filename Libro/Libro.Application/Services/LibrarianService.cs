using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Application.Services
{
    public class LibrarianService : ILibrarianService
    {
        private readonly ILibrarianRepository _librarianRepository;

        public LibrarianService(ILibrarianRepository librarianRepository)
        {
            _librarianRepository = librarianRepository;
        }

        public async Task<IEnumerable<Librarian>> GetAllLibrariansAsync()
        {
            return _librarianRepository.GetAllLibrariansAsync();
        }

        public async Task<Librarian> GetLibrarianByIdAsync(int librarianId)
        {
            var librarian = _librarianRepository.GetLibrarianByIdAsync(librarianId);
            if (librarian == null)
            {
                throw new ResourceNotFoundException("Librarian", "ID", librarianId.ToString());
            }
            return librarian;
        }

        public async Task<Librarian> AddLibrarianAsync(Librarian librarian)
        {
            return _librarianRepository.AddLibrarianAsync(librarian);
        }

        public void UpdateLibrarianAsync(Librarian librarian)
        {
            var existingLibrarian = _librarianRepository.GetLibrarianByIdAsync(librarian.LibrarianId);
            if (existingLibrarian == null)
            {
                throw new ResourceNotFoundException("Librarian", "ID", librarian.LibrarianId.ToString());
            }

            _librarianRepository.UpdateLibrarianAsync(librarian);
        }

        public void DeleteLibrarianAsync(int librarianId)
        {
            var existingLibrarian = _librarianRepository.GetLibrarianByIdAsync(librarianId);
            if (existingLibrarian == null)
            {
                throw new ResourceNotFoundException("Librarian", "ID", librarianId.ToString());
            }

            _librarianRepository.DeleteLibrarianAsync(existingLibrarian);
        }
    }

}
