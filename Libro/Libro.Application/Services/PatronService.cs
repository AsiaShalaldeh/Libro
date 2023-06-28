using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Services
{
    public class PatronService : IPatronService
    {
        private readonly IPatronRepository _patronRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PatronService> _logger;

        public PatronService(IPatronRepository patronRepository, IUserRepository userRepository, ILogger<PatronService> logger)
        {
            _patronRepository = patronRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Patron> GetPatronAsync(string patronId)
        {
            try
            {
                Patron patron = await _patronRepository.GetPatronByIdAsync(patronId);
                return patron;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in PatronService while retrieving the patron with ID: {patronId}.");
                throw;
            }
        }

        public async Task AddPatronAsync(string patronId, string name, string email)
        {
            try
            {
                Patron patron = new Patron()
                {
                    PatronId = patronId,
                    Name = name,
                    Email = email,
                };
                await _patronRepository.AddPatronAsync(patron);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in PatronService while adding a new patron.");
                throw;
            }
        }

        public async Task<Patron> UpdatePatronAsync(string patronId, PatronDto patronDto)
        {
            try
            {
                Patron existingPatron = await _patronRepository.GetPatronByIdAsync(patronId);

                if (existingPatron == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
                }

                if (!string.IsNullOrEmpty(patronDto.Email))
                {
                    existingPatron.Email = patronDto.Email;
                }

                if (!string.IsNullOrEmpty(patronDto.Name))
                {
                    existingPatron.Name = patronDto.Name;
                }

                Patron patron = await _patronRepository.UpdatePatronAsync(existingPatron);
                await _userRepository.UpdateUserAsync(patronId, patronDto.Name, patronDto.Email);
                return patron;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in PatronService while updating the patron with ID: {patronId}.");
                throw;
            }
        }

        public async Task<IEnumerable<Checkout>> GetBorrowingHistoryAsync(string patronId)
        {
            try
            {
                Patron patron = await _patronRepository.GetPatronByIdAsync(patronId);
                if (patron == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
                }
                return patron.CheckedoutBooks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in PatronService while retrieving the borrowing history for patron with ID: {patronId}.");
                throw;
            }
        }
    }
}
