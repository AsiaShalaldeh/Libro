using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface ITransactionRepository
    {
        Task<Reservation> AddReservationAsync(Reservation reservation);
        Task<Checkout> AddCheckoutAsync(Checkout checkout);
        Task UpdateCheckoutAsync(Checkout checkout);
        //Task<Checkout> GetActiveTransaction(string ISBN, string patronId);
        Task<IEnumerable<string>> GetOverdueBookIdsAsync(); 
        Task<IEnumerable<string>> GetBorrowedBookIdsAsync();
        Task<string> GetBorrowedBookByIdAsync(string ISBN);
        Task<IEnumerable<Checkout>> GetCheckoutTransactionsByPatronAsync(string patronId);
        Task<IEnumerable<Checkout>> GetOverdueTransactionsAsync();
    }
}
