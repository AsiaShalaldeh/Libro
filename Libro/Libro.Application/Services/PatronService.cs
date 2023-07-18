using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Libro.Domain.Models;
using Libro.Domain.Common;

namespace Libro.Application.Services
{
    public class PatronService : IPatronService
    {
        private readonly IPatronRepository _patronRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PatronService> _logger;
        private readonly IMapper _mapper;

        public PatronService(IPatronRepository patronRepository, IUserRepository userRepository,
            ILogger<PatronService> logger, IMapper mapper)
        {
            _patronRepository = patronRepository;
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<PatronDto>> GetAllPatrons(int pageNumber, int pageSize)
        {
            try
            {
                var patrons = await _patronRepository.GetAllPatrons();
                var patronDtos = _mapper.Map<List<PatronDto>>(patrons);
                return new PaginatedResult<PatronDto>(patronDtos, patronDtos.Count, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in PatronService while getting all patrons.");
                throw;
            }
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

        public async Task<PatronDto> UpdatePatronAsync(string patronId, PatronDto patronDto)
        {
            try
            {
                Patron existingPatron = await _patronRepository.GetPatronByIdAsync(patronId);

                if (existingPatron == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId);
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
                return _mapper.Map<PatronDto>(patron);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in PatronService while updating the patron with ID: {patronId}.");
                throw;
            }
        }

        public async Task<IEnumerable<TransactionResponseModel>> GetBorrowingHistoryAsync(string patronId)
        {
            try
            {
                Patron patron = await _patronRepository.GetPatronByIdAsync(patronId);
                if (patron == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId);
                }
                var response = _mapper.Map<List<TransactionResponseModel>>(patron.CheckedoutBooks);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in PatronService while retrieving the borrowing history for patron with ID: {patronId}.");
                throw;
            }
        }
    }
}
