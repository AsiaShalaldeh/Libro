using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Services
{
    public class LibrarianService : ILibrarianService
    {
        private readonly ILibrarianRepository _librarianRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LibrarianService> _logger;

        public LibrarianService(ILibrarianRepository librarianRepository, IUserRepository userRepository, ILogger<LibrarianService> logger)
        {
            _librarianRepository = librarianRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<PaginatedResult<Librarian>> GetAllLibrariansAsync(int pageNumber, int pageSize)
        {
            try
            {
                var paginatedResult = await _librarianRepository.GetAllLibrariansAsync(pageNumber, pageSize);

                var librarians = paginatedResult.Items;
                return new PaginatedResult<Librarian>(librarians, paginatedResult.TotalCount, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in LibrarianService while retrieving all librarians.");
                throw;
            }
        }

        public async Task<Librarian> GetLibrarianByIdAsync(string librarianId)
        {
            try
            {
                Librarian librarian = await _librarianRepository.GetLibrarianByIdAsync(librarianId);
                return librarian;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in LibrarianService while retrieving the librarian with ID: {librarianId}");
                throw;
            }
        }

        public async Task<Librarian> AddLibrarianAsync(string librarianId, string name)
        {
            try
            {
                Librarian librarian = new Librarian()
                {
                    LibrarianId = librarianId,
                    Name = name
                };
                return await _librarianRepository.AddLibrarianAsync(librarian);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in LibrarianService while adding a librarian.");
                throw;
            }
        }

        public async Task<Librarian> UpdateLibrarianAsync(string librarianId, LibrarianDto librarian)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in LibrarianService while updating the librarian with ID: {librarianId}");
                throw;
            }
        }

        public async Task DeleteLibrarianAsync(string librarianId)
        {
            try
            {
                Librarian existingLibrarian = await _librarianRepository.GetLibrarianByIdAsync(librarianId);
                if (existingLibrarian == null)
                {
                    throw new ResourceNotFoundException("Librarian", "ID", librarianId.ToString());
                }

                await _librarianRepository.DeleteLibrarianAsync(existingLibrarian);
                await _userRepository.DeleteUserAsync(librarianId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in LibrarianService while deleting the librarian with ID: {librarianId}");
                throw;
            }
        }
    }
}
