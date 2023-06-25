using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Application.Services
{
    public class PatronService : IPatronService
    {
        private readonly IPatronRepository _patronRepository;
        private readonly IMapper _mapper;

        public PatronService(IPatronRepository patronRepository, IMapper mapper)
        {
            _patronRepository = patronRepository;
            _mapper = mapper;
        }

        public async Task<Patron> GetPatronAsync(string patronId)
        {
            Patron patron = await _patronRepository.GetPatronByIdAsync(patronId);
            return patron;
        }
        public async Task AddPatronAsync(string patronId, string name, string email)
        {
            Patron patron = new Patron()
            {
                PatronId = patronId,
                Name = name,
                Email = email,
            };
            await _patronRepository.AddPatronAsync(patron);
        }
        public async Task<Patron> UpdatePatronAsync(string patronId, PatronDto patronDto)
        {
            Patron existingPatron = await _patronRepository.GetPatronByIdAsync(patronId);
            if (existingPatron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
            }
            existingPatron.Name = patronDto.Name;
            existingPatron.Email = patronDto.Email;

            Patron patron = await _patronRepository.UpdatePatronAsync(existingPatron);
            return patron;
        }
        public async Task<IEnumerable<Checkout>> GetBorrowingHistoryAsync(string patronId)
        {
            Patron patron = await _patronRepository.GetPatronByIdAsync(patronId);
            if (patron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
            }
            return patron.CheckedoutBooks;
        }
    }
}
