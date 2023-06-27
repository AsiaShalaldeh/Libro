using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Application.Services
{
    public class LibrarianService : ILibrarianService
    {
        private readonly ILibrarianRepository _librarianRepository;
        private readonly IUserRepository _userRepository;

        public LibrarianService(ILibrarianRepository librarianRepository, IUserRepository userRepository)
        {
            _librarianRepository = librarianRepository;
            _userRepository = userRepository;
        }

        public async Task<PaginatedResult<Librarian>> GetAllLibrariansAsync(int pageNumber, int pageSize)
        {
            var paginatedResult = await _librarianRepository.GetAllLibrariansAsync(pageNumber, pageSize);

            var librarians = paginatedResult.Items;
            return new PaginatedResult<Librarian>(librarians, paginatedResult.TotalCount, pageNumber, pageSize);
        }

        public async Task<Librarian> GetLibrarianByIdAsync(string librarianId)
        {
            Librarian librarian = await _librarianRepository.GetLibrarianByIdAsync(librarianId);
            return librarian;
        }

        public async Task<Librarian> AddLibrarianAsync(string librarianId, string name)
        {
            Librarian librarian = new Librarian()
            {
                LibrarianId = librarianId,
                Name = name
            };
            return await _librarianRepository.AddLibrarianAsync(librarian);
        }

        public async Task<Librarian> UpdateLibrarianAsync(string librarianId, LibrarianDto librarian)
        {
            Librarian existingLibrarian = await _librarianRepository.GetLibrarianByIdAsync(librarianId);
            if (existingLibrarian == null)
            {
                throw new ResourceNotFoundException("Librarian", "ID", librarianId.ToString());
            }
            existingLibrarian.Name = librarian.Name;
            await _userRepository.UpdateUserAsync(librarianId, librarian.Name, "");
            return await _librarianRepository.UpdateLibrarianAsync(existingLibrarian);
        }

        public async Task DeleteLibrarianAsync(string librarianId)
        {
            Librarian existingLibrarian = await _librarianRepository.GetLibrarianByIdAsync(librarianId);
            if (existingLibrarian == null)
            {
                throw new ResourceNotFoundException("Librarian", "ID", librarianId.ToString());
            }

            await _librarianRepository.DeleteLibrarianAsync(existingLibrarian);
            await _userRepository.DeleteUserAsync(librarianId);
        }
    }

}
