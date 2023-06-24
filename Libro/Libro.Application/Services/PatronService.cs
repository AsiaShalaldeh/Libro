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

        public PatronService(IPatronRepository patronRepository)
        {
            _patronRepository = patronRepository;
        }

        public async Task<Patron> GetPatronProfileAsync(int patronId)
        {
            var patron = _patronRepository.GetPatronByIdAsync(patronId);
            //if (patron == null)
            //{
            //    throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
            //}
            return patron;
        }

        public async Task<Patron> UpdatePatronProfileAsync(Patron patron)
        {
            var existingPatron = _patronRepository.GetPatronByIdAsync(patron.PatronId);
            if (patron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", patron.PatronId.ToString());
            }
            _patronRepository.UpdatePatronAsync(patron);

            return patron;
        }
        public async Task<IEnumerable<BorrowingHistoryDTO>> GetBorrowingHistoryAsync(int patronId)
        {
            var transactions = _patronRepository.GetBorrowingHistoryAsync(patronId);

            // should bereplaced with map
            var borrowingHistory = transactions.Select(t => new BorrowingHistoryDTO
            {
                TransactionId = t.CheckoutId,
                BookTitle = t.Book.Title,
                BorrowDate = t.CheckoutDate,
                DueDate = t.DueDate,
                ReturnDate = t.IsReturned ? t.ReturnDate : null,
                IsOverdue = !t.IsReturned && DateTime.Now > t.DueDate
            });

            return borrowingHistory;
        }
    }
}
