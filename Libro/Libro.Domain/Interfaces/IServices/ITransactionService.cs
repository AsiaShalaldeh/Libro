using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Models;

namespace Libro.Domain.Interfaces.IServices
{
    public interface ITransactionService
    {
        Task<ReservationDto> ReserveBookAsync(Book book, Patron patron);
        Task<TransactionResponseModel> CheckoutBookAsync(Book book, Patron patron);
        Task<TransactionResponseModel> ReturnBookAsync(Book book, Patron patron);
        Task<IEnumerable<Book>> GetOverdueBooksAsync();
        Task<IEnumerable<Book>> GetBorrowedBooksAsync();
        Task<Book> GetBorrowedBookByIdAsync(string ISBN);
        Task<IEnumerable<Checkout>> GetCheckoutTransactionsByPatron(string patronId);
        Task<IEnumerable<Checkout>> GetOverdueTransactionsAsync();
    }
}
